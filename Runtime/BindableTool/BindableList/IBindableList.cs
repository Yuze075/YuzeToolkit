#nullable enable
using System.Collections.Generic;

namespace YuzeToolkit.BindableTool
{
    public delegate void ReadOnlyBindableListChange<in TValue>(IReadOnlyBindableList<TValue> readOnlyBindableList);

    public delegate void ListChange<in TValue>(EventType eventType, TValue value, int index,
        TValue? previousValue = default);

    public delegate void BindableListChange<TValue>(IBindableList<TValue> fieldList);

    public enum EventType : byte
    {
        /// <summary>
        /// Value值为添加值, Index为插入的位置
        /// </summary>
        Add,

        /// <summary>
        /// Value值为移除值, Index为移除的位置
        /// </summary>
        Remove,

        /// <summary>
        /// Value值为替换值, Index为替换的位置, PreviousValue值为替换前的值
        /// </summary>
        Value,

        /// <summary>
        /// 清空整个列表, Value值为空值, 且Index为-1
        /// </summary>
        Clear
    }

    public interface IReadOnlyBindableList<out TValue> : IReadOnlyList<TValue>, IReadOnlyBindable
    {
        void AddBindableListChange(ReadOnlyBindableListChange<TValue>? bindableListChange);
        void RemoveBindableListChange(ReadOnlyBindableListChange<TValue>? bindableListChange);
        void AddListChange(ListChange<TValue>? listChange);
        void RemoveListChange(ListChange<TValue>? listChange);
    }

    public interface IBindableList<TValue> : IReadOnlyBindableList<TValue>, IList<TValue>
    {
        new TValue this[int index] { get; set; }
        new int Count { get; }
        void AddBindableListChange(BindableListChange<TValue>? bindableListChange);
        void RemoveBindableListChange(BindableListChange<TValue>? bindableListChange);

        void IReadOnlyBindableList<TValue>.
            AddBindableListChange(ReadOnlyBindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return;
            AddBindableListChange(new BindableListChange<TValue>(bindableListChange));
        }

        void IReadOnlyBindableList<TValue>.RemoveBindableListChange(
            ReadOnlyBindableListChange<TValue>? bindableListChange)
        {
            if (bindableListChange == null) return;
            RemoveBindableListChange(new BindableListChange<TValue>(bindableListChange));
        }
    }
}