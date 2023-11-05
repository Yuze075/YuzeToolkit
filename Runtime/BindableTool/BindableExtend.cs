using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    public static class BindableExtend
    {
        /// <summary>
        /// 判断是否为一个<see cref="IModifyReason"/>触发的修改<see cref="IModifiableOwner"/>
        /// </summary>
        public static bool IsSameOwnerReason(this IModifiable modifiable, IModifyReason reason, ILogTool logTool)
        {
            if (!modifiable.IsReadOnly || (reason is IModifiableOwnerReason otherOwnerReason
                                           && otherOwnerReason.OtherOwner == modifiable.Owner)) return true;

            logTool.Log($"当前{modifiable.GetType()}为ReadOnly, " +
                       $"只有{nameof(IModifiableOwnerReason)}的otherOwner和{modifiable.GetType()}的{nameof(IModifiable.Owner)}相同才可以修改!",
                ELogType.Warning);
            return false;
        }

        public static bool TryCastModify<TCast>(this IModifiable modifiable, IModify modify, ILogTool logTool,
            out TCast value) where TCast : IModify
        {
            if (modify is not TCast cast)
            {
                logTool.Log(
                    $"{nameof(IModifiable.Modify)}传入的{nameof(modify)}为{modify.GetType()}类型, 不是{typeof(TCast)}类型, " +
                    $"不能修正{modifiable.GetType()}!", ELogType.Error);
                value = default!;
                return false;
            }

            value = cast;
            return true;
        }

        public static bool TryCheckModify<TModify>(this IModifiable modifiable, TModify? modifyIn, IModifyReason reason, out TModify modifyOut)
            where TModify : IModify
        {
            modifiable.Owner.CheckModify(modifiable, ref modifyIn, reason);
            if (modifyIn == null)
            {
                modifyOut = default!;
                return false;
            }
            
            modifyOut = modifyIn!;
            return true;
        }

        public static bool TryCheckModifyType(this IModifiable modifiable, IModify modify, ILogTool logTool)
        {
            if (modify.TryModifyType == modifiable.GetType()) return true;
            
            logTool.Log(
                $"{nameof(IModifiable.Modify)}传入的{modify.GetType()}的TryModifyType为{modify.TryModifyType}," +
                $" 不是对应的类型{modifiable.GetType()}", ELogType.Error);
            return false;
        }
    }
}