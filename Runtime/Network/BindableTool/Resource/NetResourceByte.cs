#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="byte"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceByte : NetResource<byte>
    {
        protected NetResourceByte(byte valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override byte Min => byte.MinValue;
        public override byte Max => byte.MaxValue;
        protected sealed override float CastToFloat(byte value) => value;

        protected sealed override byte CastToTValue(float value) => value switch
        {
            >= byte.MaxValue => byte.MaxValue,
            <= byte.MinValue => byte.MinValue,
            _ => (byte)value
        };
    }
}
#endif