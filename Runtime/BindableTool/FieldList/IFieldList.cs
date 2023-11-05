using System;
using System.Collections.Generic;

namespace YuzeToolkit.BindableTool
{
    public interface IFieldList<TValue> : IBindableList<TValue>, IList<TValue> , IBindable<IFieldList<TValue>>
    {
        object IBindable.Value => this;

        IDisposable IBindable.RegisterChange(ValueChange<object> valueChange) =>
            RegisterChange(list => { valueChange.Invoke(list, list); });

        IDisposable IBindable.RegisterChangeBuff(ValueChange<object> valueChange) =>
            RegisterChange(list => { valueChange.Invoke(list, list); });
        
        IFieldList<TValue> IBindable<IFieldList<TValue>>.Value => this;

        IDisposable IBindable<IFieldList<TValue>>.RegisterChange(ValueChange<IFieldList<TValue>> valueChange)
            => RegisterChange(list => { valueChange.Invoke(list, list); });

        IDisposable IBindable<IFieldList<TValue>>.RegisterChangeBuff(ValueChange<IFieldList<TValue>> valueChange)
            => RegisterChangeBuff(list => { valueChange.Invoke(list, list); });
        
        new TValue this[int index] { get; set; }
        new int Count { get; }

        IDisposable RegisterChange(FieldListChange<TValue> fieldListChange);
        IDisposable RegisterChangeBuff(FieldListChange<TValue> fieldListChange);
    }

    public delegate void FieldListChange<TValue>(IFieldList<TValue> fieldList);
}