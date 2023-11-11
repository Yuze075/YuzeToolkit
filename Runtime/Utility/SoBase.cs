using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit
{
    public abstract class SoBase : ScriptableObject, ILogTool
    {
        #region Log

        private string[]? _logTags;
        private string[]? LogTags => _logTags ??= GetLogTags;

        protected virtual string[]? GetLogTags => null;

        public void Log<T>(T message, ELogType logType = ELogType.Log, params string[] tags) =>
            LogSys.Log(message, logType, this, LogTags.ArrayMerge(tags));

        public Exception ThrowException(Exception exception, params string[] tags) =>
            exception.ThrowException(this, LogTags.ArrayMerge(tags));

        public T IsNotNull<T>(T? isNotNull, string? name = null, string? message = null, bool additionalCheck = true) =>
            isNotNull.IsNotNull(name, message, this, additionalCheck);

        public TCastTo IsNotNull<TCastTo>(object? isNotNull, string? name = null, string? message = null,
            bool additionalCheck = false) => isNotNull.IsNotNull<TCastTo>(name, message, this, additionalCheck);

        #endregion
    }
}