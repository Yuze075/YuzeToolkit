using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YuzeToolkit.Log
{
    /// <summary>
    /// 打印系统, 可以通过这个替代unity默认的<see cref="Debug"/><br/>
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
            Log(message.ToString(), LogType.Log, null, tags);

        public static void Log<T>(T message, Object context, params string[] tags) =>
            Log(message.ToString(), LogType.Log, context, tags);

        public static void Warning<T>(T message, params string[] tags) =>
            Log(message.ToString(), LogType.Warning, null, tags);

        public static void Warning<T>(T message, Object context, params string[] tags) =>
            Log(message.ToString(), LogType.Warning, context, tags);

        public static void Error<T>(T message, params string[] tags) =>
            Log(message.ToString(), LogType.Error, null, tags);

        public static void Error<T>(T message, Object context, params string[] tags) =>
            Log(message.ToString(), LogType.Error, context, tags);

        public static void Exception<T>(T message, params string[] tags) =>
            Log(message.ToString(), LogType.Exception, null, tags);

        public static void Exception<T>(T message, Object context, params string[] tags) =>
            Log(message.ToString(), LogType.Exception, context, tags);

        public static void Log<T>(T message, LogType logType, params string[] tags) =>
            Log(message.ToString(), logType, null, tags);

        public static void Log<T>(T message, LogType logType, Object context, params string[] tags) =>
            Log(message.ToString(), logType, context, tags);

        public static void Log(string message, params string[] tags) =>
            Log(message, LogType.Log, null, tags);

        public static void Log(string message, Object context, params string[] tags) =>
            Log(message, LogType.Log, context, tags);

        public static void Warning(string message, params string[] tags) =>
            Log(message, LogType.Warning, null, tags);

        public static void Warning(string message, Object context, params string[] tags) =>
            Log(message, LogType.Warning, context, tags);

        public static void Error(string message, params string[] tags) =>
            Log(message, LogType.Error, null, tags);

        public static void Error(string message, Object context, params string[] tags) =>
            Log(message, LogType.Error, context, tags);

        public static void Exception(string message, params string[] tags) =>
            Log(message, LogType.Exception, null, tags);

        public static void Exception(string message, Object context, params string[] tags) =>
            Log(message, LogType.Exception, context, tags);

        public static void Log(string message, LogType logType, params string[] tags) =>
            Log(message, logType, null, tags);

        public static void Log(string message, LogType logType, Object context, params string[] tags)
        {
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(message, context);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case LogType.Exception:
                    Exception(new Exception(message), context, tags);
                    break;
                case LogType.Log:
                case LogType.Assert:
                default:
                    Debug.Log(message, context);
                    break;
            }
        }

        public static void Exception(Exception exception, params string[] tags) =>
            Exception(exception, null, tags);

        public static void Exception(Exception exception, Object context, params string[] tags) =>
            Debug.LogException(exception, context);

        public static Exception ThrowException(Exception exception, params string[] tags) =>
            ThrowException(exception, null, tags);

        public static Exception ThrowException(Exception exception, Object context, params string[] tags) =>
            exception;

        internal static T[] ArrayMerge<T>(T[] arrayOne, T[] arrayTwo)
        {
            if (arrayOne == null)
            {
                return arrayTwo;
            }

            if (arrayTwo == null)
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