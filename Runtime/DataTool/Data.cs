#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DataTool
{
#if USE_SERIALIZABLE_VALUE && UNITY_EDITOR
    [Serializable]
#endif
    public abstract class Data<TValue, TId> : IData<TValue> where TValue : IModel<TId> where TId : unmanaged
    {
        protected Data()
        {
            Logging = new Logging(new[] { GetType().FullName });
        }

        protected Data(ILogging? loggingParent)
        {
            Logging = new Logging(new[] { GetType().FullName }, loggingParent);
        }

        [IgnoreParent] [SerializeField] private ShowIndexMap<TId, TValue> values;
        protected Logging Logging { get; set; }
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
                Logging.LogWarning($"Key为{id}值不合法!");
                return default;
            }

            if (values.TryGetValue(id, out var value)) return value;

            Logging.LogWarning($"无法获取到Key值为{id}对应的Value!");
            return default;
        }

        public bool TryGet(TId id, [MaybeNullWhen(false)] out TValue value)
        {
            if (!CheckId(id))
            {
                Logging.LogWarning($"Key为{id}值不合法!");
                value = default;
                return false;
            }

            return values.TryGetValue(id, out value);
        }

        public int GetIndex(TId id)
        {
            var index = values.GetIndex(id);
            if (index >= 0) return index;

            Logging.LogWarning($"无法获取到Key值为{id}对应的ValueIndex!");
            return -1;
        }

        public bool TryGetIndex(TId id, out int index)
        {
            index = values.GetIndex(id);
            if (index >= 0) return true;

            Logging.LogWarning($"无法获取到Key值为{id}对应的ValueIndex!");
            return false;
        }


        #region IData<TValue>

        public int GetIndex(TValue value) => GetIndex(value.Id);
        public bool TryGetIndex(TValue value, out int index) => TryGetIndex(value.Id, out index);

        public TValue? GetByIndex(int index, int idHashCode)
        {
            if (index < 0 || index >= values.Count)
            {
                Logging.LogWarning($"尝试获取的index:{index}超出范围!");
                return default;
            }

            var value = values.GetByIndex(index);
            if (value.Id.GetFixedHashCode() != idHashCode)
            {
                Logging.LogWarning($"尝试获取的index:{index}获取到的Id的hashCode和idHashCode不相同!");
                return default;
            }

            return value;
        }

        public bool TryGetByIndex(int index, int idHashCode, [MaybeNullWhen(false)] out TValue value)
        {
            if (index < 0 || index >= values.Count)
            {
                Logging.LogWarning($"尝试获取的index:{index}超出范围!");
                value = default;
                return false;
            }

            value = values.GetByIndex(index);
            if (value.Id.GetFixedHashCode() == idHashCode) return true;

            Logging.LogWarning($"尝试获取的index:{index}获取到的Id的hashCode和idHashCode不相同!");
            return false;
        }

        void IData<TValue>.RegisterValue(TValue value)
        {
            if (values.TryGetValue(value.Id, out var v))
            {
                if (value.Equals(v)) return;

                Logging.LogError($"存在相同key值{value.Id}的{typeof(TValue)}的值!");
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

                    Logging.LogError($"存在相同key值{value.Id}的{typeof(TValue)}的值!");
                    continue;
                }

                this.values.Add(value.Id, value);
            }
        }

        #endregion

        #region IData

        void IData.Clear()
        {
            Logging.Log($"清空数据类型为{typeof(TValue)}的{typeof(Data<TValue, TId>)}!");
            values.Clear();
        }

        #endregion
    }
}