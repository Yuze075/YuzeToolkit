using System.Collections.Generic;

namespace YuzeToolkit.IoC
{
    public interface IValueGetter
    {
        T Get<T>();
        bool TyrGet<T>(out T value);
        IEnumerable<T> GetEnumerable<T>();
        bool TyrGetNotFromParent<T>(out T value);
        IReadOnlyList<T> GetList<T>();
        bool TryGetList<T>(out IReadOnlyList<T> list);
    }
}