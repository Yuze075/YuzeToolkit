#if USE_UNITY_NETCODE
using System;
using System.Collections.Generic;
using Unity.Netcode;
using YuzeToolkit.BindableTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IField{TValue}" />
    /// 并且通过<see cref="NetworkVariable{T}"/>进行网络变量的同步<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public class NetworkField<TValue> : NetworkVariable<TValue>, IField<TValue>
    {
        public NetworkField(TValue? valueBase = default,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase!, readPerm, writePerm)
        {
            OnValueChanged += (value, newValue) => { _valueChange?.Invoke(value, newValue); };
        }

        private SLogTool? _sLogTool;

        private SLogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(IField<TValue>),
            GetType().FullName
        });

        void IBindable.SetLogParent(ILogTool value) => LogTool.Parent = value;

        /// <summary>
        /// 当前<see cref="IBindable"/>的值
        /// </summary>
        public new TValue? Value
        {
            get => base.Value;
            set
            {
                if (base.Value != null && base.Value.Equals(value)) return;
                base.Value = value!;
            }
        }

        #region Register

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

        private readonly List<IDisposable> _disposables = new();

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose()
        {
            Value = default;
            _disposeGroup.Dispose();
        }

        #endregion
        
        public static implicit operator TValue?(NetworkField<TValue> networkField) => 
            networkField.Value;
    }
}
#endif