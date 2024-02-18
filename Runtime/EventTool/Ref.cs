#nullable enable
using System;

namespace YuzeToolkit.EventTool
{
    public class Ref<T> : IEquatable<T>, IEquatable<Ref<T>>
    {
        public Ref()
        {
        }

        public Ref(T? value)
        {
            Value = value;
        }

        public T? Value;

        public static explicit operator Ref<T>(T? value) => new(value);
        public static implicit operator T?(Ref<T> @ref) => @ref.Value;

        public bool Equals(Ref<T>? other)
        {
            if (other is null) return false;
            if (other.Value == null && Value == null) return true;
            return Value != null && Value.Equals(other.Value);
        }

        public bool Equals(T? other)
        {
            if (other == null && Value == null) return true;
            return Value != null && Value.Equals(other);
        }

        public override bool Equals(object? obj) =>
            (obj is Ref<T> otherRef && Equals(otherRef)) || (obj is T otherT && Equals(otherT));

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public static bool operator ==(Ref<T>? refOne, Ref<T>? refTwo) =>
            (refOne is null && refTwo is null) || (refOne is not null && refOne.Equals(refTwo));

        public static bool operator !=(Ref<T>? refOne, Ref<T>? refTwo) => !(refOne == refTwo);
    }
}