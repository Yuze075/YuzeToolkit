#if USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.BindableTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IProperty{TValue}" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class NetworkProperty<TValue> : NetworkVariable<TValue>, IProperty<TValue>
    {
        protected NetworkProperty(TValue valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, readPerm, writePerm)
        {
            this.valueBase = valueBase;
            OnValueChanged += (oldValue, newValue) => { _valueChange?.Invoke(oldValue, newValue); };
        }

        private SLogTool? _sLogTool;
        private IModifiableOwner? _owner;
        protected ILogTool LogTool => _sLogTool ??= SLogTool.Create(GetLogTags);
        protected virtual string[] GetLogTags => new[]
        {
            nameof(IProperty<TValue>),
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

        [SerializeField] private TValue valueBase;
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
        public virtual bool WhenOutOfRangeStop => true;
        protected abstract double CastToDouble(TValue value);
        protected abstract TValue CastToTValue(double value);

        #region Modify

        [SerializeReference] private List<ModifyProperty> modifyProperties = new();

        IDisposable IModifiable.Modify(IModify modify, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modify;

            if (!this.TryCastModify(modify, LogTool, out ModifyProperty modifyIn)) return modify;

            if (!this.TryCheckModify(modifyIn, reason, out var modifyOut))
                return modifyIn;

            return Modify(modifyOut);
        }

        IDisposable IProperty<TValue>.Modify(ModifyProperty modifyProperty, IModifyReason reason)
        {
            if (!this.IsSameOwnerReason(reason, LogTool)) return modifyProperty;

            if (!this.TryCheckModify(modifyProperty, reason, out var modifyOut))
                return modifyProperty;

            return Modify(modifyOut);
        }

        private IDisposable Modify(ModifyProperty modifyProperty)
        {
            if (!this.TryCheckModifyType(modifyProperty, LogTool)) return modifyProperty;

            var index = modifyProperties.BinarySearch(modifyProperty, ModifyPropertyComparer.Comparer);
            modifyProperties.Insert(index >= 0 ? index : ~index, modifyProperty);

            modifyProperty.Init(new UnRegister(() =>
            {
                if (modifyProperties.Remove(modifyProperty)) ReCheckValue();
            }), this);
            ReCheckValue();
            return modifyProperty;
        }

        public void ReCheckValue()
        {
            var retValue = CastToDouble(valueBase);
            if (modifyProperties.Count == 0)
            {
                Value = CastToTValue(retValue);
                return;
            }

            var priority = modifyProperties[0].Priority;
            double addValue = 0;
            double multValue = 0;

            foreach (var modify in modifyProperties)
            {
                if (modify.Priority != priority)
                {
                    retValue += addValue;
                    retValue *= 1 + multValue;
                    if (CheckOutOfRange(ref retValue))
                    {
                        Value = CastToTValue(retValue);
                        return;
                    }

                    priority = modify.Priority;
                    addValue = 0;
                    multValue = 0;
                }

                switch (modify)
                {
                    case AddModifyProperty addModify:
                        addValue += addModify.AddValue;
                        break;
                    case MultModifyProperty multModify:
                        multValue += multModify.MultValue;
                        break;
                }
            }

            retValue += addValue;
            retValue *= 1 + multValue;

            if (retValue < CastToDouble(Min)) retValue = CastToDouble(Min);
            if (retValue > CastToDouble(Max)) retValue = CastToDouble(Max);
            Value = CastToTValue(retValue);
        }

        private bool CheckOutOfRange(ref double beCheckValue)
        {
            if (!WhenOutOfRangeStop) return false;

            if (beCheckValue < CastToDouble(Min))
            {
                beCheckValue = CastToDouble(Min);
                return true;
            }

            if (beCheckValue > CastToDouble(Max))
            {
                beCheckValue = CastToDouble(Max);
                return true;
            }

            return false;
        }

        #endregion

        #region RegisterChange

        private ValueChange<TValue>? _valueChange;

        public IDisposable RegisterChange(ValueChange<TValue> valueChange)
        {
            _valueChange += valueChange;
            return  new UnRegister(() => { _valueChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<TValue> valueChange)
        {
            valueChange.Invoke(default, Value);
            return RegisterChange(valueChange);
        }

        #endregion

        #region IDisposable
        
        void IDisposable.Dispose()
        {
            SLogTool.Release(ref _sLogTool);
            Value = valueBase;
            modifyProperties.Clear();
            _valueChange = null;
        }

        #endregion
    }
}
#endif