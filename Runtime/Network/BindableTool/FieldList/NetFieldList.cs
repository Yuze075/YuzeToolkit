#nullable enable
#if YUZE_TOOLKIT_USE_UNITY_NETCODE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Netcode;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.BindableTool.Network
{
    [Serializable]
    public class NetFieldList<TValue> : NetworkList<TValue>, IFieldList<TValue>
        where TValue : unmanaged, IEquatable<TValue>
    {
        public NetFieldList(NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            ILogging? loggingParent = null) :
            this(default!, readPerm, writePerm)
        {
            Logging = new Logging(new[] { GetType().FullName }, loggingParent);
        }

        public NetFieldList(IEnumerable<TValue> enumerable,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server,
            ILogging? loggingParent = null) :
            base(enumerable, readPerm, writePerm)
        {
            OnListChanged += @event =>
            {
                switch (@event.Type)
                {
                    case NetworkListEvent<TValue>.EventType.Add:
                    case NetworkListEvent<TValue>.EventType.Insert:
                        _listChange?.Invoke(EventType.Add, @event.Value, @event.Index);
                        break;
                    case NetworkListEvent<TValue>.EventType.Remove:
                    case NetworkListEvent<TValue>.EventType.RemoveAt:
                        _listChange?.Invoke(EventType.Remove, @event.Value, @event.Index);
                        break;
                    case NetworkListEvent<TValue>.EventType.Value:
                        _listChange?.Invoke(EventType.Value, @event.Value, @event.Index, @event.PreviousValue);
                        break;
                    case NetworkListEvent<TValue>.EventType.Clear:
                        _listChange?.Invoke(EventType.Clear, default!, -1);
                        break;
                    case NetworkListEvent<TValue>.EventType.Full:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _fieldListChange?.Invoke(this);
            };
            Logging = new Logging(new[] { GetType().FullName }, loggingParent);
        }

        ~NetFieldList() => Dispose(false);
        [NonSerialized] private bool _disposed;
        protected Logging Logging { get; set; }
        protected virtual string[] GetLogTags => new[] { GetType().Name };
        object IBindable.Value => this;
        public void CopyTo(TValue[] array, int arrayIndex) => throw new NotImplementedException();
        public bool IsReadOnly => true;

        #region RegisterChange

        private FieldListChange<TValue>? _fieldListChange;
        private ListChange<TValue>? _listChange;

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindable.RegisterChange(ValueChange<object>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            return RegisterChange(fieldList => { valueChange(fieldList, fieldList); });
        }

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindable.RegisterChangeBuff(ValueChange<object>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            valueChange(null, this);
            return RegisterChange(fieldList => { valueChange(null, fieldList); });
        }

        [return: NotNullIfNotNull("valueChange")]
        public IDisposable? RegisterChange(FieldListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            _fieldListChange += valueChange;
            return UnRegister.Create(action => _fieldListChange -= action, valueChange);
        }

        [return: NotNullIfNotNull("valueChange")]
        public IDisposable? RegisterChangeBuff(FieldListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            valueChange(this);
            _fieldListChange += valueChange;
            return UnRegister.Create(action => _fieldListChange -= action, valueChange);
        }

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindableList<TValue>.RegisterChange(BindableListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return UnRegister.Create(action => _fieldListChange -= action, fieldListChange);
        }

        [return: NotNullIfNotNull("valueChange")]
        IDisposable? IBindableList<TValue>.RegisterChangeBuff(BindableListChange<TValue>? valueChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (valueChange == null) return null;
            valueChange(this);
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return UnRegister.Create(action => _fieldListChange -= action, fieldListChange);
        }

        [return: NotNullIfNotNull("listChange")]
        public IDisposable? RegisterListChange(ListChange<TValue>? listChange)
        {
            if (_disposed) throw new ObjectDisposedException($"{GetType().Name}已经被释放！");
            if (listChange == null) return null;
            _listChange += listChange;
            return UnRegister.Create(action => _listChange -= action, listChange);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            Clear();
            _fieldListChange = null;
            _listChange = null;

            _disposed = true;
        }

        #endregion

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
#endif