namespace YuzeToolkit.IoC
{
    public interface IBeInjectedValue
    {
        void DoInject(IValueGetter valueGetter);
    }
}