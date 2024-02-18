#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace YuzeToolkit.DataTool
{
    public abstract class Data<TValue, TId> : IData<TValue>
        where TValue : IModel<TId>
    {
        private readonly Dictionary<int, int> _valuesIndex = new();
#if UNITY_EDITOR && YUZE_INSPECTOR_TOOL_USE_SHOW_VALUE
#if YUZE_USE_EDITOR_TOOLBOX
        [IgnoreParent]
#endif
        [SerializeField]
        InspectorTool.ShowList<TValue>
#else
        readonly System.Collections.Generic.List<TValue>
#endif
            _values = new();

        public IReadOnlyList<TValue> Values => (List<TValue>)_values;

        /// <summary>
        /// 检测Id是否合法(不合法返回False， 合法返回Ture
        /// </summary>
        protected abstract bool CheckId(TId id);

        protected abstract int IdHashCode(TId id);
        protected abstract bool EqualId(TId id1, TId id2);

        public TValue? this[TId id] => Get(id);

        public TValue? Get(TId id)
        {
            if (!CheckId(id))
                throw new ArgumentOutOfRangeException($"[{this}]: Key为{id}, 不合法!");

            var idHashCode = IdHashCode(id);
            var i = 0;
            while (_valuesIndex.TryGetValue(idHashCode, out var index))
            {
                var indexValue = _values[index];
                if (EqualId(id, indexValue.Id)) return indexValue;

                idHashCode += Pow(2, i++);
            }

            return default;
        }

        public bool TryGet(TId id, [MaybeNullWhen(false)] out TValue value)
        {
            if (!CheckId(id))
                throw new ArgumentOutOfRangeException($"[{this}]: Key为{id}, 不合法!");

            var idHashCode = IdHashCode(id);
            var i = 0;
            while (_valuesIndex.TryGetValue(idHashCode, out var index))
            {
                var indexValue = _values[index];
                if (EqualId(id, indexValue.Id))
                {
                    value = indexValue;
                    return true;
                }

                idHashCode += Pow(2, i++);
            }

            value = default;
            return false;
        }

        public bool TryGetHashInData(TId id, out int hashInData)
        {
            if (!CheckId(id))
                throw new ArgumentOutOfRangeException($"[{this}]: Key为{id}, 不合法!");

            var idHashCode = IdHashCode(id);
            var i = 0;
            while (_valuesIndex.TryGetValue(idHashCode, out var index))
            {
                if (EqualId(id, _values[index].Id))
                {
                    hashInData = idHashCode;
                    return true;
                }

                idHashCode += Pow(2, i++);
            }

            hashInData = default;
            return false;
        }

        #region IData<TValue>

        public bool TryGetHashInData(TValue value, out int hashInData) => TryGetHashInData(value.Id, out hashInData);

        public TValue? GetValueByHash(int hashInData, int idHashCode)
        {
            if (hashInData < 0 || hashInData >= _values.Count)
                throw new ArgumentOutOfRangeException($"[{this}]: 尝试获取的index:{hashInData}超出范围!");

            if (_valuesIndex.TryGetValue(hashInData, out var index))
            {
                var indexValue = _values[index];
                if (IdHashCode(indexValue.Id) == idHashCode) return indexValue;
            }

            var i = 0;
            while (_valuesIndex.TryGetValue(idHashCode, out index))
            {
                var indexValue = _values[index];
                if (IdHashCode(indexValue.Id) == idHashCode) return indexValue;
                idHashCode += Pow(2, i++);
            }

            return default;
        }

        public bool TryGetValueByHash(int hashInData, int idHashCode, [MaybeNullWhen(false)] out TValue value)
        {
            if (hashInData < 0 || hashInData >= _values.Count)
                throw new ArgumentOutOfRangeException($"[{this}]: 尝试获取的index:{hashInData}超出范围!");

            if (_valuesIndex.TryGetValue(hashInData, out var index))
            {
                var indexValue = _values[index];
                if (IdHashCode(indexValue.Id) == idHashCode)
                {
                    value = indexValue;
                    return true;
                }
            }

            var i = 0;
            while (_valuesIndex.TryGetValue(idHashCode, out index))
            {
                var indexValue = _values[index];
                if (IdHashCode(indexValue.Id) == idHashCode)
                {
                    value = indexValue;
                    return true;
                }

                idHashCode += Pow(2, i++);
            }

            value = default;
            return false;
        }

        void IData<TValue>.RegisterValue(TValue value)
        {
            var id = value.Id;
            if (!CheckId(id))
                throw new ArgumentOutOfRangeException($"[{this}]: Key为{id}, 不合法!");

            var idHashCode = IdHashCode(id);
            var i = 0;
            while (_valuesIndex.TryGetValue(idHashCode, out var index))
            {
                var indexValue = _values[index];
                var indexId = indexValue.Id;
                if (EqualId(id, indexId) && !(ReferenceEquals(value, indexValue) || value.Equals(indexValue)))
                    throw new ArgumentException(
                        $"[{this}]: 存在相同key值{value.Id}的{typeof(TValue)}的值!(原值:{indexValue}, 新值:{value})");

                idHashCode += Pow(2, i++);
            }

            _valuesIndex.Add(idHashCode, _values.Count);
            _values.Add(value);
        }


        void IData<TValue>.RegisterValues(IEnumerable<TValue> values)
        {
            foreach (var value in values) ((IData<TValue>)this).RegisterValue(value);
        }

        #endregion

        void IData.Clear()
        {
            _valuesIndex.Clear();
            _values.Clear();
        }

        private static int Pow(int x, int p)
        {
            var sum = 1;
            for (var i = 0; i < p; i++) sum *= x;
            return sum;
        }
    }

    public abstract class StringData<TValue> : Data<TValue, string> where TValue : IModel<string>
    {
        protected override bool CheckId(string id) => !string.IsNullOrWhiteSpace(id);
        protected sealed override int IdHashCode(string id) => DataHashCode.Get(id);
        protected sealed override bool EqualId(string id1, string id2) => id1 == id2;
    }
}