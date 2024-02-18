#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace YuzeToolkit.BindableTool
{
    public enum EModifyPropertyType : byte
    {
        Add,
        Multiple
    }

    public interface IModifyProperty : IComparer<IModifyProperty>
    {
        /// <summary>
        /// 优先级越小越先修正, 在同一优先级先进行所有的<see cref="EModifyPropertyType.Add"/>修正,
        /// 再进行所有的<see cref="EModifyPropertyType.Multiple"/>修正
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 修正类型
        /// </summary>
        EModifyPropertyType ModifyPropertyType { get; }

        /// <summary>
        /// 修正值
        /// </summary>
        float ModifyValue { get; }

        int IComparer<IModifyProperty>.Compare(IModifyProperty x, IModifyProperty y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            if (x.Priority > y.Priority) return 1;
            if (x.Priority < y.Priority) return -1;

            return x.ModifyPropertyType switch
            {
                EModifyPropertyType.Add => y.ModifyPropertyType == EModifyPropertyType.Add ? 0 : -1,
                EModifyPropertyType.Multiple => y.ModifyPropertyType == EModifyPropertyType.Multiple ? 0 : 1,
                _ => 0
            };
        }
    }

    /// <summary>
    /// 对<see cref="IProperty{ModifyProperty}"/>的进行修正<br/>
    /// 优先级越小越先修正, 在同一优先级先进行所有的<see cref="EModifyPropertyType.Add"/>修正,
    /// 再进行所有的<see cref="EModifyPropertyType.Multiple"/>修正<br/><br/>
    /// /// </summary>
    [Serializable]
    public sealed class ModifyProperty : IModifyProperty, IDisposable
    {
        public static ModifyProperty Create(int priority, EModifyPropertyType modifyPropertyType, float modifyValue,
            IProperty<IModifyProperty> property, object? sender = null, object? reason = null)
        {
            var modifyProperty = new ModifyProperty(priority, modifyPropertyType, modifyValue, property);
            property.Modify(modifyProperty, sender, reason);
            return modifyProperty;
        }

        public static bool TryCreate(int priority, EModifyPropertyType modifyPropertyType, float modifyValue,
            IReadOnlyBindable bindable, [MaybeNullWhen(false)] out ModifyProperty modifyProperty,
            object? sender = null, object? reason = null)
        {
            if (bindable is not IProperty<IModifyProperty> property)
            {
                modifyProperty = null;
                return false;
            }

            modifyProperty = new ModifyProperty(priority, modifyPropertyType, modifyValue, property);
            property.Modify(modifyProperty, sender, reason);
            return true;
        }

        private ModifyProperty(int priority, EModifyPropertyType modifyPropertyType, float modifyValue,
            IProperty<IModifyProperty> property)
        {
            this.priority = priority;
            this.modifyPropertyType = modifyPropertyType;
            this.modifyValue = modifyValue;
            _property = property;
        }

        [SerializeField] private int priority;
        [SerializeField] private EModifyPropertyType modifyPropertyType;
        [SerializeField] private float modifyValue;
        private IProperty<IModifyProperty>? _property;

        public int Priority => priority;

        public EModifyPropertyType ModifyPropertyType
        {
            get => modifyPropertyType;
            set
            {
                if (modifyPropertyType == value) return;
                modifyPropertyType = value;
                _property?.ReCheckValue();
            }
        }

        public float ModifyValue
        {
            get => modifyValue;
            set
            {
                if (modifyValue.Equals(value)) return;
                modifyValue = value;
                _property?.ReCheckValue();
            }
        }

        public void Dispose()
        {
            priority = 0;
            modifyValue = 1;
            modifyPropertyType = EModifyPropertyType.Add;
            _property?.RemoveModify(this);
            _property = null;
        }
    }

    /// <summary>
    /// <inheritdoc cref="IProperty{TValue}"/>
    /// </summary>
    /// <typeparam name="TValue">属性的数据类型</typeparam>
    [Serializable]
    public abstract class Property<TValue> : IProperty<TValue, IModifyProperty>
        where TValue : unmanaged
    {
        protected Property()
        {
        }

        protected Property(TValue valueBase, IModifiableOwner? modifiableOwner = null)
        {
            this.valueBase = valueBase;
            value = valueBase;
            ModifiableOwner = modifiableOwner;
        }

        /// <summary>
        /// 初始化基于Unity机制序列化的<see cref="Property{TValue}"/>
        /// </summary>
        public void SetOnly(TValue valueBase, IModifiableOwner? modifiableOwner = null)
        {
            this.valueBase = valueBase;
            value = valueBase;
            ModifiableOwner = modifiableOwner;
        }

        [SerializeField] private TValue value;
        [SerializeField] private TValue valueBase;
        private ValueChange<TValue>? _valueChange;
        public IModifiableOwner? ModifiableOwner { get; private set; }

        public TValue Value
        {
            get => value;
            private set
            {
                if (this.value.Equals(value)) return;
                _valueChange?.Invoke(this.value, value);
                this.value = value;
            }
        }

        public abstract TValue Min { get; }
        public abstract TValue Max { get; }
        public virtual EOutOfRangeMode OutOfRangeMode => EOutOfRangeMode.None;
        protected abstract double CastToDouble(TValue value);
        protected abstract TValue CastToTValue(double value);

        #region Modify

#if YUZE_USE_EDITOR_TOOLBOX
        [ReorderableList(fixedSize: true, draggable: false, HasLabels = false), ReferencePicker]
#endif
        [SerializeReference]
        private List<IModifyProperty>? _modifyProperties;

        private List<IModifyProperty> ModifyProperties => _modifyProperties ??= new List<IModifyProperty>();

        public bool Modify(IModifyProperty modify, object? sender, object? reason = null)
        {
            if (!this.CheckModify(modify, out var modifyProperty, sender, reason)) return false;

            var index = ModifyProperties.BinarySearch(modifyProperty);
            ModifyProperties.Insert(index >= 0 ? index : ~index, modifyProperty);
            ReCheckValue();
            return true;
        }

        public void RemoveModify(IModifyProperty modifyProperty)
        {
            if (_modifyProperties != null && _modifyProperties.Remove(modifyProperty)) ReCheckValue();
        }

        public void ReCheckValue()
        {
            var retValue = CastToDouble(valueBase);
            if (_modifyProperties == null || _modifyProperties.Count == 0)
            {
                Value = CastToTValue(retValue);
                return;
            }

            var priority = _modifyProperties[0].Priority;
            double addValue = 0;
            double multValue = 0;

            foreach (var modifyProperty in _modifyProperties)
            {
                if (modifyProperty.Priority != priority)
                {
                    retValue += addValue;
                    retValue *= 1 + multValue;
                    if (CheckOutOfRange(ref retValue))
                    {
                        Value = CastToTValue(retValue);
                        return;
                    }

                    priority = modifyProperty.Priority;
                    addValue = 0;
                    multValue = 0;
                }

                switch (modifyProperty.ModifyPropertyType)
                {
                    case EModifyPropertyType.Add:
                        addValue += modifyProperty.ModifyValue;
                        break;
                    case EModifyPropertyType.Multiple:
                        multValue += modifyProperty.ModifyValue;
                        break;
                }
            }

            retValue += addValue;
            retValue *= 1 + multValue;

            if (retValue < CastToDouble(Min)) retValue = CastToDouble(Min);
            if (retValue > CastToDouble(Max)) retValue = CastToDouble(Max);
            Value = CastToTValue(retValue);
        }

        private bool CheckOutOfRange(ref double beCheckValue)
        {
            if (OutOfRangeMode == EOutOfRangeMode.None) return false;

            if (beCheckValue < CastToDouble(Min))
            {
                beCheckValue = CastToDouble(Min);
                if (OutOfRangeMode == EOutOfRangeMode.Stop) return true;
            }

            if (beCheckValue > CastToDouble(Max))
            {
                beCheckValue = CastToDouble(Max);
                if (OutOfRangeMode == EOutOfRangeMode.Stop) return true;
            }

            return false;
        }

        #endregion

        public void AddValueChange(ValueChange<TValue>? valueChange)
        {
            if (valueChange != null) _valueChange += valueChange;
        }

        public void RemoveValueChange(ValueChange<TValue>? valueChange)
        {
            if (valueChange != null) _valueChange -= valueChange;
        }

        public void Reset()
        {
            _valueChange = null;
            ModifiableOwner = null;
            value = default;
            valueBase = default;
            _modifyProperties?.Clear();
        }

        public static implicit operator TValue(Property<TValue> property) => property.Value;
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class PropertyFloat : Property<float>
    {
        protected PropertyFloat()
        {
        }

        protected PropertyFloat(float valueBase, IModifiableOwner? modifiableOwner = null) :
            base(valueBase, modifiableOwner)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override double CastToDouble(float value) => value;

        protected sealed override float CastToTValue(double value) => value switch
        {
            >= float.MaxValue => float.MaxValue,
            <= float.MinValue => float.MinValue,
            _ => (float)value
        };
    }

    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class PropertyInt : Property<int>
    {
        protected PropertyInt()
        {
        }

        protected PropertyInt(int valueBase, IModifiableOwner? modifiableOwner = null) :
            base(valueBase, modifiableOwner)
        {
        }

        public override int Min => int.MinValue;
        public override int Max => int.MaxValue;
        protected sealed override double CastToDouble(int value) => value;

        protected sealed override int CastToTValue(double value) => value switch
        {
            >= int.MaxValue => int.MaxValue,
            <= int.MinValue => int.MinValue,
            _ => (int)value
        };
    }
}