using System;
using UnityEngine;

namespace YuzeToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class InfoBoxAttribute : PropertyAttribute
    {
        public string Text { get; }
        public InfoType Type { get; }

        public InfoBoxAttribute(string text, InfoType type = InfoType.Normal)
        {
            Text = text;
            Type = type;
        }
    }
}