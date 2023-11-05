using System;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit.SerializeTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 对<see cref="IProperty{TValue}"/>的状态值<see cref="IProperty{TValue}.Value"/>的修饰<br/>
    /// 优先级越高越后修正, 在同一优先级先进行所有的<see cref="AddModifyProperty"/>修正, 再进行所有的<see cref="MultModifyProperty"/>修正<br/><br/>
    /// </summary>
    [Serializable]
    public abstract class ModifyProperty : IModify
    {
        protected ModifyProperty(Type tryModifyType) => this.tryModifyType = tryModifyType;
        protected ModifyProperty(Type tryModifyType, int priority) : this(tryModifyType) => this.priority = priority;

        [SerializeField] private int priority;

        [SerializeField] [TypeSelector(typeof(IProperty<>), TypeSetting = ETypeSetting.AllowNotPublic)]
        private SerializeType tryModifyType;

        public Type? TryModifyType => tryModifyType;

        /// <summary>
        /// 修改<see cref="IProperty{TValue}"/>的优先级, 优先级越高, 越后修改, 升序排列<br/>
        /// 在同一优先级先进行所有的<see cref="AddModifyProperty"/>修正, 再进行所有的<see cref="MultModifyProperty"/>修正
        /// </summary>
        public int Priority => priority;

        private IDisposable? _disposable;
        private IModifiable? _property;
        private bool _isInit;

        protected void ReCheckValue() => _property?.ReCheckValue();

        public void Init(IDisposable disposable, IModifiable property)
        {
            if (_isInit)
            {
                LogSys.Log(
                    $"重复初始化修正变量{GetType()}, 当前修正Property: {_property?.GetType()}, 新增修正Property: {property.GetType()}");
                return;
            }

            _isInit = true;
            _disposable = disposable;
            _property = property;
        }

        public void Dispose()
        {
            if (!_isInit) return;
            _isInit = false;
            _disposable?.Dispose();
            _disposable = null;
            _property = null;
        }
    }
}