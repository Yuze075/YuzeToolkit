namespace YuzeToolkit.GUITool
{
    public interface IFGUILayoutField<T>
    {
        T DrawField(T value, string? name);
    }
}