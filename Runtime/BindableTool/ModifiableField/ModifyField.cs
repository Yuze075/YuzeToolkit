using System;
using UnityEngine;
using YuzeToolkit.SerializeTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 对<see cref="IModifiableField{TValue}"/>的值进行修改
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class ModifyField<TValue> : IModify
    {
        public ModifyField(TValue? modifyValue) => this.modifyValue = modifyValue;

        public ModifyField(TValue? modifyValue, Type tryModifyType) : this(modifyValue) =>
            this.tryModifyType = tryModifyType;

        [SerializeField] private SerializeType tryModifyType;
        public Type? TryModifyType => tryModifyType;
        [SerializeField] private TValue? modifyValue;
        public TValue? ModifyValue => modifyValue;
        void IDisposable.Dispose() => modifyValue = default!;
    }
}