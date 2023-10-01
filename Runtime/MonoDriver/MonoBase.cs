﻿using System;
using UnityEngine;
using YuzeToolkit.Log;
using YuzeToolkit.Utility;
using ILogger = YuzeToolkit.Log.ILogger;

namespace YuzeToolkit.MonoDriver
{
    /// <summary>
    /// 由<see cref="MonoDriverBase"/>驱动更新的<see cref="MonoBehaviour"/><br/>
    /// 可以设置更新优先级<see cref="IMonoBase.Priority"/>和在unity中的更新优先级<see cref="IMonoBase.Type"/>
    /// </summary>
    public abstract class MonoBase : MonoBehaviour, IMonoBase, ILogger
    {
        #region LifeCycle

        public static float DeltaTime => IMonoBase.S_DeltaTime;
        public static float FixedDeltaTime => IMonoBase.S_FixedDeltaTime;
        private IDisposable _disposable;

        protected virtual void Awake()
        {
            LogGroup.DefaultTags = LogTags;
            LogGroup.DefaultContext = this;
        }

        protected virtual void OnEnable()
        {
            _disposable = this.Run();
        }

        protected virtual void OnDisable()
        {
            _disposable?.Dispose();
            _disposable = null;
        }

        protected virtual void OnDestroy()
        {
            DisposeGroup.Dispose();
        }

        #endregion

        #region Log

        /// <summary>
        /// 用于统一管理打印的组, 绑定默认的context(this)和配置默认的打印<see cref="LogTags"/>
        /// </summary>
        protected LogGroup LogGroup { get; } = new();

        protected virtual string[] LogTags => null;
        public new void print(object message) => LogGroup.Log(message.ToString(), LogType.Log);
        public void print<T>(T message) => LogGroup.Log(message.ToString(), LogType.Log);
        public void print(string message) => LogGroup.Log(message, LogType.Log);

        public void Log<T>(T message, LogType logType = LogType.Log, params string[] tags) =>
            LogGroup.Log(message.ToString(), logType, tags);

        public void Log(string message, LogType logType = LogType.Log, params string[] tags) =>
            LogGroup.Log(message, logType, tags);

        public void Exception(Exception exception, params string[] tags)
            => LogGroup.Exception(exception, tags);

        public Exception ThrowException(Exception exception, params string[] tags) =>
            LogGroup.ThrowException(exception, tags);

        #endregion

        #region IDisposable

        /// <summary>
        /// 在OnDestroy的时候会销毁一次(也只能销毁一次
        /// </summary>
        protected DisposeGroup DisposeGroup { get; } = new();

        public void AddDispose(IDisposable disposable) => DisposeGroup.Add(disposable);

        #endregion
    }
}