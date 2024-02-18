#nullable enable
namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="IReadOnlyBindable{TValue}"/>
    /// 只读的状态接口, 可以了解当前对象的状态<br/><br/>
    /// </summary>
    public interface IReadOnlyState : IReadOnlyBindable<bool>
    {
        /// <summary>
        /// 一个特殊的状态索引器, 用于获得特定条件下的状态.<br/><br/>
        /// 传入的<see cref="removeSmall"/>为<c>True</c>的化,
        /// 则排除小于<see cref="priority"/>的<see cref="ModifyState"/>再次进行计算, 返回修正的结果值<br/><br/>
        /// 传入的<see cref="removeSmall"/>为<c>False</c>的化,
        /// 则排除大于<see cref="priority"/>的<see cref="ModifyState"/>再次进行计算, 返回修正的结果值
        /// </summary>
        /// <param name="priority">判断状态的优先级</param>
        /// <param name="removeSmall">是否为剔除<see cref="priority"/>小的<see cref="ModifyState"/>值</param>
        bool this[int priority, bool removeSmall = true] { get; }

        /// <summary>
        /// 重新检测对应的属性修正
        /// </summary>
        void ReCheckValue();
    }

    /// <summary>
    /// <inheritdoc cref="IModifiable{TValue,TModify}"/>
    /// 状态接口, 可以了解当前对象的状态, 并修正当前状态<br/>
    /// 可以通过<see cref="TModifyState"/>对<see cref="IState{TModifyState}"/>的进行修正<br/><br/>
    /// </summary>
    /// <typeparam name="TModifyState">指定的可以修饰自身的类型</typeparam>
    public interface IState<in TModifyState> : IReadOnlyState, IModifiable<bool, TModifyState>
    {
        /// <summary>
        /// 移除绑定修正的<see cref="TModifyState"/>
        /// </summary>
        void RemoveModify(TModifyState modifyState);
    }
}