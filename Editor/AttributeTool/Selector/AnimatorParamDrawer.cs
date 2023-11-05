using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using YuzeToolkit.AttributeTool;

namespace YuzeToolkit.Editor.AttributeTool
{
    [CustomPropertyDrawer(typeof(AnimatorParamAttribute))]
    public class AnimatorParamDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            if (property.propertyType is not (SerializedPropertyType.Integer or SerializedPropertyType.String))
            {
                EditorHelper.DrawWarningMessage(rect, property.displayName + "(错误的特性使用!)");
                EditorGUI.EndProperty();
                return;
            }

            var animatorParamAttribute = (AnimatorParamAttribute)attribute;
            if (!EditorHelper.TryGetValue<Animator>(property, animatorParamAttribute.AnimatorName,
                    out var animatorController))
            {
                EditorHelper.DrawWarningMessage(rect, property.displayName + "(无法找到对应animator!)");
                EditorGUI.EndProperty();
                return;
            }
            
            var parametersCount = animatorController.parameters.Length;
            var animatorParameters =
                new List<AnimatorControllerParameter>(parametersCount);
            for (var i = 0; i < parametersCount; i++)
            {
                var parameter = animatorController.parameters[i];
                if (animatorParamAttribute.AnimatorParamType == null ||
                    parameter.type == animatorParamAttribute.AnimatorParamType)
                {
                    animatorParameters.Add(parameter);
                }
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, animatorParameters);
                    break;
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, animatorParameters);
                    break;
            }

            EditorGUI.EndProperty();
        }
        
        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label,
            IReadOnlyList<AnimatorControllerParameter> animatorParameters)
        {
            var paramNameHash = property.intValue;
            var index = 0;

            for (var i = 0; i < animatorParameters.Count; i++)
            {
                if (paramNameHash != animatorParameters[i].nameHash) continue;
                index = i + 1; // +1 because the first option is reserved for (None)
                break;
            }

            var displayOptions = GetDisplayOptions(animatorParameters);

            var newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            var newValue = newIndex == 0 ? 0 : animatorParameters[newIndex - 1].nameHash;

            if (property.intValue != newValue)
            {
                property.intValue = newValue;
            }
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label,
            IReadOnlyList<AnimatorControllerParameter> animatorParameters)
        {
            var paramName = property.stringValue;
            var index = 0;

            for (var i = 0; i < animatorParameters.Count; i++)
            {
                if (!paramName.Equals(animatorParameters[i].name, StringComparison.Ordinal)) continue;
                index = i + 1; // +1 because the first option is reserved for (None)
                break;
            }

            var displayOptions = GetDisplayOptions(animatorParameters);

            var newIndex = EditorGUI.Popup(rect, label.text, index, displayOptions);
            var newValue = newIndex == 0 ? null : animatorParameters[newIndex - 1].name;

            if (!property.stringValue.Equals(newValue, StringComparison.Ordinal))
            {
                property.stringValue = newValue;
            }
        }

        private static string[] GetDisplayOptions(IReadOnlyList<AnimatorControllerParameter> animatorParams)
        {
            var displayOptions = new string[animatorParams.Count + 1];
            displayOptions[0] = "(None)";

            for (var i = 0; i < animatorParams.Count; i++)
            {
                displayOptions[i + 1] = animatorParams[i].name;
            }

            return displayOptions;
        }

    }
}