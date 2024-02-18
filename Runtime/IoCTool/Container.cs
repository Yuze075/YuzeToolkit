#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.IoCTool
{
    [Serializable]
    public abstract class Container : MonoBehaviour, IContainerBuilder
    {
        #region Static

        private static readonly Dictionary<Type, Container> S_Containers = new();

        #endregion

#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
#if YUZE_USE_EDITOR_TOOLBOX
        [Help(nameof(Container)), Title("参数字典"), IgnoreParent]
#endif
        [SerializeField]
        private InspectorTool.ShowDictionary<Type, object> valueDictionary;

#if YUZE_USE_EDITOR_TOOLBOX
        [IgnoreParent]
#endif
        [SerializeField]
        private InspectorTool.ShowDictionary<Type, InspectorTool.ShowList<object>> listDictionary;
#else
        private readonly System.Collections.Generic.Dictionary<Type, object> valueDictionary = new();
        private readonly System.Collections.Generic.Dictionary<Type, List<object>> listDictionary = new();
#endif


        #region UNITY_EDITOR

#if UNITY_EDITOR
        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected string? AutoBuildHelpBox()
        {
            if (IsBuild) return $"{nameof(Container)}已经构建完成!";
            return AutoBuildScript == null ? null : $"{nameof(AutoBuildScript)}: {AutoBuildScript.Value}";
        }

        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected bool ShowAutoBuild()
        {
            if (IsBuild) return false;
            return AutoBuildScript == null;
        }

        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected string? ParentHelpBox()
        {
            if (IsBuild) return IsRoot ? $"{nameof(Container)}为根节点!" : null;
            if (ParentKeyScript == null) return null;
            return $"{nameof(ParentKeyScript)}: {ParentKeyScript}";
        }

        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected string? ParentContainerErrorHelpBox()
        {
            if (IsBuild) return null;
            if (parentContainer == null)
                return null;
            if (ParentKeyScript != null)
                return $"已经设置了{nameof(ParentKeyScript)}, {nameof(parentContainer)}应该设置为空!";
            if (parentKey.Type != null)
                return $"已经设置了{nameof(parentKey)}, {nameof(parentContainer)}应该设置为空!";
            return null;
        }

        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected string? ParentHelpKeyErrorBox()
        {
            if (IsBuild) return null;
            if (parentKey.Type == null)
                return null;
            if (ParentKeyScript != null)
                return $"已经设置了{nameof(ParentKeyScript)}, {nameof(parentKey)}应该设置为空!";
            if (parentContainer != null)
                return $"已经设置了{nameof(parentContainer)}, {nameof(parentKey)}应该设置为空!";
            return null;
        }

        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected bool ShowParentContainer()
        {
            if (IsBuild) return parentContainer != null;
            if (parentContainer != null) return true;
            return ParentKeyScript == null && parentKey.Type == null;
        }

        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected bool ShowParentKey()
        {
            if (IsBuild) return false;
            if (parentKey.Type != null) return true;
            return ParentKeyScript == null && parentContainer == null;
        }

        [Obsolete("UNITY_EDITOR_ONLY", true)]
        private protected bool DisableWhenBuild() => IsBuild;
#endif

        #endregion


#if YUZE_USE_EDITOR_TOOLBOX
        [Title("配置参数"), DynamicHelp("AutoBuildHelpBox"), ShowIf("ShowAutoBuild", true),
         DisableIf("DisableWhenBuild", true)]
#endif
        [SerializeField]
        public bool autoBuild = true;


#if YUZE_USE_EDITOR_TOOLBOX
        [DynamicHelp("ParentHelpBox"), ShowIf("ShowParentKey", true), DisableIf("DisableWhenBuild", true),
         DynamicHelp("ParentContainerErrorHelpBox", UnityMessageType.Error)]
#endif
        [SerializeField]
        [TypeSelector(typeof(Container), TypeSetting = ETypeSetting.AllowUnityObject)]
        private SerializeType parentKey;

#if YUZE_USE_EDITOR_TOOLBOX
        [ShowIf("ShowParentContainer", true), DisableIf("DisableWhenBuild", true),
         DynamicHelp("ParentHelpKeyErrorBox", UnityMessageType.Error)]
#endif
        [SerializeField]
        private Container? parentContainer;

#if YUZE_USE_EDITOR_TOOLBOX
        [ReorderableList, DisableIf("DisableWhenBuild", true)]
#endif
        [SerializeField]
        private List<GameObject> injectGameObjects = new();

        private bool _isRoot;
        protected virtual bool? AutoBuildScript => null;
        protected virtual Type? ParentKeyScript => null;
        public bool IsRoot => _isRoot;

        public Container Parent
        {
            get
            {
                this.IsNotNull(parentContainer != null, nameof(parentContainer));
                return parentContainer;
            }
        }

        protected virtual void Awake()
        {
            if (!AutoBuildScript ?? !autoBuild) return;
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
                this.LogWarning($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！");
                return;
            }

            GetParent();
            DoBuild();
        }

        public void Build(Container parentContainer)
        {
            if (IsBuild)
            {
                this.LogWarning($"{nameof(Container)}已经{nameof(Build)}完成，不能重复构建！");
                return;
            }

            if (parentContainer != this)
                this.parentContainer = parentContainer;
            else
            {
                this.LogError($"{GetType()}不能设置父容器为自身");
                GetParent();
            }

            Build();
        }

        private void GetParent()
        {
            // 如果脚本设置的父对象类型不为空, 则先从通过类型查找判断是否有合法的类型对象
            if (ParentKeyScript != null)
            {
                // 判断这个类似的对象是否存在
                if (S_Containers.TryGetValue(ParentKeyScript, out var container))
                {
                    // 如果父container不是自己, 则设置父container, 并将自己加入到S_Containers的缓存中
                    if (container != this)
                    {
                        parentContainer = container;
                        _isRoot = false;
                        S_Containers.TryAdd(GetType(), this);
                        return;
                    }

                    // 父container是自己, 报错并继续查找
                    this.LogError($"{GetType()}不能设置父容器为自身");
                }
                // 找不到父对象, 报错并继续查找
                else this.LogError($"{GetType()}无法获取到key为{parentKey}的Parent！");
            }

            // 如果设置父container是自己, 报错并继续查找
            if (parentContainer == this)
            {
                this.LogError($"{GetType()}不能设置父容器为自身");
                parentContainer = null;
            }

            // 如果设置父container对象不为空, 则将自己加入到S_Containers的缓存中
            if (parentContainer != null)
            {
                _isRoot = false;
                S_Containers.TryAdd(GetType(), this);
                return;
            }

            // 如果设置的parentKey为空, 则代表为Root, 自己加入到S_Containers的缓存中
            Type? type = parentKey;
            if (type != null)
            {
                if (S_Containers.TryGetValue(type, out var container))
                {
                    // 如果父container不是自己, 则设置父container, 并将自己加入到S_Containers的缓存中
                    if (container != this)
                    {
                        parentContainer = container;
                        _isRoot = false;
                        S_Containers.TryAdd(GetType(), this);
                        return;
                    }

                    // 父container是自己, 报错并继续查找
                    this.LogError($"{GetType()}不能设置父容器为自身");
                }
                // 找不到父对象, 报错并继续查找
                else this.LogError($"{GetType()}无法获取到key为{parentKey}的Parent！");
            }

            _isRoot = true;
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
            if (_isBuild) throw new InvalidOperationException($"{nameof(Container)}已经{nameof(Build)}完成，不能再注册！");
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
                                this.LogWarning($"{interfaceType}类型的已经被注册了, 不能再注册单例！");
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
                                list = new();
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

        protected virtual void OnDestroy()
        {
            if (S_Containers.TryGetValue(GetType(), out var value) && value == this)
                S_Containers.Remove(GetType());
        }
    }
}