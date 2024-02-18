#nullable enable
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit
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

    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface,
        Inherited = false)]
    public sealed class AddSubclassMenuAttribute : Attribute
    {
        public AddSubclassMenuAttribute(Type type, string menuName, int order = 0)
        {
            Order = order;
            MenuName = $"{Regex.Match(type.FullName!, @"(\.?[^.]+?){0,2}$").Value.Replace('.', '/')}/{menuName}";
        }

        public AddSubclassMenuAttribute(string menuName, int order = 0)
        {
            MenuName = menuName;
            Order = order;
        }

        private static readonly char[] Separate = { '/' };
        public string MenuName { get; }
        public int Order { get; }

        public string[] SplitMenuName => !string.IsNullOrWhiteSpace(MenuName)
            ? MenuName.Split(Separate, StringSplitOptions.RemoveEmptyEntries)
            : Array.Empty<string>();


        public string? TypeNameWithoutPath
        {
            get
            {
                var splitDisplayName = SplitMenuName;
                return splitDisplayName.Length != 0 ? splitDisplayName[^1] : null;
            }
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

    [Serializable]
    public struct SerializeType : ISerializationCallbackReceiver
    {
        public static string? GetTypeName(Type? type) =>
            type != null ? type.FullName + "," + type.Assembly.GetName().Name : null;

        public static Type? GetReferenceType(string? assemblyQualifiedTypeName) =>
            !string.IsNullOrEmpty(assemblyQualifiedTypeName) ? Type.GetType(assemblyQualifiedTypeName) : null;

        public SerializeType(string assemblyQualifiedTypeName) : this() => Type =
            !string.IsNullOrEmpty(assemblyQualifiedTypeName) ? Type.GetType(assemblyQualifiedTypeName) : null;

        public SerializeType(Type type) : this() => Type = type;

        [SerializeField] private string? assemblyQualifiedTypeName;
        private Type? _type;

        public Type? Type
        {
            get => _type;
            set
            {
                _type = value;
                assemblyQualifiedTypeName = GetTypeName(_type);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(assemblyQualifiedTypeName))
            {
                _type = null;
                return;
            }

            _type = Type.GetType(assemblyQualifiedTypeName);
            if (_type == null) LogSys.LogWarning($"无法找到与{assemblyQualifiedTypeName}对应的Type");
        }

        public override string? ToString() => Type != null ? Type.FullName : $"<NullType>";
        public static implicit operator string?(SerializeType serializeType) => serializeType.assemblyQualifiedTypeName;
        public static implicit operator Type?(SerializeType serializeType) => serializeType.Type;
        public static implicit operator SerializeType(Type type) => new(type);
    }
}