using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行资源的计算
    /// </summary>
    [Serializable]
    public class ResourceFloat : Resource<float>
    {
        protected ResourceFloat(float value) : base(value)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override float CastToFloat(float value) => value;
        protected sealed override float CastToTValue(float value) => value;
    }
}