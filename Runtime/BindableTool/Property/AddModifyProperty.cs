using System;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 加值修饰, 和原来值进行相加
    /// </summary>
    [Serializable]
    public sealed class AddModifyProperty : ModifyProperty
    {
        public AddModifyProperty(Type tryModifyType, float addValue) : base(tryModifyType) =>
            this.addValue = addValue;

        public AddModifyProperty(Type tryModifyType, int priority, float addValue) : base(tryModifyType, priority) =>
            this.addValue = addValue;

        [SerializeField] private float addValue;

        /// <summary>
        /// 加值修正值
        /// </summary>
        public float AddValue
        {
            get => addValue;
            set
            {
                if (addValue - value < 0.00001f) return;
                addValue = value;
                ReCheckValue();
            }
        }
    }
}