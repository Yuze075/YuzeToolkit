using System;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 乘修饰, 和原来值进行相乘
    /// </summary>
    [Serializable]
    public sealed class MultModifyProperty : ModifyProperty
    {
        public MultModifyProperty(Type tryModifyType, float multValue) : base(tryModifyType) =>
            this.multValue = multValue;

        public MultModifyProperty(Type tryModifyType, int priority, float multValue) : base(tryModifyType, priority) =>
            this.multValue = multValue;

        [SerializeField] private float multValue;

        /// <summary>
        /// 乘值修正值
        /// </summary>
        public float MultValue
        {
            get => multValue;
            set
            {
                if (multValue - value < 0.00001f) return;
                multValue = value;
                ReCheckValue();
            }
        }
    }
}