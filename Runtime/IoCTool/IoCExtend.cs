#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Obsolete("注册所有接口可能会注册部分重复的接口, 建议使用AsAllInterfacesWithout剔除重复的标记接口!")]
        public static RegistrationInfo AsAllInterfaces(this RegistrationInfo info, bool asSelf = true)
        {
            foreach (var type in (asSelf ? info.SelfType : info.RegisteredType).GetInterfaces())
                info.AddInterfaceType(type);
            return info;
        }

        [Obsolete("注册所有子类可能会注册部分重复的子类, 建议使用AsAllBaseTypesWithout剔除重复的基础子类!")]
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

        [Obsolete("注册所有子类和接口可能会注册部分重复的子类和接口, 建议使用AsAllTypesWithout剔除重复的基础子类和重复的标记接口!")]
        public static RegistrationInfo AsAllTypes(this RegistrationInfo info, bool asSelf = true)
        {
            return (asSelf ? info.AsSelf() : info).AsAllBaseTypes(asSelf).AsAllInterfaces(asSelf);
        }

        private static readonly Type ObjectType = typeof(object);

        [Obsolete("还没有写好")]
        public static readonly Type[] WithoutBaseTypes = new[]
            { typeof(UnityEngine.Object), typeof(Component), typeof(Behaviour), typeof(MonoBehaviour) ,typeof(ScriptableObject), typeof(MonoBase), typeof(SoBase)};

        [Obsolete("还没有写好")]
        public static RegistrationInfo AsAllBaseTypesWithout(this RegistrationInfo info,
            IReadOnlyList<Type>? withoutTypes = null, bool asSelf = true)
        {
            var type = asSelf ? info.SelfType : info.RegisteredType;

            while (type != ObjectType || (withoutTypes != null && withoutTypes.Contains(type)))
            {
                info.AddInterfaceType(type);
                type = type.BaseType;
            }

            return info;
        }

        [Obsolete("还没有写好")]
        public static RegistrationInfo AsAllInterfacesWithout(this RegistrationInfo info,
            IReadOnlyList<Type>? withoutTypes = null, bool asSelf = true)
        {
            foreach (var type in (asSelf ? info.SelfType : info.RegisteredType).GetInterfaces())
            {
                if (withoutTypes != null && withoutTypes.Contains(type)) continue;
                info.AddInterfaceType(type);
            }

            return info;
        }

        [Obsolete("还没有写好")]
        public static RegistrationInfo AsAllTypesWithout(this RegistrationInfo info,
            IReadOnlyList<Type>? withoutBaseTypes = null, IReadOnlyList<Type>? withoutInterfaceTypes = null,
            bool asSelf = true)
        {
            return (asSelf ? info.AsSelf() : info).AsAllBaseTypesWithout(withoutBaseTypes, asSelf)
                .AsAllInterfacesWithout(withoutInterfaceTypes, asSelf);
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