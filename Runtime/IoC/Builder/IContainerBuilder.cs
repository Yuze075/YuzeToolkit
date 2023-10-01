using System;

namespace YuzeToolkit.IoC
{
    public interface IContainerBuilder
    {
        void Register<T>(RegistrationInfo<T> registrationInfo);

        void Callback(Action<IInjectResolver> action);
    }
}