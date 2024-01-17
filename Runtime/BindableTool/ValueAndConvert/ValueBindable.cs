#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public sealed class ValueBindable<TValue> : BindableBase<TValue>
    {
        public ValueBindable(TValue value, ILogging? loggingParent = null) : base(loggingParent) =>
            this.value = value;

        [SerializeField] private TValue? value;

        public override TValue? Value
        {
            get => value;
            protected set => this.value = value;
        }

        [return: NotNullIfNotNull("valueChange")]
        public override IDisposable? RegisterChange(ValueChange<TValue>? valueChange) =>
            valueChange == null ? null : DisposeGroup.Null;

        [return: NotNullIfNotNull("valueChange")]
        public override IDisposable? RegisterChangeBuff(ValueChange<TValue>? valueChange)
        {
            if (valueChange == null) return null;
            valueChange.Invoke(default, value);
            return DisposeGroup.Null;
        }

        public static explicit operator ValueBindable<TValue>(TValue value) => new(value);
    }
}