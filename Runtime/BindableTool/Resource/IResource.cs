using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="ModifyResource" />
    /// 资源接口(类), 通过一个数值类型进行资源数值的管理<br/>
    /// 可以通过<see cref="IModifiable"/>对基础值进行修正, 进行加或者减<br/><br/>
    /// </summary>
    public interface IResource<out TValue> : IModifiable, IBindable<TValue>
    {
        /// <summary>
        /// 数值范围, 超出范围的值会被修正为范围内值
        /// </summary>
        TValue Min { get; }

        /// <summary>
        /// 数值范围, 超出范围的值会被修正为范围内值
        /// </summary>
        TValue Max { get; }

        /// <summary>
        /// 使用一个<see cref="ModifyResource"/>修改<see cref="IResource{TValue}.Value"/>
        /// </summary>
        /// <param name="modifyResource">修改<see cref="IResource{TValue}"/>的接口</param>
        /// <param name="reason"></param>
        IDisposable Modify(ModifyResource modifyResource, IModifyReason reason);

        /// <summary>
        /// 资源剩余是否支持这次修改<br/>
        /// 返回0：修改之后不会超出范围<br/>
        /// 返回1：修改之后超出最大值<br/>
        /// 返回-1：修改之后超出最小值
        /// </summary>
        EEnoughType Enough(ModifyResource modifyResource);

        /// <summary>
        /// 注册超出最大值的事件，返回超出的值和最大值
        /// </summary>
        IDisposable RegisterOutOfMaxRange(OutOfMaxRange<TValue> outOfMaxRange);

        /// <summary>
        /// 注册超出最小值的事件，返回超出的值和最小值
        /// </summary>
        IDisposable RegisterOutOfMinRange(OutOfMinRange<TValue> outOfMinRange);
    }

    public delegate void OutOfMaxRange<in TValue>(TValue maxValue, float outMaxValue);

    public delegate void OutOfMinRange<in TValue>(TValue minValue, float outMinValue);
}