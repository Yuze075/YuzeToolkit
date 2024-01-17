#nullable enable
using System;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 可被修饰的变量<br/><br/>
    /// </summary>
    public interface IModifiable : IBindable
    {
        /// <summary>
        /// 是否是只读变量<br/>
        /// 如果是只读变量只能在拥有<see cref="IModifiable"/>的<see cref="IModifiableOwner"/>才可以进行<see cref="Modify{TModify}"/>的修饰
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// 拥有<see cref="IModifiable"/>的<see cref="IModifiableOwner"/>, 指向具体的对象<br/>
        /// Set方法为内部方法, 仅限内部调用, 请勿外部调用
        /// </summary>
        IModifiableOwner? Owner { get; }

        /// <summary>
        /// 被修饰的方法
        /// </summary>
        /// <param name="modify">修饰的数据</param>
        /// <param name="sender">发生修改事件的<see cref="IModifiableOwner"/>对象</param>
        /// <param name="reason">修饰的原因</param>
        bool Modify<TModify>(TModify modify, IModifiableOwner? sender, object? reason) where TModify : IModify;
    }
}