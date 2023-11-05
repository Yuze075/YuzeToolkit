using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit.DriverTool;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.SerializeTool;

namespace YuzeToolkit.IoCTool
{
    public abstract class Container : MonoBase, IContainerBuilder
    {
        #region Static

        private static readonly Dictionary<Type, Container> S_Containers = new();

        #endregion

        [IgnoreParent] [SerializeField] private ShowDictionary<Type, object> valueDictionary = new();
        [IgnoreParent] [SerializeField] private ShowDictionary<Type, ShowObjectList> listDictionary = new();
        [SerializeField] public bool autoBuild = true;

        [SerializeField] [TypeSelector(typeof(Container), TypeSetting = ETypeSetting.AllowUnityObject)]
        private SerializeType parentKey;

        [InLineEditor] [SerializeField] private Container? parentContainer;

        [InLineEditor] [ReorderableList] [SerializeField]
        private List<GameObject> injectGameObjects = new();

        private bool _isRoot;
        public bool IsRoot => _isRoot;
        public Container Parent => IsNotNull(parentContainer);

        protected virtual void Awake()
        {
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

        private Action? _build;
        private readonly List<IBeInjectedValue> _beInjectedValues = new();
        private Action<Container>? _buildCallback;
        private bool _isBuild;
        public bool IsBuild => _isBuild;

        public void Build()
        {
            if (IsBuild)
            {
                Log($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！", ELogType.Warning);
                return;
            }

            GetParent();
            DoBuild();
        }

        public void Build(Container parentContainer)
        {
            if (IsBuild)
            {
                Log($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！", ELogType.Warning);
                return;
            }

            this.parentContainer = parentContainer;
            Build();
        }

        private void GetParent()
        {
            if (parentContainer == this)
            {
                parentContainer = null;
            }

            if (parentContainer != null)
            {
                _isRoot = false;
                S_Containers.TryAdd(GetType(), this);
                return;
            }

            Type? type = parentKey;
            if (type == null)
            {
                _isRoot = true;
            }
            else
            {
                if (!S_Containers.TryGetValue(type, out var container))
                {
                    throw new IoCException($"{GetType()}无法获取到key为{parentKey}的Parent！");
                }

                parentContainer = container;
                _isRoot = false;
            }

            S_Containers.TryAdd(GetType(), this);
        }

        private void DoBuild()
        {
            Configure(this);
            this.Register(this);

            _build?.Invoke();
            _build = null;

            _buildCallback?.Invoke(this);
            _buildCallback = null;

            _isBuild = true;

            Inject(_beInjectedValues);
            _beInjectedValues.Clear();

            foreach (var obj in injectGameObjects) Inject(obj);
        }

        protected virtual void Configure(IContainerBuilder builder)
        {
        }

        #endregion

        #region IContainerBuilder

        void IContainerBuilder.Register(RegistrationInfo registrationInfo)
        {
            switch (registrationInfo.Lifetime)
            {
                case ELifetime.Singleton:
                {
                    foreach (var interfaceType in registrationInfo.InterfaceTypes)
                    {
                        if (valueDictionary.ContainsKey(interfaceType))
                        {
                            Log($"{interfaceType}类型的已经被注册了, 不能再注册单例！", ELogType.Warning);
                            continue;
                        }

                        valueDictionary.Add(interfaceType, registrationInfo.Value);
                    }

                    break;
                }
                case ELifetime.ListItem:
                {
                    foreach (var interfaceType in registrationInfo.InterfaceTypes)
                    {
                        if (!listDictionary.TryGetValue(interfaceType, out var list))
                        {
                            list = new ShowObjectList();
                            listDictionary.Add(interfaceType, list);
                        }

                        list.Add(registrationInfo.Value);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Inject(registrationInfo.Value);
        }

        void IContainerBuilder.Callback(Action<Container> buildCallback) => _buildCallback += buildCallback;

        void IContainerBuilder.Callback(object tryInjectValue) => Inject(tryInjectValue);

        #endregion

        #region GetValue

        public T? Get<T>()
        {
            if (valueDictionary.TryGetValue(typeof(T), out var o)) return (T)o;
            if (TryGetListFirstValue<T>(out var value)) return value;
            return IsRoot ? default : Parent.Get<T>();
        }

        public bool TryGet<T>(out T value)
        {
            if (valueDictionary.TryGetValue(typeof(T), out var o))
            {
                value = (T)o;
                return true;
            }

            if (TryGetListFirstValue(out value)) return true;
            if (!IsRoot) return Parent.TryGet(out value);

            value = default!;
            return false;
        }

        public IEnumerable<T> GetEnumerable<T>()
        {
            if (TryGetEnumerableNotFromParent<T>(out var list))
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
            if (valueDictionary.TryGetValue(typeof(T), out var o))
            {
                value = (T)o;
                return true;
            }

            value = default!;
            return false;
        }

        public IEnumerable<T>? GetEnumerableNotFromParent<T>() =>
            listDictionary.TryGetValue(typeof(T), out var showObjectList)
                ? showObjectList.GetTEnumerable<T>()
                : null;

        public IEnumerable<T>? GetEnumerableNotFromParent<T>(out int count)
        {
            if (listDictionary.TryGetValue(typeof(T), out var showObjectList))
            {
                count = showObjectList.Count;
                return showObjectList.GetTEnumerable<T>();
            }

            count = 0;
            return null;
        }

        public bool TryGetEnumerableNotFromParent<T>(out IEnumerable<T> list)
        {
            if (listDictionary.TryGetValue(typeof(T), out var showObjectList))
            {
                list = showObjectList.GetTEnumerable<T>();
                return true;
            }

            list = null!;
            return false;
        }

        public bool TryGetEnumerableNotFromParent<T>(out IEnumerable<T> list, out int count)
        {
            if (listDictionary.TryGetValue(typeof(T), out var showObjectList))
            {
                count = showObjectList.Count;
                list = showObjectList.GetTEnumerable<T>();
                return true;
            }

            count = 0;
            list = null!;
            return false;
        }

        private bool TryGetListFirstValue<T>(out T value)
        {
            if (listDictionary.TryGetValue(typeof(T), out var showObjectList) && showObjectList.Count > 0)
            {
                value = (T)showObjectList[0];
                return true;
            }

            value = default!;
            return false;
        }

        #endregion

        #region InjectValue

        public virtual void Inject(object tryInjectValue)
        {
            if (IsBuild)
            {
                if (tryInjectValue is IBeInjectedValue beInjectedValue)
                    beInjectedValue.BeInjected(this);
            }
            else
            {
                if (tryInjectValue is not IBeInjectedValue beInjectedValue) return;
                if (_beInjectedValues.Contains(beInjectedValue)) return;
                _beInjectedValues.Add(beInjectedValue);
            }
        }

        public virtual void Inject(GameObject gameObject)
        {
            foreach (var beInjectedValue in gameObject.GetComponents<IBeInjectedValue>()) Inject(beInjectedValue);
            foreach (var beInjectedValue in gameObject.GetComponentsInChildren<IBeInjectedValue>())
                Inject(beInjectedValue);
        }

        public virtual void Inject(IEnumerable<IBeInjectedValue> enumerable)
        {
            foreach (var beInjectedValue in enumerable) Inject(beInjectedValue);
        }

        #endregion

        protected override void DoDispose()
        {
            if (S_Containers.TryGetValue(GetType(), out var value) && value == this) 
                S_Containers.Remove(GetType());
            base.DoDispose();
        }
    }
}