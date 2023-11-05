using System;

namespace YuzeToolkit.LogTool
{
    /// <summary>
    /// 用于<see cref="System.Object"/>对象类型的打印类<br/>
    /// 没有绑定的<see cref="UnityEngine.Object"/>的context, 但可以通过绑定<see cref="SLogTool.Parent"/>绑定<see cref="ULogTool"/><br/>
    /// 四种打印类型<br/>
    /// Log: 打印各种显示信息<br/>
    /// Warning: 打印各种警告信息, 这些逻辑暂时不影响游戏运行, 但是也不应该出现<br/>
    /// Error: 打印各种错误信息, 这些逻辑已经影响到游戏运行了, 需要立刻修复<br/>
    /// ThrowException: 打印异常信息, 是一种特别的错误, 应该在现在终止游戏, 避免继续运行导致其他模块出现问题, 需要立刻修复<br/><br/>
    ///
    /// 可以通过传入<see cref="T:string[]"/> <c>tags</c>来进行标签的记录<br/>
    /// </summary>
    public class SLogTool : ILogTool
    {
        public SLogTool()
        {
        }

        public SLogTool(string[] defaultTags)
        {
            DefaultTags = defaultTags;
            Parent = default;
        }

        public SLogTool(ILogTool parent)
        {
            DefaultTags = default;
            Parent = parent;
        }

        public SLogTool(string[] defaultTags, ILogTool parent)
        {
            DefaultTags = defaultTags;
            Parent = parent;
        }

        /// <summary>
        /// 绑定的打印父<see cref="ILogTool"/>, 用于非<see cref="UnityEngine.Object"/>对象打印时可以获取打印源
        /// </summary>
        public ILogTool? Parent { get; set; }

        /// <summary>
        /// 默认的打印Tag
        /// </summary>
        public string[]? DefaultTags { get; set; }

        public void Log<T>(T message, ELogType logType = ELogType.Log, params string[] tags)
        {
            if (Parent != null)
                Parent.Log(message, logType, DefaultTags.ArrayMerge(tags));
            else
                LogSys.Log(message, logType, DefaultTags.ArrayMerge(tags));
        }
        public Exception ThrowException(Exception exception, params string[] tags) => Parent != null
            ? Parent.ThrowException(exception, DefaultTags.ArrayMerge(tags))
            : exception.ThrowException(DefaultTags.ArrayMerge(tags));
        public T IsNotNull<T>(T? isNotNull, string? name = null, string? message = null, bool additionalCheck = true) =>
            Parent != null
                ? Parent.IsNotNull(isNotNull, name, message, additionalCheck)
                : isNotNull.IsNotNull(name, message, null, additionalCheck);
        public TCastTo IsNotNull<TCastTo>(object? isNotNull, string? name = null, string? message = null,
            bool additionalCheck = false) => Parent != null
            ? Parent.IsNotNull<TCastTo>(isNotNull, name, message, additionalCheck)
            : isNotNull.IsNotNull<TCastTo>(name, message, null, additionalCheck);
    }
}