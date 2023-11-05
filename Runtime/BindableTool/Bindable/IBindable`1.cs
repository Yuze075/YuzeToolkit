using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 由一个<see cref="Value"/>的只读数据<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IBindable<out TValue> : IBindable
    {
        object? IBindable.Value => Value;
        IDisposable IBindable.RegisterChange(ValueChange<object> valueChange) => RegisterChange(valueChange);
        IDisposable IBindable.RegisterChangeBuff(ValueChange<object> valueChange) => RegisterChangeBuff(valueChange);

        /// <summary>
        /// 最终的变量值
        /// </summary>
        new TValue? Value { get; }

        /// <summary>
        /// 注册数值改变的回调, 获取到旧数值和新数值
        /// </summary>
        IDisposable RegisterChange(ValueChange<TValue> valueChange);

        /// <summary>
        /// 注册数值改变的回调, 获取到旧数值和新数值, 并且注册时就获得缓存的数值
        /// </summary>
        IDisposable RegisterChangeBuff(ValueChange<TValue> valueChange);
    }

    public delegate void ValueChange<in TValue>(TValue? oldValue, TValue? newValue);
}