using UnityEditor;
using UnityEngine;
using YuzeToolkit.Attributes;

namespace YuzeToolkit.Editor.Attributes.Info
{
    [CustomPropertyDrawer(typeof(DividerLineAttribute))]
    public class DividerLineDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            var dividerLineAttribute = (DividerLineAttribute)attribute;
            return EditorGUIUtility.singleLineHeight + dividerLineAttribute.Height;
        }

        public override void OnGUI(Rect position)
        {
            var dividerLineAttribute = (DividerLineAttribute)attribute;
            var rect = EditorGUI.IndentedRect(position);
            rect.y += EditorGUIUtility.singleLineHeight / 3.0f;
            rect.height = dividerLineAttribute.Height;
            EditorGUI.DrawRect(rect, dividerLineAttribute.Color.GetColor());
        }
    }
}