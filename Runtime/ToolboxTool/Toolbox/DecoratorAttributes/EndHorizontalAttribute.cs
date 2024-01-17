#if !YUZE_TOOLKIT_USE_EDITOR_TOOLBOX
using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Use this attribute to close previously created <see cref="BeginHorizontalAttribute"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class EndHorizontalAttribute : ToolboxDecoratorAttribute
    { }
}
#endif