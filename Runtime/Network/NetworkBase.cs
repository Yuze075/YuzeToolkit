#if Netcode && NetcodeComponents && UnityTransport
using System;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.Log;
using YuzeToolkit.MonoDriver;
using YuzeToolkit.Utility;
using ILogger = YuzeToolkit.Log.ILogger;

namespace YuzeToolkit.Network
{
    public abstract class NetworkBase : NetworkBehaviour, IMonoBase, ILogger
    {
        #region Network

        protected new NetworkObject GetNetworkObject(ulong objectId)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var networkObject))
                return networkObject;

            Log($"无法从{nameof(NetworkSpawnManager)}中获取到Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}!",
                LogType.Warning);
            return null;
        }

        protected bool TryGetNetworkObject(ulong objectId, out NetworkObject networkObject)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out networkObject))
                return true;

            Log($"无法从{nameof(NetworkSpawnManager)}中获取到Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}!",
                LogType.Warning);
            return false;
        }

        protected TComponent GetNetworkObjectAndComponent<TComponent>(ulong objectId)
        {
            if (!TryGetNetworkObject(objectId, out var networkObject)) return default;

            if (networkObject.TryGetComponent<TComponent>(out var component)) return component;

            Log($"无法从Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}中获取到{typeof(TComponent).Name}的组件!",
                LogType.Warning);
            return default;
        }

        protected bool TryGetNetworkObjectAndComponent<TComponent>(ulong objectId, out TComponent component)
        {
            if (!TryGetNetworkObject(objectId, out var networkObject))
            {
                component = default;
                return false;
            }

            if (networkObject.TryGetComponent(out component)) return true;

            Log($"无法从Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}中获取到{typeof(TComponent).Name}的组件!",
                LogType.Warning);
            return default;
        }

        #endregion

        #region LifeCycle

        public static float DeltaTime => IMonoBase.S_DeltaTime;
        public static float FixedDeltaTime => IMonoBase.S_FixedDeltaTime;
        private IDisposable _disposable;

        protected virtual void Awake()
        {
            LogGroup.DefaultTags = LogTags;
            LogGroup.DefaultContext = this;
        }

        protected virtual void OnEnable()
        {
            _disposable = this.Run();
        }

        protected virtual void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            DisposeGroup.Dispose();
        }

        #endregion

        #region Log

        /// <summary>
        /// 用于统一管理打印的组, 绑定默认的context(this)和配置默认的打印<see cref="LogTags"/>
        /// </summary>
        protected LogGroup LogGroup { get; } = new();

        protected virtual string[] LogTags => null;
        public new void print(object message) => LogGroup.Log(message.ToString(), LogType.Log);
        public void print<T>(T message) => LogGroup.Log(message.ToString(), LogType.Log);
        public void print(string message) => LogGroup.Log(message, LogType.Log);

        public void Log<T>(T message, LogType logType = LogType.Log, params string[] tags) =>
            LogGroup.Log(message.ToString(), logType, tags);

        public void Log(string message, LogType logType = LogType.Log, params string[] tags) =>
            LogGroup.Log(message, logType, tags);

        public void Exception(Exception exception, params string[] tags)
            => LogGroup.Exception(exception, tags);

        public Exception ThrowException(Exception exception, params string[] tags) =>
            LogGroup.ThrowException(exception, tags);

        #endregion

        #region IDisposable

        /// <summary>
        /// 在OnDestroy的时候会销毁一次(也只能销毁一次
        /// </summary>
        protected DisposeGroup DisposeGroup { get; } = new();

        public void AddDispose(IDisposable disposable) => DisposeGroup.Add(disposable);

        #endregion
    }
}
#endif