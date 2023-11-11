#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.SerializeTool;
using UnityObject = UnityEngine.Object;

namespace YuzeToolkit.InspectorTool
{
    public interface IShowValue
    {
        /// <summary>
        /// 获取一个序列化显示一个标签为Value的值
        /// </summary>
        /// <param name="tValue">需要序列化显示的值(可以是UnityObject也可以是SystemObject)</param>
        /// <param name="upLayers">向上查询的层数</param>
        public static IShowValue GetShowValue<T>(T tValue, int upLayers = 1) => tValue switch
        {
            null => null!,
            IGetShowValue getShowValue => GetShowValue(getShowValue.GetShowValue(), upLayers),
            UnityObject value => new UnityObjectValue(value, upLayers),
            string value => new StringValue(value, upLayers),
            bool value => new BoolValue(value, upLayers),
            char value => new CharValue(value, upLayers),
            byte value => new ByteValue(value, upLayers),
            sbyte value => new SByteValue(value, upLayers),
            short value => new ShortValue(value, upLayers),
            ushort value => new UShortValue(value, upLayers),
            int value => new IntValue(value, upLayers),
            uint value => new UIntValue(value, upLayers),
            long value => new LongValue(value, upLayers),
            ulong value => new ULongValue(value, upLayers),
            float value => new FloatValue(value, upLayers),
            double value => new DoubleValue(value, upLayers),
            decimal value => new DecimalValue(value, upLayers),
            Enum value => new EnumValue(value, upLayers),
            Type value => new TypeValue(value, upLayers),
            // IEnumerable value when IsListOrArray(value.GetType()) => new ListValue(value, label),
            _ => new SystemObjectValue(tValue, upLayers)
        };

        private static readonly Type ListType = typeof(List<>);
        private static readonly Type ArrayType = typeof(Array);

        private static bool IsListOrArray(Type type) =>
            (type.IsGenericType
                ? ListType.IsAssignableFrom(type.GetGenericTypeDefinition())
                : ListType.IsAssignableFrom(type))
            || ArrayType.IsAssignableFrom(type);
    }

    #region Class

    [Serializable]
    public class SystemObjectValue : IShowValue
    {
        public SystemObjectValue(object value, int upLayers)
        {
            this.value = value;
            _disableValue = value.GetType().IsValueType;
            _upLayers = upLayers;
        }

        // ReSharper disable once NotAccessedField.Local
        [DisableIf(nameof(_disableValue), true)]
        [LabelByParent(UpLayersSourceHandle = nameof(_upLayers))]
        [ReferencePicker(typeof(object), TypeGrouping.ByNamespace)]
        [SerializeReference]
        private object value;

        private bool _disableValue;
        private int _upLayers;
    }

    [Serializable]
    public class UnityObjectValue : IShowValue
    {
        public UnityObjectValue(UnityObject value, int upLayers)
        {
            this.value = value;
            _upLayers = upLayers;
        }

        // ReSharper disable once NotAccessedField.Local
        [InLineEditor] [LabelByParent(UpLayersSourceHandle = nameof(_upLayers))] [SerializeField]
        private UnityObject value;

        private int _upLayers;
    }

    [Serializable]
    public abstract class BaseTypeValue<T> : IShowValue
    {
        protected BaseTypeValue(T value, int upLayers)
        {
            this.value = value;
            _upLayers = upLayers;
        }

        // ReSharper disable once NotAccessedField.Local
        [Disable] [LabelByParent(UpLayersSourceHandle = nameof(_upLayers))] [SerializeField]
        private T value;

        private int _upLayers;
    }

    #region BaseType

    [Serializable]
    public class StringValue : BaseTypeValue<string>
    {
        public StringValue(string value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class BoolValue : BaseTypeValue<bool>
    {
        public BoolValue(bool value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class CharValue : BaseTypeValue<char>
    {
        public CharValue(char value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class ByteValue : BaseTypeValue<byte>
    {
        public ByteValue(byte value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class SByteValue : BaseTypeValue<sbyte>
    {
        public SByteValue(sbyte value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class ShortValue : BaseTypeValue<short>
    {
        public ShortValue(short value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class UShortValue : BaseTypeValue<ushort>
    {
        public UShortValue(ushort value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class IntValue : BaseTypeValue<int>
    {
        public IntValue(int value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class UIntValue : BaseTypeValue<uint>
    {
        public UIntValue(uint value, int upLayers) : base(value, upLayers)
        {
        }
    }


    [Serializable]
    public class LongValue : BaseTypeValue<long>
    {
        public LongValue(long value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class ULongValue : BaseTypeValue<ulong>
    {
        public ULongValue(ulong value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class FloatValue : BaseTypeValue<float>
    {
        public FloatValue(float value, int upLayers) : base(value, upLayers)
        {
        }
    }


    [Serializable]
    public class DoubleValue : BaseTypeValue<double>
    {
        public DoubleValue(double value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    public class DecimalValue : BaseTypeValue<decimal>
    {
        public DecimalValue(decimal value, int upLayers) : base(value, upLayers)
        {
        }
    }

    #endregion

    [Serializable]
    public class EnumValue : IShowValue
    {
        public EnumValue(Enum value, int upLayers)
        {
            this.value = value;
            enumStr = $"Enum({value.GetType().Name}):{value.ToString()}";
            _upLayers = upLayers;
        }


        // ReSharper disable once NotAccessedField.Local
        [Disable] [LabelByParent(UpLayersSourceHandle = nameof(_upLayers))] [SerializeReference]
        private Enum value;

        // ReSharper disable once NotAccessedField.Local
        [Disable] [SerializeField] private string enumStr;
        private int _upLayers;
    }

    [Serializable]
    public class TypeValue : IShowValue
    {
        public TypeValue(Type value, int upLayers)
        {
            this.value = value;
            _upLayers = upLayers;
        }

        // ReSharper disable once NotAccessedField.Local
        [Disable]
        [SerializeField]
        [LabelByParent(UpLayersSourceHandle = nameof(_upLayers))]
        [TypeSelector(typeof(object),
            TypeSetting = ETypeSetting.AllowUnityObject | ETypeSetting.AllowAbstract | ETypeSetting.AllowGeneric |
                          ETypeSetting.AllowNotClass | ETypeSetting.AllowNotPublic |
                          ETypeSetting.AllowNotSerializable)]
        private SerializeType value;

        private int _upLayers;
    }

    #endregion
}
#endif