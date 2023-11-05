#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    [Serializable]
    public struct VarValue : INetworkSerializable, IEquatable<VarValue>
    {
        public VarValue(long value) : this() => Value = value;
        public VarValue(int value) : this() => Value = value;
        public VarValue(short value) : this() => Value = value;
        public VarValue(sbyte value) : this() => Value = value;

        private sbyte _sbyteId;
        private short _shortId;
        private int _intId;
        private long _longId;

        private void SetDefault()
        {
            _sbyteId = default;
            _shortId = default;
            _intId = default;
            _longId = default;
        }

        public long Value
        {
            get
            {
                if (_longId != 0) return _longId;
                if (_intId != 0) return _intId;
                if (_shortId != 0) return _shortId;
                return _sbyteId;
            }
            set
            {
                SetDefault();
                switch (value)
                {
                    case <= byte.MaxValue:
                        _sbyteId = (sbyte)value;
                        return;
                    case <= ushort.MaxValue:
                        _shortId = (short)value;
                        return;
                    case <= uint.MaxValue:
                        _intId = (int)value;
                        return;
                    default:
                        _longId = value;
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
                        reader.ReadValueSafe(out _sbyteId);
                        return;
                    case 2:
                        reader.ReadValueSafe(out _shortId);
                        return;
                    case 4:
                        reader.ReadValueSafe(out _intId);
                        return;
                    case 8:
                        reader.ReadValueSafe(out _longId);
                        return;
                }

                if (reader.TryBeginReadValue(_longId))
                {
                    reader.ReadValue(out _longId);
                    return;
                }

                if (reader.TryBeginReadValue(_intId))
                {
                    reader.ReadValue(out _intId);
                    return;
                }

                if (reader.TryBeginReadValue(_shortId))
                {
                    reader.ReadValue(out _shortId);
                    return;
                }

                reader.ReadValueSafe(out _sbyteId);
            }
            else
            {
                var writer = serializer.GetFastBufferWriter();

                if (_longId != 0)
                {
                    writer.WriteValueSafe(_longId);
                    return;
                }

                if (_intId != 0)
                {
                    writer.WriteValueSafe(_intId);
                    return;
                }

                if (_shortId != 0)
                {
                    writer.WriteValueSafe(_shortId);
                    return;
                }

                writer.WriteValueSafe(_sbyteId);
            }
        }

        public static explicit operator sbyte(VarValue varValue) =>
            varValue.Value switch
            {
                >= sbyte.MaxValue => sbyte.MaxValue,
                <= sbyte.MinValue => sbyte.MinValue,
                _ => (sbyte)varValue.Value
            };

        public static explicit operator short(VarValue varValue) =>
            varValue.Value switch
            {
                >= short.MaxValue => short.MaxValue,
                <= short.MinValue => short.MinValue,
                _ => (short)varValue.Value
            };

        public static explicit operator int(VarValue varValue) =>
            varValue.Value switch
            {
                >= int.MaxValue => int.MaxValue,
                <= int.MinValue => int.MinValue,
                _ => (int)varValue.Value
            };

        public static implicit operator long(VarValue varValue) => varValue.Value;

        public static explicit operator VarValue(sbyte id) => new(id);
        public static explicit operator VarValue(short id) => new(id);
        public static explicit operator VarValue(int id) => new(id);
        public static implicit operator VarValue(long id) => new(id);

        public override string ToString() => $"[{nameof(VarValue)}] Value: {Value}";
        public bool Equals(VarValue other) => other.Value == Value;
        public override bool Equals(object? obj) => obj is VarValue other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
    }
}
#endif