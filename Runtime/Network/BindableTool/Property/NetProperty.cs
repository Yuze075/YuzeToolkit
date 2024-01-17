#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="IProperty{TValue}" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class NetProperty<TValue> : NetModifiableBase<TValue, ModifyProperty>, IProperty<TValue>
        where TValue : unmanaged
    {
        protected NetProperty(TValue valueBase, bool isReadOnly, NetworkVariableReadPermission readPerm,
            NetworkVariableWritePermission writePerm, IModifiableOwner? modifiableOwner,
            ILogging? loggingParent) : base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
            this.valueBase = valueBase;
        }

        ~NetProperty() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [NonSerialized] private bool _isDisposing;

        [SerializeField] private TValue valueBase;

        public override TValue Value
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return BaseValue;
            }
            protected set
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                if (BaseValue.Equals(value)) return;
                BaseValue = value;
            }
        }

        public abstract TValue Min { get; }
        public abstract TValue Max { get; }
        public virtual EOutOfRangeType OutOfRangeType => EOutOfRangeType.None;
        protected abstract double CastToDouble(TValue value);
        protected abstract TValue CastToTValue(double value);

        #region Modify

        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false)] [ReferencePicker] [SerializeReference]
        private List<ModifyProperty>? modifyProperties;

        private List<ModifyProperty> ModifyProperties => modifyProperties ??= new List<ModifyProperty>();

        protected sealed override void Modify(ModifyProperty modifyProperty)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var index = ModifyProperties.BinarySearch(modifyProperty);
            ModifyProperties.Insert(index >= 0 ? index : ~index, modifyProperty);
            ReCheckValue();
        }

        void IProperty.RemoveModify(ModifyProperty modifyProperty)
        {
            if (_disposed || _isDisposing) return;
            if (modifyProperties != null && modifyProperties.Remove(modifyProperty)) ReCheckValue();
        }

        public void ReCheckValue()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var retValue = CastToDouble(valueBase);
            if (ModifyProperties.Count == 0)
            {
                Value = CastToTValue(retValue);
                return;
            }

            var priority = ModifyProperties[0].Priority;
            double addValue = 0;
            double multValue = 0;

            foreach (var modify in ModifyProperties)
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
            if (OutOfRangeType == EOutOfRangeType.None) return false;

            if (beCheckValue < CastToDouble(Min))
            {
                beCheckValue = CastToDouble(Min);
                if (OutOfRangeType == EOutOfRangeType.Stop) return true;
            }

            if (beCheckValue > CastToDouble(Max))
            {
                beCheckValue = CastToDouble(Max);
                if (OutOfRangeType == EOutOfRangeType.Stop) return true;
            }

            return false;
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (modifyProperties != null)
                {
                    if (disposing)
                    {
                        _isDisposing = true;
                        for (var i = modifyProperties.Count - 1; i >= 0; i--) modifyProperties[i].Dispose();
                    }

                    modifyProperties.Clear();
                }

                valueBase = default;
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
#endif