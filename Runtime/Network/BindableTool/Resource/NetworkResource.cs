#if USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.BindableTool;
using YuzeToolkit.LogTool;


namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IResource{TValue}" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">资源的数据类型</typeparam>
    [Serializable]
    public abstract class NetworkResource<TValue> : NetworkVariable<TValue>, IResource<TValue>
    {
        protected NetworkResource(TValue valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, readPerm, writePerm)
        {
            OnValueChanged += (value, newValue) =>
            {
                _valueChange?.Invoke(value, newValue);
                if (newValue != null && value != null && value.Equals(Max) && newValue.Equals(Max))
                {
                    _outOfMaxRange?.Invoke(Max, CastToFloat(Max));
                }

                if (newValue != null && value != null && value.Equals(Min) && newValue.Equals(Min))
                {
                    _outOfMinRange?.Invoke(Min, CastToFloat(Min));
                }
            };
        }

        private SLogTool? _sLogTool;
        private IModifiableOwner? _owner;
        protected ILogTool LogTool => _sLogTool ??= SLogTool.Create(GetLogTags);

        protected virtual string[] GetLogTags => new[]
        {
            nameof(IResource<TValue>),
            GetType().FullName
        };

        void IBindable.SetLogParent(ILogTool parent) => ((SLogTool)LogTool).Parent = parent;

        IModifiableOwner IModifiable.Owner => LogTool.IsNotNull(_owner);

        void IModifiable.SetOwner(IModifiableOwner value)
        {
            if (_owner != null)
                LogTool.Log(
                    $"类型为{GetType()}的{nameof(IModifiable)}的{nameof(_owner)}从{_owner.GetType()}替换为{value.GetType()}",
                    ELogType.Warning);
            _owner = value;
        }

        [SerializeField] private bool isReadOnly;

        public new TValue Value
        {
            get => base.Value;
            protected set
            {
                if (base.Value != null && base.Value.Equals(value)) return;
                base.Value = value;
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
            return new UnRegister(() => { _valueChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<TValue> valueChange)
        {
            valueChange.Invoke(default, Value);
            return RegisterChange(valueChange);
        }

        public IDisposable RegisterOutOfMaxRange(OutOfMaxRange<TValue> outOfMaxRange)
        {
            _outOfMaxRange += outOfMaxRange;
            return new UnRegister(() => { _outOfMaxRange -= outOfMaxRange; });
        }

        public IDisposable RegisterOutOfMinRange(OutOfMinRange<TValue> outOfMinRange)
        {
            _outOfMinRange += outOfMinRange;
            return new UnRegister(() => { _outOfMinRange -= outOfMinRange; });
        }

        #endregion

        #region IDisposable

        void IDisposable.Dispose()
        {
            SLogTool.Release(ref _sLogTool);
            Value = CastToTValue(default);
            _valueChange = null;
            _outOfMaxRange = null;
            _outOfMinRange = null;
        }

        #endregion
    }
}
#endif