using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="bool" />
    /// 状态接口(类), 通过一个<see cref="AndModifyState"/>值进行状态的管理<br/>
    /// 可以通过<see cref="OrModifyState"/>和<see cref="IState"/>对<see cref="OrModifyState"/>的基础值进行修正<br/>
    /// 优先级越高越后修正, 在同一优先级先进行所有的<see cref="AndModifyState"/>修正, 再进行所有的<see cref="IModifiable"/>修正<br/><br/>
    /// </summary>
    public interface IState : IModifiable, IBindable<bool>
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
        /// 注册一个<see cref="ModifyState"/>, 修饰<see cref="IState"/>的状态值
        /// </summary>
        /// <param name="modifyState">修饰<see cref="IState"/>的接口</param>
        /// <param name="reason"></param>
        /// <returns>返回<see cref="IDisposable"/>接口,
        /// 调用<see cref="IDisposable.Dispose"/>方法解除对<see cref="IState"/>状态的修饰</returns>
        IDisposable Modify(ModifyState modifyState, IModifyReason reason);
    }
}