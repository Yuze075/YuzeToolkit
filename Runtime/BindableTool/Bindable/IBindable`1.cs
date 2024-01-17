#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 由一个<see cref="Value"/>的只读数据<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IBindable<out TValue> : IBindable
    {
        /// <summary>
        /// 最终的变量值
        /// </summary>
        new TValue? Value { get; }

        /// <summary>
        /// 注册数值改变的回调, 获取到旧数值和新数值
        /// </summary>
        [return: NotNullIfNotNull("valueChange")]
        IDisposable? RegisterChange(ValueChange<TValue>? valueChange);

        /// <summary>
        /// 注册数值改变的回调, 获取到旧数值和新数值, 并且注册时就获得缓存的数值
        /// </summary>
        [return: NotNullIfNotNull("valueChange")]
        IDisposable? RegisterChangeBuff(ValueChange<TValue>? valueChange);

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindable.RegisterChange(ValueChange<object>? valueChange) => RegisterChange(valueChange);

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindable.RegisterChangeBuff(ValueChange<object>? valueChange) => RegisterChangeBuff(valueChange);
    }
}