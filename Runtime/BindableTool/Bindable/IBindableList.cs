using System;
using System.Collections.Generic;

namespace YuzeToolkit.BindableTool
{
    public interface IBindableList<out TValue> : IReadOnlyList<TValue>, IBindable<IBindableList<TValue>>
    {
        object IBindable.Value => this;

        IDisposable IBindable.RegisterChange(ValueChange<object> valueChange) =>
            RegisterChange(list => { valueChange.Invoke(list, list); });

        IDisposable IBindable.RegisterChangeBuff(ValueChange<object> valueChange) =>
            RegisterChange(list => { valueChange.Invoke(list, list); });

        IBindableList<TValue> IBindable<IBindableList<TValue>>.Value => this;

        IDisposable IBindable<IBindableList<TValue>>.RegisterChange(ValueChange<IBindableList<TValue>> valueChange)
            => RegisterChange(list => { valueChange.Invoke(list, list); });

        IDisposable IBindable<IBindableList<TValue>>.RegisterChangeBuff(ValueChange<IBindableList<TValue>> valueChange)
            => RegisterChangeBuff(list => { valueChange.Invoke(list, list); });

        IDisposable RegisterChange(BindableListChange<TValue> bindableListChange);
        IDisposable RegisterChangeBuff(BindableListChange<TValue> bindableListChange);
        IDisposable RegisterAdd(AddValue<TValue> addValue);
        IDisposable RegisterRemove(RemoveValue<TValue> removeValue);
        IDisposable RegisterChange(ChangeValue<TValue> changeValue);
        IDisposable RegisterChange(ClearAllValue clearAllValue);
    }

    public delegate void BindableListChange<in TValue>(IBindableList<TValue> bindableList);

    public delegate void AddValue<in TValue>(TValue addValue, int index);

    public delegate void RemoveValue<in TValue>(TValue removeValue, int index);

    public delegate void ChangeValue<in TValue>(TValue oldValue, TValue newValue, int index);

    public delegate void ClearAllValue();
}