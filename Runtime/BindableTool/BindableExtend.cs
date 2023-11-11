using System;
using System.Collections.Generic;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    public static class BindableExtend
    {
        #region IBindableSystemOwner

        /// <summary>
        /// <see cref="IBindableSystemOwner.BindableSystem"/>中所有的<see cref="IBindable"/>值
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static IReadOnlyList<IBindable> Bindables(this IBindableSystemOwner self) => self.BindableSystem.Bindables;

        /// <summary>
        /// 获取<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public static T? GetBindable<T>(this IBindableSystemOwner self) where T : IBindable => self.BindableSystem.GetBindable<T>();

        /// <summary>
        /// 尝试获取到<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public static bool TryGetBindable<T>(this IBindableSystemOwner self, out T t) where T : IBindable =>
            self.BindableSystem.TryGetBindable(out t);

        /// <summary>
        /// 是否存在<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public static bool ContainsBindable<T>(this IBindableSystemOwner self) where T : IBindable =>
            self.BindableSystem.ContainsBindable<T>();

        /// <summary>
        /// 重新检测所有的值
        /// </summary>
        public static void ReCheckBindable(this IBindableSystemOwner self) => self.BindableSystem.ReCheckBindable();

        #endregion
        
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

        public static bool TryCheckModify<TModify>(this IModifiable modifiable, TModify? modifyIn, IModifyReason reason,
            out TModify modifyOut)
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

        public static RegisterChange<object> GetRegisterChange(this IBindable bindable) => bindable.RegisterChange;

        public static RegisterChangeBuff<object> GetRegisterChangeBuff(this IBindable bindable) =>
            bindable.RegisterChangeBuff;

        public static RegisterChange<TValue> GetRegisterChange<TValue>(this IBindable<TValue> bindable) =>
            bindable.RegisterChange;

        public static RegisterChangeBuff<TValue> GetRegisterChangeBuff<TValue>(this IBindable<TValue> bindable) =>
            bindable.RegisterChangeBuff;

        public static RegisterChange<TCastTo> GetRegisterChange<TValue, TCastTo>(this IBindable<TValue> bindable,
            Func<TValue?, TCastTo?> predicate) => valueChange => bindable.RegisterChange(
            (value, newValue) => valueChange(predicate(value), predicate(newValue)));

        public static RegisterChangeBuff<TCastTo> GetRegisterChangeBuff<TValue, TCastTo>(
            this IBindable<TValue> bindable, Func<TValue?, TCastTo?> predicate) => valueChange =>
            bindable.RegisterChangeBuff((value, newValue) => valueChange(predicate(value), predicate(newValue)));

        public static RegisterChangeBuff<TValue> GetRegisterChangeBuff<TValue>(this TValue value) => valueChange =>
        {
            valueChange(default, value);
            return NullDisposable;
        };

        private static readonly IDisposable NullDisposable = new UnRegister((Action?)null);
    }
}