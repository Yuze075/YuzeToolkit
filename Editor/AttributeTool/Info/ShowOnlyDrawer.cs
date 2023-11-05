using UnityEditor;
using UnityEngine;
using YuzeToolkit.AttributeTool;

namespace YuzeToolkit.Editor.AttributeTool.Info
{
    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var showPropertyAttribute = (ShowOnlyAttribute)attribute;
        
            // 如果没有对应路径则和ReadOnly效果一样
            if (string.IsNullOrWhiteSpace(showPropertyAttribute.Path))
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(rect, property, label, true);
                GUI.enabled = true;
                EditorGUI.EndProperty();
                return;
            }

            // 获取路径对象和自己的对象
            var target = EditorHelper.GetValue(property, showPropertyAttribute.Path);
            var self = EditorHelper.GetValue(property, property.name);

            // 判断类型是否相同(或者是否可以继承
            if (target != null && self != null && self.GetType().IsInstanceOfType(target) &&
                EditorHelper.SetValue(property, property.name, target))
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(rect, property, label, true);
                GUI.enabled = true;
            }
            else
            {
                var warningContent = new GUIContent(property.displayName + "(Incorrect Attribute Used)")
                {
                    image = EditorGUIUtility.IconContent("console.warnicon").image
                };
                EditorGUI.LabelField(rect, warningContent);
            }
            EditorGUI.EndProperty();
        }
    }
}