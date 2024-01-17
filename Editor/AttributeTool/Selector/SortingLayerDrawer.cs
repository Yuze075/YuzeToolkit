#nullable enable
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.AttributeTool.Editor
{
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerDrawer : PropertyDrawer
    {
        private static string[]? Layers
        {
            get
            {
                var internalEditorUtilityType = typeof(UnityEditorInternal.InternalEditorUtility);
                var sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames",
                    BindingFlags.Static | BindingFlags.NonPublic);
                return sortingLayersProperty != null
                    ? (string[])sortingLayersProperty.GetValue(null, Array.Empty<object>())
                    : null;
            }
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, Layers);
                    break;
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, Layers);
                    break;
                default:
                    EditorHelper.DrawWarningMessage(rect, property.displayName + "(错误的特性使用!)");
                    break;
            }

            EditorGUI.EndProperty();
        }


        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label,
            string[]? layers)
        {
            if (layers == null) return;
            var index = IndexOf(layers, property.stringValue);
            var newIndex = EditorGUI.Popup(rect, label.text, index, layers);
            var newLayer = layers[newIndex];
            if (!property.stringValue.Equals(newLayer, StringComparison.Ordinal))
            {
                property.stringValue = layers[newIndex];
            }
        }

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label,
            string[]? layers)
        {
            if (layers == null) return;
            var index = 0;
            var layerName = SortingLayer.IDToName(property.intValue);
            for (var i = 0; i < layers.Length; i++)
            {
                if (!layerName.Equals(layers[i], StringComparison.Ordinal)) continue;
                index = i;
                break;
            }

            var newIndex = EditorGUI.Popup(rect, label.text, index, layers);
            var newLayerName = layers[newIndex];
            var newLayerNumber = SortingLayer.NameToID(newLayerName);
            if (property.intValue != newLayerNumber)
            {
                property.intValue = newLayerNumber;
            }
        }

        private static int IndexOf(string[] layers, string layer)
        {
            var index = Array.IndexOf(layers, layer);
            return Mathf.Clamp(index, 0, layers.Length - 1);
        }
    }
}