#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    public class NetHook : NetworkBehaviour
    {
        public event Action? OnNetworkSpawnHook;
        public event Action? OnNetworkDespawnHook;
        public override void OnNetworkSpawn() => OnNetworkSpawnHook?.Invoke();
        public override void OnNetworkDespawn() => OnNetworkDespawnHook?.Invoke();

        public override void OnDestroy()
        {
            OnNetworkSpawnHook = null;
            OnNetworkDespawnHook = null;
            base.OnDestroy();
        }
    }
}
#endif