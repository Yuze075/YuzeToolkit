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
        /// <param name="label">标签内容</param>
        public static IShowValue GetShowValue<T>(T tValue, string label = "") => tValue switch
        {
            null => null!,
            IGetShowValue getShowValue => GetShowValue(getShowValue.GetShowValue(), label),
            UnityObject value => new UnityObjectValue(value, label),
            string value => new StringValue(value, label),
            bool value => new BoolValue(value, label),
            char value => new CharValue(value, label),
            byte value => new ByteValue(value, label),
            sbyte value => new SByteValue(value, label),
            short value => new ShortValue(value, label),
            ushort value => new UShortValue(value, label),
            int value => new IntValue(value, label),
            uint value => new UIntValue(value, label),
            long value => new LongValue(value, label),
            ulong value => new ULongValue(value, label),
            float value => new FloatValue(value, label),
            double value => new DoubleValue(value, label),
            decimal value => new DecimalValue(value, label),
            Enum value => new EnumValue(value, label),
            Type value => new TypeValue(value, label),
            IEnumerable value when IsListOrArray(value.GetType()) => new ListValue(value, label),
            _ => new SystemObjectValue(tValue, label)
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
        public SystemObjectValue(object value, string label) =>
            (this.value, _label, _disableValue) = (value, label, value.GetType().IsValueType);

        // ReSharper disable once NotAccessedField.Local
        [DisableIf(nameof(_disableValue), true)]
        [GetLabel(nameof(_label))]
        [ReferencePicker(typeof(object), TypeGrouping.ByNamespace)]
        [SerializeReference]
        private object value;

        private bool _disableValue;
        private string _label;
    }

    [Serializable]
    public class UnityObjectValue : IShowValue
    {
        public UnityObjectValue(UnityObject value, string label) => (this.value, _label) = (value, label);

        // ReSharper disable once NotAccessedField.Local
        [InLineEditor] [GetLabel(nameof(_label))] [SerializeField]
        private UnityObject value;

        private string _label;
    }

    [Serializable]
    public abstract class BaseTypeValue<T> : IShowValue
    {
        protected BaseTypeValue(T value, string label) => (this.value, _label) = (value, label);

        // ReSharper disable once NotAccessedField.Local
        [Disable] [GetLabel(nameof(_label))] [SerializeField]
        private T value;

        private string _label;
    }

    #region BaseType

    [Serializable]
    public class StringValue : BaseTypeValue<string>
    {
        public StringValue(string value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class BoolValue : BaseTypeValue<bool>
    {
        public BoolValue(bool value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class CharValue : BaseTypeValue<char>
    {
        public CharValue(char value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class ByteValue : BaseTypeValue<byte>
    {
        public ByteValue(byte value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class SByteValue : BaseTypeValue<sbyte>
    {
        public SByteValue(sbyte value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class ShortValue : BaseTypeValue<short>
    {
        public ShortValue(short value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class UShortValue : BaseTypeValue<ushort>
    {
        public UShortValue(ushort value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class IntValue : BaseTypeValue<int>
    {
        public IntValue(int value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class UIntValue : BaseTypeValue<uint>
    {
        public UIntValue(uint value, string label) : base(value, label)
        {
        }
    }


    [Serializable]
    public class LongValue : BaseTypeValue<long>
    {
        public LongValue(long value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class ULongValue : BaseTypeValue<ulong>
    {
        public ULongValue(ulong value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class FloatValue : BaseTypeValue<float>
    {
        public FloatValue(float value, string label) : base(value, label)
        {
        }
    }


    [Serializable]
    public class DoubleValue : BaseTypeValue<double>
    {
        public DoubleValue(double value, string label) : base(value, label)
        {
        }
    }

    [Serializable]
    public class DecimalValue : BaseTypeValue<decimal>
    {
        public DecimalValue(decimal value, string label) : base(value, label)
        {
        }
    }

    #endregion

    [Serializable]
    public class EnumValue : IShowValue
    {
        public EnumValue(Enum value, string label) =>
            (this.value, _label, enumStr) = (value, label, $"Enum({value.GetType().Name}):{value.ToString()}");


        // ReSharper disable once NotAccessedField.Local
        [Disable] [GetLabel(nameof(_label))] [SerializeReference]
        private Enum value;

        // ReSharper disable once NotAccessedField.Local
        [Disable] [SerializeField] private string enumStr;
        private string _label;
    }

    [Serializable]
    public class TypeValue : IShowValue
    {
        public TypeValue(Type value, string label) => (this.value, _label) = (value, label);

        // ReSharper disable once NotAccessedField.Local
        [Disable]
        [SerializeField]
        [GetLabel(nameof(_label))]
        [TypeSelector(typeof(object),
            TypeSetting = ETypeSetting.AllowUnityObject | ETypeSetting.AllowAbstract | ETypeSetting.AllowGeneric |
                          ETypeSetting.AllowNotClass | ETypeSetting.AllowNotPublic |
                          ETypeSetting.AllowNotSerializable)]
        private SerializeType value;
        private string _label;
    }

    [Serializable]
    public class ListValue : IShowValue
    {
        public ListValue(IEnumerable value, string label) => (this.value, _label) = (new ShowObjectList(value), label);

        // ReSharper disable once NotAccessedField.Local
        [GetLabel(nameof(_label))] [SerializeField]
        private ShowObjectList value;

        private string _label;
    }

    #endregion
}
#endif