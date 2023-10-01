﻿using System;
using UnityEngine;

namespace YuzeToolkit.Log
{
    /// <summary>
    /// 打印类型的结构体<br/>
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
    public class Logger : ILogger
    {
        public Logger()
        {
        }
        
        public Logger(string[] defaultTags)
        {
            DefaultTags = defaultTags;
            Parent = default;
        }

        public Logger(ILogger parent)
        {
            DefaultTags = default;
            Parent = parent;
        }

        public Logger(string[] defaultTags, ILogger parent)
        {
            DefaultTags = defaultTags;
            Parent = parent;
        }

        /// <summary>
        /// 绑定的打印父<see cref="ILogger"/>, 用于非<see cref="UnityEngine.Object"/>对象打印时可以获取打印源
        /// </summary>
        public ILogger Parent { get; set; }

        /// <summary>
        /// 默认的打印Tag
        /// </summary>
        public string[] DefaultTags { get; set; }

        public void Log<T>(T message, LogType logType = LogType.Log, params string[] tags)
        {
            if (Parent != null)
                Parent.Log(message.ToString(), logType, LogSys.ArrayMerge(DefaultTags, tags));
            else
                LogSys.Log(message.ToString(), logType, null, LogSys.ArrayMerge(DefaultTags, tags));
        }

        public void Log(string message, LogType logType = LogType.Log, params string[] tags)
        {
            if (Parent != null)
                Parent.Log(message.ToString(), logType, LogSys.ArrayMerge(DefaultTags, tags));
            else
                LogSys.Log(message, logType, null, LogSys.ArrayMerge(DefaultTags, tags));
        }

        public void Exception(Exception exception, params string[] tags)
        {
            if (Parent != null)
                Parent.Exception(exception, LogSys.ArrayMerge(DefaultTags, tags));
            else
                LogSys.Exception(exception, null, LogSys.ArrayMerge(DefaultTags, tags));
        }

        public Exception ThrowException(Exception exception, params string[] tags)
        {
            return Parent != null
                ? Parent.ThrowException(exception, LogSys.ArrayMerge(DefaultTags, tags))
                : LogSys.ThrowException(exception, null, LogSys.ArrayMerge(DefaultTags, tags));
        }
    }
}