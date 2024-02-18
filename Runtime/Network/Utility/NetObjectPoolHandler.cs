#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.Network
{
    public class NetObjectPoolHandler : ObjectPool<NetworkObject>, INetworkPrefabInstanceHandler
    {
        public NetObjectPoolHandler(Func<NetworkObject> createFunc, Action<NetworkObject>? actionOnGet = null,
            Action<NetworkObject>? actionOnRelease = null, Action<NetworkObject>? actionOnDestroy = null,
            Action<NetworkObject, ulong, UnityEngine.Vector3, UnityEngine.Quaternion>? actionOnNetGet = null,
            Action<NetworkObject>? actionOnNetRelease = null, bool collectionCheck = true, int defaultCapacity = 10,
            int maxSize = 1000) :
            base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {
            _actionOnNetGet = actionOnNetGet;
            _actionOnNetRelease = actionOnNetRelease;
        }

        private readonly Action<NetworkObject, ulong, UnityEngine.Vector3, UnityEngine.Quaternion>? _actionOnNetGet;
        private readonly Action<NetworkObject>? _actionOnNetRelease;

        public NetworkObject Instantiate(ulong ownerClientId, UnityEngine.Vector3 position,
            UnityEngine.Quaternion rotation)
        {
            var networkObject = Get();
            var tarn = networkObject.transform;
            tarn.position = position;
            tarn.rotation = rotation;

            _actionOnNetGet?.Invoke(networkObject, ownerClientId, position, rotation);

            return networkObject;
        }

        public void Destroy(NetworkObject networkObject)
        {
            _actionOnNetRelease?.Invoke(networkObject);
            Release(networkObject);
        }
    }
}
#endif