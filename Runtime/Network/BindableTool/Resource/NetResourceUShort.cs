#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="ushort"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceUShort : NetResource<ushort>
    {
        protected NetResourceUShort(ushort valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override ushort Min => ushort.MinValue;
        public override ushort Max => ushort.MaxValue;
        protected sealed override float CastToFloat(ushort value) => value;

        protected sealed override ushort CastToTValue(float value) => value switch
        {
            >= ushort.MaxValue => ushort.MaxValue,
            <= ushort.MinValue => ushort.MinValue,
            _ => (ushort)value
        };
    }
}
#endif