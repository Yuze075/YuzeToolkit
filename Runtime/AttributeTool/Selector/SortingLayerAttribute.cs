#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SortingLayerAttribute : PropertyAttribute
    {
    }
}