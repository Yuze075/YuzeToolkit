#if USE_UNITY_NETCODE
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    public static class NetworkExtend
    {
        public static TValue Get<TValue>(this NetworkVariable<NetworkClass<TValue>> networkClass)
            where TValue : class, INetworkSerializable, new() => networkClass.Value;

        public static void Set<TValue>(this NetworkVariable<NetworkClass<TValue>> networkClass, TValue value)
            where TValue : class, INetworkSerializable, new() =>
            networkClass.Value = networkClass.Value.GetChangeStruct(value);
    }
}
#endif