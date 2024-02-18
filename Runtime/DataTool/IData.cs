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
        bool TryGetHashInData(TValue value, out int hashInData);
        TValue? GetValueByHash(int hashInData, int idHashCode);
        bool TryGetValueByHash(int hashInData, int idHashCode, [MaybeNullWhen(false)] out TValue value);
        internal void RegisterValue(TValue value);
        internal void RegisterValues(IEnumerable<TValue> values);
    }
    
    public interface IModel<out TId>
    {
        /// <summary>
        /// 对应Model的Id
        /// </summary>
        TId Id { get; }
    }
}