using System;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.SerializeTool
{
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

        [SerializeField]
        private string? assemblyQualifiedTypeName;
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
            if (_type == null) LogSys.Log($"无法找到与{assemblyQualifiedTypeName}对应的Type", ELogType.Warning);
        }

        public override string? ToString() => Type != null ? Type.FullName : $"<NullType>";
        public static implicit operator string?(SerializeType serializeType) => serializeType.assemblyQualifiedTypeName;
        public static implicit operator Type?(SerializeType serializeType) => serializeType.Type;
        public static implicit operator SerializeType(Type type) => new(type);
    }
}