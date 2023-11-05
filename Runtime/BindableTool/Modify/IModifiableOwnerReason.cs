namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 由<see cref="IModifiableOwner"/>产生的修饰原因<br/><br/>
    /// </summary>
    public interface IModifiableOwnerReason : IModifyReason
    {
        IModifiableOwner OtherOwner { get; }
    }
}