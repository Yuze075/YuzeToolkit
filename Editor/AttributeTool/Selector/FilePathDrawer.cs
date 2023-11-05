using System.IO;
using UnityEditor;
using UnityEngine;
using FilePathAttribute = YuzeToolkit.AttributeTool.FilePathAttribute;

namespace YuzeToolkit.Editor.AttributeTool
{
    [CustomPropertyDrawer(typeof(FilePathAttribute))]
    public class FilePathDrawer : PropertyDrawer
    {
        #region static

        private static bool IsPathValid(string propertyPath, string assetRelativePath)
        {
            var targetPath = string.IsNullOrEmpty(assetRelativePath)
                ? Path.Combine(Application.dataPath[..^7], propertyPath)
                : Path.Combine(Application.dataPath[..^7], assetRelativePath, propertyPath);

            return File.Exists(targetPath);
        }

        private static Rect DrawWarningMessage(Rect rect)
        {
            rect = new Rect(rect.x,
                rect.y,
                rect.width, Style.BoxHeight);
            EditorGUI.HelpBox(rect, "Provided file does not exist.", MessageType.Warning);
            return rect;
        }

        private static void UseFilePicker(SerializedProperty property, string relativePath)
        {
            var baseDataPath = Application.dataPath[..^7];
            var baseOpenPath = string.Empty;
            if (!string.IsNullOrEmpty(relativePath))
            {
                if (baseDataPath != null) baseDataPath = Path.Combine(baseDataPath, relativePath);
                baseOpenPath = Path.Combine(baseOpenPath, relativePath);
            }

            var selectedPath = EditorUtility.OpenFilePanel("Pick file", baseOpenPath, "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (baseDataPath != null)
                {
                    baseDataPath = baseDataPath.Replace('\\', '/');
                    selectedPath = selectedPath.Replace(baseDataPath, "");
                }

                selectedPath = selectedPath.Remove(0, 1);

                property.serializedObject.Update();
                property.stringValue = selectedPath;
                property.serializedObject.ApplyModifiedProperties();
            }

            GUIUtility.ExitGUI();
        }

        private static class Style
        {
            internal static readonly float RowHeight = EditorGUIUtility.singleLineHeight;
            internal static readonly float BoxHeight = EditorGUIUtility.singleLineHeight * 2.1f;
            internal static readonly float Spacing = EditorGUIUtility.standardVerticalSpacing;
            internal const float PickerWidth = 30.0f;
            internal static readonly GUIContent PickerContent;

            static Style()
            {
                PickerContent = EditorGUIUtility.IconContent("DefaultAsset Icon");
                PickerContent.tooltip = "Pick file";
            }
        }

        #endregion

        private FilePathAttribute Attribute => attribute as FilePathAttribute;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return IsPathValid(property.stringValue, Attribute.RelativePath)
                ? EditorGUI.GetPropertyHeight(property, label)
                : EditorGUI.GetPropertyHeight(property, label) + Style.BoxHeight + Style.Spacing * 2;
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorHelper.DrawWarningMessage(rect, property.displayName + "(错误的特性使用!)");
                EditorGUI.EndProperty();
                return;
            }

            if (!IsPathValid(property.stringValue, Attribute.RelativePath))
            {
                rect = DrawWarningMessage(rect);
                rect.yMin = rect.yMax + Style.Spacing;
                rect.yMax = rect.yMin + Style.RowHeight;
            }

            rect.xMax -= Style.PickerWidth + Style.Spacing;
            EditorGUI.PropertyField(rect, property, label);
            rect.xMax += Style.PickerWidth + Style.Spacing;
            rect.xMin = rect.xMax - Style.PickerWidth;
            if (GUI.Button(rect, Style.PickerContent, EditorStyles.miniButton))
            {
                UseFilePicker(property, Attribute.RelativePath);
            }

            EditorGUI.EndProperty();
        }
    }
}