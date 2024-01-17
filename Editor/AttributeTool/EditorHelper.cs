#nullable enable
using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;

namespace YuzeToolkit.AttributeTool.Editor
{
    public static class EditorHelper
    {
        /// <summary>
        /// 获取到那个<see cref="object"/>拥有当前这个<see cref="SerializedProperty"/><br/>
        /// 当嵌套查询中存在结构体时, 获取的是对应的值克隆, 而不是原来的值, 所有对应结构体数据的任何操作都不会反应到原来的对象
        /// (但是从结构体中获得的类引用, 基于类引用的更改是可以改到具体的类上的
        /// </summary>
        /// <param name="property">需要查询的<see cref="SerializedProperty"/></param>
        /// <param name="upLayers">向上查询的层数, 默认为1层(如果超过最多层数, 默认返回最上层的<see cref="SerializedObject"/>)</param>
        /// <returns>返回查询到的拥有者<see cref="object"/></returns>
        public static object? GetPropertyOwnerObject(this SerializedProperty? property, int upLayers = 1)
        {
            if (property == null) return null;
            var path = property.propertyPath.Replace(".Array.data[", "[");
            object? obj = property.serializedObject.targetObject;
            var elements = path.Split('.');

            if (upLayers >= elements.Length) return obj;

            for (var i = 0; i < elements.Length - upLayers; i++)
            {
                var element = elements[i];
                if (element.Contains("["))
                {
                    var elementName = Regex.Match(element, @"^.*(?=\[)").Value;
                    var index = Convert.ToInt32(Regex.Match(element, @"(?<=\[)\d+(?=\])").Value);
                    obj = GetValueInSourceList(obj, elementName, index);
                    if (obj == null) return null;
                }
                else
                {
                    obj = GetValueInSource(obj, element);
                    if (obj == null) return null;
                }
            }

            return obj;

            static object? GetValueInSource(object? source, string? name)
            {
                if (source == null) return null;
                if (string.IsNullOrWhiteSpace(name)) return null;

                var type = source.GetType();

                while (type != null)
                {
                    var field = type.GetField(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    if (field != null) return field.GetValue(source);

                    var property = type.GetProperty(name,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property != null) return property.GetValue(source, null);

                    type = type.BaseType;
                }

                return null;
            }

            static object? GetValueInSourceList(object? source, string? name, int index)
            {
                if (source == null) return null;
                if (string.IsNullOrWhiteSpace(name)) return null;
                if (GetValueInSource(source, name) is not IEnumerable enumerable) return null;

                var enumerator = enumerable.GetEnumerator();
                for (var i = 0; i <= index; i++)
                {
                    if (!enumerator.MoveNext()) return null;
                }

                return enumerator.Current;
            }
        }


        /// <summary>
        /// 获取到那个<see cref="SerializedProperty"/>拥有当前这个<see cref="SerializedProperty"/>
        /// </summary>
        /// <param name="property">需要查询的<see cref="SerializedProperty"/></param>
        /// <param name="upLayers">向上查询的层数, 默认为1层(如果超过最多层数, 默认返回最上层的<see cref="SerializedObject"/>)</param>
        /// <returns>返回查询到的拥有者<see cref="SerializedWrapper"/></returns>
        public static SerializedWrapper GetPropertyOwnerProperty(this SerializedProperty? property, int upLayers = 1)
        {
            if (property == null) return (SerializedProperty?)null;
            var path = property.propertyPath.Replace(".Array.data[", "[");
            var serializedObject = property.serializedObject;
            var elements = path.Split('.');

            if (elements.Length >= upLayers) return serializedObject;
            property = serializedObject.FindProperty(elements[0]);
            for (var i = 1; i < elements.Length - upLayers; i++)
            {
                var element = elements[i];
                property = property.FindPropertyRelative(element.Contains("[")
                    ? element.Replace("[", ".Array.data[")
                    : element);
            }

            return property;
        }

        public readonly struct SerializedWrapper
        {
            public SerializedWrapper(SerializedProperty? property)
            {
                isProperty = true;
                this.property = property;
                serializedObject = null;
            }

            public SerializedWrapper(SerializedObject? serializedObject)
            {
                isProperty = true;
                property = null;
                this.serializedObject = serializedObject;
            }

            public readonly bool isProperty;
            public readonly SerializedProperty? property;
            public readonly SerializedObject? serializedObject;

            public static implicit operator SerializedProperty?(SerializedWrapper wrapper) => wrapper.property;
            public static implicit operator SerializedObject?(SerializedWrapper wrapper) => wrapper.serializedObject;
            public static implicit operator SerializedWrapper(SerializedProperty? property) => new(property);

            public static implicit operator SerializedWrapper(SerializedObject? serializedObject) =>
                new(serializedObject);
        }

        /// <summary>
        /// 获取和<see cref="SerializedProperty"/>同一层的value的值
        /// </summary>
        public static bool TryGetValue<T>(this SerializedProperty property, string valueName, out T value)
        {
            if (GetValue(property, valueName) is T t)
            {
                value = t;
                return true;
            }

            value = default!;
            return false;
        }

        /// <summary>
        /// 尝试获取和<see cref="SerializedProperty"/>同一层的value的值
        /// </summary>
        public static T? GetValue<T>(this SerializedProperty? property, string? valueName) =>
            property.GetValue(valueName) is T t ? t : default;

        /// <summary>
        /// 尝试获取和<see cref="SerializedProperty"/>同一层的value的值
        /// </summary>
        public static object? GetValue(this SerializedProperty? property, string? valueName) =>
            Helper.GetValue(property.GetPropertyOwnerObject(), valueName);

        /// <summary>
        /// 尝试获取<see cref="SerializedProperty"/>对应的值
        /// </summary>
        public static object? GetSelf(this SerializedProperty? property) =>
            property.GetPropertyOwnerObject(0);


        /// <summary>
        /// 尝试去设置<see cref="SerializedProperty"/>同一层的value的值
        /// </summary>
        public static bool SetValue(this SerializedProperty? property, string? valueName, object? value) =>
            Helper.SetValue(property.GetPropertyOwnerObject(), valueName, value);

        public static void DrawWarningMessage(UnityEngine.Rect rect, string massage)
        {
            var warningContent = new UnityEngine.GUIContent(massage)
            {
                image = EditorGUIUtility.IconContent("console.warnicon").image
            };
            EditorGUI.LabelField(rect, warningContent);
        }
    }
}