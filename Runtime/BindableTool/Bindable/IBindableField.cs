#nullable enable
namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IReadOnlyBindable{TValue}" />
    /// 可读可写的可绑定泛型<see cref="TValue"/>变量<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IBindableField<TValue> : IReadOnlyBindable<TValue>
    {
        new TValue? Value { get; set; }
    }
}