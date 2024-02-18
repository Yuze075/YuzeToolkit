#nullable enable
using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IBindableField{TValue}" />
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    [Serializable]
    public class BindableField<TValue> : IBindableField<TValue>
    {
        public BindableField()
        {
        }

        public BindableField(TValue? value) => this.value = value;

        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="BindableField{TValue}"/>
        /// </summary>
        public void SetOnly(TValue? value) => this.value = value;

        [UnityEngine.SerializeField] private TValue? value;
        private ValueChange<TValue>? _valueChange;

        public TValue? Value
        {
            get => value;
            set
            {
                if (this.value != null && this.value.Equals(value)) return;
                _valueChange?.Invoke(this.value, value);
                this.value = value;
            }
        }

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
            value = default;
        }

        public static implicit operator TValue?(BindableField<TValue> bindableField) => bindableField.Value;
    }
}