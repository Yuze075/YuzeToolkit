using System;

namespace YuzeToolkit.Utility
{
    public static class MonoBaseExtend
    {
        public static IDisposable Run(this IMonoBase monoBase)
        {
            return MonoDriverManager.Run(monoBase);
        }

        public static ILifeCycle RunLifeCycle(this IMonoBase monoBase)
        {
            return MonoDriverManager.RunLifeCycle(monoBase);
        }

        public static IDisposable Run(this Action action, EUpdateType updateType = EUpdateType.Update, int priority = 0,
            OrderType type = OrderType.After)
        {
            return RunLifeCycle(action, updateType, priority, type);
        }

        public static ILifeCycle RunLifeCycle(this Action action, EUpdateType updateType = EUpdateType.Update,
            int priority = 0, OrderType type = OrderType.After)
        {
            return MonoDriverManager.RunLifeCycle(updateType switch
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