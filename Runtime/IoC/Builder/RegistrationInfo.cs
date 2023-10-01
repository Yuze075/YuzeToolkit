using System;
using System.Collections.Generic;

namespace YuzeToolkit.IoC
{
    public class RegistrationInfo<T> : IRegistrationInfo
    {
        internal RegistrationInfo(T value, ELifetime lifetime)
        {
            _implementationType = typeof(T);
            Lifetime = lifetime;
            Value = value;
            InterfaceTypes = new List<Type>();
        }

        private readonly Type _implementationType;
        internal readonly ELifetime Lifetime;
        internal readonly T Value;
        internal readonly List<Type> InterfaceTypes;
        Type IRegistrationInfo.ImplementationType => _implementationType;

        void IRegistrationInfo.AddInterfaceType(Type interfaceType)
        {
            if (!interfaceType.IsAssignableFrom(_implementationType))
            {
                throw new IoCException($"{_implementationType}不是继承子{interfaceType}");
            }

            if (!InterfaceTypes.Contains(interfaceType))
                InterfaceTypes.Add(interfaceType);
        }
    }
}