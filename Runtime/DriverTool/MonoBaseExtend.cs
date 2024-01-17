#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DriverTool
{
    public static class MonoBaseExtend
    {
        public static UpdateToken Run(this IMonoBase monoBase) => MonoDriverManager.Run(monoBase);

        public static UpdateToken Run(this Action action, EUpdateType updateType = EUpdateType.Update,
            int priority = 0, EOrderType type = EOrderType.After) => MonoDriverManager.Run(updateType switch
        {
            EUpdateType.Update => new ActionUpdateWrapper(action, priority, type),
            EUpdateType.FixedUpdate => new ActionFixedUpdateWrapper(action, priority, type),
            EUpdateType.LateUpdate => new ActionLateUpdateWrapper(action, priority, type),
            _ => throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null)
        });
    }

    public enum EUpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }
}