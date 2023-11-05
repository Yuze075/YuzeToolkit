using System;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AnimatorParamAttribute : PropertyAttribute
    {
        public string AnimatorName { get; }
        public AnimatorControllerParameterType? AnimatorParamType { get; }

        public AnimatorParamAttribute(string animatorName)
        {
            AnimatorName = animatorName;
            AnimatorParamType = null;
        }

        public AnimatorParamAttribute(string animatorName, AnimatorControllerParameterType animatorParamType)
        {
            AnimatorName = animatorName;
            AnimatorParamType = animatorParamType;
        }
    }
}