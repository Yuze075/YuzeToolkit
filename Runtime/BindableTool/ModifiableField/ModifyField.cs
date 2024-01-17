#nullable enable
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
    public struct ModifyField<TValue> : IModify
    {
        public ModifyField(TValue? modifyValue) => this.modifyValue = modifyValue;

        [SerializeField] public TValue? modifyValue;
    }
}