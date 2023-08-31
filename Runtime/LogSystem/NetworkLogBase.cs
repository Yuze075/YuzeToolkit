using System;
using Unity.Netcode;

namespace YuzeToolkit.Utility
{
    public class NetworkLogBase : NetworkBehaviour
    {
        /// <summary>
        /// 用于统一管理打印的组, 绑定默认的context(this)和配置默认的打印<see cref="LogTags"/>
        /// </summary>
        protected LogGroup LogGroup;

        protected virtual string[] LogTags => null;

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// <see cref="Utility"/>内部实现的打印方法, 会自动绑定<see cref="LogGroup.DefaultContext"/><br/>
        /// 可以通过<see cref="LogTags"/>配置默认打印Tag
        /// </summary>
        /// <param name="message">打印的信息</param>
        /// <param name="logType">打印的类型</param>
        /// <param name="tags">打印时额外附加的Tag</param>
        protected void Log(object message, LogType logType = LogType.Log, string[] tags = null)
        {
            LogGroup.Log(message, tags, logType);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        ///  <see cref="Utility"/>内部实现的异常抛出, 会自动绑定<see cref="LogGroup.DefaultContext"/><br/>
        /// 可以通过<see cref="LogTags"/>配置默认打印Tag
        /// </summary>
        /// <param name="exception">抛出的异常</param>
        /// <param name="tags">抛出的异常时额外附加的Tag</param>
        /// <returns></returns>
        protected Exception ThrowException(Exception exception, string[] tags = null)
        {
            return LogGroup.ThrowException(exception, tags);
        }


        protected virtual void Awake()
        {
            LogGroup = new LogGroup(LogTags, this);
        }
    }
}