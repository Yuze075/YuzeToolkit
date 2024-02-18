#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace YuzeToolkit.BindableTool
{
    public struct BindableDictionary
    {
        private Dictionary<IModifiableOwner, HashSet<IReadOnlyBindable>>? _readOnlyBindables;

        internal IReadOnlyDictionary<IModifiableOwner, HashSet<IReadOnlyBindable>>? ReadOnlyBindables =>
            _readOnlyBindables;

        internal void AddBindable(IModifiableOwner modifiableOwner, IReadOnlyBindable bindable)
        {
            _readOnlyBindables ??= DictionaryPool<IModifiableOwner, HashSet<IReadOnlyBindable>>.Get();
            // 存在对应的modifiableOwner, 直接添加掉列表中
            if (_readOnlyBindables.TryGetValue(modifiableOwner, out var hashSet))
            {
                hashSet.Add(bindable);
                return;
            }

            // 不存在对应的modifiableOwner, 则创建一个List<IReadOnlyBindable>
            var newHashSet = HashSetPool<IReadOnlyBindable>.Get();
            newHashSet.Add(bindable);
            _readOnlyBindables[modifiableOwner] = newHashSet;
        }

        #region RemoveBindable

        internal bool RemoveBindable<T>([MaybeNullWhen(false)] out T bindable)
        {
            if (_readOnlyBindables == null)
            {
                bindable = default;
                return false;
            }

            foreach (var (modifiableOwner, bindables) in _readOnlyBindables)
                if (RemoveBindableInList(modifiableOwner, bindables, out bindable))
                    return true;

            bindable = default;
            return false;
        }

        internal bool RemoveBindable<T>(IModifiableOwner modifiableOwner, [MaybeNullWhen(false)] out T bindable)
        {
            if (_readOnlyBindables == null)
            {
                bindable = default;
                return false;
            }

            if (_readOnlyBindables.TryGetValue(modifiableOwner, out var bindables))
                return RemoveBindableInList(modifiableOwner, bindables, out bindable);

            bindable = default;
            return false;
        }

        private bool RemoveBindableInList<T>(IModifiableOwner modifiableOwner, HashSet<IReadOnlyBindable> bindables,
            [MaybeNullWhen(false)] out T bindable)
        {
            if (_readOnlyBindables == null)
            {
                bindable = default;
                return false;
            }

            bindable = bindables.OfType<T>().FirstOrDefault();
            if (bindable == null) return false;
            
            bindables.Remove((IReadOnlyBindable)bindable);
            if (bindables.Count == 0)
            {
                _readOnlyBindables.Remove(modifiableOwner);
                HashSetPool<IReadOnlyBindable>.Release(bindables);
            }
            return true;
        }

        internal bool RemoveBindable(Type targetType, [MaybeNullWhen(false)] out IReadOnlyBindable bindable)
        {
            if (_readOnlyBindables == null)
            {
                bindable = default;
                return false;
            }

            foreach (var (modifiableOwner, bindables) in _readOnlyBindables)
                if (RemoveBindableInList(targetType, modifiableOwner, bindables, out bindable))
                    return true;

            bindable = default;
            return false;
        }

        internal bool RemoveBindable(Type targetType, IModifiableOwner modifiableOwner,
            [MaybeNullWhen(false)] out IReadOnlyBindable bindable)
        {
            if (_readOnlyBindables == null)
            {
                bindable = default;
                return false;
            }

            if (_readOnlyBindables.TryGetValue(modifiableOwner, out var bindables))
                return RemoveBindableInList(targetType, modifiableOwner, bindables, out bindable);

            bindable = default;
            return false;
        }

        private bool RemoveBindableInList(Type targetType, IModifiableOwner modifiableOwner,
            HashSet<IReadOnlyBindable> bindables, [MaybeNullWhen(false)] out IReadOnlyBindable bindable)
        {
            if (_readOnlyBindables == null)
            {
                bindable = default;
                return false;
            }

            bindable = bindables.Where(targetType.IsInstanceOfType).FirstOrDefault();
            if (bindable == null) return false;
            
            bindables.Remove(bindable);
            if (bindables.Count == 0)
            {
                _readOnlyBindables.Remove(modifiableOwner);
                HashSetPool<IReadOnlyBindable>.Release(bindables);
            }
            return true;
        }

        internal bool RemoveBindable(IModifiableOwner modifiableOwner, IReadOnlyBindable bindable) =>
            _readOnlyBindables != null && _readOnlyBindables.TryGetValue(modifiableOwner, out var bindables) &&
            bindables.Remove(bindable);

        #endregion

        internal void Clear()
        {
            if (_readOnlyBindables == null) return;
            foreach (var hashSet in _readOnlyBindables.Values)
            {
                foreach (var readOnlyBindable in hashSet) readOnlyBindable.Reset();
                hashSet.Clear();
                HashSetPool<IReadOnlyBindable>.Release(hashSet);
            }

            DictionaryPool<IModifiableOwner, HashSet<IReadOnlyBindable>>.RefRelease(ref _readOnlyBindables);
        }
    }

    public interface IBindableNode
    {
        protected internal BindableDictionary Bindables { get; }
    }
}