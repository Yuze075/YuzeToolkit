#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool
{
    public interface IBindableList<out TValue> : IReadOnlyList<TValue>, IBindable
    {
        [return: NotNullIfNotNull("bindableListChange")]
        IDisposable? RegisterChange(BindableListChange<TValue>? bindableListChange);

        [return: NotNullIfNotNull("bindableListChange")]
        IDisposable? RegisterChangeBuff(BindableListChange<TValue>? bindableListChange);

        [return: NotNullIfNotNull("listChange")]
        IDisposable? RegisterListChange(ListChange<TValue>? listChange);
    }
}