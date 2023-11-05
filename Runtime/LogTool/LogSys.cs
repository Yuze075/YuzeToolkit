using System;
using System.Collections;
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
    /// Exception(ThrowException): 打印异常信息, 是一种特别的错误, 应该在现在终止游戏, 避免继续运行导致其他模块出现问题, 需要立刻修复<br/><br/>
    ///
    /// 可以通过传入<see cref="T:string[]"/> <c>tags</c>来进行标签的记录<br/>
    /// 可以通过传入<see cref="UnityEngine.Object"/> <c>context</c>来进行输出对象的记录<br/><br/>
    ///
    /// Exception和ThrowException的区别, Exception是在函数内部抛出异常, ThrowException是记录异常信息最终将异常返回但不抛出
    /// </summary>
    public static class LogSys
    {
        public static void Log<T>(T message, params string[] tags) =>
            Log(message, ELogType.Log, null, tags);

        public static void Log<T>(T message, UnityObject? context, params string[] tags) =>
            Log(message, ELogType.Log, context, tags);

        public static void Warning<T>(T message, params string[] tags) =>
            Log(message, ELogType.Warning, null, tags);

        public static void Warning<T>(T message, UnityObject? context, params string[] tags) =>
            Log(message, ELogType.Warning, context, tags);

        public static void Error<T>(T message, params string[] tags) =>
            Log(message, ELogType.Error, null, tags);

        public static void Error<T>(T message, UnityObject? context, params string[] tags) =>
            Log(message, ELogType.Error, context, tags);

        public static void Exception<T>(T message, params string[] tags) =>
            Log(message, ELogType.Exception, null, tags);

        public static void Exception<T>(T message, UnityObject? context, params string[] tags) =>
            Log(message, ELogType.Exception, context, tags);

        public static void Log<T>(T message, ELogType logType, params string[] tags) =>
            Log(message, logType, null, tags);

        public static void Log<T>(T message, ELogType logType, UnityObject? context, params string[] tags)
        {
            var strMessage = message == null ? string.Empty : message.ToString();
            switch (logType)
            {
                case ELogType.Error:
                    UnityDebug.LogError(strMessage, context);
                    break;
                case ELogType.Warning:
                    UnityDebug.LogWarning(strMessage, context);
                    break;
                case ELogType.Exception:
                    UnityDebug.LogException(new Exception(strMessage), context);
                    break;
                case ELogType.Log:
                default:
                    UnityDebug.Log(strMessage, context);
                    break;
            }
        }

        public static Exception ThrowException(this Exception exception, params string[] tags) =>
            ThrowException(exception, null, tags);

        public static Exception ThrowException(this Exception exception, UnityObject? context, params string[] tags) =>
            exception;

        public static Exception ThrowWithLogTool(this Exception exception, ILogTool? logTool, params string[] tags) =>
            logTool == null ? ThrowException(exception, tags) : logTool.ThrowException(exception, tags);

        #region Assert

        private static string? Name(string? name) => name != null ? $"[{nameof(name)}]: {name}, " : null;
        private static string? Type(Type? type) => type != null ? $"[{nameof(type)}]: {type}, " : null;
        private static string? Message(string? message) => message != null ? $"[{nameof(message)}]: {message}, " : null;

        private static string? CheckInfo(string? checkInfo) =>
            checkInfo != null ? $"[{nameof(checkInfo)}]: {checkInfo}, " : null;

        // ReSharper disable Unity.PerformanceAnalysis
        public static T IsNotNull<T>(this T? isNotNull, string? name = null, string? message = null,
            UnityObject? context = null, bool additionalCheck = true)
        {
#if UNITY_EDITOR
            if (isNotNull == null)
                UnityDebug.LogException(
                    new NullReferenceException($"{Name(name)}{Type(typeof(T))}{Message(message)}对应对象为空!"), context);
            if (additionalCheck)
                switch (isNotNull)
                {
                    case string str when string.IsNullOrWhiteSpace(str):
                        UnityDebug.LogException(new NullReferenceException(
                            $"{Name(name)}{Type(typeof(string))}{Message(message)}对应字符串为空或者空白!"), context);
                        break;
                    case ICollection { Count: 0 }:
                        UnityDebug.LogException(
                            new NullReferenceException($"{Name(name)}{Type(typeof(T))}{Message(message)}对应数组对象长度为0!"),
                            context);
                        break;
                    case IAdditionalCheck check when check.DoAdditionalCheck(out var checkInfo):
                        UnityDebug.LogException(new NullReferenceException(
                                $"{Name(name)}{Type(typeof(T))}{Message(message)}{CheckInfo(checkInfo)}对应对象未通过自身检查!"),
                            context);
                        break;
                }
#endif
            return isNotNull!;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public static TCastTo IsNotNull<TCastTo>(this object? isNotNull, string? name = null, string? message = null,
            UnityObject? context = null, bool additionalCheck = false)
        {
            if (isNotNull is TCastTo castTo) return castTo.IsNotNull(name, message, context, additionalCheck);

#if UNITY_EDITOR
            UnityDebug.LogException(new NullReferenceException(
                $"{Name(name)}{isNotNull!.GetType()}{Message(message)}无法转化到{typeof(TCastTo)}类型!"), context);
#endif
            return default!;
        }

        public static TCastTo IsNotNull<TCastTo>(this object? isNotNull, ILogTool? logTool, string? name = null,
            string? message = null, bool additionalCheck = false)
            => logTool == null
                ? IsNotNull<TCastTo>(isNotNull, name, message, additionalCheck: additionalCheck)
                : logTool.IsNotNull<TCastTo>(isNotNull, name, message, additionalCheck);

        public static T IsNotNull<T>(this T? isNotNull, ILogTool? logTool, string? name = null, string? message = null,
            bool additionalCheck = true) =>
            logTool == null
                ? IsNotNull(isNotNull, name, message, additionalCheck: additionalCheck)
                : logTool.IsNotNull(isNotNull, name, message, additionalCheck);

        #endregion

        public static T[] ArrayMerge<T>(this T[]? arrayOne, T[]? arrayTwo)
        {
            if (arrayOne == null || arrayOne.Length == 0)
            {
                return arrayTwo ?? Array.Empty<T>();
            }

            if (arrayTwo == null || arrayTwo.Length == 0)
            {
                return arrayOne;
            }

            var newArray = new T[arrayOne.Length + arrayTwo.Length];

            for (var index = 0; index < newArray.Length; index++)
            {
                if (index < arrayOne.Length)
                {
                    newArray[index] = arrayOne[index];
                }
                else
                {
                    newArray[index] = arrayTwo[index - arrayOne.Length];
                }
            }

            return newArray;
        }
    }
}