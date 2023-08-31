using System;

namespace YuzeToolkit.Utility
{
    public static class MonoBaseExtend
    {
        public static IDisposable Run(this IMonoBase monoBase)
        {
            return MonoDriverManager.Run(monoBase);
        }

        public static IDisposable Run(this Action action, EUpdateType updateType = EUpdateType.Update, int priority = 0,
            OrderType type = OrderType.After)
        {
            return MonoDriverManager.Run(updateType switch
            {
                EUpdateType.Update => new UpdateWrapper(action, priority, type),
                EUpdateType.FixedUpdate => new FixedUpdateWrapper(action, priority, type),
                EUpdateType.LateUpdate => new LateUpdateWrapper(action, priority, type),
                _ => throw new ArgumentOutOfRangeException(nameof(updateType), updateType, null)
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