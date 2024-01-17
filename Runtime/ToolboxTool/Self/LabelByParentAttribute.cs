#nullable enable
#if !YUZE_TOOLKIT_USE_EDITOR_TOOLBOX
using System;
using System.Diagnostics;
using Toolbox.Attributes;

namespace UnityEngine
{
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class LabelByParentAttribute : PropertyAttribute, ILabelProcessorAttribute
    {
        public LabelByParentAttribute(int upLayers = 1) => UpLayers = upLayers;
        public int UpLayers { get; private set; }
        public string? UpLayersSourceHandle { get; set; }
    }
}
#endif