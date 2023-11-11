using System;
using System.Collections.Generic;

namespace YuzeToolkit.PoolTool
{
    public class CollectionPool<TCollection, TItem> where TCollection : class, ICollection<TItem>, new()
    {
        private static ObjectPool<TCollection>? s_pool;

        private static ObjectPool<TCollection> S_Pool => s_pool ??= new ObjectPool<TCollection>(() => new TCollection(),
            actionOnRelease: collection => collection.Clear());

        public static bool CreatePool(Func<TCollection>? createFunc = null,
            Action<TCollection>? actionOnGet = null, Action<TCollection>? actionOnRelease = null,
            Action<TCollection>? actionOnDestroy = null,
            bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 1000)
        {
            if (s_pool == null) return false;
            s_pool = new ObjectPool<TCollection>(createFunc ?? (() => new TCollection()),
                actionOnGet, actionOnRelease ?? (collection => collection.Clear()), actionOnDestroy,
                collectionCheck, defaultCapacity, maxSize);
            return true;
        }

        public static int MaxSize
        {
            get => S_Pool.MaxSize;
            set => S_Pool.MaxSize = value;
        }

        public static TCollection Get() => S_Pool.Get();
        public static IDisposable Get(out TCollection value) => S_Pool.Get(out value);
        public static void Release(TCollection toRelease) => S_Pool.Release(toRelease);
    }
}