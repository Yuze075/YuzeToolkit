#if !YUZE_TOOLKIT_USE_EDITOR_TOOLBOX
using System.Diagnostics;

namespace UnityEngine
{
    [Conditional("UNITY_EDITOR")]
    public abstract class DynamicMinMaxBaseAttribute : ToolboxSelfPropertyAttribute
    {
        protected DynamicMinMaxBaseAttribute(string minValueSource, string maxValueSource)
        {
            MinValueSource = minValueSource;
            MaxValueSource = maxValueSource;
        }

        public string MinValueSource { get; protected set; }

        public string MaxValueSource { get; protected set; }
    }
}
#endif