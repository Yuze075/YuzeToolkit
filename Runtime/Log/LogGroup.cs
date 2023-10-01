using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YuzeToolkit.Log
{
    /// <summary>
    /// 用于分组管理打印内容<br/>
    /// 四种打印类型&lt;br/&gt;
    /// Log: 打印各种显示信息&lt;br/&gt;
    /// Warning: 打印各种警告信息, 这些逻辑暂时不影响游戏运行, 但是也不应该出现&lt;br/&gt;
    /// Error: 打印各种错误信息, 这些逻辑已经影响到游戏运行了, 需要立刻修复&lt;br/&gt;
    /// Exception(ThrowException): 打印异常信息, 是一种特别的错误, 应该在现在终止游戏, 避免继续运行导致其他模块出现问题, 需要立刻修复&lt;br/&gt;&lt;br/&gt;
    ///
    /// 可以通过传入&lt;see cref="T:string[]"/&gt; &lt;c&gt;tags&lt;/c&gt;来进行标签的记录&lt;br/&gt;
    /// 可以通过传入&lt;see cref="UnityEngine.Object"/&gt; &lt;c&gt;context&lt;/c&gt;来进行输出对象的记录&lt;br/&gt;&lt;br/&gt;
    ///
    /// Exception和ThrowException的区别, Exception是在函数内部抛出异常, ThrowException是记录异常信息最终将异常返回但不抛出
    /// </summary>
    public class LogGroup : ILogger
    {
        public LogGroup()
        {
        }

        public LogGroup(string[] defaultTags)
        {
            DefaultTags = defaultTags;
        }

        public LogGroup(Object defaultContext)
        {
            DefaultContext = defaultContext;
        }

        public LogGroup(string[] defaultTags, Object defaultContext)
        {
            DefaultTags = defaultTags;
            DefaultContext = defaultContext;
        }

        /// <summary>
        /// 默认的打印Tag
        /// </summary>
        public string[] DefaultTags { get; set; }

        /// <summary>
        /// 默认发送消息的对象
        /// </summary>
        public Object DefaultContext { get; set; }


        public void Log<T>(T message, LogType logType = LogType.Log, params string[] tags) =>
            LogSys.Log(message.ToString(), logType, DefaultContext, LogSys.ArrayMerge(DefaultTags, tags));

        public void Log(string message, LogType logType = LogType.Log, params string[] tags) =>
            LogSys.Log(message, logType, DefaultContext, LogSys.ArrayMerge(DefaultTags, tags));

        public void Exception(Exception exception, params string[] tags) =>
            LogSys.Exception(exception, DefaultContext, LogSys.ArrayMerge(DefaultTags, tags));

        public Exception ThrowException(Exception exception, params string[] tags) =>
            LogSys.ThrowException(exception, DefaultContext, LogSys.ArrayMerge(DefaultTags, tags));
    }
}