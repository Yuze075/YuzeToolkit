#if !USE_EDITOR_TOOLBOX
using System;
using System.Diagnostics;
using Toolbox.Attributes;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class GetLabelAttribute :  PropertyAttribute, ILabelProcessorAttribute
    {
        public GetLabelAttribute(string sourceHandle) => SourceHandle = sourceHandle;
        public string SourceHandle { get; private set; }
    }
}
#endif