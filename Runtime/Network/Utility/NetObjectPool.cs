#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network
{
    public class NetObjectPool : NetworkBehaviour
    {
        #region Static

        private static NetObjectPool? _s_instance;

        /// <summary>
        /// Server生成: 
        /// <see cref="NetObjectPool"/>通常需要在一个单独的场景中完成单例的加载和<see cref="NetworkObject"/>的<see cref="NetworkObject.Spawn"/><br/>
        /// Client生成:
        /// 在连接到服务器加载场景的时候, 和其他的网络同步对象一起同时的被加载出来<br/><br/>
        /// 
        /// 此单例在<see cref="Awake"/>的时候绑定单例, 在<see cref="OnDestroy"/>解除单例的绑定<br/>
        /// 在<see cref="NetObjectPool"/>单例存在时创建其他<see cref="NetObjectPool"/>, 会销毁新创建的<see cref="NetObjectPool"/>的<see cref="NetObjectPool.gameObject"/><br/>
        /// 在完成加载之后, 会在整个网络连接的周期中一直存在(一直作为网络的对象池, 管理对象的回收和获取)<br/><br/>
        /// </summary>
        public static NetObjectPool S_Instance
        {
            get
            {
                LogSys.IsNotNull(_s_instance != null, nameof(_s_instance));
                return _s_instance;
            }
        }

        #endregion

#if YUZE_USE_EDITOR_TOOLBOX
        [ReorderableList, ReferencePicker]
#endif
        [SerializeReference]
        private List<INetworkObjectPoolCreater> _creaters = new();

        private readonly Dictionary<GameObject, NetObjectPoolHandler> _networkObjectPools = new();
        private bool _isInit;

        #region LifeCycle

        private void Awake()
        {
            if (_s_instance != null)
            {
                this.LogError($"{nameof(_s_instance)}不为空！");
                Destroy(gameObject);
                return;
            }

            _s_instance = this;
            this.Log($"{nameof(NetObjectPool)}已经创建!");
        }

        public override void OnNetworkSpawn() => InitPool();
        public override void OnNetworkDespawn() => TempPool();

        public override void OnDestroy()
        {
            if (_s_instance != this) return;
            _s_instance = null;
            this.Log($"{nameof(NetObjectPool)}已经销毁!");
            base.OnDestroy();
        }

        #endregion

        public void InitPool()
        {
            if (_isInit) return;

            foreach (var creater in _creaters)
            {
                var netObjectPoolHandler = creater.GetNetObjectPoolHandler();
                _networkObjectPools.Add(creater.Prefab, netObjectPoolHandler);
                NetworkManager.Singleton.PrefabHandler.AddHandler(creater.Prefab, netObjectPoolHandler);
            }

            _isInit = true;
        }

        public void TempPool()
        {
            if (!_isInit) return;

            foreach (var (obj, objectPool) in _networkObjectPools)
            {
                objectPool.Clear();
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(obj);
            }

            _networkObjectPools.Clear();
            _isInit = false;
        }

        #region Get & Release

        public NetworkObject? GetNetworkObject(GameObject prefab,
            ulong clientId = NetworkManager.ServerClientId, Vector3 position = default,
            Quaternion rotation = default)
            => _networkObjectPools.TryGetValue(prefab, out var netObjectPoolHandler)
                ? netObjectPoolHandler.Instantiate(clientId, position, rotation)
                : null;

        public bool TryGetNetworkObject(GameObject prefab,
            [MaybeNullWhen(false)] out NetworkObject networkObject, ulong clientId = NetworkManager.ServerClientId,
            Vector3 position = default, Quaternion rotation = default)
        {
            if (_networkObjectPools.TryGetValue(prefab, out var netObjectPoolHandler))
            {
                networkObject = netObjectPoolHandler.Instantiate(clientId, position, rotation);
                return true;
            }

            networkObject = null;
            return false;
        }

        public void ReleaseNetworkObject(GameObject prefab, NetworkObject networkObject)
        {
            if (_networkObjectPools.TryGetValue(prefab, out var netObjectPoolHandler))
            {
                netObjectPoolHandler.Destroy(networkObject);
                return;
            }

            this.LogWarning($"无法找到{prefab}的{nameof(NetObjectPoolHandler)}!");
            Destroy(networkObject.gameObject);
        }

        #endregion

        #region UNITY_EDITOR

#if UNITY_EDITOR
        private void OnValidate()
        {
            for (var index = 0; index < _creaters.Count; index++)
            {
                var creater = _creaters[index];
                if (creater == null || creater.Prefab.TryGetComponent<NetworkObject>(out _)) continue;

                this.LogWarning(
                    $"{nameof(NetObjectPool)}: index为{index}的{creater.Prefab.name}没有{nameof(NetworkObject)}组件!");
            }
        }
#endif

        #endregion
    }

    public interface INetworkObjectPoolCreater
    {
        GameObject Prefab { get; }
        NetObjectPoolHandler GetNetObjectPoolHandler();
    }

    [Serializable]
    public class BaseNetworkObjectPoolCreater : INetworkObjectPoolCreater
    {
        [SerializeField] private GameObject? prefab;
        [SerializeField] private int defaultCreateCount;

        public GameObject Prefab
        {
            get
            {
                LogSys.IsNotNull(prefab != null, nameof(prefab));
                return prefab;
            }
        }


        private NetworkObject NetworkObjectPrefab
        {
            get
            {
                var networkObjectPrefab = Prefab.GetComponent<NetworkObject>();
                LogSys.IsNotNull(networkObjectPrefab != null, nameof(networkObjectPrefab));
                return networkObjectPrefab;
            }
        }

        public NetObjectPoolHandler GetNetObjectPoolHandler()
        {
            var objectPool = new NetObjectPoolHandler(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy);
            if (defaultCreateCount <= 0) return objectPool;

            var array = new NetworkObject[defaultCreateCount];
            for (var i = 0; i < defaultCreateCount; i++) array[i] = objectPool.Get();
            for (var i = 0; i < defaultCreateCount; i++) objectPool.Release(array[i]);
            return objectPool;
        }

        private NetworkObject CreateFunc() => UnityEngine.Object.Instantiate(NetworkObjectPrefab);
        private static void ActionOnGet(NetworkObject obj) => obj.gameObject.SetActive(true);
        private static void ActionOnRelease(NetworkObject obj) => obj.gameObject.SetActive(false);
        private static void ActionOnDestroy(NetworkObject obj) => UnityEngine.Object.Destroy(obj.gameObject);
    }
}
#endif