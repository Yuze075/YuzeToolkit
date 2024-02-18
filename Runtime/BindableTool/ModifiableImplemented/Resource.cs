#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// 对<see cref="IResource{TValue,TModifyResource}"/>的<see cref="IResource{TValue,TModifyResource}.Value"/>进行修饰<br/>
    /// 对其对应值进行加, 或者减的操作
    /// </summary>
    [Serializable]
    public struct ModifyResource
    {
        public ModifyResource(float modifyValue) => this.modifyValue = modifyValue;
        [UnityEngine.SerializeField] public float modifyValue;
    }

    /// <summary>
    /// <inheritdoc cref="IResource{TValue,TModifyResource}" />
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    [Serializable]
    public abstract class Resource<TValue> : IResource<TValue, ModifyResource>
        where TValue : unmanaged
    {
        protected Resource()
        {
        }

        protected Resource(TValue value, IModifiableOwner? modifiableOwner = null)
        {
            this.value = value;
            ModifiableOwner = modifiableOwner;
        }

        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="Resource{TValue}"/>
        /// </summary>
        public void SetOnly(TValue value, IModifiableOwner? modifiableOwner = null)
        {
            this.value = value;
            ModifiableOwner = modifiableOwner;
        }

        [SerializeField] private TValue value;
        private ValueChange<TValue>? _valueChange;
        private OutOfMaxRange<TValue>? _outOfMaxRange;
        private OutOfMinRange<TValue>? _outOfMinRange;
        public IModifiableOwner? ModifiableOwner { get; private set; }

        public TValue Value
        {
            get => value;
            private set
            {
                if (this.value.Equals(value)) return;
                _valueChange?.Invoke(this.value, value);
                this.value = value;
            }
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
            value = default;
        }

        public static implicit operator TValue(Resource<TValue> resource) => resource.Value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行资源的计算
    /// </summary>
    [Serializable]
    public class ResourceFloat : Resource<float>
    {
        protected ResourceFloat()
        {
        }

        public ResourceFloat(float value, IModifiableOwner? modifiableOwner = null) : base(value, modifiableOwner)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override float CastToFloat(float value) => value;
        protected sealed override float CastToTValue(float value) => value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行资源的计算
    /// </summary>
    [Serializable]
    public abstract class ResourceInt : Resource<int>
    {
        protected ResourceInt()
        {
        }

        protected ResourceInt(int value, IModifiableOwner? modifiableOwner = null) : base(value, modifiableOwner)
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
}