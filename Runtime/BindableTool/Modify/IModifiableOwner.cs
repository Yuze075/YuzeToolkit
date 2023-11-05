namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <see cref="IModifiable"/>的拥有者, 用于接受对自己拥有的属性的修饰发生的事件
    /// </summary>
    public interface IModifiableOwner
    {
        /// <summary>
        /// 检测这次修饰是否能发生<br/>
        /// 内部调用方法, 请勿外部调用
        /// </summary>
        /// <param name="modifiable">被修饰的变量</param>
        /// <param name="modify">修饰值</param>
        /// <param name="reason">修饰原因</param>
        void CheckModify<TModify>(IModifiable modifiable, ref TModify? modify,
            IModifyReason reason) where TModify : IModify;
    }
}