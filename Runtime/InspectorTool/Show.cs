#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
    [Serializable]
#endif
    public struct Show<T> : IEquatable<Show<T>>
    {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
        public Show(T? value)
        {
            _value = value;
            _showValue = IShowValue.GetShowValue(value, 2);
        }

        [Disable] [IgnoreParent] [SerializeReference] [LabelByParent]
        private IShowValue _showValue;

        [NonSerialized] private T? _value;


        public T? Value
        {
            get => _value;
            set
            {
                _showValue = IShowValue.GetShowValue(value, 2);
                _value = value;
            }
        }
#else
        public Show(T? value) => Value = value;
        [NonSerialized] public T? Value;
#endif
        public static implicit operator T?(Show<T> show) => show.Value;
        public static implicit operator Show<T>(T? value) => new(value);
        public bool Equals(Show<T> other) => Equals(Value, other.Value);
        public override bool Equals(object? obj) => obj is Show<T> other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value?.ToString() ?? string.Empty;
    }
}