#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    [AttributeUsage(AttributeTargets.Field)]
    [Obsolete]
    public class HelpBoxAttribute : PropertyAttribute
    {
        public string GetSting { get; }
        public InfoType InfoType { get; }

        public HelpBoxAttribute(string getSting = "", InfoType infoType = InfoType.Warning)
        {
            GetSting = getSting;
            InfoType = infoType;
        }
    }
}