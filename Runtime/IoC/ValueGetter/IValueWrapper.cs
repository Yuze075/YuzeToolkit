namespace YuzeToolkit.IoC
{
    public interface IValueWrapper
    {
        bool TryGet<T>(out T value);
    }
}