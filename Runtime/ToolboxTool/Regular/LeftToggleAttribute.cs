#if !YUZE_TOOLKIT_USE_EDITOR_TOOLBOX
using System;
using System.Diagnostics;

namespace UnityEngine
{
    /// <summary>
    /// Draws the left-ordered toggle.
    /// 
    /// <para>Supported types: <see cref="bool"/>.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class LeftToggleAttribute : PropertyAttribute
    { }
}
#endif