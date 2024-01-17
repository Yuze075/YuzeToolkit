#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="float"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceFloat : NetResource<float>
    {
        protected NetResourceFloat(float valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override float Min => float.MinValue;
        public override float Max => float.MaxValue;
        protected sealed override float CastToFloat(float value) => value;

        protected sealed override float CastToTValue(float value) => value switch
        {
            >= float.MaxValue => float.MaxValue,
            <= float.MinValue => float.MinValue,
            _ => value
        };
    }
}
#endif