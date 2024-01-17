#nullable enable
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace YuzeToolkit.LogTool
{
    /// <summary>
    /// 用于<see cref="System.Object"/>对象类型的打印类<br/>
    /// 四种打印类型<br/>
    /// Log: 打印各种显示信息<br/>
    /// Warning: 打印各种警告信息, 这些逻辑暂时不影响游戏运行, 但是也不应该出现<br/>
    /// Error: 打印各种错误信息, 这些逻辑已经影响到游戏运行了, 需要立刻修复<br/>
    ///
    /// 可以通过传入<see cref="T:string[]"/> <c>tags</c>来进行标签的记录<br/>
    /// </summary>
    public readonly struct Logging : ILogging
    {
        #region Logging

        public Logging(string[]? tags)
        {
            _tags = tags;
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            _context = null;
#endif
        }

        public Logging(Logging parent)
        {
            _tags = parent._tags;
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            _context = parent._context;
#endif
        }

        public Logging(string[]? tags, Logging parent)
        {
            _tags = parent._tags.Combine(tags);
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            _context = parent._context;
#endif
        }

        public Logging(ILogging? parent)
        {
            if (parent == null)
            {
                _tags = null;
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                _context = null;
#endif
                return;
            }

            _tags = parent.Tags;
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            _context = parent switch
            {
                UnityObject obj => obj,
                Logging logging => logging._context,
                _ => null
            };
#endif
        }

        public Logging(string[]? tags, ILogging? parent)
        {
            if (parent == null)
            {
                _tags = tags;
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                _context = null;
#endif
                return;
            }

            _tags = parent.Tags.Combine(tags);
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            _context = parent switch
            {
                UnityObject obj => obj,
                Logging logging => logging._context,
                _ => null
            };
#endif
        }

        public Logging(UnityObject? context, string[]? tags)
        {
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
            _context = context;
#endif
            _tags = tags;
        }

        #endregion

#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
        private readonly UnityObject? _context;
#endif
        private readonly string[]? _tags;
        public string[]? Tags => _tags;

        #region ILogging

#pragma warning disable CS0618 // 类型或成员已过时
        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        void ILogging.Log(object? message, ELogType logType, string[]? tags) =>
            LogSys.LogInternal(message, logType, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        void ILogging.Assert([DoesNotReturnIf(false)] bool isTrue,
            string? name, string? message, string[]? tags) =>
            LogSys.AssertInternal(isTrue, name, message, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public void Log(object? message, string[]? tags = null) =>
            LogSys.LogInternal(message, ELogType.Log, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public void LogWarning(object? message, string[]? tags = null) =>
            LogSys.LogInternal(message, ELogType.Warning, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public void LogError(object? message, string[]? tags = null) =>
            LogSys.LogInternal(message, ELogType.Error, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );
        
        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void Assert([DoesNotReturnIf(false)] bool isTrue, string? name, string? message) =>
            LogSys.AssertInternal(isTrue, name, message, Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void Assert([DoesNotReturnIf(false)] bool isTrue, string? name, string? message,
            string[]? tags) =>
            LogSys.AssertInternal(isTrue, name, message, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void IsNotNull([DoesNotReturnIf(false)] bool isTrue, string? name = null,
            object? value = null, string[]? tags = null) =>
            LogSys.AssertInternal(isTrue, name, LogSys.C_IsNull, Tags.Combine(tags)
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , _context
#endif
            );
#pragma warning restore CS0618 // 类型或成员已过时

        #endregion
    }
}