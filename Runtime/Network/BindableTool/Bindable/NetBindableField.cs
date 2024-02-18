#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using Unity.Netcode;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc cref="IBindableField{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    [Serializable]
    public class NetBindableField<TValue> : NetworkVariable<TValue>, IBindableField<TValue>
    {
        public NetBindableField(NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(default!, readPerm, writePerm) => OnValueChanged = InvokeValueChanged;

        public NetBindableField(TValue? valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(valueBase!, readPerm, writePerm) => OnValueChanged = InvokeValueChanged;


        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="NetBindableField{TValue}"/>
        /// </summary>
        public void SetOnly(TValue? value) => base.Value = value!;

        private ValueChange<TValue>? _valueChange;

        public new TValue? Value
        {
            get => base.Value;
            set => base.Value = value!;
        }

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
            base.Value = default!;
            _valueChange = null;
        }
        
        public static implicit operator TValue?(NetBindableField<TValue> netBindableField) => netBindableField.Value;
    }
}
#endif