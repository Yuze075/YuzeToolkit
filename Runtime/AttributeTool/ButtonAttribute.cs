#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    public enum ButtonEnableMode
    {
        /// <summary>
        /// Button should be active always
        /// </summary>
        Always,

        /// <summary>
        /// Button should be active only in editor
        /// </summary>
        Editor,

        /// <summary>
        /// Button should be active only in playmode
        /// </summary>
        Playmode
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string MethodName { get; }
        public string? Text { get; }
        public ButtonEnableMode SelectedEnableMode { get; }

        public ButtonAttribute(string methodName, string? text = null,
            ButtonEnableMode enabledMode = ButtonEnableMode.Always)
        {
            MethodName = methodName;
            Text = text;
            SelectedEnableMode = enabledMode;
        }
    }
}