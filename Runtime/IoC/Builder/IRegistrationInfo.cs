using System;
using System.Collections.Generic;

namespace YuzeToolkit.IoC
{
    public interface IRegistrationInfo
    {
        internal Type ImplementationType { get; }
        internal void AddInterfaceType(Type interfaceType);
    }


}