using System;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 或值修饰, 和原来值进行相或
    /// </summary>
    [Serializable]
    public sealed class OrModifyState : ModifyState
    {
        public OrModifyState(Type tryModifyType, bool orValue) : base(tryModifyType) => this.orValue = orValue;

        public OrModifyState(Type tryModifyType, int priority, bool orValue) : base(tryModifyType, priority) =>
            this.orValue = orValue;

        [SerializeField] private bool orValue;

        /// <summary>
        /// 或值修正
        /// </summary>
        public bool OrValue
        {
            get => orValue;
            set
            {
                if (orValue == value) return;
                orValue = value;
                ReCheckValue();
            }
        }
    }
}