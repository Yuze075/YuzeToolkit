#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.SerializeTool
{
    [Serializable]
    public struct USerialize<T> : IEquatable<USerialize<T>> where T : class
    {
        public USerialize(T value) : this() => Value = value;

#if YUZE_USE_EDITOR_TOOLBOX
        [InLineEditor, LabelByParent]
#endif
        [SerializeField]
        private UnityEngine.Object? value;

        public T? Value
        {
            get
            {
                switch (value)
                {
                    case T t:
                        return t;
                    case GameObject gameObject when gameObject.TryGetComponent<T>(out var t1):
                        value = t1 as UnityEngine.Object;
                        return t1;
                }

                if (value == null) return null;
                value = null;
                return null;
            }
            set
            {
                if (value is UnityEngine.Object o) this.value = o;
            }
        }

        public UnityEngine.Object? UnityObjectValue => value;

        public static implicit operator T?(USerialize<T> show) => show.Value;
        public static implicit operator UnityEngine.Object?(USerialize<T> show) => show.value;
        public static implicit operator USerialize<T>(T value) => new(value);
        public bool Equals(USerialize<T> other) => Equals(Value, other.Value);
        public override bool Equals(object? obj) => obj is USerialize<T> other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
    }
}