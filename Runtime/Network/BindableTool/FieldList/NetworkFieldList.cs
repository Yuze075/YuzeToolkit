#if USE_UNITY_NETCODE
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using YuzeToolkit.BindableTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.Network.BindableTool
{
    [Serializable]
    public class NetworkFieldList<TValue> : NetworkList<TValue>, IFieldList<TValue>
        where TValue : unmanaged, IEquatable<TValue>
    {
        public NetworkFieldList(ILogTool parent = null!,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            this(default!, parent, readPerm, writePerm)
        {
        }

        public NetworkFieldList(IEnumerable<TValue> enumerable, ILogTool parent = null!,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(enumerable, readPerm, writePerm)
        {
            LogTool.Parent = parent;
            OnListChanged += @event =>
            {
                switch (@event.Type)
                {
                    case NetworkListEvent<TValue>.EventType.Add:
                    case NetworkListEvent<TValue>.EventType.Insert:
                        _addValue?.Invoke(@event.Value, @event.Index);
                        break;
                    case NetworkListEvent<TValue>.EventType.Remove:
                    case NetworkListEvent<TValue>.EventType.RemoveAt:
                        _removeValue?.Invoke(@event.Value, @event.Index);
                        break;
                    case NetworkListEvent<TValue>.EventType.Value:
                        _changeValue?.Invoke(@event.PreviousValue, @event.Value, @event.Index);
                        break;
                    case NetworkListEvent<TValue>.EventType.Clear:
                        _clearAllValue?.Invoke();
                        break;
                    case NetworkListEvent<TValue>.EventType.Full:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _fieldListChange?.Invoke(this);
            };
        }

        private SLogTool? _sLogTool;

        private SLogTool LogTool => _sLogTool ??= new SLogTool(new[]
        {
            nameof(IBindable),
            GetType().FullName
        });

        void IBindable.SetLogParent(ILogTool value) => LogTool.Parent = value;

        public void CopyTo(TValue[] array, int arrayIndex) => throw new NotImplementedException();
        public bool IsReadOnly => true;

        #region RegisterChange

        private FieldListChange<TValue>? _fieldListChange;
        private AddValue<TValue>? _addValue;
        private RemoveValue<TValue>? _removeValue;
        private ChangeValue<TValue>? _changeValue;
        private ClearAllValue? _clearAllValue;

        public IDisposable RegisterChange(FieldListChange<TValue> valueChange)
        {
            _fieldListChange += valueChange;
            return _disposeGroup.UnRegister(() => { _fieldListChange -= valueChange; });
        }

        public IDisposable RegisterChangeBuff(FieldListChange<TValue> valueChange)
        {
            valueChange.Invoke(this);
            return RegisterChange(valueChange);
        }

        IDisposable IBindableList<TValue>.RegisterChange(BindableListChange<TValue> valueChange)
        {
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return _disposeGroup.UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        IDisposable IBindableList<TValue>.RegisterChangeBuff(BindableListChange<TValue> valueChange)
        {
            valueChange.Invoke(this);
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return _disposeGroup.UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        public IDisposable RegisterAdd(AddValue<TValue> addValue)
        {
            _addValue += addValue;
            return _disposeGroup.UnRegister(() => { _addValue -= addValue; });
        }

        public IDisposable RegisterRemove(RemoveValue<TValue> removeValue)
        {
            _removeValue += removeValue;
            return _disposeGroup.UnRegister(() => { _removeValue -= removeValue; });
        }

        public IDisposable RegisterChange(ChangeValue<TValue> changeValue)
        {
            _changeValue += changeValue;
            return _disposeGroup.UnRegister(() => { _changeValue -= changeValue; });
        }

        public IDisposable RegisterChange(ClearAllValue clearAllValue)
        {
            _clearAllValue += clearAllValue;
            return _disposeGroup.UnRegister(() => { _clearAllValue -= clearAllValue; });
        }


        #endregion

        #region IDisposable

        private DisposeGroup _disposeGroup;

        void IDisposable.Dispose()
        {
            Clear();
            _disposeGroup.Dispose();
        }

        #endregion
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
#endif