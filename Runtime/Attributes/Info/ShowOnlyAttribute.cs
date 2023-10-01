using System;
using UnityEngine;

namespace YuzeToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowOnlyAttribute : PropertyAttribute
    {
        public string Path { get; set; }

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