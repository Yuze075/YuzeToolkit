#nullable enable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.UITool
{
    /// <summary>
    /// 用于空返回值的<see cref="ICommand{TResult}"/>
    /// </summary>
    public struct Empty
    {
    }

    /// <summary>
    /// 命令接口的接口，可以封装各种命令，用于调用和处理逻辑（此命令带有返回值）<br/>
    /// 通常命令用于处理相关<see cref="IUIController"/>到<see cref="IUIModel"/>的各种操控逻辑
    /// </summary>
    public interface ICommand<out TResult> : ICanSendEvent, ICanSendCommand, ICanGetNotNullModel, ICanGetNotNullUtility
    {
        TResult Execute();
    }

    /// <inheritdoc/>
    public abstract class CommandBase : CommandBase<Empty>
    {
        public sealed override Empty Execute()
        {
            OnExecute();
            return default;
        }

        protected abstract void OnExecute();
    }

    /// <inheritdoc/>
    public abstract class CommandBase<TResult> : ICommand<TResult>
    {
        private IUICore? _core;
        private ILogging? _logging;
        IUICore IBelongUICore.Core
        {
            get
            {
                LogSys.IsNotNull(_core != null, nameof(_core));
                return _core;
            }
            set
            {
                _core = value;
                _logging = _core as ILogging;
            }
        }

        public abstract TResult Execute();
        
#pragma warning disable CS0618 // 类型或成员已过时
        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public void Log(object? message)
        {
            if (_logging != null) _logging.Log(message, ELogType.Log);
            else LogSys.Log(message);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public void Log(object? message, string[]? tags)
        {
            if (_logging != null) _logging.Log(message, ELogType.Log, tags);
            LogSys.Log(message, tags);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public void LogWarning(object? message)
        {
            if (_logging != null) _logging.Log(message, ELogType.Warning);
            LogSys.LogWarning(message);
        }


        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public void LogWarning(object? message, string[]? tags)
        {
            if (_logging != null) _logging.Log(message, ELogType.Warning, tags);
            LogSys.LogWarning(message, tags);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public void LogError(object? message)
        {
            if (_logging != null) _logging.Log(message, ELogType.Warning);
            LogSys.LogError(message);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public void LogError(object? message, string[]? tags)
        {
            if (_logging != null) _logging.Log(message, ELogType.Warning, tags);
            LogSys.LogError(message, tags);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void Assert([DoesNotReturnIf(false)] bool isTrue, string? name, string? message = null,
            string[]? tags = null)
        {
            if (_logging != null) _logging.Assert(isTrue, name, message);
            LogSys.Assert(isTrue, name, message, tags);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void IsNotNull([DoesNotReturnIf(false)] bool isTrue, string? name)
        {
            if (_logging != null) _logging.Assert(isTrue, name, LogSys.C_IsNull);
            LogSys.Assert(isTrue, name, LogSys.C_IsNull);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public void IsNotNull([DoesNotReturnIf(false)] bool isTrue, string? name, string[]? tags)
        {
            if (_logging != null) _logging.Assert(isTrue, name, LogSys.C_IsNull, tags);
            LogSys.Assert(isTrue, name, LogSys.C_IsNull, tags);
        }
#pragma warning restore CS0618 // 类型或成员已过时
    }
}