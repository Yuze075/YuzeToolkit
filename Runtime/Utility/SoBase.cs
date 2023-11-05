using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit
{
    public abstract class SoBase : ScriptableObject, ILogTool
    {
        #region Log

        private ULogTool? _uLogTool;

        protected ULogTool LogTool =>
            _uLogTool ??= new ULogTool(new[] { nameof(SoBase), GetType().FullName }, this);

        public void Log<T>(T message, ELogType logType = ELogType.Log, params string[] tags) =>
            LogTool.Log(message, logType, tags);

        public Exception ThrowException(Exception exception, params string[] tags) =>
            LogTool.ThrowException(exception, tags);

        public T IsNotNull<T>(T? isNotNull, string? name = null, string? message = null, bool additionalCheck = true) =>
            LogTool.IsNotNull(isNotNull, name, message, additionalCheck);

        public TCastTo IsNotNull<TCastTo>(object? isNotNull, string? name = null, string? message = null,
            bool additionalCheck = false) => LogTool.IsNotNull<TCastTo>(isNotNull, name, message, additionalCheck);

        #endregion
    }
}