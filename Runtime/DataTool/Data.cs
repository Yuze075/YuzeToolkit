using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DataTool
{
    [Serializable]
    public abstract class Data<TValue, TId> : IData<TValue> where TValue : IModel<TId> where TId : unmanaged
    {
        [IgnoreParent] [SerializeField] private ShowDictionaryIndex<TId, TValue> values = new();

        private SLogTool? _sLogTool;
        protected ILogTool LogTool => _sLogTool ??= SLogTool.Create(GetLogTags);
        protected virtual string[] GetLogTags => new[]
        {
            typeof(Data<TValue, TId>).FullName,
            GetType().FullName
        };

        public IReadOnlyList<TValue> Values => values.Values;

        /// <summary>
        /// 检测Id是否合法(不合法返回False， 合法返回Ture
        /// </summary>
        protected abstract bool CheckId(TId id);

        public TValue? this[TId id] => Get(id);

        public TValue? Get(TId id)
        {
            if (!CheckId(id))
            {
                LogTool.Log($"Key为{id}值不合法!", ELogType.Warning);
                return default;
            }

            if (!values.TryGetValue(id, out var value))
            {
                LogTool.Log($"无法获取到Key值为{id}对应的Value!", ELogType.Warning);
                return default;
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            return value is ICloneSelf<TValue> cloneSelf
                ? cloneSelf.GetClone()
                : value;
        }

        public bool TryGet(TId id, out TValue value)
        {
            if (!CheckId(id))
            {
                LogTool.Log($"Key为{id}值不合法!", ELogType.Warning);
                value = default!;
                return false;
            }

            if (!values.TryGetValue(id, out value))
            {
                LogTool.Log($"无法获取到Key值为{id}对应的Value!", ELogType.Warning);
                return false;
            }

            return true;
        }

        public int GetIndex(TId id)
        {
            var index = values.GetIndex(id);
            if (index >= 0) return index;

            LogTool.Log($"无法获取到Key值为{id}对应的ValueIndex!", ELogType.Warning);
            return -1;
        }


        #region IData<TValue>

        TValue? IData<TValue>.Get<TGetId>(TGetId getId)
        {
            if (getId is not TId id)
            {
                LogTool.Log($"传入的Id类型为{typeof(TGetId)}, 不是需要的{typeof(TId)}类型, 类型错误!", ELogType.Warning);
                return default;
            }

            return Get(id);
        }

        bool IData<TValue>.TryGet<TGetId>(TGetId getId, out TValue value)
        {
            if (getId is not TId id)
            {
                LogTool.Log($"传入的Id类型为{typeof(TGetId)}, 不是需要的{typeof(TId)}类型, 类型错误!", ELogType.Warning);
                value = default!;
                return false;
            }

            return TryGet(id, out value);
        }

        public int GetIndex(TValue value)
        {
            if (value is not IModel<TId> model)
            {
                LogTool.Log($"传入的Value类型为{typeof(TValue)}, 不能转化为{typeof(IModel<TId>)}类型, 类型错误!", ELogType.Warning);
                return -1;
            }

            return GetIndex(model.Id);
        }

        int IData<TValue>.GetIndex<TGetId>(TGetId getId)
        {
            if (getId is not TId id)
            {
                LogTool.Log($"传入的Id类型为{typeof(TGetId)}, 不是需要的{typeof(TId)}类型, 类型错误!", ELogType.Warning);
                return -1;
            }

            return GetIndex(id);
        }

        public TValue? GetByIndex(int index, int idHashCode)
        {
            if (index < 0 || index >= values.Count)
            {
                LogTool.Log($"尝试获取的index:{index}超出范围!", ELogType.Warning);
                return default;
            }

            var value = values.GetByIndex(index);
            if (value.Id.GetFixedHashCode() != idHashCode)
            {
                LogTool.Log($"尝试获取的index:{index}获取到的Id的hashCode和idHashCode不相同!", ELogType.Warning);
                return default;
            }

            return value;
        }

        public bool TryGetByIndex(int index, int idHashCode, out TValue value)
        {
            if (index < 0 || index >= values.Count)
            {
                LogTool.Log($"尝试获取的index:{index}超出范围!", ELogType.Warning);
                value = default!;
                return false;
            }

            value = values.GetByIndex(index);

            if (value.Id.GetFixedHashCode() != idHashCode)
            {
                LogTool.Log($"尝试获取的index:{index}获取到的Id的hashCode和idHashCode不相同!", ELogType.Warning);
                return false;
            }

            return true;
        }

        void IData<TValue>.RegisterValue(TValue value)
        {
            if (values.TryGetValue(value.Id, out var v))
            {
                if (value.Equals(v)) return;

                LogTool.Log($"存在相同key值{value.Id}的{typeof(TValue)}的值!", ELogType.Error);
                return;
            }

            values.Add(value.Id, value);
        }

        void IData<TValue>.RegisterValues(IEnumerable<TValue> values)
        {
            foreach (var value in values)
            {
                if (this.values.TryGetValue(value.Id, out var v))
                {
                    if (value.Equals(v)) continue;

                    LogTool.Log($"存在相同key值{value.Id}的{typeof(TValue)}的值!", ELogType.Error);
                    continue;
                }

                this.values.Add(value.Id, value);
            }
        }

        #endregion

        #region IData

        void IData.Clear()
        {
            LogTool.Log($"清空数据类型为{typeof(TValue)}的{typeof(Data<TValue, TId>)}!");
            values.Clear();
        }

        ILogTool IData.Parent
        {
            set => ((SLogTool)LogTool).Parent = value;
        }

        #endregion

        void IDisposable.Dispose()
        {
            ((IData)this).Clear();
            SLogTool.Release(ref _sLogTool);
        }
    }
}