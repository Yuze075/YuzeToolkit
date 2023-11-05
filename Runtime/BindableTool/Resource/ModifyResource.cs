using System;
using UnityEngine;
using YuzeToolkit.SerializeTool;

namespace YuzeToolkit.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 对<see cref="IResource{TValue}"/>的资源值<see cref="IResource{TValue}.Value"/>的修饰, 调整<see cref="modifyValue"/>对其进行加或者减
    /// </summary>
    [Serializable]
    public sealed class ModifyResource : IModify
    {
        public ModifyResource(Type tryModifyType, float modifyValue) =>
            (this.tryModifyType, this.modifyValue) = (tryModifyType, modifyValue);

        [SerializeField] private SerializeType tryModifyType;
        public Type? TryModifyType => tryModifyType;
        [SerializeField] public float modifyValue;
        void IDisposable.Dispose() => modifyValue = default;
    }
}