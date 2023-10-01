#if Netcode && NetcodeComponents && UnityTransport
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    public class NetcodeHook : NetworkBehaviour
    {
        public event Action OnNetworkSpawnHook;

        public event Action OnNetworkDespawnHook;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            OnNetworkSpawnHook?.Invoke();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            OnNetworkDespawnHook?.Invoke();
        }
    }
}
#endif