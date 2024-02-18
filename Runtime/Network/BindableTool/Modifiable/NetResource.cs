#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="IResource{TValue,TModifyResource}" />
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    [Serializable]
    public abstract class NetResource<TValue> : NetworkVariable<TValue>, IResource<TValue, ModifyResource>
        where TValue : unmanaged
    {
        protected NetResource(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(default!, readPerm, writePerm)
        {
            OnValueChanged = InvokeValueChanged;
        }

        protected NetResource(TValue valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, readPerm, writePerm)
        {
            OnValueChanged = InvokeValueChanged;
        }

        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="NetResource{TValue}"/>
        /// </summary>
        public void SetOnly(TValue value, IModifiableOwner? modifiableOwner = null)
        {
            OnValueChanged = null;
            base.Value = value;
            ModifiableOwner = modifiableOwner;
            OnValueChanged = InvokeValueChanged;
        }

        private ValueChange<TValue>? _valueChange;
        private OutOfMaxRange<TValue>? _outOfMaxRange;
        private OutOfMinRange<TValue>? _outOfMinRange;
        public IModifiableOwner? ModifiableOwner { get; private set; }

        public new TValue Value
        {
            get => base.Value;
            private set => base.Value = value;
        }

        public abstract TValue Min { get; }
        public abstract TValue Max { get; }

        protected abstract float CastToFloat(TValue value);
        protected abstract TValue CastToTValue(float value);

        #region Modify

        public bool Modify(ModifyResource modify, object? sender, object? reason = null)
        {
            if (!this.CheckModify(modify, out var modifyResource, sender, reason)) return false;

            if (modifyResource.modifyValue == 0) return true;

            var result = CastToFloat(Value) + modifyResource.modifyValue;
            if (result > CastToFloat(Max))
            {
                Value = Max;
                _outOfMaxRange?.Invoke(Max, result);
                return true;
            }

            if (result < CastToFloat(Min))
            {
                Value = Min;
                _outOfMinRange?.Invoke(Min, result);
                return true;
            }

            Value = CastToTValue(result);
            return true;
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

        private void InvokeValueChanged(TValue value, TValue newValue) => _valueChange?.Invoke(value, newValue);

        public void AddOutOfMaxRange(OutOfMaxRange<TValue>? outOfMaxRange)
        {
            if (outOfMaxRange != null) _outOfMaxRange += outOfMaxRange;
        }

        public void RemoveOutOfMaxRange(OutOfMaxRange<TValue>? outOfMaxRange)
        {
            if (outOfMaxRange != null) _outOfMaxRange -= outOfMaxRange;
        }

        public void AddOutOfMinRange(OutOfMinRange<TValue>? outOfMinRange)
        {
            if (outOfMinRange != null) _outOfMinRange += outOfMinRange;
        }

        public void RemoveOutOfMinRange(OutOfMinRange<TValue>? outOfMinRange)
        {
            if (outOfMinRange != null) _outOfMinRange -= outOfMinRange;
        }

        public void AddValueChange(ValueChange<TValue>? valueChange)
        {
            if (valueChange != null) _valueChange += valueChange;
        }

        public void RemoveValueChange(ValueChange<TValue>? valueChange)
        {
            if (valueChange != null) _valueChange -= valueChange;
        }

        #endregion

        public void Reset()
        {
            _outOfMaxRange = null;
            _outOfMinRange = null;
            _valueChange = null;
            ModifiableOwner = null;
            base.Value = default;
        }
        
        public static implicit operator TValue(NetResource<TValue> netResource) => netResource.Value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="byte"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceByte : NetResource<byte>
    {
        protected NetResourceByte(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetResourceByte(byte valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override byte Min => byte.MinValue;
        public override byte Max => byte.MaxValue;
        protected sealed override float CastToFloat(byte value) => value;

        protected sealed override byte CastToTValue(float value) => value switch
        {
            >= byte.MaxValue => byte.MaxValue,
            <= byte.MinValue => byte.MinValue,
            _ => (byte)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="sbyte"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceSByte : NetResource<sbyte>
    {
        protected NetResourceSByte(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetResourceSByte(sbyte valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override sbyte Min => sbyte.MinValue;
        public override sbyte Max => sbyte.MaxValue;
        protected sealed override float CastToFloat(sbyte value) => value;

        protected sealed override sbyte CastToTValue(float value) => value switch
        {
            >= sbyte.MaxValue => sbyte.MaxValue,
            <= sbyte.MinValue => sbyte.MinValue,
            _ => (sbyte)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="short"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceShort : NetResource<short>
    {
        protected NetResourceShort(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetResourceShort(short valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override short Min => short.MinValue;
        public override short Max => short.MaxValue;
        protected sealed override float CastToFloat(short value) => value;

        protected sealed override short CastToTValue(float value) => value switch
        {
            >= short.MaxValue => short.MaxValue,
            <= short.MinValue => short.MinValue,
            _ => (short)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="ushort"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceUShort : NetResource<ushort>
    {
        protected NetResourceUShort(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetResourceUShort(ushort valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override ushort Min => ushort.MinValue;
        public override ushort Max => ushort.MaxValue;
        protected sealed override float CastToFloat(ushort value) => value;

        protected sealed override ushort CastToTValue(float value) => value switch
        {
            >= ushort.MaxValue => ushort.MaxValue,
            <= ushort.MinValue => ushort.MinValue,
            _ => (ushort)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceInt : NetResource<int>
    {
        protected NetResourceInt(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetResourceInt(int valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override int Min => int.MinValue;
        public override int Max => int.MaxValue;
        protected sealed override float CastToFloat(int value) => value;

        protected sealed override int CastToTValue(float value) => value switch
        {
            >= int.MaxValue => int.MaxValue,
            <= int.MinValue => int.MinValue,
            _ => (int)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceFloat : NetResource<float>
    {
        protected NetResourceFloat(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(readPerm, writePerm)
        {
        }

        protected NetResourceFloat(float valueBase, IModifiableOwner? modifiableOwner = null,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase, modifiableOwner, readPerm, writePerm)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override float CastToFloat(float value) => value;

        protected sealed override float CastToTValue(float value) => value switch
        {
            >= float.MaxValue => float.MaxValue,
            <= float.MinValue => float.MinValue,
            _ => value
        };
    }
}
#endif