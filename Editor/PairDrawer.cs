using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.Framework.Utility.Editor
{
    [CustomPropertyDrawer(typeof(KvAttribute))]
    public class KvDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var kProperty = property.FindPropertyRelative("key");
            var vProperty = property.FindPropertyRelative("value");
            if (kProperty != null && vProperty != null)
                return EditorGUI.GetPropertyHeight(kProperty) + EditorGUI.GetPropertyHeight(vProperty);
            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var kProperty = property.FindPropertyRelative("key");
            var vProperty = property.FindPropertyRelative("value");
            rect.height /= 2;
            if (kProperty != null && vProperty != null)
            {
                var kLabel = new GUIContent
                {
                    text = "K"
                };
                var vRect = rect;
                vRect.y += vRect.height;
                var vLabel = new GUIContent
                {
                    text = "V"
                };
                EditorGUI.PropertyField(rect, kProperty, kLabel);
                EditorGUI.PropertyField(vRect, vProperty, vLabel);
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