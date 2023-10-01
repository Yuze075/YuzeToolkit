using System.Collections.Generic;
using System.Linq;

namespace YuzeToolkit.IoC
{
    public class ListValueWrapper<T> : IListValueWrapper
    {
        private readonly List<T> _list = new();

        public bool TryGet<TValue>(out TValue tValue)
        {
            if (_list is TValue t1)
            {
                tValue = t1;
                return true;
            }

            var item = _list.FirstOrDefault(i => i != null);
            if (item is TValue t2)
            {
                tValue = t2;
                return true;
            }

            tValue = default;
            return false;
        }

        public bool TryAdd<TValue>(TValue value)
        {
            if (value is not T tValue) return false;
            
            _list.Add(tValue);
            return true;
        }
    }
}