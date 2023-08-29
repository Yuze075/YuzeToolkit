using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 打印系统
    /// </summary>
    public static class LogSystem
    {
        /// <summary>
        /// Log类型打印打印<br/>
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="logType">打印类型</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Log(object message, LogType logType, Object context = null)
        {
            switch (logType)
            {
                case LogType.Log:
                    Log(message, context);
                    break;
                case LogType.Warning:
                    Warning(message, context);
                    break;
                case LogType.Error:
                    Error(message, context);
                    break;
                case LogType.Exception:
                    Exception(new Exception(message.ToString()), context);
                    break;
                default:
                    Log(message, context);
                    break;
            }
        }

        /// <summary>
        /// Log类型打印打印<br/>
        /// 使用<see cref="tags"/>作为默认Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <param name="logType">打印类型</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Log(object message, string[] tags, LogType logType, Object context = null)
        {
            var str = message;
            if (tags is { Length: > 0 })
            {
                str = tags.Aggregate(str, (current, tag) => current + $"\ntag: {tag}");
            }

            Log(str, logType, context);
        }

        /// <summary>
        /// 普通的Log打印, 用于打印一些展示信息<br/>
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Log(object message, Object context = null)
        {
            Debug.Log(message, context);
        }

        /// <summary>
        /// 普通的Log打印, 用于打印一些展示信息<br/>
        /// 使用<see cref="tags"/>作为默认Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Log(object message, string[] tags, Object context = null)
        {
            var str = message;
            if (tags is { Length: > 0 })
            {
                str = tags.Aggregate(str, (current, tag) => current + $"\ntag: {tag}");
            }

            Log(str, context);
        }

        /// <summary>
        /// 警告的Log打印, 用于打印一些警告信息, 提示一些危险操作<br/>
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Warning(object message, Object context = null)
        {
            Debug.LogWarning(message, context);
        }

        /// <summary>
        /// 警告的Log打印, 用于打印一些警告信息, 提示一些危险操作<br/>
        /// 使用<see cref="tags"/>作为默认Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Warning(object message, string[] tags, Object context = null)
        {
            var str = message;
            if (tags is { Length: > 0 })
            {
                str = tags.Aggregate(str, (current, tag) => current + $"\ntag: {tag}");
            }

            Warning(str, context);
        }

        /// <summary>
        /// 错误的Log打印, 用于打印一些错误信息, 如果为游戏上层内容, 可以避免抛出异常, 使用打印错误避免打断游戏进程<br/>
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Error(object message, Object context = null)
        {
            Debug.LogError(message, context);
        }

        /// <summary>
        /// 错误的Log打印, 用于打印一些错误信息, 如果为游戏上层内容, 可以避免抛出异常, 使用打印错误避免打断游戏进程<br/>
        /// 使用<see cref="tags"/>作为默认Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Error(object message, string[] tags, Object context = null)
        {
            var str = message;
            if (tags is { Length: > 0 })
            {
                str = tags.Aggregate(str, (current, tag) => current + $"\ntag: {tag}");
            }

            Error(str, context);
        }

        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// </summary>
        /// <param name="exception">异常内容</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Exception(Exception exception, Object context = null)
        {
            Debug.LogException(exception, context);
        }

        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// 使用<see cref="tags"/>作为默认Tags
        /// </summary>
        /// <param name="exception">异常内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        public static void Exception(Exception exception, string[] tags, Object context = null)
        {
            Exception(exception, context);
        }
        
        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// </summary>
        /// <param name="exception">异常内容</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        /// <returns>会返回对应异常, 此异常信息仅用于记录</returns>
        public static Exception ThrowException(Exception exception, Object context = null)
        {
            
            return exception;
        }

        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// 使用<see cref="tags"/>作为默认Tags
        /// </summary>
        /// <param name="exception">异常内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <param name="context">发送消息的<see cref="UnityEngine.Object"/>, 用于索引对象</param>
        /// <returns>会返回对应异常, 此异常信息仅用于记录</returns>
        public static Exception ThrowException(Exception exception, string[] tags, Object context = null)
        {
            return exception;
        }
    }
}