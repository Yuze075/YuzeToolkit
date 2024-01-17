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
            this.value = value;
        }

        public T? value;

        public static explicit operator Ref<T>(T value) => new(value);
        public static explicit operator T?(Ref<T> @ref) => @ref.value;

        public bool Equals(Ref<T>? other)
        {
            if (other is null) return false;
            if (other.value == null && value == null) return true;
            return value != null && value.Equals(other.value);
        }

        public bool Equals(T? other)
        {
            if (other == null && value == null) return true;
            return value != null && value.Equals(other);
        }

        public override bool Equals(object? obj) =>
            (obj is Ref<T> otherRef && Equals(otherRef)) || (obj is T otherT && Equals(otherT));

        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => value?.GetHashCode() ?? 0;

        public static bool operator ==(Ref<T>? refOne, Ref<T>? refTwo) =>
            (refOne is null && refTwo is null) || (refOne is not null && refOne.Equals(refTwo));

        public static bool operator !=(Ref<T>? refOne, Ref<T>? refTwo) => !(refOne == refTwo);
    }
}