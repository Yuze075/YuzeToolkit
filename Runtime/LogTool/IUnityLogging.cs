#nullable enable
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.LogTool
{
    /// <summary>
    /// 打印类型的接口<br/>
    /// 四种打印类型<br/>
    /// Log: 打印各种显示信息<br/>
    /// Warning: 打印各种警告信息, 这些逻辑暂时不影响游戏运行, 但是也不应该出现<br/>
    /// Error: 打印各种错误信息, 这些逻辑已经影响到游戏运行了, 需要立刻修复<br/>
    /// Exception(ThrowException): 打印异常信息, 是一种特别的错误, 应该在现在终止游戏, 避免继续运行导致其他模块出现问题, 需要立刻修复<br/><br/>
    ///
    /// 可以通过传入<see cref="T:string[]"/> <c>tags</c>来进行标签的记录<br/>
    /// 可以通过传入<see cref="UnityEngine.Object"/> <c>context</c>来进行输出对象的记录<br/><br/>
    ///
    /// Exception和ThrowException的区别, Exception是在函数内部抛出异常, ThrowException是记录异常信息最终将异常返回但不抛出
    /// </summary>
    public interface IUnityLogging
    {
        string[]? Tags { get; }
    }

    public static class UnityLoggingExtensions
    {
#pragma warning disable CS0618 // 类型或成员已过时
        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public static void Log(this UnityEngine.Object self, object? message) =>
            LogSys.LogInternal(message, ELogType.Log, self is IUnityLogging unityLogging ? unityLogging.Tags : null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public static void LogWarning(this UnityEngine.Object self, object? message) =>
            LogSys.LogInternal(message, ELogType.Warning, self is IUnityLogging unityLogging ? unityLogging.Tags : null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public static void LogError(this UnityEngine.Object self, object? message) =>
            LogSys.LogInternal(message, ELogType.Error, self is IUnityLogging unityLogging ? unityLogging.Tags : null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void Assert(this UnityEngine.Object self, [DoesNotReturnIf(false)] bool isTrue, string? name,
            string? message) =>
            LogSys.AssertInternal(isTrue, name, message, self is IUnityLogging unityLogging ? unityLogging.Tags : null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void IsNotNull(this UnityEngine.Object self, [DoesNotReturnIf(false)] bool isTrue,
            string? name) => LogSys.AssertInternal(isTrue, name, LogSys.C_IsNull,
                self is IUnityLogging unityLogging ? unityLogging.Tags : null
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self
#endif
            );

#pragma warning restore CS0618 // 类型或成员已过时
    }
}