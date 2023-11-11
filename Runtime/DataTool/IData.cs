using System;
using System.Collections.Generic;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DataTool
{
    public interface IData : IDisposable
    {
        void Clear();
        ILogTool Parent { set; }
    }

    public interface IData<TValue> : IData
    {
        IReadOnlyList<TValue> Values { get; }
        TValue? Get<TGetId>(TGetId getId);
        bool TryGet<TGetId>(TGetId getId, out TValue value);
        int GetIndex(TValue value);
        int GetIndex<TGetId>(TGetId getId);
        TValue? GetByIndex(int index, int idHashCode);
        bool TryGetByIndex(int index, int idHashCode, out TValue value);
        internal void RegisterValue(TValue value);
        internal void RegisterValues(IEnumerable<TValue> values);
    }
}