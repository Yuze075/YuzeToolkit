using System;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit;


namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IResource{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">资源的数据类型</typeparam>
    [Serializable]
    public abstract class Resource<TValue> : IResource<TValue>
    {
        protected Resource(TValue value) => this.value = value;

        private SLogTool? _sLogTool;
        private IModifiableOwner? _owner;

        private SLogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(IResource<TValue>),
            GetType().FullName
        });

        void IBindable.SetLogParent(ILogTool value) => LogTool.Parent = value;

        IModifiableOwner IModifiable.Owner => LogTool.IsNotNull(_owner);

        void IModifiable.SetOwner(IModifiableOwner value)
        {
            if (_owner != null)
                LogTool.Log(
                    $"类型为{GetType()}的{nameof(IModifiable)}的{nameof(_owner)}从{_owner.GetType()}替换为{value.GetType()}",
                    ELogType.Warning);
            _owner = value;
        }

        [SerializeField] private TValue value;
        [SerializeField] private bool isReadOnly;

        public TValue Value
        {
            get => value;
            protected set
            {
                if (this.value != null && this.value.Equals(value)) return;
                _valueChange?.Invoke(this.value, value);
                this.value = value;
            }
        }

        public bool IsReadOnly => isReadOnly;
        public abstract TValue Min { get; }
        public abstract TValue Max { get; }

        protected abstract float CastToFloat(TValue value);
        protected abstract TValue CastToTValue(float value);

        #region Modify

        IDisposable IModifiable.Modify(IModify modify, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modify;

            if (!this.TryCastModify(modify, LogTool, out ModifyResource modifyIn)) return modify;

            if (!this.TryCheckModify(modifyIn, reason, out var modifyOut))
                return modifyIn;

            return Modify(modifyOut);
        }

        IDisposable IResource<TValue>.Modify(ModifyResource modifyResource, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modifyResource;

            if (!this.TryCheckModify(modifyResource, reason, out var modifyOut))
                return modifyResource;

            return Modify(modifyOut);
        }

        private IDisposable Modify(ModifyResource modifyResource)
        {
            if (!this.TryCheckModifyType(modifyResource, LogTool)) return modifyResource;

            if (modifyResource.modifyValue == 0) return modifyResource;

            var result = CastToFloat(Value) + modifyResource.modifyValue;
            if (result > CastToFloat(Max))
            {
                Value = Max;
                _outOfMaxRange?.Invoke(Max, result);
                return modifyResource;
            }

            if (result < CastToFloat(Min))
            {
                Value = Min;
                _outOfMinRange?.Invoke(Min, result);
                return modifyResource;
            }

            Value = CastToTValue(result);
            return modifyResource;
        }

        void IModifiable.ReCheckValue()
        {
        }

        public EEnoughType Enough(ModifyResource modifyResource)
        {
            var result = CastToFloat(Value) + modifyResource.modifyValue;
            if (result > CastToFloat(Max)) return EEnoughType.OutOfMaxRange;
            if (result < CastToFloat(Min)) return EEnoughType.OutOfMinRange;
            return EEnoughType.IsEnough;
        }

        #endregion

        #region Register

        private ValueChange<TValue>? _valueChange;
        private OutOfMaxRange<TValue>? _outOfMaxRange;
        private OutOfMinRange<TValue>? _outOfMinRange;

        public IDisposable RegisterChange(ValueChange<TValue> valueChange)
        {
            _valueChange += valueChange;
            return _disposeGroup.UnRegister(() => { _valueChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<TValue> valueChange)
        {
            valueChange.Invoke(default, Value);
            return RegisterChange(valueChange);
        }

        public IDisposable RegisterOutOfMaxRange(OutOfMaxRange<TValue> outOfMaxRange)
        {
            _outOfMaxRange += outOfMaxRange;
            return _disposeGroup.UnRegister(() => { _outOfMaxRange -= outOfMaxRange; });
        }

        public IDisposable RegisterOutOfMinRange(OutOfMinRange<TValue> outOfMinRange)
        {
            _outOfMinRange += outOfMinRange;
            return _disposeGroup.UnRegister(() => { _outOfMinRange -= outOfMinRange; });
        }

        #endregion

        #region IDisposable

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose() => _disposeGroup.Dispose();

        #endregion
    }
}