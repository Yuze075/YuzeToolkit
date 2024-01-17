#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.PoolTool
{
    public sealed class GenericObjectPool<TValue> where TValue : class
    {
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

        public GenericObjectPool(Func<TValue> createFunc,
            Action<TValue>? actionOnGet = null,
            Action<TValue>? actionOnRelease = null,
            Action<TValue>? actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000)
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

    public abstract class GenericPoolBase<T> where T : class
    {
        private static GenericObjectPool<T>? s_pool;
        protected static GenericObjectPool<T>? S_Pool => s_pool;

        public static void CreatePool(Func<T> createFunc, Action<T>? actionOnGet = null,
            Action<T>? actionOnRelease = null, Action<T>? actionOnDestroy = null, bool collectionCheck = true,
            int defaultCapacity = 10, int maxSize = 1000)
        {
            if (s_pool != null) throw new ArgumentException("已经创建了对应的pool！");
            s_pool = new GenericObjectPool<T>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy,
                collectionCheck,
                defaultCapacity, maxSize);
        }

        public static T? Get() => s_pool?.Get();

        public static bool TryGet([NotNullWhen(true)] out T? value)
        {
            if (s_pool != null)
            {
                value = s_pool.Get();
                return true;
            }

            value = null;
            return false;
        }

        public static bool Release(T obj)
        {
            if (s_pool == null) return false;
            s_pool.Release(obj);
            return true;
        }

        public static bool RefRelease(ref T? obj)
        {
            if (s_pool == null) return false;
            if (obj == null) return false;
            s_pool.Release(obj);
            obj = null;
            return true;
        }

        public static bool TryGetMaxSize(out int maxSize)
        {
            if (s_pool == null)
            {
                maxSize = -1;
                return false;
            }

            maxSize = s_pool.MaxSize;
            return true;
        }

        public static bool TrySetMaxSize(int maxSize)
        {
            if (s_pool == null) return false;
            s_pool.MaxSize = maxSize;
            return true;
        }
    }

    public abstract class GenericPool<T> : GenericPoolBase<T> where T : class, new()
    {
        private static void CreatePoolDefault() => CreatePool(() => new T());

        public new static T Get()
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            return S_Pool.Get();
        }

        public new static bool TryGet(out T value)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            value = S_Pool.Get();
            return true;
        }

        public new static bool Release(T obj)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) return false;
            S_Pool.Release(obj);
            return true;
        }

        public new static bool RefRelease(ref T? obj)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) return false;
            if (obj == null) return false;
            S_Pool.Release(obj);
            obj = null;
            return true;
        }

        public static int MaxSize
        {
            get
            {
                if (S_Pool == null) CreatePoolDefault();
                if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
                return S_Pool.MaxSize;
            }
            set
            {
                if (S_Pool == null) CreatePoolDefault();
                if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
                S_Pool.MaxSize = value;
            }
        }

        public new static bool TryGetMaxSize(out int maxSize)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            maxSize = S_Pool.MaxSize;
            return true;
        }

        public new static bool TrySetMaxSize(int maxSize)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            S_Pool.MaxSize = maxSize;
            return true;
        }
    }

    public abstract class GenericPool<TCollection, TItem> : GenericPoolBase<TCollection>
        where TCollection : class, ICollection<TItem>, new()
    {
        private static void CreatePoolDefault() =>
            CreatePool(() => new TCollection(), actionOnRelease: collection => collection.Clear());

        public new static TCollection Get()
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            return S_Pool.Get();
        }

        public new static bool TryGet(out TCollection value)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            value = S_Pool.Get();
            return true;
        }

        public new static bool Release(TCollection obj)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) return false;
            S_Pool.Release(obj);
            return true;
        }

        public new static bool RefRelease(ref TCollection? obj)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) return false;
            if (obj == null) return false;
            S_Pool.Release(obj);
            obj = null;
            return true;
        }

        public static int MaxSize
        {
            get
            {
                if (S_Pool == null) CreatePoolDefault();
                if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
                return S_Pool.MaxSize;
            }
            set
            {
                if (S_Pool == null) CreatePoolDefault();
                if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
                S_Pool.MaxSize = value;
            }
        }

        public new static bool TryGetMaxSize(out int maxSize)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            maxSize = S_Pool.MaxSize;
            return true;
        }

        public new static bool TrySetMaxSize(int maxSize)
        {
            if (S_Pool == null) CreatePoolDefault();
            if (S_Pool == null) throw new ArgumentException("无法正常创建默认new()对象的对象池");
            S_Pool.MaxSize = maxSize;
            return true;
        }
    }

    public abstract class DictionaryPool<TKey, TValue> :
        GenericPool<Dictionary<TKey, TValue>, KeyValuePair<TKey, TValue>>
    {
    }

    public abstract class ListPool<T> : GenericPool<List<T>, T>
    {
    }
}