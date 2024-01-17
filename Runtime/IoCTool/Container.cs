#nullable enable
// #define SHOW_IOC_TOOL_IN_INSPECTOR

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.SerializeTool;

namespace YuzeToolkit.IoCTool
{
    [Serializable]
    public abstract class Container : MonoBase, IContainerBuilder
    {
        #region Static

        private static readonly Dictionary<Type, Container> S_Containers = new();

        #endregion

#if UNITY_EDITOR && USE_SHOW_VALUE && SHOW_IOC_TOOL_IN_INSPECTOR
        [Help(nameof(Container))] [Title("参数字典")] [IgnoreParent] [SerializeField]
        private InspectorTool.ShowDictionary<Type, object> valueDictionary;

        [IgnoreParent] [SerializeField]
        private InspectorTool.ShowDictionary<Type, InspectorTool.ShowList<object>> listDictionary;
#else
        // ReSharper disable once InconsistentNaming
        private readonly Dictionary<Type, object> valueDictionary = new();

        // ReSharper disable once InconsistentNaming
        private readonly Dictionary<Type, List<object>> listDictionary = new();
#endif


        [Title("配置参数")] [SerializeField] public bool autoBuild = true;

        [SerializeField] [TypeSelector(typeof(Container), TypeSetting = ETypeSetting.AllowUnityObject)]
        private SerializeType parentKey;

        [InLineEditor] [SerializeField] private Container? parentContainer;

        [InLineEditor] [ReorderableList] [SerializeField]
        private List<GameObject> injectGameObjects = new();

        private bool _isRoot;
        public bool IsRoot => _isRoot;

        public Container Parent
        {
            get
            {
                IsNotNull(parentContainer != null, nameof(parentContainer));
                return parentContainer;
            }
        }

        protected virtual void Awake()
        {
            if (!autoBuild) return;
            Build();
        }

        #region Build

        private readonly List<IBeInjectedValue> _beInjectedValues = new();
        private readonly List<RegistrationInfo> _registrationInfos = new();
        private Action<Container>? _buildCallback;
        private bool _isBuild;
        public bool IsBuild => _isBuild;

        public void Build()
        {
            if (IsBuild)
            {
                LogWarning($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！");
                return;
            }

            GetParent();
            DoBuild();
        }

        public void Build(Container parentContainer)
        {
            if (IsBuild)
            {
                LogWarning($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！");
                return;
            }

            this.parentContainer = parentContainer;
            Build();
        }

        private void GetParent()
        {
            if (parentContainer == this) parentContainer = null;

            if (parentContainer != null)
            {
                _isRoot = false;
                S_Containers.TryAdd(GetType(), this);
                return;
            }

            Type? type = parentKey;
            if (type == null) _isRoot = true;
            else
            {
                if (!S_Containers.TryGetValue(type, out var container))
                    throw new KeyNotFoundException($"{GetType()}无法获取到key为{parentKey}的Parent！");
                parentContainer = container;
                _isRoot = false;
            }

            S_Containers.TryAdd(GetType(), this);
        }

        private void DoBuild()
        {
            Configure(this);
            this.Register(this);

            DoRegister();

            _isBuild = true;

            _buildCallback?.Invoke(this);
            _buildCallback = null;
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
            if (_isBuild) throw new IoCException($"{nameof(Container)}已经{nameof(Build)}完成，不能再注册！");
            _registrationInfos.Add(registrationInfo);
        }

        protected virtual void DoRegister()
        {
            var count = _registrationInfos.Count;
            for (var index = 0; index < count; index++)
            {
                var registrationInfo = _registrationInfos[index];
                switch (registrationInfo.Lifetime)
                {
                    case ELifetime.Singleton:
                    {
                        foreach (var interfaceType in registrationInfo.InterfaceTypes)
                        {
                            if (valueDictionary.ContainsKey(interfaceType))
                            {
                                LogWarning($"{interfaceType}类型的已经被注册了, 不能再注册单例！");
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
                                list =
#if UNITY_EDITOR && USE_SHOW_VALUE && SHOW_IOC_TOOL_IN_INSPECTOR
                                    new InspectorTool.ShowList<object>(4);
#else
                                    new List<object>();
#endif

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

            _registrationInfos.Clear();
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

        public bool TryGet<T>([MaybeNullWhen(false)] out T value)
        {
            if (valueDictionary.TryGetValue(typeof(T), out var o))
            {
                value = (T)o;
                return true;
            }

            if (TryGetListFirstValue(out value)) return true;
            if (!IsRoot) return Parent.TryGet(out value);

            value = default;
            return false;
        }

        public IEnumerable<T> GetEnumerable<T>()
        {
            foreach (var t in GetIEnumerableNotFromParent<T>()) yield return t;
            if (IsRoot) yield break;
            foreach (var value in Parent.GetEnumerable<T>()) yield return value;
        }

        public bool TyrGetNotFromParent<T>([MaybeNullWhen(false)] out T value)
        {
            if (valueDictionary.TryGetValue(typeof(T), out var o))
            {
                value = (T)o;
                return true;
            }

            value = default;
            return false;
        }

        public IEnumerable<T> GetIEnumerableNotFromParent<T>()
        {
            if (!listDictionary.TryGetValue(typeof(T), out var showObjectList)) yield break;
            foreach (T value in showObjectList) yield return value;
        }

        private bool TryGetListFirstValue<T>([MaybeNullWhen(false)] out T value)
        {
            if (listDictionary.TryGetValue(typeof(T), out var showObjectList) && showObjectList.Count > 0)
            {
                value = (T)showObjectList[0];
                return true;
            }

            value = default;
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
            Inject(gameObject.GetComponents<IBeInjectedValue>());
            Inject(gameObject.GetComponentsInChildren<IBeInjectedValue>());
        }

        public virtual void Inject(IEnumerable<IBeInjectedValue> enumerable)
        {
            foreach (var beInjectedValue in enumerable) Inject(beInjectedValue);
        }

        #endregion

        protected override void OnDestroy()
        {
            if (S_Containers.TryGetValue(GetType(), out var value) && value == this)
                S_Containers.Remove(GetType());
            base.OnDestroy();
        }
    }
}