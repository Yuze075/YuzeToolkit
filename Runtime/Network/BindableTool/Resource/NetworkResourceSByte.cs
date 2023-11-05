#if USE_UNITY_NETCODE
using Unity.Netcode;

namespace YuzeToolkit.Network.BindableTool
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="sbyte"/>的数据类型进行资源的计算
    /// </summary>
    public abstract class NetworkResourceSByte : NetworkResource<sbyte>
    {
        protected NetworkResourceSByte(sbyte valueBase,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) : base(valueBase,
            readPerm, writePerm)
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