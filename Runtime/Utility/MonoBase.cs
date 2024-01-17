#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.DriverTool;
using YuzeToolkit.LogTool;
using UnityComponent = UnityEngine.Component;

namespace YuzeToolkit
{
    public abstract class MonoBase : MonoBehaviour, ILogging, IMonoBase
    {
        public virtual EOrderType Type => EOrderType.Before;
        public virtual int UpdatePriority => 0;

        #region IDisposable

        /// <summary>
        /// 在OnDestroy的时候会销毁一次(也只能销毁一次
        /// </summary>
        private DisposeGroup _disposeGroup;

        private bool _isDisposed;

        public void AddDispose(IDisposable? disposable) => _disposeGroup.Add(disposable);

        protected virtual void OnDestroy()
        {
            _disposeGroup.Dispose();
        }

        #endregion

        #region GetComponent

#if UNITY_EDITOR
        public new UnityComponent? GetComponent(string type) => base.GetComponent(type);
        public new UnityComponent? GetComponent(Type type) => base.GetComponent(type);
        public new T? GetComponent<T>() => base.GetComponent<T>();

        public new bool TryGetComponent<T>([MaybeNullWhen(false)] out T value) => base.TryGetComponent(out value);

        public new bool TryGetComponent(Type type, [MaybeNullWhen(false)] out UnityComponent value) =>
            base.TryGetComponent(type, out value);

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

        public new T? GetComponentInChildren<T>() => base.GetComponentInChildren<T>();
#endif

        #endregion

        #region Log

        private string[]? _tags;
        public string[] Tags => _tags ??= GetLogTags;
        protected virtual string[] GetLogTags => Array.Empty<string>();

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        // ReSharper disable once InconsistentNaming
        public new void print(object message) => Log(message);

#pragma warning disable CS0618 // 类型或成员已过时
        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        void ILogging.Log(object? message, ELogType logType, string[]? tags) =>
            LogSys.LogInternal(message, logType, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        void ILogging.Assert([DoesNotReturnIf(false)] bool isTrue,
            string? name, string? message, string[]? tags) =>
            LogSys.AssertInternal(isTrue, name, message, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public void Log(object? message) => LogSys.LogInternal(message, ELogType.Log, Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            , this
#endif
        );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public void Log(object? message, string[]? tags) =>
            LogSys.LogInternal(message, ELogType.Log, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public void LogWarning(object? message) =>
            LogSys.LogInternal(message, ELogType.Warning, Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public void LogWarning(object? message, string[]? tags) =>
            LogSys.LogInternal(message, ELogType.Warning, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public void LogError(object? message) =>
            LogSys.LogInternal(message, ELogType.Error, Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public void LogError(object? message, string[]? tags) =>
            LogSys.LogInternal(message, ELogType.Error, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void Assert([DoesNotReturnIf(false)] bool isTrue,
            string? name, string? message) =>
            LogSys.AssertInternal(isTrue, name, message, Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void Assert([DoesNotReturnIf(false)] bool isTrue,
            string? name, string? message, string[]? tags) =>
            LogSys.AssertInternal(isTrue, name, message, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );


        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void IsNotNull([DoesNotReturnIf(false)] bool isTrue, string? name) =>
            LogSys.AssertInternal(isTrue, name, LogSys.C_IsNull, Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void IsNotNull([DoesNotReturnIf(false)] bool isTrue, string? name, string[]? tags) =>
            LogSys.AssertInternal(isTrue, name, LogSys.C_IsNull, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , this
#endif
            );
#pragma warning restore CS0618 // 类型或成员已过时

        #endregion

        #region LifeCycle

        public static float DeltaTime => IMonoBase.S_DeltaTime;
        public static float FixedDeltaTime => IMonoBase.S_FixedDeltaTime;
        private IDisposable? _runDisposable;

        private UpdateToken _updateToken;
        protected virtual void OnEnable() => _updateToken = this.Run();

        protected virtual void OnDisable() => _updateToken.Dispose();

        #endregion
    }
}