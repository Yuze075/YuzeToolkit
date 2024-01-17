#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="int"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetResourceInt : NetResource<int>
    {
        protected NetResourceInt(int valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override int Min => int.MinValue;
        public override int Max => int.MaxValue;
        protected sealed override float CastToFloat(int value) => value;

        protected sealed override int CastToTValue(float value) => value switch
        {
            >= int.MaxValue => int.MaxValue,
            <= int.MinValue => int.MinValue,
            _ => (int)value
        };
    }
}
#endif