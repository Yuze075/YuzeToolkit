#nullable enable
namespace YuzeToolkit.BindableTool
{
    public delegate void ValueChange<in TValue>(TValue? oldValue, TValue? newValue);

    /// <summary>
    /// 只读可绑定变量底层接口, 包括的<see cref="Reset"/>方法<br/><br/>
    /// </summary>
    public interface IReadOnlyBindable
    {
        void Reset();
    }

    /// <summary>
    /// 只读的可绑定变量的泛型<see cref="TValue"/>接口<br/>
    /// 用于获取<see cref="Value"/>值, 和注册<see cref="Value"/>的改变回调<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IReadOnlyBindable<out TValue> : IReadOnlyBindable
    {
        /// <summary>
        /// 只读的泛型变量
        /// </summary>
        TValue? Value { get; }

        void AddValueChange(ValueChange<TValue>? valueChange);
        void RemoveValueChange(ValueChange<TValue>? valueChange);
    }
}