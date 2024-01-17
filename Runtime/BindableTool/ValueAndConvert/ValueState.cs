#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    [Serializable]
    public sealed class ValueState : ModifiableBase<bool, ModifyState>, IState
    {
        public ValueState(bool value, int priority = 0, IModifiableOwner? modifiableOwner = null,
            ILogging? loggingParent = null) : base(true, modifiableOwner, loggingParent)
        {
            this.value = value;
            this.priority = priority;
        }

        [SerializeField] private int priority;
        [SerializeField] private bool value;

        public override bool Value
        {
            get => value;
            protected set => this.value = value;
        }

        protected override void Modify(ModifyState modify)
        {
            throw new NotImplementedException();
        }

        public void RemoveModify(ModifyState modifyState)
        {
            throw new NotImplementedException();
        }

        public void ReCheckValue()
        {
        }

        [return: NotNullIfNotNull("valueChange")]
        public override IDisposable? RegisterChange(ValueChange<bool>? valueChange) =>
            valueChange == null ? null : DisposeGroup.Null;

        [return: NotNullIfNotNull("valueChange")]
        public override IDisposable? RegisterChangeBuff(ValueChange<bool>? valueChange)
        {
            if (valueChange == null) return null;
            valueChange.Invoke(false, value);
            return DisposeGroup.Null;
        }

        public bool this[int priority, bool removeSmall = true] => priority <= this.priority ? Value : removeSmall;

        public static explicit operator ValueState(bool value) => new(value);
    }
}