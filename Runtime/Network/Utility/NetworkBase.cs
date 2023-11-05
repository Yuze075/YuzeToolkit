#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit.DriverTool;
using UnityComponent = UnityEngine.Component;

namespace YuzeToolkit.Network
{
    public abstract class NetworkBase : NetworkBehaviour, IMonoBase, ILogTool, IDisposable
    {
        #region Network

        protected new NetworkObject? GetNetworkObject(ulong objectId)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var networkObject))
                return networkObject;

            Log($"无法从{nameof(NetworkSpawnManager)}中获取到Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}!",
                ELogType.Warning);
            return null;
        }

        protected NetworkObject GetNotNullNetworkObject(ulong objectId, string? name = null, string? message = null,
            bool additionalCheck = true) =>
            IsNotNull(GetNetworkObject(objectId), name, message, additionalCheck);

        protected bool TryGetNetworkObject(ulong objectId, out NetworkObject networkObject)
        {
            if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out networkObject))
                return true;

            Log($"无法从{nameof(NetworkSpawnManager)}中获取到Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}!",
                ELogType.Warning);
            return false;
        }

        protected TComponent? GetNetworkComponent<TComponent>(ulong objectId)
        {
            if (!TryGetNetworkObject(objectId, out var networkObject)) return default;

            if (networkObject.TryGetComponent<TComponent>(out var component)) return component;

            Log($"无法从Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}中获取到{typeof(TComponent).Name}的组件!",
                ELogType.Warning);
            return default;
        }

        protected TComponent GetNotNullNetworkComponent<TComponent>(ulong objectId, string? name = null,
            string? message = null, bool additionalCheck = true) =>
            IsNotNull(GetNetworkComponent<TComponent>(objectId), name, message, additionalCheck);

        protected bool TryGetNetworkComponent<TComponent>(ulong objectId, out TComponent component)
        {
            if (!TryGetNetworkObject(objectId, out var networkObject))
            {
                component = default!;
                return false;
            }

            if (networkObject.TryGetComponent(out component)) return true;

            Log($"无法从Id为{objectId}的{nameof(Unity.Netcode.NetworkObject)}中获取到{typeof(TComponent).Name}的组件!",
                ELogType.Warning);
            return default;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 在OnDestroy的时候会销毁一次(也只能销毁一次
        /// </summary>
        protected DisposeGroup DisposeGroup;

        private bool _isDisposed;

        public void AddDispose(IDisposable? disposable) => DisposeGroup.Add(disposable);

        public IDisposable UnRegister(Action action) => DisposeGroup.UnRegister(action);

        public override void OnDestroy()
        {
            if (!_isDisposed)
                DoDispose();
            base.OnDestroy();
        }

        void IDisposable.Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            Destroy(this);
            DoDispose();
        }

        protected virtual void DoDispose()
        {
            DisposeGroup.Dispose();
        }

        #endregion

        #region GetComponent

#if UNITY_EDITOR
        public new UnityComponent? GetComponent(string type) => base.GetComponent(type);
        public new UnityComponent? GetComponent(Type type) => base.GetComponent(type);
        public new T? GetComponent<T>() => base.GetComponent<T>();

        public new UnityComponent? GetComponentInParent(Type type, bool includeInactive) =>
            base.GetComponentInParent(type, includeInactive);

        public new UnityComponent? GetComponentInParent(Type type) => base.GetComponentInParent(type);
        public new T? GetComponentInParent<T>(bool includeInactive) => base.GetComponentInParent<T>(includeInactive);
        public new T? GetComponentInParent<T>() => base.GetComponentInParent<T>();

        public new UnityComponent? GetComponentInChildren(Type type, bool includeInactive) =>
            base.GetComponentInChildren(type, includeInactive);

        public new UnityComponent? GetComponentInChildren(Type type) => base.GetComponentInChildren(type);

        public new T? GetComponentInChildren<T>(bool includeInactive) =>
            base.GetComponentInChildren<T>(includeInactive);

        public new T? GetComponentInChildren<T>() => base.GetComponentInParent<T>();
#endif

        #endregion

        #region Log

        protected ULogTool? ULogTool;

        protected virtual ILogTool LogTool =>
            ULogTool ??= new ULogTool(new[] { nameof(MonoBase), GetType().FullName }, this);

        public void Log<T>(T message, ELogType logType = ELogType.Log, params string[] tags) =>
            LogTool.Log(message, logType, tags);
        public Exception ThrowException(Exception exception, params string[] tags) =>
            LogTool.ThrowException(exception, tags);
        public T IsNotNull<T>(T? isNotNull, string? name = null, string? message = null, bool additionalCheck = true) =>
            LogTool.IsNotNull(isNotNull, name, message, additionalCheck);
        public TCastTo IsNotNull<TCastTo>(object? isNotNull, string? name = null, string? message = null,
            bool additionalCheck = false) => LogTool.IsNotNull<TCastTo>(isNotNull, name, message, additionalCheck);

        #endregion

        #region CustomLog

        /// <summary>
        /// 一个大的阶段的开始
        /// </summary>
        protected void LogStart(string message)
        {
            Log($"Start----{message}----Start");
        }

        /// <summary>
        /// 一个大的阶段的结束
        /// </summary>
        protected void LogEnd(string message)
        {
            Log($"End----{message}----End");
        }

        /// <summary>
        /// 一个小阶段结束
        /// </summary>
        protected void LogComplete(string message)
        {
            Log($"Complete: {message}");
        }

        /// <summary>
        /// 创建一个重要对象
        /// </summary>
        protected void LogCreate(string message)
        {
            Log($"Create: {message}");
        }

        #endregion

        #region GetNotNullComponent

        public T GetNotNullComponent<T>(string? name = null, string? message = null, bool additionalCheck = true) =>
            IsNotNull(base.GetComponent<T>(), name, message, additionalCheck);

        public T GetNotNullComponentInParent<T>(bool includeInactive, string? name = null, string? message = null,
            bool additionalCheck = true) =>
            IsNotNull(base.GetComponentInParent<T>(includeInactive), name, message, additionalCheck);

        public T GetNotNullComponentInParent<T>(string? name = null, string? message = null,
            bool additionalCheck = true) =>
            IsNotNull(base.GetComponentInParent<T>(), name, message, additionalCheck);

        public T GetNotNullComponentInChildren<T>(bool includeInactive, string? name = null, string? message = null,
            bool additionalCheck = true) =>
            IsNotNull(base.GetComponentInChildren<T>(includeInactive), name, message, additionalCheck);

        public T GetNotNullComponentInChildren<T>(string? name = null, string? message = null,
            bool additionalCheck = true) =>
            IsNotNull(base.GetComponentInParent<T>(), name, message, additionalCheck);

        #endregion

        #region LifeCycle

        public static float DeltaTime => IMonoBase.S_DeltaTime;
        public static float FixedDeltaTime => IMonoBase.S_FixedDeltaTime;
        private IDisposable? _runDisposable;

        protected virtual void OnEnable() => _runDisposable = this.Run();

        protected virtual void OnDisable()
        {
            _runDisposable?.Dispose();
            _runDisposable = null;
        }

        #endregion
    }
}
#endif