#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    [Serializable]
    public struct UVarValue : INetworkSerializable, IEquatable<UVarValue>
    {
        public UVarValue(ulong value) : this() => Value = value;
        public UVarValue(uint value) : this() => Value = value;
        public UVarValue(ushort value) : this() => Value = value;
        public UVarValue(byte value) : this() => Value = value;

        private byte _byteId;
        private ushort _ushortId;
        private uint _uintId;
        private ulong _ulongId;

        private void SetDefault()
        {
            _byteId = default;
            _ushortId = default;
            _uintId = default;
            _ulongId = default;
        }

        public ulong Value
        {
            get
            {
                if (_ulongId != 0) return _ulongId;
                if (_uintId != 0) return _uintId;
                if (_ushortId != 0) return _ushortId;
                return _byteId;
            }
            set
            {
                SetDefault();
                switch (value)
                {
                    case <= byte.MaxValue:
                        _byteId = (byte)value;
                        return;
                    case <= ushort.MaxValue:
                        _ushortId = (ushort)value;
                        return;
                    case <= uint.MaxValue:
                        _uintId = (uint)value;
                        return;
                    default:
                        _ulongId = value;
                        break;
                }
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                var reader = serializer.GetFastBufferReader();
                SetDefault();

                switch (reader.Length - reader.Position - 5)
                {
                    case 1:
                        reader.ReadValueSafe(out _byteId);
                        return;
                    case 2:
                        reader.ReadValueSafe(out _ushortId);
                        return;
                    case 4:
                        reader.ReadValueSafe(out _uintId);
                        return;
                    case 8:
                        reader.ReadValueSafe(out _ulongId);
                        return;
                }

                if (reader.TryBeginReadValue(_ulongId))
                {
                    reader.ReadValue(out _ulongId);
                    return;
                }

                if (reader.TryBeginReadValue(_uintId))
                {
                    reader.ReadValue(out _uintId);
                    return;
                }

                if (reader.TryBeginReadValue(_ushortId))
                {
                    reader.ReadValue(out _ushortId);
                    return;
                }

                reader.ReadValueSafe(out _byteId);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();

                if (_ulongId != 0)
                {
                    writer.WriteValueSafe(_ulongId);
                    return;
                }

                if (_uintId != 0)
                {
                    writer.WriteValueSafe(_uintId);
                    return;
                }

                if (_ushortId != 0)
                {
                    writer.WriteValueSafe(_ushortId);
                    return;
                }

                writer.WriteValueSafe(_byteId);
            }
        }

        public static explicit operator byte(UVarValue uVarValue) =>
            uVarValue.Value switch
            {
                >= byte.MaxValue => byte.MaxValue,
                _ => (byte)uVarValue.Value
            };

        public static explicit operator ushort(UVarValue uVarValue) =>
            uVarValue.Value switch
            {
                >= ushort.MaxValue => ushort.MaxValue,
                _ => (ushort)uVarValue.Value
            };

        public static explicit operator uint(UVarValue uVarValue) =>
            uVarValue.Value switch
            {
                >= uint.MaxValue => uint.MaxValue,
                _ => (uint)uVarValue.Value
            };

        public static implicit operator ulong(UVarValue uVarValue) => uVarValue.Value;

        public static explicit operator UVarValue(byte id) => new(id);
        public static explicit operator UVarValue(ushort id) => new(id);
        public static explicit operator UVarValue(uint id) => new(id);
        public static implicit operator UVarValue(ulong id) => new(id);


        public override string ToString() => $"[{nameof(UVarValue)}] Value: {Value}";
        public bool Equals(UVarValue other) => other.Value == Value;
        public override bool Equals(object? obj) => obj is UVarValue other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
    }
}
#endif