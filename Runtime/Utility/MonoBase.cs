using System;
using UnityEngine;
using YuzeToolkit.DriverTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit
{
    public abstract class MonoBase : MonoBehaviour, IDisposable, ILogTool, IMonoBase
    {
        public virtual OrderType Type => OrderType.Before;
        public virtual int UpdatePriority => 0;

        #region IDisposable

        /// <summary>
        /// 在OnDestroy的时候会销毁一次(也只能销毁一次
        /// </summary>
        protected DisposeGroup DisposeGroup;

        private bool _isDisposed;

        public void AddDispose(IDisposable? disposable) => DisposeGroup.Add(disposable);

        public IDisposable UnRegister(Action action) => DisposeGroup.UnRegister(action);

        protected virtual void OnDestroy()
        {
            if (!_isDisposed)
                DoDispose();
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
        public new Component? GetComponent(string type) => base.GetComponent(type);
        public new Component? GetComponent(Type type) => base.GetComponent(type);
        public new T? GetComponent<T>() => base.GetComponent<T>();

        public new Component? GetComponentInParent(Type type, bool includeInactive) =>
            base.GetComponentInParent(type, includeInactive);

        public new Component? GetComponentInParent(Type type) => base.GetComponentInParent(type);
        public new T? GetComponentInParent<T>(bool includeInactive) => base.GetComponentInParent<T>(includeInactive);
        public new T? GetComponentInParent<T>() => base.GetComponentInParent<T>();

        public new Component? GetComponentInChildren(Type type, bool includeInactive) =>
            base.GetComponentInChildren(type, includeInactive);

        public new Component? GetComponentInChildren(Type type) => base.GetComponentInChildren(type);

        public new T? GetComponentInChildren<T>(bool includeInactive) =>
            base.GetComponentInChildren<T>(includeInactive);

        public new T? GetComponentInChildren<T>() => base.GetComponentInParent<T>();
#endif

        #endregion

        #region Log

        private string[]? _logTags;
        private string[]? LogTags => _logTags ??= GetLogTags;

        protected virtual string[]? GetLogTags => null;

        public void Log<T>(T message, ELogType logType = ELogType.Log, params string[] tags) =>
            LogSys.Log(message, logType, this, LogTags.ArrayMerge(tags));

        public Exception ThrowException(Exception exception, params string[] tags) =>
            exception.ThrowException(this, LogTags.ArrayMerge(tags));

        public T IsNotNull<T>(T? isNotNull, string? name = null, string? message = null, bool additionalCheck = true) =>
            isNotNull.IsNotNull(name, message, this, additionalCheck);

        public TCastTo IsNotNull<TCastTo>(object? isNotNull, string? name = null, string? message = null,
            bool additionalCheck = false) => isNotNull.IsNotNull<TCastTo>(name, message, this, additionalCheck);

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