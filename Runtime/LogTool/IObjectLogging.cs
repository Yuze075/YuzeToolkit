#nullable enable
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.LogTool
{
    public interface IObjectLogging
    {
        string[]? Tags { get; set; }

        UnityEngine.Object? Context { get; set; }
    }

    public static class ObjectLoggingExtensions
    {
#pragma warning disable CS0618 // 类型或成员已过时
        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        public static void Log(this IObjectLogging self, object? message) =>
            LogSys.LogInternal(message, ELogType.Log, self.Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self.Context
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        public static void LogWarning(this IObjectLogging self, object? message) =>
            LogSys.LogInternal(message, ELogType.Warning, self.Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self.Context
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_LOG")]
        [Conditional("YUZE_LOG_TOOL_WARNING")]
        [Conditional("YUZE_LOG_TOOL_ERROR")]
        public static void LogError(this IObjectLogging self, object? message) =>
            LogSys.LogInternal(message, ELogType.Error, self.Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self.Context
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void Assert(this IObjectLogging self, [DoesNotReturnIf(false)] bool isTrue, string? name,
            string? message) =>
            LogSys.AssertInternal(isTrue, name, message, self.Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self.Context
#endif
            );

        [UnityEngine.HideInCallstack]
        [Conditional("YUZE_LOG_TOOL_ASSERT_CHECK")]
        public static void IsNotNull(this IObjectLogging self, [DoesNotReturnIf(false)] bool isTrue,
            string? name) =>
            LogSys.AssertInternal(isTrue, name, LogSys.C_IsNull, self.Tags
#if UNITY_EDITOR && YUZE_LOG_TOOL_USE_CONTEXT
                , self.Context
#endif
            );

#pragma warning restore CS0618 // 类型或成员已过时

        public static void SetLogging(this IObjectLogging self, string[]? tags) => self.Tags = tags;

        public static void SetLogging(this IObjectLogging self, UnityEngine.Object? loggingParent)
        {
            if (loggingParent == null) return;
            if (loggingParent is IUnityLogging unityLogging) self.Tags = unityLogging.Tags;
            self.Context = loggingParent;
        }

        public static void SetLogging(this IObjectLogging self, object? mayIsParent)
        {
            if (mayIsParent == null) return;
            if (mayIsParent is IUnityLogging parent) self.Tags = parent.Tags;
            if (mayIsParent is UnityEngine.Object context) self.Context = context;
        }

        public static void SetLogging(this IObjectLogging self, string[]? tags, UnityEngine.Object? parent)
        {
            if (parent == null)
            {
                self.Tags = tags;
                return;
            }

            if (parent is IUnityLogging unityLogging) self.Tags = tags.Combine(unityLogging.Tags);
            else self.Tags = tags;
            if (parent) self.Context = parent;
        }

        public static void SetLogging(this IObjectLogging self, string[]? tags, object? mayIsParent)
        {
            if (mayIsParent == null)
            {
                self.Tags = tags;
                return;
            }

            if (mayIsParent is IUnityLogging parent) self.Tags = tags.Combine(parent.Tags);
            else self.Tags = tags;
            if (mayIsParent is UnityEngine.Object context) self.Context = context;
        }

        private static T[]? Combine<T>(this T[]? array1, T[]? array2)
        {
            if (array1 == null || array1.Length == 0)
            {
                if (array2 == null || array2.Length == 0) return null;
                return array2;
            }

            if (array2 == null || array2.Length == 0) return array1;

            var newArray = new T[array1.Length + array2.Length];
            var newArrayCount = newArray.Length;
            var array1Count = array1.Length;
            for (var index = 0; index < newArrayCount; index++)
            {
                if (index < array1Count) newArray[index] = array1[index];
                else newArray[index] = array2[index - array1Count];
            }

            return newArray;
        }
    }
}