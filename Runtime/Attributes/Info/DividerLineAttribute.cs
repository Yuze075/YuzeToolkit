using System;
using UnityEngine;

namespace YuzeToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DividerLineAttribute : PropertyAttribute
    {
        public float Height { get; }
        public  ColorType Color { get; }

        public DividerLineAttribute(float height = 2.0f, ColorType color = ColorType.Gray)
        {
            Height = height;
            Color = color;
        }
    }
}