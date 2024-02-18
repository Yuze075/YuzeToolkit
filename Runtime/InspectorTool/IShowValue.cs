#nullable enable
#pragma warning disable CS0414 // 定义了成员但是未使用
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
using System;
using UnityEngine;

namespace YuzeToolkit.InspectorTool
{
    [Serializable]
    internal struct ShowKeyValuePair
    {
        public static ShowKeyValuePair GetKeyValuePair<TKey, TValue>(TKey key, TValue value) => new()
        {
            _key = IShowValue.GetShowValue(key),
            _value = IShowValue.GetShowValue(value)
        };

#if YUZE_USE_EDITOR_TOOLBOX
        [Line, IgnoreParent]
#endif
        [SerializeReference]
        private IShowValue? _key;

#if YUZE_USE_EDITOR_TOOLBOX
        [IgnoreParent] 
#endif
        [SerializeReference]
        private IShowValue? _value;
    }

    internal interface IShowValue
    {
        /// <summary>
        /// 获取一个序列化显示一个标签为Value的值
        /// </summary>
        /// <param name="tValue">需要序列化显示的值(可以是UnityObject也可以是SystemObject)</param>
        /// <param name="upLayers">向上查询的层数</param>
        public static IShowValue? GetShowValue<T>(T tValue, int upLayers = 1) => tValue switch
        {
            null => null,
            UnityEngine.Object value => new UnityObjectValue(value, upLayers),
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
            _ => GetSystemObjectValueOrNull(tValue, upLayers)
        };

        private static IShowValue? GetSystemObjectValueOrNull<T>(T tValue, int upLayers)
        {
            if (tValue is Array) return null;
            var type = tValue?.GetType();
            if (type == null) return null;
            if (type.IsGenericType) return null;
            return new SystemObjectValue(tValue, upLayers);
        }
    }

    #region Class

    [Serializable]
    internal class SystemObjectValue : IShowValue
    {
        public SystemObjectValue(object? value, int upLayers)
        {
            _value = value;
            _disableValue = value?.GetType().IsValueType ?? false;
            _upLayers = upLayers;
        }

#if YUZE_USE_EDITOR_TOOLBOX
        [DisableIf(nameof(_disableValue), true),
         LabelByParent(UpLayersSourceHandle = nameof(_upLayers)),
         ReferencePicker(typeof(object), TypeGrouping.ByNamespace)]
#endif
        [SerializeReference]
        private object? _value;

        [NonSerialized] private bool _disableValue;
        [NonSerialized] private int _upLayers;
    }

    [Serializable]
    internal class UnityObjectValue : IShowValue
    {
        public UnityObjectValue(UnityEngine.Object value, int upLayers)
        {
            this.value = value;
            _upLayers = upLayers;
        }

#if YUZE_USE_EDITOR_TOOLBOX
        [InLineEditor, LabelByParent(UpLayersSourceHandle = nameof(_upLayers))]
#endif
        [SerializeField]
        private UnityEngine.Object value;

        [NonSerialized] private int _upLayers;
    }

    [Serializable]
    internal abstract class BaseTypeValue<T> : IShowValue
    {
        protected BaseTypeValue(T value, int upLayers)
        {
            this.value = value;
            _upLayers = upLayers;
        }

#if YUZE_USE_EDITOR_TOOLBOX
        [Disable, LabelByParent(UpLayersSourceHandle = nameof(_upLayers))]
#endif
        [SerializeField]
        private T value;

        [NonSerialized] private int _upLayers;
    }

    #region BaseType

    [Serializable]
    internal class StringValue : BaseTypeValue<string>
    {
        public StringValue(string value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class BoolValue : BaseTypeValue<bool>
    {
        public BoolValue(bool value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class CharValue : BaseTypeValue<char>
    {
        public CharValue(char value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class ByteValue : BaseTypeValue<byte>
    {
        public ByteValue(byte value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class SByteValue : BaseTypeValue<sbyte>
    {
        public SByteValue(sbyte value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class ShortValue : BaseTypeValue<short>
    {
        public ShortValue(short value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class UShortValue : BaseTypeValue<ushort>
    {
        public UShortValue(ushort value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class IntValue : BaseTypeValue<int>
    {
        public IntValue(int value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class UIntValue : BaseTypeValue<uint>
    {
        public UIntValue(uint value, int upLayers) : base(value, upLayers)
        {
        }
    }


    [Serializable]
    internal class LongValue : BaseTypeValue<long>
    {
        public LongValue(long value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class ULongValue : BaseTypeValue<ulong>
    {
        public ULongValue(ulong value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class FloatValue : BaseTypeValue<float>
    {
        public FloatValue(float value, int upLayers) : base(value, upLayers)
        {
        }
    }


    [Serializable]
    internal class DoubleValue : BaseTypeValue<double>
    {
        public DoubleValue(double value, int upLayers) : base(value, upLayers)
        {
        }
    }

    [Serializable]
    internal class DecimalValue : BaseTypeValue<decimal>
    {
        public DecimalValue(decimal value, int upLayers) : base(value, upLayers)
        {
        }
    }

    #endregion

    [Serializable]
    internal class EnumValue : IShowValue
    {
        public EnumValue(Enum value, int upLayers)
        {
            this.value = value;
            enumStr = $"Enum({value.GetType().Name}):{value.ToString()}";
            _upLayers = upLayers;
        }

#if YUZE_USE_EDITOR_TOOLBOX
        [Disable, LabelByParent(UpLayersSourceHandle = nameof(_upLayers))]
#endif
        [SerializeReference]
        private Enum value;

#if YUZE_USE_EDITOR_TOOLBOX
        [Disable]
#endif
        [SerializeField]
        private string enumStr;

        private int _upLayers;
    }

    [Serializable]
    internal class TypeValue : IShowValue
    {
        public TypeValue(Type value, int upLayers)
        {
            this.value = value;
            _upLayers = upLayers;
        }
#if YUZE_USE_EDITOR_TOOLBOX
        [Disable, LabelByParent(UpLayersSourceHandle = nameof(_upLayers))]
#endif
        [SerializeField]
        [TypeSelector(typeof(object),
            TypeSetting = ETypeSetting.AllowUnityObject | ETypeSetting.AllowAbstract | ETypeSetting.AllowGeneric |
                          ETypeSetting.AllowNotClass | ETypeSetting.AllowNotPublic |
                          ETypeSetting.AllowNotSerializable)]
        private SerializeType value;

        [NonSerialized] private int _upLayers;
    }

    #endregion
}
#endif
#pragma warning restore CS0414 // 定义了成员但是未使用