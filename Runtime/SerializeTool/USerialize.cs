using System;
using UnityEngine;
using YuzeToolkit.LogTool;
using UnityObject = UnityEngine.Object;

namespace YuzeToolkit.SerializeTool
{
    [Serializable]
    public struct USerialize<T> : IEquatable<USerialize<T>> where T : class
    {
        public USerialize(T value) : this() => Value = value;

        [PrefabObjectOnly] [InLineEditor] [SerializeField]
        private UnityObject? value;

        public T? Value
        {
            get
            {
                if (value is T t) return t;
                LogSys.Log($"{GetType()}中的{nameof(value)}不是目标的{typeof(T)}类型！", ELogType.Warning);
                return null;
            }
            set
            {
                if (value is UnityObject o) this.value = o;
                LogSys.Log($"{GetType()}中, 传人的{nameof(value)}不是{typeof(UnityObject)}类型！", ELogType.Warning);
            }
        }

        public static implicit operator T?(USerialize<T> show) => show.Value;
        public static implicit operator USerialize<T>(T value) => new(value);
        public bool Equals(USerialize<T> other) => Equals(Value, other.Value);
        public override bool Equals(object? obj) => obj is USerialize<T> other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
    }
}