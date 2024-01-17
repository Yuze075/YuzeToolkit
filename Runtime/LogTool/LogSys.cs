#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityDebug = UnityEngine.Debug;
using UnityObject = UnityEngine.Object;

namespace YuzeToolkit.LogTool
{
    /// <summary>
    /// 打印系统, 可以通过这个替代unity默认的<see cref="UnityEngine.Debug"/><br/>
    /// 四种打印类型<br/>
    /// Log: 打印各种显示信息<br/>
    /// Warning: 打印各种警告信息, 这些逻辑暂时不影响游戏运行, 但是也不应该出现<br/>
    /// Error: 打印各种错误信息, 这些逻辑已经影响到游戏运行了, 需要立刻修复<br/>
    /// Assert: 断言检测, 如果检测失败, 则会抛出异常(用于处理一些在编辑器中需要检测处理的任务, 例如是否为空)<br/><br/>
    ///
    /// 可以通过传入<see cref="T:string[]"/> <c>tags</c>来进行标签的记录<br/><br/>
    /// </summary>
    public static class LogSys
    {
        public const string C_IsNull = "对象数据为空!";

        #region Internal

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        [Obsolete("内部调用的方法, 请使用默认的Log方法打印日志!")]
        public static void LogInternal(object? message, ELogType logType, string[]? tags = null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            , UnityObject? context = null
#endif
        )
        {
#if YUZE_LOG_TOOL_LOG || YUZE_LOG_TOOL_WARNING || YUZE_LOG_TOOL_ERROR
            switch (logType)
            {
                case ELogType.Error:
                    UnityDebug.LogError(message
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                        , context
#endif
                    );
                    break;

#if YUZE_LOG_TOOL_LOG || YUZE_LOG_TOOL_WARNING
                case ELogType.Warning:
                    UnityDebug.LogWarning(message
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                        , context
#endif
                    );
                    break;
#endif

#if YUZE_LOG_TOOL_LOG
                case ELogType.Log:
                default:
                    UnityDebug.Log(message
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                        , context
#endif
                    );
                    break;
#endif
            }
#endif
        }


        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        [Obsolete("内部调用的方法, 请使用默认的Assert方法进行断言判断!")]
        public static void AssertInternal([DoesNotReturnIf(false)] bool isTrue, string? name, string? message,
            string[]? tags = null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            , UnityObject? context = null
#endif
        )
        {
            if (isTrue) return;
            UnityDebug.LogException(new Exception($"{(name == null ? null : $"[Name: {name}]")}" +
                                                  $"{(message == null ? null : $"[Message: {message}]")}")
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , context
#endif
            );
        }

        #endregion

        #region Log

#pragma warning disable CS0618 // 类型或成员已过时
        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public static void Log(object? message) => LogInternal(message, ELogType.Log);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public static void Log(object? message, string[]? tags) => LogInternal(message, ELogType.Log, tags);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public static void LogWarning(object? message) => LogInternal(message, ELogType.Warning);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public static void LogWarning(object? message, string[]? tags) => LogInternal(message, ELogType.Warning, tags);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public static void LogError(object? message) => LogInternal(message, ELogType.Error);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public static void LogError(object? message, string[]? tags) => LogInternal(message, ELogType.Error, tags);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void Assert([DoesNotReturnIf(false)] bool isTrue, string? name, string? message) =>
            AssertInternal(isTrue, name, message);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void Assert([DoesNotReturnIf(false)] bool isTrue, string? name, string? message,
            string[]? tags) => AssertInternal(isTrue, name, message, tags);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void IsNotNull([DoesNotReturnIf(false)] bool isTrue, string? name) =>
            AssertInternal(isTrue, name, C_IsNull);

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void IsNotNull([DoesNotReturnIf(false)] bool isTrue, string? name, string[]? tags) =>
            AssertInternal(isTrue, name, C_IsNull, tags);
#pragma warning restore CS0618 // 类型或成员已过时

        #endregion
    }
}