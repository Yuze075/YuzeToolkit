using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using YuzeToolkit.AttributeTool;

namespace YuzeToolkit.Editor.AttributeTool
{
    public static class EditorHelper
    {
        /// <summary>
        /// 获取到具体哪一个对象拥有这个<see cref="UnityEditor.SerializedProperty"/>
        /// </summary>
        public static object GetTargetObjectWithProperty(this SerializedProperty property, int leftValue = 1)
        {
            var path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');

            for (var i = 0; i < elements.Length - leftValue; i++)
            {
                var element = elements[i];
                if (element.Contains("["))
                {
                    var elementName = element[..element.IndexOf("[", StringComparison.Ordinal)];
                    var index = Convert.ToInt32(element[element.IndexOf("[", StringComparison.Ordinal)..]
                        .Replace("[", "").Replace("]", ""));
                    obj = GetValueInList(obj, elementName, index);
                }
                else
                {
                    obj = GetValueIn(obj, element);
                }
            }

            return obj;

            static object GetValueInList(object source, string name, int index)
            {
                if (GetValueIn(source, name) is not IEnumerable enumerable)
                {
                    return null;
                }

                var enumerator = enumerable.GetEnumerator();
                for (var i = 0; i <= index; i++)
                {
                    if (!enumerator.MoveNext())
                    {
                        return null;
                    }
                }

                return enumerator.Current;
            }

            static object GetValueIn(object source, string name)
            {
                if (source == null)
                {
                    return null;
                }

                var type = source.GetType();

                while (type != null)
                {
                    var field = type.GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (field != null)
                    {
                        return field.GetValue(source);
                    }

                    var property = type.GetProperty(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property != null)
                    {
                        return property.GetValue(source, null);
                    }

                    type = type.BaseType;
                }

                return null;
            }
        }

        /// <summary>
        /// 获取和<see cref="UnityEditor.SerializedProperty"/>同一层的value的值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this SerializedProperty property, string valueName, out T value)
        {
            var obj = GetValue(property, valueName);
            if (obj is T t)
            {
                value = t;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// 尝试获取和<see cref="UnityEditor.SerializedProperty"/>同一层的value的值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="valueName"></param>
        /// <returns></returns>
        public static object GetValue(this SerializedProperty property, string valueName)
        {
            return GetTargetObjectWithProperty(property).GetValue(valueName);
        }
        
        public static T? GetValue<T>(this SerializedProperty property, string valueName)
        {
            return property.GetValue(valueName) is T t ? t : default;
        }

        /// <summary>
        /// 尝试去设置<see cref="UnityEditor.SerializedProperty"/>同一层的value的值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="valueName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetValue(this SerializedProperty property, string valueName, object value)
        {
            return Helper.SetValue(GetTargetObjectWithProperty(property), valueName, value);
        }

        public static void DrawWarningMessage(Rect rect, string massage)
        {
            var warningContent = new GUIContent(massage)
            {
                image = EditorGUIUtility.IconContent("console.warnicon").image
            };
            EditorGUI.LabelField(rect, warningContent);
        }
    }
}