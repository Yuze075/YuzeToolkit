using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行资源的计算
    /// </summary>
    [Serializable]
    public abstract class ResourceInt : Resource<int>
    {
        protected ResourceInt(int value) : base(value)
        {
        }

        public override int Min => int.MinValue;
        public override int Max => int.MaxValue;
        protected sealed override float CastToFloat(int value) => value;

        protected sealed override int CastToTValue(float value) => value switch
        {
            >= int.MaxValue => int.MaxValue,
            <= int.MinValue => int.MinValue,
            _ => (int)value
        };
    }
}