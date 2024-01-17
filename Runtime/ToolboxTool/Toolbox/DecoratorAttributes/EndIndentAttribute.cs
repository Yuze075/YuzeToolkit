#if !YUZE_TOOLKIT_USE_EDITOR_TOOLBOX
using System;
using System.Diagnostics;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [Conditional("UNITY_EDITOR")]
    public class EndIndentAttribute : ToolboxDecoratorAttribute
    {
        public EndIndentAttribute(int indentToSubtract = 1)
        {
            IndentToSubtract = indentToSubtract;
        }

        public int IndentToSubtract { get; private set; }
    }
}
#endif