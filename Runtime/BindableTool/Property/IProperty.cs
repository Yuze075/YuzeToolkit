using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="AddModifyProperty" />
    /// 属性接口, 通过一个数值类型进行属性数值的管理<br/>
    /// 可以通过<see cref="MultModifyProperty"/>和<see cref="IProperty{TValue}"/>对<see cref="AddModifyProperty"/>的基础值进行修正<br/>
    /// 优先级越高越后修正, 在同一优先级先进行所有的<see cref="MultModifyProperty"/>修正, 再进行所有的<see cref="IModifiable"/>修正<br/><br/>
    /// </summary>
    public interface IProperty<out TValue> : IModifiable, IBindable<TValue>
    {
        /// <summary>
        /// 注册一个<see cref="ModifyProperty"/>, 修饰<see cref="IProperty{TValue}"/>的状态值
        /// </summary>
        /// <param name="modifyProperty">修饰<see cref="IProperty{TValue}"/>的接口</param>
        /// <param name="reason"></param>
        /// <returns>返回<see cref="IDisposable"/>接口,
        /// 调用<see cref="IDisposable.Dispose"/>方法解除对<see cref="IProperty{TValue}"/>状态的修饰</returns>
        IDisposable Modify(ModifyProperty modifyProperty, IModifyReason reason);

        /// <summary>
        /// 数值范围, 超出范围的值会被修正为范围内值
        /// </summary>
        TValue Min { get; }

        /// <summary>
        /// 数值范围, 超出范围的值会被修正为范围内值
        /// </summary>
        TValue Max { get; }

        /// <summary>
        /// 当一个优先级的数值结算完成之后判断一次范围, 如果超出范围就在这次优先级判断之后就返回
        /// </summary>
        bool WhenOutOfRangeStop { get; }
    }
}