#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit
{
    public abstract class SoBase : ScriptableObject, ILogging
    {
        #region Log

        private string[]? _tags;
        public string[] Tags => _tags ??= GetLogTags;
        protected virtual string[] GetLogTags => Array.Empty<string>();

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
    }
}