using System;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
    [Serializable]
    public struct Show<T> : IEquatable<Show<T>>
    {
        public Show(T? value) : this()
        {
#if UNITY_EDITOR
            _showValue = IShowValue.GetShowValue(value, "Value");
#endif
            _value = value;
        }

        private T? _value;

#if UNITY_EDITOR
        [IgnoreParent] [SerializeReference] private IShowValue _showValue;
#endif

        public T? Value
        {
            get => _value;
            set
            {
#if UNITY_EDITOR
                _showValue = IShowValue.GetShowValue(value, "Value");
#endif
                _value = value;
            }
        }

        public static implicit operator T?(Show<T> show) => show.Value;
        public static implicit operator Show<T>(T? value) => new(value);
        public bool Equals(Show<T> other) => Equals(Value, other.Value);
        public override bool Equals(object? obj) => obj is Show<T> other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
    }
}