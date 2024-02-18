#nullable enable
namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 可被修饰的<see cref="object"/>类型接口<br/><br/>
    /// </summary>
    /// <typeparam name="TModify">指定的可以修饰自身的类型</typeparam>
    public interface IModifiable<in TModify> : IReadOnlyBindable
    {
        /// <summary>
        /// 拥有当前<see cref="IModifiable{TModify}"/>的对象<br/>
        /// 可能为空, 通常意味着对象还未初始化(基于Unity序列化机制产生的)<br/>
        /// 但也可能是对象本身就不属于任何<see cref="IModifiableOwner"/>对象(通常不建议出现这种情况)
        /// </summary>
        IModifiableOwner? ModifiableOwner { get; }

        /// <summary>
        /// 修饰当前<see cref="IModifiable{TModify}"/>对象的方法
        /// </summary>
        /// <param name="modify">修饰的数据</param>
        /// <param name="sender">发生修改事件的对象</param>
        /// <param name="reason">修饰的原因</param>
        bool Modify(TModify modify, object? sender, object? reason = null);
    }

    /// <summary>
    /// <inheritdoc cref="IReadOnlyBindable{TValue}" />
    /// 可被修饰的泛型<see cref="TValue"/>接口<br/><br/>
    /// </summary>
    /// <typeparam name="TValue">数据类型</typeparam>
    /// <typeparam name="TModify">指定的可以修饰自身的类型</typeparam>
    public interface IModifiable<out TValue, in TModify> : IReadOnlyBindable<TValue>, IModifiable<TModify>
    {
    }
}