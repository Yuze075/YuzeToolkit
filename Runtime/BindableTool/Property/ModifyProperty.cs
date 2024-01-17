#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.LogTool;
using YuzeToolkit.SerializeTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc cref="YuzeToolkit.BindableTool.IModify" />
    /// 对<see cref="IProperty{TValue}"/>的状态值<see cref="IProperty{TValue}.Value"/>的修饰<br/>
    /// 优先级越高越后修正, 在同一优先级先进行所有的<see cref="AddModifyProperty"/>修正, 再进行所有的<see cref="MultModifyProperty"/>修正<br/><br/>
    /// </summary>
    [Serializable]
    public abstract class ModifyProperty : IModify, IComparer<ModifyProperty>, IDisposable
    {
        protected ModifyProperty(int priority, IProperty property)
        {
            this.priority = priority;
            _property = property;
        }

        ~ModifyProperty() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private int priority;

        /// <summary>
        /// 修改<see cref="IProperty{TValue}"/>的优先级, 优先级越高, 越后修改, 升序排列<br/>
        /// 在同一优先级先进行所有的<see cref="AddModifyProperty"/>修正, 再进行所有的<see cref="MultModifyProperty"/>修正
        /// </summary>
        public int Priority =>
            _disposed ? default : priority; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");

        private IProperty? _property;

        protected void ReCheckValue()
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            _property?.ReCheckValue();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            priority = 0;
            if (disposing && _property != null) _property.RemoveModify(this);
            _property = null;
            _disposed = true;
        }

        int IComparer<ModifyProperty>.Compare(ModifyProperty x, ModifyProperty y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;

            if (x.Priority > y.Priority) return 1;
            if (x.Priority < y.Priority) return -1;

            return x switch
            {
                AddModifyProperty => y is AddModifyProperty ? 0 : -1,
                _ => y is MultModifyProperty ? 0 : 1
            };
        }
    }


    /// <summary>
    /// <inheritdoc/>
    /// 加值修饰, 和原来值进行相加
    /// </summary>
    [Serializable]
    public sealed class AddModifyProperty : ModifyProperty
    {
        public static AddModifyProperty Create(IProperty property, int priority, float addValue,
            IModifiableOwner? sender, object? reason = null)
        {
            var addModifyProperty = new AddModifyProperty(priority, addValue, property);
            property.Modify(addModifyProperty, sender, reason);
            return addModifyProperty;
        }

        private AddModifyProperty(int priority, float addValue, IProperty property) : base(priority, property) =>
            this.addValue = addValue;

        ~AddModifyProperty() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private float addValue;

        /// <summary>
        /// 加值修正值
        /// </summary>
        public float AddValue
        {
            get => _disposed ? default : addValue; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            set
            {
                if (_disposed) return; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (addValue == value) return;
                addValue = value;
                ReCheckValue();
            }
        }

        protected override void Dispose(bool _)
        {
            if (!_disposed)
            {
                addValue = 0;
                _disposed = true;
            }

            base.Dispose(_);
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// 乘修饰, 和原来值进行相乘
    /// </summary>
    [Serializable]
    public sealed class MultModifyProperty : ModifyProperty
    {
        public static MultModifyProperty Create(IProperty property, int priority, float addValue,
            IModifiableOwner? sender, object? reason = null)
        {
            var multModifyProperty = new MultModifyProperty(priority, addValue, property);
            property.Modify(multModifyProperty, sender, reason);
            return multModifyProperty;
        }

        private MultModifyProperty(int priority, float multValue, IProperty property) : base(priority, property) =>
            this.multValue = multValue;

        ~MultModifyProperty() => Dispose(false);
        [NonSerialized] private bool _disposed;
        [SerializeField] private float multValue;

        /// <summary>
        /// 乘值修正值
        /// </summary>
        public float MultValue
        {
            get => _disposed ? default : multValue; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            set
            {
                if (_disposed) return; // throw new ObjectDisposedException($"{GetType().Name}已经被释放！");

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (multValue == value) return;
                multValue = value;
                ReCheckValue();
            }
        }

        protected override void Dispose(bool _)
        {
            if (!_disposed)
            {
                multValue = 0;
                _disposed = true;
            }

            base.Dispose(_);
        }
    }
}