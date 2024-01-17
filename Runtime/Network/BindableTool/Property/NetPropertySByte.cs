#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="sbyte"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertySByte : NetProperty<sbyte>
    {
        protected NetPropertySByte(sbyte valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override sbyte Min => sbyte.MinValue;
        public override sbyte Max => sbyte.MaxValue;
        protected sealed override double CastToDouble(sbyte value) => value;

        protected sealed override sbyte CastToTValue(double value) => value switch
        {
            >= sbyte.MaxValue => sbyte.MaxValue,
            <= sbyte.MinValue => sbyte.MinValue,
            _ => (sbyte)value
        };
    }
}
#endif