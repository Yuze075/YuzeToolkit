#nullable enable
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

        internal ELifetime Lifetime { get; }
        internal object Value { get; }
        internal List<Type> InterfaceTypes { get; }
        internal Type SelfType { get; }
        internal Type RegisteredType { get; }

        internal void AddInterfaceType(Type interfaceType)
        {
            if (!interfaceType.IsAssignableFrom(SelfType))
                throw new ArgumentOutOfRangeException($"{SelfType}不继承自{interfaceType}");

            if (!InterfaceTypes.Contains(interfaceType)) InterfaceTypes.Add(interfaceType);
        }
    }
}