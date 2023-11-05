using System;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 与值修饰, 和原来值进行相与
    /// </summary>
    [Serializable]
    public sealed class AndModifyState : ModifyState
    {
        public AndModifyState(Type tryModifyType, bool andValue) : base(tryModifyType) => this.andValue = andValue;

        public AndModifyState(Type tryModifyType, int priority, bool andValue) : base(tryModifyType, priority) =>
            this.andValue = andValue;

        [SerializeField] private bool andValue;

        /// <summary>
        /// 与值修正
        /// </summary>
        public bool AndValue
        {
            get => andValue;
            set
            {
                if (andValue == value) return;
                andValue = value;
                ReCheckValue();
            }
        }
    }
}