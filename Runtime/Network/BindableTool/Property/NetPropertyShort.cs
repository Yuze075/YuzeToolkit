#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    /// <summary>
    /// <inheritdoc/>
    /// 使用<see cref="short"/>的数据类型进行属性的计算
    /// </summary>
    [Serializable]
    public abstract class NetPropertyShort : NetProperty<short>
    {
        protected NetPropertyShort(short valueBase = default, bool isReadOnly = true,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            IModifiableOwner? modifiableOwner = null, ILogging? loggingParent = null) :
            base(valueBase, isReadOnly, readPerm, writePerm, modifiableOwner, loggingParent)
        {
        }

        public override short Min => short.MinValue;
        public override short Max => short.MaxValue;
        protected sealed override double CastToDouble(short value) => value;

        protected sealed override short CastToTValue(double value) => value switch
        {
            >= short.MaxValue => short.MaxValue,
            <= short.MinValue => short.MinValue,
            _ => (short)value
        };
    }
}
#endif