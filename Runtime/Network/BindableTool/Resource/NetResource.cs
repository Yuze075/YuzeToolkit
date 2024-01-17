#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using YuzeToolkit.LogTool;


namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="IResource{TValue}" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">资源的数据类型</typeparam>
    [Serializable]
    public abstract class NetResource<TValue> : NetModifiableBase<TValue, ModifyResource>, IResource<TValue>
        where TValue : unmanaged
    {
        protected NetResource(TValue valueBase, bool isReadOnly, NetworkVariableReadPermission readPerm,
            NetworkVariableWritePermission writePerm, IModifiableOwner? modifiableOwner,
            ILogging? loggingParent) : base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
            // if (!GetBehaviour().IsServer)
            // {
            //     OnValueChanged += (value, newValue) =>
            //     {
            //         if (value.Equals(Max) && newValue.Equals(Max))
            //         {
            //             _outOfMaxRange.Invoke(Max, CastToFloat(Max));
            //         }
            //
            //         if (value.Equals(Min) && newValue.Equals(Min))
            //         {
            //             _outOfMinRange.Invoke(Min, CastToFloat(Min));
            //         }
            //     };
            // }
        }

        ~NetResource() => Dispose(false);
        [NonSerialized] private bool _disposed;

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
                _outOfMaxRange = null;
                _outOfMinRange = null;
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
#endif