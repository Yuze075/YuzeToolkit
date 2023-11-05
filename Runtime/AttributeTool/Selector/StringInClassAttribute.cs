using System;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringInClassAttribute : PropertyAttribute
    {
        public Type TargetType { get; }
        public string MatchRule { get; set; }
        public Type MatchRuleType { get; }
        public bool HasLabel { get; }
        public bool UseValueToName { get; }

        public StringInClassAttribute(Type targetType, string matchRule = "", Type matchRuleType = null,
            bool hasLabel = true, bool useValueToName = false)
        {
            TargetType = targetType;
            MatchRule = matchRule;
            MatchRuleType = matchRuleType;
            HasLabel = hasLabel;
            UseValueToName = useValueToName;
        }
    }
}