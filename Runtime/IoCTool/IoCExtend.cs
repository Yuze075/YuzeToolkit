using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.IoCTool
{
    public static class IoCExtend
    {
        public static RegistrationInfo As<TInterface>(this RegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface));
            return info;
        }

        public static RegistrationInfo As<TInterface1, TInterface2>(this RegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface1));
            info.AddInterfaceType(typeof(TInterface2));
            return info;
        }

        public static RegistrationInfo As<TInterface1, TInterface2, TInterface3>(this RegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface1));
            info.AddInterfaceType(typeof(TInterface2));
            info.AddInterfaceType(typeof(TInterface3));
            return info;
        }

        public static RegistrationInfo As<TInterface1, TInterface2, TInterface3, TInterface4>(
            this RegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface1));
            info.AddInterfaceType(typeof(TInterface2));
            info.AddInterfaceType(typeof(TInterface3));
            info.AddInterfaceType(typeof(TInterface4));
            return info;
        }

        public static RegistrationInfo AsSelf(this RegistrationInfo info)
        {
            info.AddInterfaceType(info.SelfType);
            return info;
        }

        public static RegistrationInfo AsAllInterfaces(this RegistrationInfo info, bool asSelf = true)
        {
            foreach (var type in (asSelf ? info.SelfType : info.RegisteredType).GetInterfaces())
                info.AddInterfaceType(type);
            return info;
        }

        public static RegistrationInfo AsAllBaseTypes(this RegistrationInfo info, bool asSelf = true)
        {
            var type = asSelf ? info.SelfType : info.RegisteredType;
            while (type.BaseType != null)
            {
                info.AddInterfaceType(type.BaseType);
                type = type.BaseType;
            }

            return info;
        }

        public static RegistrationInfo AsAllTypes(this RegistrationInfo info, bool asSelf = true)
        {
            return (asSelf ? info.AsSelf() : info).AsAllBaseTypes(asSelf).AsAllInterfaces(asSelf);
        }

        public static RegistrationInfo Register<T>(this IContainerBuilder builder,
            ELifetime lifetime = ELifetime.Singleton) where T : notnull, new()
        {
            var info = new RegistrationInfo(new T(), lifetime, typeof(T));
            builder.Register(info);
            return info;
        }

        public static RegistrationInfo Register<T>(this IContainerBuilder builder, T value,
            ELifetime lifetime = ELifetime.Singleton) where T : notnull
        {
            var info = new RegistrationInfo(value, lifetime, typeof(T));
            builder.Register(info);
            return info;
        }

        public static RegistrationInfo Register<TBase, T>(this IContainerBuilder builder,
            ELifetime lifetime = ELifetime.Singleton) where T : notnull, TBase, new()
        {
            var info = new RegistrationInfo(new T(), lifetime, typeof(TBase));
            builder.Register(info);
            return info;
        }

        public static RegistrationInfo Register<TBase, T>(this IContainerBuilder builder, T value,
            ELifetime lifetime = ELifetime.Singleton) where T : notnull, TBase, new()
        {
            var info = new RegistrationInfo(value, lifetime, typeof(TBase));
            builder.Register(info);
            return info;
        }

        public static void Inject(this Container container, GameObject gameObject)
        {
            foreach (var beInjectedValue in gameObject.GetComponents<IBeInjectedValue>())
            {
                container.Inject(beInjectedValue);
            }

            foreach (var beInjectedValue in gameObject.GetComponentsInChildren<IBeInjectedValue>())
            {
                container.Inject(beInjectedValue);
            }
        }

        public static void Inject(this Container container, IEnumerable<IBeInjectedValue> enumerable)
        {
            foreach (var beInjectedValue in enumerable)
            {
                container.Inject(beInjectedValue);
            }
        }
    }
}