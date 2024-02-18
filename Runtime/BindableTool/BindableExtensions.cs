#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace YuzeToolkit.BindableTool
{
    public static class BindableExtensions
    {
        #region IReadOnlyBindable

        /// <summary>
        /// 注册<see cref="IReadOnlyBindable{TValue}.Value"/>改变的回调, 获取到旧数值和新数值
        /// </summary>
        [return: NotNullIfNotNull("valueChange")]
        public static IDisposable? RegisterValueChange<TValue>(this IReadOnlyBindable<TValue> self,
            ValueChange<TValue>? valueChange)
        {
            if (valueChange == null) return null;
            self.AddValueChange(valueChange);
            return DisposableExtensions.UnRegister(self.RemoveValueChange, valueChange);
        }

        public static void AddValueChangeBuff<TValue>(this IReadOnlyBindable<TValue> self,
            ValueChange<TValue>? valueChange)
        {
            if (valueChange == null) return;
            self.AddValueChange(valueChange);
            valueChange.Invoke(default, self.Value);
        }

        /// <summary>
        /// 注册<see cref="IReadOnlyBindable{TValue}.Value"/>改变的回调, 获取到旧数值和新数值, 并且注册时就获得缓存的数值
        /// </summary>
        [return: NotNullIfNotNull("valueChange")]
        public static IDisposable? RegisterValueChangeBuff<TValue>(this IReadOnlyBindable<TValue> self,
            ValueChange<TValue>? valueChange)
        {
            if (valueChange == null) return null;
            self.AddValueChange(valueChange);
            valueChange(default, self.Value);
            return DisposableExtensions.UnRegister(self.RemoveValueChange, valueChange);
        }

        #endregion

        #region IModifiable

        public static bool CheckModify<TModifiable, TModify>(this TModifiable modifiable, TModify modifyIn,
            [MaybeNullWhen(false)] out TModify modifyOut, object? sender, object? reason)
            where TModifiable : IModifiable<TModify>
        {
            modifyOut = modifyIn;
            var owner = modifiable.ModifiableOwner;
            // Owner为空, 直接修正返回
            if (owner == null) return true;

            // Owner不为空, 先进行检查, 然后再修正
            return owner.CheckModify(modifiable, ref modifyOut, sender, reason) && modifyOut != null;
        }

        #endregion

        #region IReadOnlyResource

        /// <summary>
        /// 注册超出最大值的事件，返回超出的值和最大值
        /// </summary>
        [return: NotNullIfNotNull("outOfMaxRange")]
        public static IDisposable? RegisterOutOfMaxRange<TValue>(this IReadOnlyResource<TValue> self,
            OutOfMaxRange<TValue>? outOfMaxRange)
        {
            if (outOfMaxRange == null) return null;
            self.AddOutOfMaxRange(outOfMaxRange);
            return DisposableExtensions.UnRegister(self.RemoveOutOfMaxRange, outOfMaxRange);
        }

        /// <summary>
        /// 注册超出最小值的事件，返回超出的值和最小值
        /// </summary>
        [return: NotNullIfNotNull("outOfMinRange")]
        public static IDisposable? RegisterOutOfMinRange<TValue>(this IReadOnlyResource<TValue> self,
            OutOfMinRange<TValue>? outOfMinRange)
        {
            if (outOfMinRange == null) return null;
            self.AddOutOfMinRange(outOfMinRange);
            return DisposableExtensions.UnRegister(self.RemoveOutOfMinRange, outOfMinRange);
        }

        #endregion

        #region IReadOnlyBindableList

        [return: NotNullIfNotNull("bindableListChange")]
        public static IDisposable? RegisterBindableListChange<TValue>(this IReadOnlyBindableList<TValue> self,
            ReadOnlyBindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return null;
            self.AddBindableListChange(bindableListChange);
            return DisposableExtensions.UnRegister(self.RemoveBindableListChange, bindableListChange);
        }

        [return: NotNullIfNotNull("bindableListChange")]
        public static IDisposable? RegisterBindableListChangeBuff<TValue>(this IReadOnlyBindableList<TValue> self,
            ReadOnlyBindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return null;
            self.AddBindableListChange(bindableListChange);
            bindableListChange(self);
            return DisposableExtensions.UnRegister(self.RemoveBindableListChange, bindableListChange);
        }

        [return: NotNullIfNotNull("listChange")]
        public static IDisposable? RegisterListChange<TValue>(this IReadOnlyBindableList<TValue> self,
            ListChange<TValue>? listChange)
        {
            if (listChange == null) return null;
            self.AddListChange(listChange);
            return DisposableExtensions.UnRegister(self.AddListChange, listChange);
        }


        [return: NotNullIfNotNull("bindableListChange")]
        public static IDisposable? RegisterBindableListChange<TValue>(this IBindableList<TValue> self,
            BindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return null;
            self.AddBindableListChange(bindableListChange);
            return DisposableExtensions.UnRegister(self.RemoveBindableListChange, bindableListChange);
        }

        [return: NotNullIfNotNull("bindableListChange")]
        public static IDisposable? RegisterBindableListChangeBuff<TValue>(this IBindableList<TValue> self,
            BindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return null;
            self.AddBindableListChange(bindableListChange);
            bindableListChange(self);
            return DisposableExtensions.UnRegister(self.RemoveBindableListChange, bindableListChange);
        }

        #endregion

        #region IReadOnlyBindableRegister

        public static void AddBindable<TValue>(this IBindableNode self, BindableField<TValue> bindableField,
            IModifiableOwner modifiableOwner, TValue? value)
        {
            bindableField.SetOnly(value);
            self.AddBindable(bindableField, modifiableOwner);
        }

        public static void AddBindable<TValue>(this IBindableNode self, Property<TValue> property,
            IModifiableOwner modifiableOwner, TValue valueBase) where TValue : unmanaged
        {
            property.SetOnly(valueBase, modifiableOwner);
            self.AddBindable(property, modifiableOwner);
        }

        public static void AddBindable<TValue>(this IBindableNode self, Resource<TValue> resource,
            IModifiableOwner modifiableOwner, TValue value)
            where TValue : unmanaged
        {
            resource.SetOnly(value, modifiableOwner);
            self.AddBindable(resource, modifiableOwner);
        }

        public static void AddBindable(this IBindableNode self, State state, IModifiableOwner modifiableOwner,
            bool valueBase)
        {
            state.SetOnly(valueBase, modifiableOwner);
            self.AddBindable(state, modifiableOwner);
        }

        #endregion

        #region IBindableNode

        public static void AddBindable(this IBindableNode self, IReadOnlyBindable bindable,
            IModifiableOwner modifiableOwner) => self.Bindables.AddBindable(modifiableOwner, bindable);

        public static void ClearAllBindables(this IBindableNode self) => self.Bindables.Clear();

        public static bool RemoveBindable<T>(this IBindableNode self) =>
            self.Bindables.RemoveBindable<T>(out _);

        public static bool RemoveBindable<T>(this IBindableNode self, [MaybeNullWhen(false)] out T bindable) =>
            self.Bindables.RemoveBindable(out bindable);

        public static bool RemoveBindable<T>(this IBindableNode self, IModifiableOwner modifiableOwner) =>
            self.Bindables.RemoveBindable<T>(modifiableOwner, out _);

        public static bool RemoveBindable<T>(this IBindableNode self, IModifiableOwner modifiableOwner,
            [MaybeNullWhen(false)] out T bindable) =>
            self.Bindables.RemoveBindable(modifiableOwner, out bindable);

        public static bool RemoveBindable(this IBindableNode self, IModifiableOwner modifiableOwner,
            IReadOnlyBindable bindable) => self.Bindables.RemoveBindable(modifiableOwner, bindable);

        public static T? GetBindable<T>(this IBindableNode self) =>
            self.TryGetBindable<T>(out var bindable) ? bindable : default;

        public static bool TryGetBindable<T>(this IBindableNode self, [MaybeNullWhen(false)] out T bindable)
        {
            if (self.Bindables.ReadOnlyBindables is not { } dictionary)
            {
                bindable = default;
                return false;
            }

            foreach (var bindables in dictionary.Values)
                if (TryGetBindableInList(bindables, out bindable))
                    return true;

            bindable = default;
            return false;
        }

        private static bool TryGetBindableInList<T>(IEnumerable<IReadOnlyBindable> bindables,
            [MaybeNullWhen(false)] out T bindable)
        {
            foreach (var readOnlyBindable in bindables)
            {
                if (readOnlyBindable is not T value) continue;
                bindable = value;
                return true;
            }

            bindable = default;
            return false;
        }

        public static T? GetBindable<T>(this IBindableNode self, IModifiableOwner modifiableOwner) =>
            self.TryGetBindable<T>(modifiableOwner, out var bindable) ? bindable : default;

        public static bool TryGetBindable<T>(this IBindableNode self, IModifiableOwner modifiableOwner,
            [MaybeNullWhen(false)] out T bindable)
        {
            if (self.Bindables.ReadOnlyBindables is not { } dictionary)
            {
                bindable = default;
                return false;
            }

            if (dictionary.TryGetValue(modifiableOwner, out var bindables))
                return TryGetBindableInList(bindables, out bindable);

            bindable = default;
            return false;
        }

        public static IEnumerable<T> GetBindables<T>(this IBindableNode self)
        {
            if (self.Bindables.ReadOnlyBindables is not { } dictionary) yield break;
            foreach (var bindables in dictionary.Values)
            foreach (var bindable in bindables.OfType<T>())
                yield return bindable;
        }

        public static bool ContainsBindable<T>(this IBindableNode self) =>
            self.Bindables.ReadOnlyBindables is { } dictionary &&
            dictionary.Values.Any(bindables => bindables.OfType<T>().Any());

        public static bool ContainsBindable<T>(this IBindableNode self, IModifiableOwner modifiableOwner) =>
            self.Bindables.ReadOnlyBindables is { } dictionary &&
            dictionary.TryGetValue(modifiableOwner, out var bindables) &&
            bindables.OfType<T>().Any();

        public static bool RemoveBindable(this IBindableNode self, Type targetType) =>
            self.Bindables.RemoveBindable(targetType, out _);

        public static bool RemoveBindable(this IBindableNode self, Type targetType,
            [MaybeNullWhen(false)] out IReadOnlyBindable bindable) =>
            self.Bindables.RemoveBindable(targetType, out bindable);

        public static bool RemoveBindable(this IBindableNode self, Type targetType, IModifiableOwner modifiableOwner) =>
            self.Bindables.RemoveBindable(targetType, modifiableOwner, out _);

        public static bool RemoveBindable(this IBindableNode self, Type targetType, IModifiableOwner modifiableOwner,
            [MaybeNullWhen(false)] out IReadOnlyBindable bindable) =>
            self.Bindables.RemoveBindable(targetType, modifiableOwner, out bindable);

        public static IReadOnlyBindable? GetBindable(this IBindableNode self, Type targetType) =>
            self.TryGetBindable(targetType, out var bindable) ? bindable : default;

        public static bool TryGetBindable(this IBindableNode self, Type targetType,
            [MaybeNullWhen(false)] out IReadOnlyBindable bindable)
        {
            if (self.Bindables.ReadOnlyBindables is not { } dictionary)
            {
                bindable = default;
                return false;
            }

            foreach (var bindables in dictionary.Values)
                if (TryGetBindableInList(targetType, bindables, out bindable))
                    return true;

            bindable = default;
            return false;
        }

        public static IReadOnlyBindable? GetBindable(this IBindableNode self, Type targetType,
            IModifiableOwner modifiableOwner) =>
            self.TryGetBindable(targetType, modifiableOwner, out var bindable) ? bindable : default;

        public static bool TryGetBindable(this IBindableNode self, Type targetType,
            IModifiableOwner modifiableOwner, [MaybeNullWhen(false)] out IReadOnlyBindable bindable)
        {
            if (self.Bindables.ReadOnlyBindables is not { } dictionary)
            {
                bindable = default;
                return false;
            }

            if (dictionary.TryGetValue(modifiableOwner, out var bindables))
                return TryGetBindableInList(targetType, bindables, out bindable);

            bindable = default;
            return false;
        }

        private static bool TryGetBindableInList(Type targetType, IEnumerable<IReadOnlyBindable> bindables,
            [MaybeNullWhen(false)] out IReadOnlyBindable bindable)
        {
            foreach (var readOnlyBindable in bindables.Where(targetType.IsInstanceOfType))
            {
                bindable = readOnlyBindable;
                return true;
            }

            bindable = default;
            return false;
        }

        public static IEnumerable<IReadOnlyBindable> GetBindables(this IBindableNode self, Type targetType)
        {
            if (self.Bindables.ReadOnlyBindables is not { } dictionary) yield break;
            foreach (var bindables in dictionary.Values)
            foreach (var readOnlyBindable in bindables.Where(targetType.IsInstanceOfType))
                yield return readOnlyBindable;
        }

        public static bool ContainsBindable(this IBindableNode self, Type targetType) =>
            self.Bindables.ReadOnlyBindables is { } dictionary &&
            dictionary.Values.Any(bindables => bindables.Where(targetType.IsInstanceOfType).Any());

        public static bool ContainsBindable(this IBindableNode self, Type targetType, IModifiableOwner modifiableOwner)
            => self.Bindables.ReadOnlyBindables is { } dictionary &&
               dictionary.TryGetValue(modifiableOwner, out var bindables) &&
               bindables.Where(targetType.IsInstanceOfType).Any();

        #endregion
    }
}