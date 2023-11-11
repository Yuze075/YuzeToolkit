using System;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DriverTool
{
    public static class MonoBaseExtend
    {
        public static IDisposable Run(this IMonoBase monoBase) => MonoDriverManager.Run(monoBase);
        
        public static IDisposable Run(this Action action, EUpdateType updateType = EUpdateType.Update, int priority = 0,
            OrderType type = OrderType.After)
        {
            return MonoDriverManager.Run(updateType switch
            {
                EUpdateType.Update => new ActionUpdateWrapper(action, priority, type),
                EUpdateType.FixedUpdate => new ActionFixedUpdateWrapper(action, priority, type),
                EUpdateType.LateUpdate => new ActionLateUpdateWrapper(action, priority, type),
                _ => throw new ArgumentOutOfRangeException(nameof(updateType), updateType,
                    null).ThrowException()
            });
        }
    }

    public enum EUpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate
    }
}