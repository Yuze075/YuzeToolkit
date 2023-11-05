using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IProperty{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class Property<TValue> : IProperty<TValue>
    {
        protected Property(TValue valueBase)
        {
            this.valueBase = valueBase;
            value = valueBase;
        }

        private SLogTool? _sLogTool;
        private IModifiableOwner? _owner;

        private SLogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(IProperty<TValue>),
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
        [SerializeField] private TValue valueBase;
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
                if (modifyProperties.Remove(modifyProperty))
                    ReCheckValue();
            }), this);
            ReCheckValue();
            _disposeGroup.Add(modifyProperty);
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
            return _disposeGroup.UnRegister(() => { _valueChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(ValueChange<TValue> valueChange)
        {
            valueChange.Invoke(default, Value);
            return RegisterChange(valueChange);
        }

        #endregion

        #region IDisposable

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose()
        {
            Value = valueBase;
            _disposeGroup.Dispose();
        }

        #endregion
    }
}