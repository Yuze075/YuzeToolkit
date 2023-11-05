using System.Collections.Generic;

namespace YuzeToolkit.DataTool
{
    public interface IValueRegister
    {
        void AddValue<TValue>(TValue value);
        void AddValue<TValue>(IEnumerable<TValue> values);
    }
}