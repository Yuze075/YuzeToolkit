#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="byte"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyByte : NetProperty<byte>
    {
        protected NetPropertyByte(byte valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override byte Min => byte.MinValue;
        public override byte Max => byte.MaxValue;
        protected override double CastToDouble(byte value) => value;

        protected override byte CastToTValue(double value) => value switch
        {
            >= byte.MaxValue => byte.MaxValue,
            <= byte.MinValue => byte.MinValue,
            _ => (byte)value
        };
    }
}
#endif