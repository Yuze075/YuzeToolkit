namespace YuzeToolkit.IoC
{
    public interface IListValueWrapper : IValueWrapper
    {
        bool TryAdd<T>(T value);
    }
}