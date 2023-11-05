using System.Collections.Generic;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DataTool
{
    public interface IData
    {
        TGetValue? Get<TGetValue, TGetId>(TGetId getId);
        bool TryGet<TGetValue, TGetId>(TGetId getId, out TGetValue getValue);
        internal ILogTool Parent { set; }
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
        internal void AddData(TValue value);
        internal void AddData(IEnumerable<TValue> values);
        internal void Clear();
    }
}