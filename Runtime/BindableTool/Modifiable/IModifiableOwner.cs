#nullable enable
namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <see cref="IModifiable{TModify}"/>的拥有者<br/>
    /// 处理自己拥有<see cref="IModifiable{TModify}"/>发生修正时的逻辑, 判断能否发生修正
    /// </summary>
    public interface IModifiableOwner
    {
        /// <summary>
        /// 检测这次修饰是否能发生
        /// </summary>
        /// <param name="modifiable">被修饰的变量</param>
        /// <param name="modify">修饰值(当返回的引用为null时, 也不会发生修正)</param>
        /// <param name="reason">修饰原因</param>
        /// <param name="sender"></param>
        /// <returns>返回是否能够发生这次修正, True代表可以, False代表不可以</returns>
        bool CheckModify<TModifiable, TModify>(TModifiable modifiable, ref TModify? modify, object? sender,
            object? reason) where TModifiable : IModifiable<TModify>;
    }
}