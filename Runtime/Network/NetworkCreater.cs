#if Netcode && NetcodeComponents && UnityTransport
using Unity.Netcode;
using UnityEngine;

namespace YuzeToolkit.Network
{
    public class NetworkCreater : NetworkBase
    {
        [SerializeField] private NetworkObject[] prefabs;
        [SerializeField] private bool autoSpawn;


        public override void OnNetworkSpawn()
        {
            if (IsServer && autoSpawn)
            {
                foreach (var prefab in prefabs)
                {
                    Instantiate(prefab).Spawn();
                }
            }
        }
    }
}
#endif