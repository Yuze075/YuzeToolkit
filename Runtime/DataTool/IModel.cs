namespace YuzeToolkit.DataTool
{
    public interface IModel<out TId>
    {
        /// <summary>
        /// 对应Model的Id
        /// </summary>
        TId Id { get; }
    }
}