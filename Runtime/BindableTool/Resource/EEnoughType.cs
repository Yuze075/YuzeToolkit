namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// 用于判断是否<see cref="IResource{TValue}"/>中资源是否足够
    /// </summary>
    public enum EEnoughType
    {
        /// <summary>
        /// 超出最小值
        /// </summary>
        OutOfMinRange,

        /// <summary>
        /// 足够
        /// </summary>
        IsEnough,

        /// <summary>
        /// 超出最大值
        /// </summary>
        OutOfMaxRange
    }
}