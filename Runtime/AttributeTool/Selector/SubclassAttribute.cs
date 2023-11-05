using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace YuzeToolkit.AttributeTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SubclassAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,
        Inherited = false)]
    public sealed class AddSubclassMenuAttribute : Attribute
    {
        public AddSubclassMenuAttribute(Type type, string menuName, int order = 0)
        {
            Order = order;
            MenuName = $"{Regex.Match(type.FullName!, @"(\.?[^.]+?){0,2}$").Value.Replace('.', '/')}/{menuName}";
        }

        public AddSubclassMenuAttribute(string menuName, int order = 0)
        {
            MenuName = menuName;
            Order = order;
        }

        private static readonly char[] Separate = { '/' };
        public string MenuName { get; }
        public int Order { get; }

        public string[] SplitMenuName => !string.IsNullOrWhiteSpace(MenuName)
            ? MenuName.Split(Separate, StringSplitOptions.RemoveEmptyEntries)
            : Array.Empty<string>();


        public string TypeNameWithoutPath
        {
            get
            {
                var splitDisplayName = SplitMenuName;
                return splitDisplayName.Length != 0 ? splitDisplayName[^1] : null;
            }
        }
    }
}