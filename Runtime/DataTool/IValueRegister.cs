using System.Collections.Generic;

namespace YuzeToolkit.DataTool
{
    public interface IValueRegister
    {
        void RegisterValue<TValue>(TValue value);
        void RegisterValues<TValue>(IEnumerable<TValue> values);
    }
}