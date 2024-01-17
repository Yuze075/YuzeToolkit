#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.BindableTool
{
    public interface IFieldList<TValue> : IBindableList<TValue>, IList<TValue>
    {
        new TValue this[int index] { get; set; }
        new int Count { get; }

        [return: NotNullIfNotNull("fieldListChange")]
        IDisposable? RegisterChange(FieldListChange<TValue>? fieldListChange);

        [return: NotNullIfNotNull("fieldListChange")]
        IDisposable? RegisterChangeBuff(FieldListChange<TValue>? fieldListChange);
    }
}