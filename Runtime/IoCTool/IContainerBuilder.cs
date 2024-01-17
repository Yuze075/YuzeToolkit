#nullable enable
using System;

namespace YuzeToolkit.IoCTool
{
    public interface IContainerBuilder
    {
        void Register(RegistrationInfo registrationInfo);
        void Callback(Action<Container> action);
        void Callback(object tryInjectValue);
    }
}