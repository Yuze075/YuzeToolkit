#nullable enable
using System;

namespace YuzeToolkit.PoolTool
{
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE && YUZE_POOL_TOOL_USE_SHOW_VALUE
    [Serializable]
#endif
    public abstract class ObjectPoolBase<T> : IObjectPool where T : class
    {
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE && YUZE_POOL_TOOL_USE_SHOW_VALUE
        [UnityEngine.SerializeField] private InspectorTool.ShowList<T> list;
#else
        private readonly System.Collections.Generic.List<T> list;
#endif
        private readonly int _maxSize;
        private readonly bool _collectionCheck;

        public int MaxSize { get; set; }
        public int CountAll { get; private set; }

        public int CountActive => CountAll - CountInactive;
        public int CountInactive => list.Count;

        protected ObjectPoolBase(bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
        {
            if (maxSize <= 0)
                throw new ArgumentException("最大数组大小必须大于0", nameof(maxSize));
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE && YUZE_POOL_TOOL_USE_SHOW_VALUE
            list = new InspectorTool.ShowList<T>(defaultCapacity);
#else
            list = new System.Collections.Generic.List<T>(defaultCapacity);
#endif
            _maxSize = maxSize;
            _collectionCheck = collectionCheck;
        }

        ~ObjectPoolBase() => Dispose(false);
        [NonSerialized] private bool _disposed;

        public T Get()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            T obj;
            if (list.Count == 0)
            {
                obj = CreateFunc();
                ++CountAll;
            }
            else
            {
                var index = list.Count - 1;
                obj = list[index];
                list.RemoveAt(index);
            }

            ActionOnGet(obj);
            return obj;
        }

        public IDisposable Get(out T value)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var getValue = Get();
            value = getValue;
            return UnRegister.Create(Release, value);
        }

        public void Release(T value)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
#if UNITY_EDITOR
            if (_collectionCheck && list.Contains(value))
                throw new InvalidOperationException($"尝试Release的{value}对象已经在对象池中了!");
#endif
            ActionOnRelease(value);
            if (CountInactive < _maxSize) list.Add(value);
            else ActionOnDestroy(value);
        }

        object IObjectPool.Get() => Get();

        IDisposable IObjectPool.Get(out object value)
        {
            var disposable = Get(out var tValue);
            value = tValue;
            return disposable;
        }

        bool IObjectPool.Release(object value)
        {
            if (value is not T tValue) return false;
            Release(tValue);
            return true;
        }

        public void Clear()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var count = list.Count;
            for (var index = 0; index < count; index++) ActionOnDestroy(list[index]);
            list.Clear();
            CountAll = 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed) return;
            Clear();
            _disposed = true;
        }

        protected abstract T CreateFunc();

        protected virtual void ActionOnGet(T value)
        {
        }

        protected virtual void ActionOnRelease(T value)
        {
        }

        protected virtual void ActionOnDestroy(T value)
        {
        }
    }
}