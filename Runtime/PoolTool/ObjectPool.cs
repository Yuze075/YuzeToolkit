using System;
using UnityEngine;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.PoolTool
{
    [Serializable]
    public class ObjectPool<T> : IObjectPool where T : class
    {
        [SerializeField] private ShowList<T> list;
        private readonly Func<T> _createFunc;
        private readonly Action<T>? _actionOnGet;
        private readonly Action<T>? _actionOnRelease;
        private readonly Action<T>? _actionOnDestroy;
        private int _maxSize;
        private bool _collectionCheck;

        public int MaxSize
        {
            get => _maxSize;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Max Size must be greater than 0", nameof(value)).ThrowException();
                _maxSize = value;
                if (CountInactive <= _maxSize) return;
                for (var i = 0; i < _maxSize - CountInactive; i++)
                {
                    var index = list.Count - 1;
                    var obj = list[index];
                    list.RemoveAt(index);
                    _actionOnDestroy?.Invoke(obj);
                }
            }
        }

        public int CountAll { get; private set; }

        public int CountActive => CountAll - CountInactive;
        public int CountInactive => list.Count;

        public ObjectPool(
            Func<T> createFunc,
            Action<T>? actionOnGet = null,
            Action<T>? actionOnRelease = null,
            Action<T>? actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000)
        {
            if (maxSize <= 0)
                throw new ArgumentException("Max Size must be greater than 0", nameof(maxSize)).ThrowException();
            list = new ShowList<T>(defaultCapacity);
            _createFunc = createFunc;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _maxSize = maxSize;
            _collectionCheck = collectionCheck;
        }

        public T Get()
        {
            T obj;
            if (list.Count == 0)
            {
                obj = _createFunc();
                ++CountAll;
            }
            else
            {
                var index = list.Count - 1;
                obj = list[index];
                list.RemoveAt(index);
            }

            _actionOnGet?.Invoke(obj);
            return obj;
        }

        public IDisposable Get(out T value)
        {
            var getValue = Get();
            value = getValue;
            return new UnRegister(() => { Release(getValue); });
        }

        public void Release(T value)
        {
            if (_collectionCheck && list.Contains(value))
            {
                throw new InvalidOperationException(
                    "Trying to release an object that has already been released to the pool.").ThrowException();
            }

            _actionOnRelease?.Invoke(value);
            if (CountInactive < _maxSize) list.Add(value);
            else _actionOnDestroy?.Invoke(value);
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
            if (_actionOnDestroy != null)
            {
                foreach (var o in list)
                {
                    _actionOnDestroy(o);
                }
            }

            list.Clear();
            CountAll = 0;
        }

        public void Dispose() => Clear();
    }
}