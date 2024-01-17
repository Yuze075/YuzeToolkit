#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace YuzeToolkit.DataTool
{
    public interface IData
    {
        void Clear();
    }

    public interface IData<TValue> : IData
    {
        IReadOnlyList<TValue> Values { get; }
        int GetIndex(TValue value);
        bool TryGetIndex(TValue value, out int index);
        TValue? GetByIndex(int index, int idHashCode);
        bool TryGetByIndex(int index, int idHashCode, [MaybeNullWhen(false)] out TValue value);
        internal void RegisterValue(TValue value);
        internal void RegisterValues(IEnumerable<TValue> values);
    }
}