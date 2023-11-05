#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    [Serializable]
    public struct NetworkClass<TValue> : INetworkSerializable
        where TValue : class, INetworkSerializable, new()
    {
        public NetworkClass(TValue value) => (_b, _value) = (false, value);
        public NetworkClass(TValue value, bool b) => (_b, _value) = (b, value);

        private bool _b;
        private TValue _value;
        public TValue Value => _value;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            _value ??= new TValue();
            _value.NetworkSerialize(serializer);
        }

        public NetworkClass<TValue> GetChangeStruct(TValue value) => new(value, !_b);

        public static implicit operator NetworkClass<TValue>(TValue value) => new(value);
        public static implicit operator TValue(NetworkClass<TValue> networkStruct) => networkStruct.Value;
    }
}
#endif