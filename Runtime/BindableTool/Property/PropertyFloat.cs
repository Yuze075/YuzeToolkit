using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class PropertyFloat : Property<float>
    {
        protected PropertyFloat(float valueBase) : base(valueBase)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override double CastToDouble(float value) => value;

        protected sealed override float CastToTValue(double value) => value switch
        {
            >= float.MaxValue => float.MaxValue,
            <= float.MinValue => float.MinValue,
            _ => (float)value
        };
    }
}