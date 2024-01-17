#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.BindableTool
{
    public delegate void ValueChange<in TValue>(TValue? oldValue, TValue? newValue);

    public delegate void BindableListChange<in TValue>(IBindableList<TValue> bindableList);

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
        Clear,

        // /// <summary>
        // /// 填充整个列表(初始化操作)
        // /// </summary>
        // Full,
    }

    public delegate void ListChange<in TValue>(EventType eventType, TValue value, int index,
        TValue? previousValue = default);

    public delegate void FieldListChange<TValue>(IFieldList<TValue> fieldList);

    public delegate void OutOfMaxRange<in TValue>(TValue maxValue, float outMaxValue);

    public delegate void OutOfMinRange<in TValue>(TValue minValue, float outMinValue);

    public static class BindableExtend
    {
        /// <summary>
        /// 获取<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public static T? GetBindable<T>(this IBindableSystemOwner self) where T : IBindable =>
            self.BindableSystem.GetBindable<T>();

        /// <summary>
        /// 尝试获取到<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public static bool TryGetBindable<T>(this IBindableSystemOwner self, [MaybeNullWhen(false)] out T t)
            where T : IBindable => self.BindableSystem.TryGetBindable(out t);

        /// <summary>
        /// 是否存在<see cref="T"/>类型的<see cref="IBindable"/>
        /// </summary>
        public static bool ContainsBindable<T>(this IBindableSystemOwner self) where T : IBindable =>
            self.BindableSystem.ContainsBindable<T>();
    }
}