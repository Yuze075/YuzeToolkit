#nullable enable
namespace YuzeToolkit.BindableTool
{
    public delegate void OutOfMaxRange<in TValue>(TValue maxValue, float outMaxValue);

    public delegate void OutOfMinRange<in TValue>(TValue minValue, float outMinValue);

    /// <summary>
    /// <inheritdoc cref="IReadOnlyBindable{TValue}"/>
    /// 只读的资源接口, 可以注册对应的超出范围回调<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    public interface IReadOnlyResource<out TValue> : IReadOnlyBindable<TValue>
    {
        /// <summary>
        /// 数值范围, 超出范围的值会被修正为范围内值
        /// </summary>
        TValue Min { get; }

        /// <summary>
        /// 数值范围, 超出范围的值会被修正为范围内值
        /// </summary>
        TValue Max { get; }

        void AddOutOfMaxRange(OutOfMaxRange<TValue>? outOfMaxRange);
        void RemoveOutOfMaxRange(OutOfMaxRange<TValue>? outOfMaxRange);
        void AddOutOfMinRange(OutOfMinRange<TValue>? outOfMinRange);
        void RemoveOutOfMinRange(OutOfMinRange<TValue>? outOfMinRange);
    }

    /// <summary>
    /// <inheritdoc cref="IModifiable{TValue,TModify}"/>
    /// 资源接口, 可以注册对应的超出范围回调, 也可以检测释放可以修正资源, 和对资源进行修正<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    /// <typeparam name="TModifyResource">指定的可以修饰自身的类型</typeparam>
    public interface IResource<out TValue, in TModifyResource> : IReadOnlyResource<TValue>,
        IModifiable<TValue, TModifyResource>
    {
        /// <summary>
        /// 资源剩余是否支持这次修改<br/>
        /// 返回0：修改之后不会超出范围<br/>
        /// 返回1：修改之后超出最大值<br/>
        /// 返回-1：修改之后超出最小值
        /// </summary>
        EEnoughType Enough(TModifyResource modifyResource);
    }

    /// <summary>
    /// 资源是否足够的状态
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