#nullable enable
using System;
using UnityEngine;

namespace YuzeToolkit.SerializeTool
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeSelectorAttribute : PropertyAttribute
    {
        private static readonly Type UnityObjectType = typeof(UnityEngine.Object);
        private static readonly Type SerializableType = typeof(SerializableAttribute);

        public TypeSelectorAttribute(Type baseType) => BaseType = baseType;
        public Type BaseType { get; }
        public ETypeSetting TypeSetting { get; set; }
        public string AssemblyQualifiedBaseTypeName => BaseType.FullName + "," + BaseType.Assembly.GetName().Name;

        public bool CheckType(Type t)
        {
            if ((TypeSetting & ETypeSetting.AllowNotClass) == 0 && t.IsInterface) return false;
            if ((TypeSetting & ETypeSetting.AllowNotPublic) == 0 && !t.IsPublic && !t.IsNestedPublic) return false;
            if ((TypeSetting & ETypeSetting.AllowAbstract) == 0 && t.IsAbstract) return false;
            if ((TypeSetting & ETypeSetting.AllowGeneric) == 0 && t.IsGenericType) return false;
            var isUnityObject = UnityObjectType.IsAssignableFrom(t);
            if ((TypeSetting & ETypeSetting.AllowUnityObject) == 0 && isUnityObject) return false;
            var isSerializable = isUnityObject || IsDefined(t, SerializableType);
            if ((TypeSetting & ETypeSetting.AllowNotSerializable) == 0 && !isSerializable) return false;
            return true;
        }
    }

    [Flags]
    public enum ETypeSetting
    {
        /// <summary>
        /// 包含这个枚举, 可以选中Class和Interface
        /// </summary>
        AllowNotClass = 1,

        /// <summary>
        /// 包含这个枚举, 可以选中非Public的类型
        /// </summary>
        AllowNotPublic = 2,

        /// <summary>
        /// 包含这个枚举, 可以选中Abstract的类型
        /// </summary>
        AllowAbstract = 4,

        /// <summary>
        /// 包含这个枚举, 可以选中Generic的类型
        /// </summary>
        AllowGeneric = 8,

        /// <summary>
        /// 包含这个枚举, 可以选中没有<see cref="SerializableAttribute"/>的类型<br/>
        /// 注意, 在<see cref="AllowUnityObject"/>时, 且引用类型为<see cref="UnityEngine.Object"/>类型时,
        /// 会无视是否拥有<see cref="SerializableAttribute"/>, 因为默认unity的类型都是序列化的.
        /// </summary>
        AllowNotSerializable = 16,

        /// <summary>
        /// 包含这个枚举, 可以选中继承自<see cref="UnityEngine.Object"/>的类型<br/>
        /// </summary>
        AllowUnityObject = 32
    }
}