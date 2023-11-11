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
        public NetworkFieldList(
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            this(default!, readPerm, writePerm)
        {
        }

        public NetworkFieldList(IEnumerable<TValue> enumerable,
            NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server) :
            base(enumerable, readPerm, writePerm)
        {
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
        protected ILogTool LogTool => _sLogTool ??= SLogTool.Create(GetLogTags);
        protected virtual string[] GetLogTags => new[]
        {
            nameof(IBindable),
            GetType().FullName
        };

        void IBindable.SetLogParent(ILogTool parent) => ((SLogTool)LogTool).Parent = parent;

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
            return  new UnRegister(() => { _fieldListChange -= valueChange; });
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
            return new UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        IDisposable IBindableList<TValue>.RegisterChangeBuff(BindableListChange<TValue> valueChange)
        {
            valueChange.Invoke(this);
            var fieldListChange = new FieldListChange<TValue>(valueChange);
            _fieldListChange += fieldListChange;
            return  new UnRegister(() => { _fieldListChange -= fieldListChange; });
        }

        public IDisposable RegisterAdd(AddValue<TValue> addValue)
        {
            _addValue += addValue;
            return  new UnRegister(() => { _addValue -= addValue; });
        }

        public IDisposable RegisterRemove(RemoveValue<TValue> removeValue)
        {
            _removeValue += removeValue;
            return  new UnRegister(() => { _removeValue -= removeValue; });
        }

        public IDisposable RegisterChange(ChangeValue<TValue> changeValue)
        {
            _changeValue += changeValue;
            return  new UnRegister(() => { _changeValue -= changeValue; });
        }

        public IDisposable RegisterChange(ClearAllValue clearAllValue)
        {
            _clearAllValue += clearAllValue;
            return new UnRegister(() => { _clearAllValue -= clearAllValue; });
        }


        #endregion

        #region IDisposable


        void IDisposable.Dispose()
        {
            SLogTool.Release(ref _sLogTool);
            Clear();
            _fieldListChange = null;
            _addValue = null;
            _removeValue = null;
            _changeValue = null;
            _clearAllValue = null;
        }

        #endregion
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
#endif