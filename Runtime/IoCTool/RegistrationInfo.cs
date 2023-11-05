using System;
using System.Collections.Generic;

namespace YuzeToolkit.IoCTool
{
    public class RegistrationInfo
    {
        public RegistrationInfo(object value, ELifetime lifetime) : this(value, lifetime, value.GetType())
        {
        }

        public RegistrationInfo(object value, ELifetime lifetime, Type registeredType)
        {
            SelfType = value.GetType();
            RegisteredType = registeredType;
            Lifetime = lifetime;
            Value = value;
            InterfaceTypes = new List<Type>();
            AddInterfaceType(RegisteredType);
        }

        public ELifetime Lifetime { get; }
        public object Value { get; }
        public List<Type> InterfaceTypes { get; }
        public Type SelfType { get; }
        public Type RegisteredType { get; }

        public void AddInterfaceType(Type interfaceType)
        {
            if (!interfaceType.IsAssignableFrom(SelfType))
            {
                throw new IoCException($"{SelfType}不继承自{interfaceType}");
            }

            if (!InterfaceTypes.Contains(interfaceType)) InterfaceTypes.Add(interfaceType);
        }
    }
}