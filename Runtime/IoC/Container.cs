using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.MonoDriver;

namespace YuzeToolkit.IoC
{
    public class Container : MonoBase, IInjectResolver, IValueGetter, IDisposable
    {
        private readonly Dictionary<Type, IValueWrapper> _valueDictionary = new();
        [SerializeField] private bool autoBuild;
        [SerializeField] private ParentReference parent;
        public bool IsRoot => parent.IsRoot;
        public Container Parent => parent.Parent;

        protected override void Awake()
        {
            base.Awake();
            if (!autoBuild) return;
            try
            {
                Build();
            }
            catch (Exception e) when (e is IoCException)
            {
                throw ThrowException(e);
            }
        }

        #region Build

        private Action<IInjectResolver> _buildCallback;
        private bool _isBuild;
        public bool IsBuild => _isBuild;

        public void Build()
        {
            if (IsBuild)
            {
                Log($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！", LogType.Warning);
                return;
            }

            parent.Init(this);
            DoBuild();
        }

        public void Build(Container parentContainer)
        {
            if (IsBuild)
            {
                Log($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！", LogType.Warning);
                return;
            }

            if (IsRoot)
            {
                Log($"为{nameof(IsRoot)}的{nameof(Container)}，不能添加父{nameof(Container)}！", LogType.Warning);
                Build();
                return;
            }

            parent.Init(this, parentContainer);
            DoBuild();
        }

        private void DoBuild()
        {
            var builder = new ContainerBuilder(this);
            Configure(builder);
            builder.Register(this).AsAllTypes();

            builder.Build();

            _buildCallback?.Invoke(this);
            _buildCallback = null;
        }

        private void Register<T>(RegistrationInfo<T> registrationInfo)
        {
            switch (registrationInfo.Lifetime)
            {
                case ELifetime.Singleton:
                {
                    var valueWrapper = new ValueWrapper<T>(registrationInfo.Value);
                    foreach (var type in registrationInfo.InterfaceTypes)
                    {
                        if (_valueDictionary.ContainsKey(type))
                        {
                            Log($"{typeof(T)}类型的子类{type}已经被注册了, 不能再注册单例！", LogType.Error);
                            continue;
                        }

                        _valueDictionary.Add(type, valueWrapper);
                    }

                    break;
                }
                case ELifetime.ListItem:
                {
                    foreach (var type in registrationInfo.InterfaceTypes)
                    {
                        if (_valueDictionary.TryGetValue(type, out var value))
                        {
                            if (value is not IListValueWrapper listValueWrapper)
                            {
                                Log($"{typeof(T)}类型的子类{type}被注册为单例了，无法注册为数组！", LogType.Error);
                                continue;
                            }

                            if (!listValueWrapper.TryAdd(registrationInfo.Value))
                            {
                                Log($"{typeof(T)}类型的子类{type}无法添加进对应数组！", LogType.Error);
                            }

                            continue;
                        }

                        var listValue = new ListValueWrapper<T>();
                        listValue.TryAdd(registrationInfo.Value);
                        _valueDictionary.Add(type, listValue);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void Configure(IContainerBuilder builder)
        {
        }

        #endregion

        #region IValueGetter

        public T Get<T>()
        {
            if (_valueDictionary.TryGetValue(typeof(T), out var valueWrapper))
                return valueWrapper.TryGet(out T tValue) ? tValue : default;

            return IsRoot ? default : Parent.Get<T>();
        }

        public bool TyrGet<T>(out T value)
        {
            if (_valueDictionary.TryGetValue(typeof(T), out var valueWrapper)) return valueWrapper.TryGet(out value);

            if (!IsRoot) return Parent.TyrGet(out value);

            value = default;
            return false;
        }

        public IEnumerable<T> GetEnumerable<T>()
        {
            if (TryGetList<T>(out var list))
            {
                foreach (var t in list)
                {
                    yield return t;
                }
            }

            if (IsRoot) yield break;

            foreach (var t in Parent.GetEnumerable<T>())
            {
                yield return t;
            }
        }

        public bool TyrGetNotFromParent<T>(out T value)
        {
            if (_valueDictionary.TryGetValue(typeof(T), out var valueWrapper)) return valueWrapper.TryGet(out value);
            value = default;
            return false;
        }

        public IReadOnlyList<T> GetList<T>()
        {
            return _valueDictionary.TryGetValue(typeof(T), out var valueWrapper) &&
                   valueWrapper.TryGet(out IReadOnlyList<T> list)
                ? list
                : null;
        }

        public bool TryGetList<T>(out IReadOnlyList<T> list)
        {
            if (_valueDictionary.TryGetValue(typeof(T), out var valueWrapper)) return valueWrapper.TryGet(out list);
            list = default;
            return false;
        }

        #endregion

        #region IInjectResolver

        public void Inject(IBeInjectedValue beInjectedValue) => beInjectedValue.DoInject(this);

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            Destroy(gameObject);
        }

        protected virtual void DoDispose()
        {
        }

        #endregion

        #region Struct

        private class ContainerBuilder : IContainerBuilder
        {
            public ContainerBuilder(Container container) => _container = container;

            private readonly Container _container;
            private Action _build;

            internal void Build()
            {
                _build?.Invoke();
                _build = null;
            }

            public void Register<T>(RegistrationInfo<T> registrationInfo)
            {
                _build += () => { _container.Register(registrationInfo); };
                if (registrationInfo.Value is IBeInjectedValue injectedValue)
                    Callback(resolver => { resolver.Inject(injectedValue); });
            }

            public void Callback(Action<IInjectResolver> action) => _container._buildCallback += action;
        }

        #endregion
    }
}