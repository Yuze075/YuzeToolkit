#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="ushort"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyUShort : NetProperty<ushort>
    {
        protected NetPropertyUShort(ushort valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }
        public override ushort Min => ushort.MinValue;
        public override ushort Max => ushort.MaxValue;
        protected sealed override double CastToDouble(ushort value) => value;

        protected sealed override ushort CastToTValue(double value) => value switch
        {
            >= ushort.MaxValue => ushort.MaxValue,
            <= ushort.MinValue => ushort.MinValue,
            _ => (ushort)value
        };
    }
}
#endif