using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.IoC
{
    public static class IoCExtend
    {
        public static IRegistrationInfo As<TInterface>(this IRegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface));
            return info;
        }

        public static IRegistrationInfo As<TInterface1, TInterface2>(this IRegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface1));
            info.AddInterfaceType(typeof(TInterface2));
            return info;
        }

        public static IRegistrationInfo As<TInterface1, TInterface2, TInterface3>(this IRegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface1));
            info.AddInterfaceType(typeof(TInterface2));
            info.AddInterfaceType(typeof(TInterface3));
            return info;
        }

        public static IRegistrationInfo As<TInterface1, TInterface2, TInterface3, TInterface4>(
            this IRegistrationInfo info)
        {
            info.AddInterfaceType(typeof(TInterface1));
            info.AddInterfaceType(typeof(TInterface2));
            info.AddInterfaceType(typeof(TInterface3));
            info.AddInterfaceType(typeof(TInterface4));
            return info;
        }

        public static IRegistrationInfo AsSelf(this IRegistrationInfo info)
        {
            info.AddInterfaceType(info.ImplementationType);
            return info;
        }

        public static IRegistrationInfo AsAllInterfaces(this IRegistrationInfo info)
        {
            foreach (var type in info.ImplementationType.GetInterfaces())
            {
                info.AddInterfaceType(type);
            }

            return info;
        }

        public static IRegistrationInfo AsAllBaseTypes(this IRegistrationInfo info)
        {
            var type = info.ImplementationType;
            while (type.BaseType != null)
            {
                info.AddInterfaceType(type.BaseType);
                type = type.BaseType;
            }

            return info;
        }

        public static IRegistrationInfo AsAllTypes(this IRegistrationInfo info)
        {
            return info.AsSelf().AsAllBaseTypes().AsAllInterfaces();
        }

        public static IRegistrationInfo Register<T>(this IContainerBuilder builder,
            ELifetime lifetime = ELifetime.Singleton) where T : new()
        {
            var info = new RegistrationInfo<T>(new T(), lifetime);
            builder.Register(info);
            return info;
        }

        public static IRegistrationInfo Register<TBase, T>(this IContainerBuilder builder,
            ELifetime lifetime = ELifetime.Singleton) where T : TBase, new()
        {
            var info = new RegistrationInfo<TBase>(new T(), lifetime);
            builder.Register(info);
            return info;
        }
        
        public static IRegistrationInfo Register<T>(this IContainerBuilder builder, T value, ELifetime lifetime = ELifetime.Singleton) 
        {
            var info = new RegistrationInfo<T>(value, lifetime);
            builder.Register(info);
            return info;
        }

        public static void Inject(this IInjectResolver resolver, GameObject gameObject)
        {
            foreach (var beInjectedValue in gameObject.GetComponents<IBeInjectedValue>())
            {
                resolver.Inject(beInjectedValue);
            }
            
            foreach (var beInjectedValue in gameObject.GetComponentsInChildren<IBeInjectedValue>())
            {
                resolver.Inject(beInjectedValue);
            }
        }

        public static void Inject(this IInjectResolver resolver, IEnumerable<IBeInjectedValue> enumerable)
        {
            foreach (var beInjectedValue in enumerable)
            {
                resolver.Inject(beInjectedValue);
            }
        }
    }
}