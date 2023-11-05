namespace YuzeToolkit.DataTool
{
    public interface ICloneSelf<out TSelf>
    {
        TSelf GetClone();
    }
}