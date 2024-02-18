#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="IProperty{TValue}"/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class NetProperty<TValue> : NetworkVariable<TValue>, IProperty<TValue, IModifyProperty>
        where TValue : unmanaged
    {
        protected NetProperty(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(default!, readPerm, writePerm)
        {
            OnValueChanged = InvokeValueChanged;
        }

        protected NetProperty(TValue valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, readPerm, writePerm)
        {
            this.valueBase = valueBase;
            ModifiableOwner = modifiableOwner;
            OnValueChanged = InvokeValueChanged;
        }

        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="NetProperty{TValue}"/>
        /// </summary>
        public void SetOnly(TValue valueBase, IModifiableOwner? modifiableOwner = null)
        {
            OnValueChanged = null;
            this.valueBase = valueBase;
            base.Value = valueBase;
            ModifiableOwner = modifiableOwner;
            OnValueChanged = InvokeValueChanged;
        }

        [SerializeField] private TValue valueBase;
        private ValueChange<TValue>? _valueChange;
        public IModifiableOwner? ModifiableOwner { get; private set; }

        public new TValue Value
        {
            get => base.Value;
            private set => base.Value = value;
        }

        public abstract TValue Min { get; }
        public abstract TValue Max { get; }
        public virtual EOutOfRangeMode OutOfRangeMode => EOutOfRangeMode.None;
        protected abstract double CastToDouble(TValue value);
        protected abstract TValue CastToTValue(double value);

        #region Modify

#if YUZE_USE_EDITOR_TOOLBOX
        [ReorderableList(fixedSize: true, draggable: false), ReferencePicker]
#endif
        [SerializeReference]
        private List<IModifyProperty>? _modifyProperties;

        private List<IModifyProperty> ModifyProperties => _modifyProperties ??= new List<IModifyProperty>();

        public bool Modify(IModifyProperty modify, object? sender, object? reason = null)
        {
            if (!this.CheckModify(modify, out var modifyProperty, sender, reason)) return false;

            var index = ModifyProperties.BinarySearch(modifyProperty);
            ModifyProperties.Insert(index >= 0 ? index : ~index, modifyProperty);
            ReCheckValue();
            return true;
        }


        public void RemoveModify(IModifyProperty modifyProperty)
        {
            if (_modifyProperties != null && _modifyProperties.Remove(modifyProperty)) ReCheckValue();
        }

        public void ReCheckValue()
        {
            var retValue = CastToDouble(valueBase);
            if (_modifyProperties == null || _modifyProperties.Count == 0)
            {
                Value = CastToTValue(retValue);
                return;
            }

            var priority = _modifyProperties[0].Priority;
            double addValue = 0;
            double multValue = 0;

            foreach (var modifyProperty in _modifyProperties)
            {
                if (modifyProperty.Priority != priority)
                {
                    retValue += addValue;
                    retValue *= 1 + multValue;
                    if (CheckOutOfRange(ref retValue))
                    {
                        Value = CastToTValue(retValue);
                        return;
                    }

                    priority = modifyProperty.Priority;
                    addValue = 0;
                    multValue = 0;
                }

                switch (modifyProperty.ModifyPropertyType)
                {
                    case EModifyPropertyType.Add:
                        addValue += modifyProperty.ModifyValue;
                        break;
                    case EModifyPropertyType.Multiple:
                        multValue += modifyProperty.ModifyValue;
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
            if (OutOfRangeMode == EOutOfRangeMode.None) return false;

            if (beCheckValue < CastToDouble(Min))
            {
                beCheckValue = CastToDouble(Min);
                if (OutOfRangeMode == EOutOfRangeMode.Stop) return true;
            }

            if (beCheckValue > CastToDouble(Max))
            {
                beCheckValue = CastToDouble(Max);
                if (OutOfRangeMode == EOutOfRangeMode.Stop) return true;
            }

            return false;
        }

        #endregion

        private void InvokeValueChanged(TValue value, TValue newValue) => _valueChange?.Invoke(value, newValue);

        public void AddValueChange(ValueChange<TValue>? valueChange)
        {
            if (valueChange != null) _valueChange += valueChange;
        }

        public void RemoveValueChange(ValueChange<TValue>? valueChange)
        {
            if (valueChange != null) _valueChange -= valueChange;
        }

        public void Reset()
        {
            _valueChange = null;
            ModifiableOwner = null;
            valueBase = default;
            base.Value = default;
            _modifyProperties?.Clear();
        }

        public static implicit operator TValue(NetProperty<TValue> netProperty) => netProperty.Value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="byte"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyByte : NetProperty<byte>
    {
        protected NetPropertyByte(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetPropertyByte(byte valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override byte Min => byte.MinValue;
        public override byte Max => byte.MaxValue;
        protected override double CastToDouble(byte value) => value;

        protected override byte CastToTValue(double value) => value switch
        {
            >= byte.MaxValue => byte.MaxValue,
            <= byte.MinValue => byte.MinValue,
            _ => (byte)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="sbyte"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertySByte : NetProperty<sbyte>
    {
        protected NetPropertySByte(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetPropertySByte(sbyte valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override sbyte Min => sbyte.MinValue;
        public override sbyte Max => sbyte.MaxValue;
        protected sealed override double CastToDouble(sbyte value) => value;

        protected sealed override sbyte CastToTValue(double value) => value switch
        {
            >= sbyte.MaxValue => sbyte.MaxValue,
            <= sbyte.MinValue => sbyte.MinValue,
            _ => (sbyte)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="short"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyShort : NetProperty<short>
    {
        protected NetPropertyShort(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetPropertyShort(short valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override short Min => short.MinValue;
        public override short Max => short.MaxValue;
        protected sealed override double CastToDouble(short value) => value;

        protected sealed override short CastToTValue(double value) => value switch
        {
            >= short.MaxValue => short.MaxValue,
            <= short.MinValue => short.MinValue,
            _ => (short)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="ushort"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyUShort : NetProperty<ushort>
    {
        protected NetPropertyUShort(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetPropertyUShort(ushort valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override ushort Min => ushort.MinValue;
        public override ushort Max => ushort.MaxValue;
        protected sealed override double CastToDouble(ushort value) => value;

        protected sealed override ushort CastToTValue(double value) => value switch
        {
            >= ushort.MaxValue => ushort.MaxValue,
            <= ushort.MinValue => ushort.MinValue,
            _ => (ushort)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyInt : NetProperty<int>
    {
        protected NetPropertyInt(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetPropertyInt(int valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override int Min => int.MinValue;
        public override int Max => int.MaxValue;
        protected sealed override double CastToDouble(int value) => value;

        protected sealed override int CastToTValue(double value) => value switch
        {
            >= int.MaxValue => int.MaxValue,
            <= int.MinValue => int.MinValue,
            _ => (int)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyFloat : NetProperty<float>
    {
        protected NetPropertyFloat(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetPropertyFloat(float valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override double CastToDouble(float value) => value;

        protected sealed override float CastToTValue(double value) => value switch
        {
            >= float.MaxValue => float.MaxValue,
            <= float.MinValue => float.MinValue,
            _ => (float)value
        };
    }
}
#endif