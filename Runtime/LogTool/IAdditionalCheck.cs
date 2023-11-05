namespace YuzeToolkit.LogTool
{
    public interface IAdditionalCheck
    {
        bool DoAdditionalCheck(out string? checkInfo);
    }
}