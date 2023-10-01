using System;
using UnityEngine;

namespace YuzeToolkit.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
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