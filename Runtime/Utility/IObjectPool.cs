#nullable enable
using System;
using System.Collections.Generic;

namespace YuzeToolkit
{
    public interface IObjectPool
    {
        int MaxSize { get; set; }
        int CountInactive { get; }
        object Get();
        IDisposable Get(out object value);
        bool Release(object value);
        void Clear();
    }

    public interface IObjectPool<TValue> : IObjectPool where TValue : class
    {
        object IObjectPool.Get() => Get();

        IDisposable IObjectPool.Get(out object value)
        {
            var pooledObject = Get(out var t);
            value = t;
            return pooledObject;
        }

        bool IObjectPool.Release(object value)
        {
            if (value is not TValue t) return false;
            Release(t);
            return true;
        }

        new TValue Get();
        PooledObject<TValue> Get(out TValue value);
        void Release(TValue value);
    }

    public class ObjectPool<TValue> : IObjectPool<TValue> where TValue : class
    {
        public ObjectPool(Func<TValue> createFunc,
            Action<TValue>? actionOnGet = null,
            Action<TValue>? actionOnRelease = null,
            Action<TValue>? actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 1000)
        {
            if (maxSize <= 0)
                throw new ArgumentException("最大数组大小必须大于0", nameof(maxSize));
            _list = new List<TValue>(defaultCapacity);
            _createFunc = createFunc;
            _actionOnGet = actionOnGet;
            _actionOnRelease = actionOnRelease;
            _actionOnDestroy = actionOnDestroy;
            _maxSize = maxSize;
            _collectionCheck = collectionCheck;
        }


        private readonly List<TValue> _list;
        private readonly Func<TValue> _createFunc;
        private readonly Action<TValue>? _actionOnGet;
        private readonly Action<TValue>? _actionOnRelease;
        private readonly Action<TValue>? _actionOnDestroy;
        private readonly bool _collectionCheck;
        private int _maxSize;

        public int MaxSize
        {
            get => _maxSize;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("最大数组大小必须大于0", nameof(value));
                _maxSize = value;
                if (_list.Count <= _maxSize) return;
                for (var i = 0; i < _maxSize - _list.Count; i++)
                {
                    var index = _list.Count - 1;
                    var obj = _list[index];
                    _list.RemoveAt(index);
                    _actionOnDestroy?.Invoke(obj);
                }
            }
        }

        public int CountInactive => _list.Count;

        public TValue Get()
        {
            TValue obj;
            if (_list.Count == 0) obj = _createFunc();
            else
            {
                var index = _list.Count - 1;
                obj = _list[index];
                _list.RemoveAt(index);
            }

            _actionOnGet?.Invoke(obj);
            return obj;
        }

        public PooledObject<TValue> Get(out TValue value) => new(value = Get(), this);

        public void Release(TValue value)
        {
#if UNITY_EDITOR
            if (_collectionCheck && _list.Contains(value))
                throw new InvalidOperationException($"尝试Release的{value}对象已经在对象池中了!");
#endif

            _actionOnRelease?.Invoke(value);
            if (_list.Count < _maxSize) _list.Add(value);
            else _actionOnDestroy?.Invoke(value);
        }

        public void Clear()
        {
            if (_actionOnDestroy != null)
            {
                var count = _list.Count;
                for (var index = 0; index < count; index++)
                    _actionOnDestroy(_list[index]);
            }

            _list.Clear();
        }
    }

    public struct PooledObject<TValue> : IDisposable where TValue : class
    {
        private TValue? _toReleaseValue;
        private ObjectPool<TValue>? _objectPool;

        internal PooledObject(TValue value, ObjectPool<TValue> pool)
        {
            _toReleaseValue = value;
            _objectPool = pool;
        }

        void IDisposable.Dispose()
        {
            if (_objectPool == null || _toReleaseValue == null) return;
            _objectPool.Release(_toReleaseValue);
            _objectPool = null;
            _toReleaseValue = null;
        }
    }

    public abstract class GenericPoolBase<T> where T : class
    {
        protected static ObjectPool<T>? S_Pool { get; private set; }
        protected static void CreatePool(Func<T> createFunc, Action<T>? actionOnGet = null,
            Action<T>? actionOnRelease = null, Action<T>? actionOnDestroy = null, bool collectionCheck = true,
            int defaultCapacity = 10, int maxSize = 1000)
        {
            if (S_Pool != null) throw new ArgumentException("已经创建了对应的pool！");
            S_Pool = new ObjectPool<T>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy,
                collectionCheck, defaultCapacity, maxSize);
        }
    }

    public abstract class GenericPool<T> : GenericPoolBase<T> where T : class, new()
    {
        protected new static ObjectPool<T> S_Pool
        {
            get
            {
                if (GenericPoolBase<T>.S_Pool == null) CreatePool(() => new T());
#if UNITY_EDITOR
                if (GenericPoolBase<T>.S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
#endif
                return GenericPoolBase<T>.S_Pool!;
            }
        }

        public static T Get() => S_Pool.Get();

        public static bool TryGet(out T value)
        {
            value = S_Pool.Get();
            return true;
        }

        public static bool Release(T obj)
        {
            S_Pool.Release(obj);
            return true;
        }

        public static bool RefRelease(ref T? obj)
        {
            if (obj == null) return false;
            S_Pool.Release(obj);
            obj = null;
            return true;
        }

        public static int MaxSize
        {
            get => S_Pool.MaxSize;
            set => S_Pool.MaxSize = value;
        }

        public static bool TryGetMaxSize(out int maxSize)
        {
            maxSize = S_Pool.MaxSize;
            return true;
        }

        public static bool TrySetMaxSize(int maxSize)
        {
            S_Pool.MaxSize = maxSize;
            return true;
        }
    }

    public abstract class CollectionPool<TCollection, TItem> : GenericPoolBase<TCollection>
        where TCollection : class, ICollection<TItem>, new()
    {
        protected new static ObjectPool<TCollection> S_Pool
        {
            get
            {
                if (GenericPoolBase<TCollection>.S_Pool == null)
                    CreatePool(() => new TCollection(), actionOnRelease: collection => collection.Clear());
#if UNITY_EDITOR
                if (GenericPoolBase<TCollection>.S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
#endif
                return GenericPoolBase<TCollection>.S_Pool!;
            }
        }

        public static TCollection Get() => S_Pool.Get();

        public static bool TryGet(out TCollection value)
        {
            value = S_Pool.Get();
            return true;
        }

        public static bool Release(TCollection obj)
        {
            S_Pool.Release(obj);
            return true;
        }

        public static bool RefRelease(ref TCollection? obj)
        {
            if (obj == null) return false;
            S_Pool.Release(obj);
            obj = null;
            return true;
        }

        public static int MaxSize
        {
            get => S_Pool.MaxSize;
            set => S_Pool.MaxSize = value;
        }

        public static bool TryGetMaxSize(out int maxSize)
        {
            maxSize = S_Pool.MaxSize;
            return true;
        }

        public static bool TrySetMaxSize(int maxSize)
        {
            S_Pool.MaxSize = maxSize;
            return true;
        }
    }

    public abstract class DictionaryPool<TKey, TValue> :
        CollectionPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    {
    }

    public abstract class ListPool<T> : CollectionPool<List<T>, T>
    {
    }
    
    public abstract class HashSetPool<T> : CollectionPool<HashSet<T>, T>
    {
    }
}