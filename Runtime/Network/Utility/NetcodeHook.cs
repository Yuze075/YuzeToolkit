#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    public class NetcodeHook : NetworkBehaviour
    {
        private Action? _onNetworkSpawnHook;
        private Action? _onNetworkDespawnHook;

        public event Action? OnNetworkSpawnHook
        {
            add => _onNetworkSpawnHook += value;
            remove => _onNetworkSpawnHook -= value;
        }

        public event Action? OnNetworkDespawnHook
        {
            add => _onNetworkDespawnHook += value;
            remove => _onNetworkDespawnHook -= value;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _onNetworkSpawnHook?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _onNetworkDespawnHook?.Invoke();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _onNetworkSpawnHook = null;
            _onNetworkDespawnHook = null;
        }
    }
}
#endif