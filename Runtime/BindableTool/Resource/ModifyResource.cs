#nullable enable
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
    public struct ModifyResource : IModify
    {
        public ModifyResource(float modifyValue) => this.modifyValue = modifyValue;
        [SerializeField] public float modifyValue;
    }
}