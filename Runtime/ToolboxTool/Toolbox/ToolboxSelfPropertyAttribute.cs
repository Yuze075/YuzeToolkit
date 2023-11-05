#if !USE_EDITOR_TOOLBOX
using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public abstract class ToolboxSelfPropertyAttribute : ToolboxPropertyAttribute
    { }
}
#endif