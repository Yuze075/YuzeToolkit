#nullable enable
namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 只读的<see cref="object"/>属性接口, 可以了解当前属性的状态<br/><br/>
    /// </summary>
    public interface IReadOnlyProperty : IReadOnlyBindable
    {
        /// <summary>
        /// 当超出范围时的处理逻辑
        /// </summary>
        EOutOfRangeMode OutOfRangeMode { get; }

        /// <summary>
        /// 重新检测对应的属性修正
        /// </summary>
        void ReCheckValue();
    }

    /// <summary>
    /// <inheritdoc cref="IReadOnlyBindable{TValue}" />
    /// 只读的泛型<see cref="TValue"/>属性接口, 可以了解当前属性的状态<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IReadOnlyProperty<out TValue> : IReadOnlyProperty, IReadOnlyBindable<TValue>
    {
        /// <summary>
        /// 数值最小值
        /// </summary>
        TValue Min { get; }

        /// <summary>
        /// 数值最大值
        /// </summary>
        TValue Max { get; }
    }

    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IModifiable{TModify}" />
    /// <see cref="object"/>属性接口, 可以了解当前属性的状态, 并使用<see cref="TModifyProperty"/>对属性进行修正<br/><br/>
    /// </summary>
    /// <typeparam name="TModifyProperty">指定的可以修饰自身的类型</typeparam>
    public interface IProperty<in TModifyProperty> : IReadOnlyProperty, IModifiable<TModifyProperty>
    {
        /// <summary>
        /// 移除绑定的<see cref="TModifyProperty"/>
        /// </summary>
        void RemoveModify(TModifyProperty modifyProperty);
    }

    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IModifiable{TModify,TModifyProperty}" />
    /// 泛型<see cref="TValue"/>属性接口, 可以了解当前属性的状态, 并使用<see cref="TModifyProperty"/>对属性进行修正<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    /// <typeparam name="TModifyProperty">指定的可以修饰自身的类型</typeparam>
    public interface IProperty<out TValue, in TModifyProperty> : IProperty<TModifyProperty>, IReadOnlyProperty<TValue>,
        IModifiable<TValue, TModifyProperty>
    {
    }


    public enum EOutOfRangeMode
    {
        /// <summary>
        /// 不做任何处理, 继续进行计算直到结束
        /// </summary>
        None,

        /// <summary>
        /// 当超出范围时, 直接停止返回超出范围的值
        /// </summary>
        Stop,

        /// <summary>
        /// 当超出范围时, 将当前值调整至范围内, 并继续进行计算
        /// </summary>
        Clamp
    }
}