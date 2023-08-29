using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 用于分组管理打印内容
    /// </summary>
    public class LogGroup
    {
        public LogGroup()
        {
        }

        public LogGroup(string[] defaultTags)
        {
            this.DefaultTags = defaultTags;
        }

        public LogGroup(Object defaultContext)
        {
            this.DefaultContext = defaultContext;
        }

        public LogGroup(string[] defaultTags, Object defaultContext)
        {
            this.DefaultTags = defaultTags;
            this.DefaultContext = defaultContext;
        }

        /// <summary>
        /// 默认的打印Tag
        /// </summary>
        public string[] DefaultTags { get; set; }

        /// <summary>
        /// 默认发送消息的对象
        /// </summary>
        public Object DefaultContext { get; set; }

        /// <summary>
        /// Log类型打印打印<br/>
        /// 使用<see cref="LogGroup"/>的<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="logType">打印类型</param>
        public void Log(object message, LogType logType)
        {
            LogSystem.Log(message, DefaultTags, logType, DefaultContext);
        }

        /// <summary>
        /// Log类型打印打印<br/>
        /// 使用<see cref="tags"/>和<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <param name="logType">打印类型</param>
        public void Log(object message, string[] tags, LogType logType)
        {
            LogSystem.Log(message, ArrayMerge(DefaultTags, tags), logType, DefaultContext);
        }

        /// <summary>
        /// 普通的Log打印, 用于打印一些展示信息<br/>
        /// 使用<see cref="LogGroup"/>的<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        public void Log(object message)
        {
            LogSystem.Log(message, DefaultTags, DefaultContext);
        }

        /// <summary>
        /// 普通的Log打印, 用于打印一些展示信息<br/>
        /// 使用<see cref="tags"/>和<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        public void Log(object message, string[] tags)
        {
            LogSystem.Log(message, ArrayMerge(DefaultTags, tags), DefaultContext);
        }

        /// <summary>
        /// 警告的Log打印, 用于打印一些警告信息, 提示一些危险操作<br/>
        /// 使用<see cref="LogGroup"/>的<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        public void Warning(object message)
        {
            LogSystem.Warning(message, DefaultTags, DefaultContext);
        }

        /// <summary>
        /// 警告的Log打印, 用于打印一些警告信息, 提示一些危险操作<br/>
        /// 使用<see cref="tags"/>和<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        public void Warning(object message, string[] tags)
        {
            LogSystem.Warning(message, ArrayMerge(DefaultTags, tags), DefaultContext);
        }

        /// <summary>
        /// 错误的Log打印, 用于打印一些错误信息, 如果为游戏上层内容, 可以避免抛出异常, 使用打印错误避免打断游戏进程<br/>
        /// 使用<see cref="LogGroup"/>的<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        public void Error(object message)
        {
            LogSystem.Error(message, DefaultTags, DefaultContext);
        }

        /// <summary>
        /// 错误的Log打印, 用于打印一些错误信息, 如果为游戏上层内容, 可以避免抛出异常, 使用打印错误避免打断游戏进程<br/>
        /// 使用<see cref="tags"/>和<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="message">打印内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        public void Error(object message, string[] tags)
        {
            LogSystem.Error(message, ArrayMerge(DefaultTags, tags), DefaultContext);
        }

        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// 使用<see cref="LogGroup"/>的<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="exception">异常内容</param>
        public void Exception(Exception exception)
        {
            LogSystem.Exception(exception, DefaultTags, DefaultContext);
        }

        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// 使用<see cref="tags"/>和<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="exception">异常内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        public void Exception(Exception exception, string[] tags)
        {
            LogSystem.Exception(exception, ArrayMerge(DefaultTags, tags), DefaultContext);
        }

        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// 使用<see cref="LogGroup"/>的<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="exception">异常内容</param>
        /// <returns>会返回对应异常, 此异常信息仅用于记录</returns>
        public Exception ThrowException(Exception exception)
        {
            return LogSystem.ThrowException(exception, DefaultTags, DefaultContext);
        }

        /// <summary>
        /// 异常的内容抛出, 当继续进行会影响游戏整体逻辑, 并且会出现不可控制的问题时, 抛出异常<br/>
        /// 使用<see cref="tags"/>和<see cref="DefaultTags"/>作为Tags
        /// </summary>
        /// <param name="exception">异常内容</param>
        /// <param name="tags">传入的tag, 用于标记信息类型</param>
        /// <returns>会返回对应异常, 此异常信息仅用于记录</returns>
        public Exception ThrowException(Exception exception, string[] tags)
        {
            return LogSystem.ThrowException(exception, ArrayMerge(DefaultTags, tags), DefaultContext);
        }

        private static T[] ArrayMerge<T>(T[] arrayOne, T[] arrayTwo)
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