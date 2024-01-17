#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="sbyte"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceSByte : NetResource<sbyte>
    {
        protected NetResourceSByte(sbyte valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }
        public override sbyte Min => sbyte.MinValue;
        public override sbyte Max => sbyte.MaxValue;
        protected sealed override float CastToFloat(sbyte value) => value;

        protected sealed override sbyte CastToTValue(float value) => value switch
        {
            >= sbyte.MaxValue => sbyte.MaxValue,
            <= sbyte.MinValue => sbyte.MinValue,
            _ => (sbyte)value
        };
    }
}
#endif