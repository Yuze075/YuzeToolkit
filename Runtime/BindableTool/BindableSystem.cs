using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public class BindableSystem : IDisposable
    {
        #region UNITY_EDITOR

#if UNITY_EDITOR
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [SerializeReference]
        private List<IBindable> _fields = new();

        // ReSharper disable once CollectionNeverQueried.Local
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [SerializeReference]
        private List<IBindable> _fieldLists = new();

        // ReSharper disable once CollectionNeverQueried.Local
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [SerializeReference]
        private List<IBindable> _modifiableFields = new();

        // ReSharper disable once CollectionNeverQueried.Local
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [SerializeReference]
        private List<IBindable> _properties = new();

        // ReSharper disable once CollectionNeverQueried.Local
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [SerializeReference]
        private List<IBindable> _resources = new();

        // ReSharper disable once CollectionNeverQueried.Local
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [SerializeReference]
        private List<IBindable> _states = new();

        private static readonly Type FieldType = typeof(IField<>);
        private static readonly Type FieldListType = typeof(IFieldList<>);
        private static readonly Type ModifiableFieldType = typeof(IModifiableField<>);
        private static readonly Type PropertyType = typeof(IProperty<>);
        private static readonly Type ResourceType = typeof(IResource<>);
        private static readonly Type StateType = typeof(IState);
#endif

        #endregion

        public BindableSystem()
        {
        }

        public BindableSystem(ILogTool parent) => SetLogParent(parent);


        [IgnoreParent] [SerializeField] private ShowDictionaryIndex<Type, IBindable> bindables = new();

        public IReadOnlyList<IBindable> Bindables => bindables.Values;

        private SLogTool? _logger;

        private SLogTool LogTool => _logger ??= new SLogTool(new[]
        {
            nameof(BindableSystem)
        });

        public void SetLogParent(ILogTool value) => LogTool.Parent = value;
        public IBindableRegister GetRegister(IModifiableOwner owner) => new BindableRegister(this, owner);

        public void RegisterBindable(IBindable bindable, IModifiableOwner owner)
        {
            if (bindable is IModifiable modifiable) modifiable.SetOwner(owner);
            RegisterBindable(bindable);
        }

        public void RegisterBindable(IBindable bindable)
        {
            var type = bindable.GetType();
            if (bindables.ContainsKey(type))
            {
                LogTool.Log($"已经存在{type}类型的{nameof(IBindable)}在{nameof(bindables)}中!", ELogType.Warning);
                return;
            }

#if UNITY_EDITOR
            if (FieldType.IsAssignableFrom(type)) _fields.Add(bindable);
            if (FieldListType.IsAssignableFrom(type)) _fieldLists.Add(bindable);
            if (ModifiableFieldType.IsAssignableFrom(type)) _modifiableFields.Add(bindable);
            if (PropertyType.IsAssignableFrom(type)) _properties.Add(bindable);
            if (ResourceType.IsAssignableFrom(type)) _resources.Add(bindable);
            if (StateType.IsAssignableFrom(type)) _states.Add(bindable);
#endif
            bindable.SetLogParent(LogTool);
            bindables.Add(type, bindable);
            _disposeGroup.Add(bindable);
        }

        /// <summary>
        /// 对<see cref="IBindable"/>进行修正
        /// </summary>
        public IDisposable ModifyBindable(IModify modify, IModifyReason reason)
        {
            var type = modify.TryModifyType;
            if (type == null)
            {
                LogTool.Log($"传入的{modify}的{nameof(IModify.TryModifyType)}为空!", ELogType.Warning);
                return modify;
            }

            if (!bindables.TryGetValue(type, out var bindable))
            {
                LogTool.Log($"不存在{type}类型的{nameof(IBindable)}在{nameof(bindables)}中!", ELogType.Warning);
                return modify;
            }

            if (bindable is not IModifiable modifiable)
            {
                LogTool.Log($"{type}类型不为{nameof(IModifiable)}, 无法进行修饰!", ELogType.Error);
                return modify;
            }

            return modifiable.Modify(modify, reason);
        }

        /// <summary>
        /// 获取<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public T? GetBindable<T>() where T : IBindable
        {
            var type = typeof(T);
            if (!bindables.TryGetValue(type, out var bindable))
            {
                LogTool.Log($"不存在{type}类型的{nameof(IBindable)}在{nameof(bindables)}中!", ELogType.Warning);
                return default;
            }

            return (T)bindable;
        }

        /// <summary>
        /// 尝试获取到<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public bool TryGetBindable<T>(out T t) where T : IBindable
        {
            var type = typeof(T);
            if (!bindables.TryGetValue(type, out var bindable))
            {
                LogTool.Log($"不存在{type}类型的{nameof(IBindable)}在{nameof(bindables)}中!", ELogType.Warning);
                t = default!;
                return false;
            }

            t = (T)bindable;
            return true;
        }

        /// <summary>
        /// 是否存在<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public bool ContainsBindable<T>() where T : IBindable => bindables.ContainsKey(typeof(T));

        /// <summary>
        /// 重新检测所有的值
        /// </summary>
        public void ReCheckBindable()
        {
            foreach (var modifiable in bindables.Values.OfType<IModifiable>())
            {
                modifiable.ReCheckValue();
            }
        }

        #region IDisposable

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose() => _disposeGroup.Dispose();

        #endregion

        private class BindableRegister : IBindableRegister
        {
            public BindableRegister(BindableSystem bindableSystem, IModifiableOwner owner) =>
                (_bindableSystem, _owner) = (bindableSystem, owner);

            private readonly BindableSystem _bindableSystem;
            private readonly IModifiableOwner _owner;
            void IBindableRegister.Register<T>(T bindable) => _bindableSystem.RegisterBindable(bindable, _owner);
        }
    }
}