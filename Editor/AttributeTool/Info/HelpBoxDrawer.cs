using System;
using UnityEditor;
using UnityEngine;
using YuzeToolkit.AttributeTool;

namespace YuzeToolkit.Editor.AttributeTool.Info
{
    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (string.IsNullOrWhiteSpace(property.stringValue))
                return 0;

            var i = 0;
            if (property.propertyType == SerializedPropertyType.String)
            {
                i = property.stringValue.Split(new[] { '\n' }).Length;
            }

            return EditorGUI.GetPropertyHeight(property) * (i + 1) * 0.7f;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorHelper.DrawWarningMessage(rect, property.displayName + "(错误的特性使用!)");
                EditorGUI.EndProperty();
                return;
            }

            if (!string.IsNullOrWhiteSpace(helpBoxAttribute.GetSting) &&
                EditorHelper.TryGetValue(property, helpBoxAttribute.GetSting, out string info))
            {
                property.stringValue = info;
            }

            var str = property.stringValue;
            if (!string.IsNullOrWhiteSpace(str))
            {
                switch (helpBoxAttribute.InfoType)
                {
                    case InfoType.Normal:
                        EditorGUI.HelpBox(rect, str, MessageType.Info);
                        break;
                    case InfoType.Warning:
                        EditorGUI.HelpBox(rect, str, MessageType.Warning);
                        break;
                    case InfoType.Error:
                        EditorGUI.HelpBox(rect, str, MessageType.Error);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            EditorGUI.EndProperty();
        }
    }
}