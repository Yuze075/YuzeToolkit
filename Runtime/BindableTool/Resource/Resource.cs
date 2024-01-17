#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.LogTool;


namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IResource{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">资源的数据类型</typeparam>
    [Serializable]
    public abstract class Resource<TValue> : ModifiableBase<TValue, ModifyResource>, IResource<TValue>
        where TValue : unmanaged
    {
        protected Resource(TValue value, bool isReadOnly, IModifiableOwner? modifiableOwner, ILogging? loggingParent) :
            base(isReadOnly, modifiableOwner, loggingParent) => this.value = value;

        ~Resource() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private TValue value;

        public sealed override TValue Value
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                return value;
            }
            protected set
            {
                if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                if (this.value.Equals(value)) return;
                ValueChange?.Invoke(this.value, value);
                this.value = value;
            }
        }

        public abstract TValue Min { get; }
        public abstract TValue Max { get; }

        protected abstract float CastToFloat(TValue value);
        protected abstract TValue CastToTValue(float value);

        #region Modify

        protected sealed override void Modify(ModifyResource modifyResource)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (modifyResource.modifyValue == 0) return;

            var result = CastToFloat(Value) + modifyResource.modifyValue;
            if (result > CastToFloat(Max))
            {
                Value = Max;
                _outOfMaxRange?.Invoke(Max, result);
                return;
            }

            if (result < CastToFloat(Min))
            {
                Value = Min;
                _outOfMinRange?.Invoke(Min, result);
                return;
            }

            Value = CastToTValue(result);
        }

        public EEnoughType Enough(ModifyResource modifyResource)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            var result = CastToFloat(Value) + modifyResource.modifyValue;
            if (result > CastToFloat(Max)) return EEnoughType.OutOfMaxRange;
            if (result < CastToFloat(Min)) return EEnoughType.OutOfMinRange;
            return EEnoughType.IsEnough;
        }

        #endregion

        #region Register

        private OutOfMaxRange<TValue>? _outOfMaxRange;
        private OutOfMinRange<TValue>? _outOfMinRange;

        [return: NotNullIfNotNull("outOfMaxRange")]
        public IDisposable? RegisterOutOfMaxRange(OutOfMaxRange<TValue>? outOfMaxRange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (outOfMaxRange == null) return null;
            _outOfMaxRange += outOfMaxRange;
            return UnRegister.Create(action => _outOfMaxRange -= action, outOfMaxRange);
        }

        [return: NotNullIfNotNull("outOfMinRange")]
        public IDisposable? RegisterOutOfMinRange(OutOfMinRange<TValue>? outOfMinRange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (outOfMinRange == null) return null;
            _outOfMinRange += outOfMinRange;
            return UnRegister.Create(action => _outOfMinRange -= action, outOfMinRange);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                value = default;
                _outOfMaxRange = null;
                _outOfMinRange = null;
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行资源的计算
    /// </summary>
    [Serializable]
    public class ResourceFloat : Resource<float>
    {
        public ResourceFloat(float value = default, bool isReadOnly = true, IModifiableOwner? modifiableOwner = null,
            ILogging? loggingParent = null) : base(value, isReadOnly, modifiableOwner, loggingParent)
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
        protected ResourceInt(int value = default, bool isReadOnly = true, IModifiableOwner? modifiableOwner = null,
            ILogging? loggingParent = null) : base(value, isReadOnly, modifiableOwner, loggingParent)
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