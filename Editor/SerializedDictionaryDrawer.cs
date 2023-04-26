using UnityEditor;
using UnityEngine;

namespace YuzeToolkit.Framework.Utility.Editor
{
    [CustomPropertyDrawer(typeof(SdAttribute))]
    public class SdDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var p = property.FindPropertyRelative("pairs");
            return EditorGUI.GetPropertyHeight(p ?? property);
        }

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var p = property.FindPropertyRelative("pairs");
            if (p != null)
            {
                EditorGUI.PropertyField(rect, p, label);
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