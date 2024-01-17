#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    [AttributeUsage(AttributeTargets.Field)]
    [Obsolete]
    public class ShowOnlyAttribute : PropertyAttribute
    {
        public string? Path { get; set; }

        public ShowOnlyAttribute(string path)
        {
            Path = path;
        }

        public ShowOnlyAttribute()
        {
            Path = null;
        }
    }
}