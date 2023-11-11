using System;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.PoolTool
{
    public abstract class GenericPool<T> where T : class, new()
    {
        private static ObjectPool<T>? s_pool;
        private static ObjectPool<T> S_Pool => s_pool ??= new ObjectPool<T>(() => new T());

        public static void CreatePool(Func<T>? createFunc = null,
            Action<T>? actionOnGet = null, Action<T>? actionOnRelease = null, Action<T>? actionOnDestroy = null,
            bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            if (s_pool == null)
            {
                LogSys.Error($"已经创建了对应的{S_Pool}！");
                return;
            }
            s_pool = new ObjectPool<T>(createFunc ?? (() => new T()),
                actionOnGet, actionOnRelease, actionOnDestroy,
                collectionCheck, defaultCapacity, maxSize);
        }

        public static int MaxSize
        {
            get => S_Pool.MaxSize;
            set => S_Pool.MaxSize = value;
        }

        public static T Get() => S_Pool.Get();

        public static IDisposable Get(out T value) => S_Pool.Get(out value);

        public static void Release(T toRelease) => S_Pool.Release(toRelease);
    }
}