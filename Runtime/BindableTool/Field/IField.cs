namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IBindable{TValue}" />
    /// 用于共享可读可写字段显示<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IField<TValue> : IBindable<TValue>
    {
        /// <summary>
        /// 最终的变量值
        /// </summary>
        new TValue? Value { get; set; }
    }
}