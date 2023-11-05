using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using YuzeToolkit.DriverTool;
using YuzeToolkit.InspectorTool;
using YuzeToolkit.IoCTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DataTool
{
    public abstract class DataManager : MonoBase, IValueRegister
    {
        #region Static

        private static DataManager? instance;

        public static DataManager Instance => LogSys.IsNotNull(instance);

        public static IData<TValue>? SGetValueData<TValue>() =>
            Instance.GetValueData<TValue>();

        public static bool STryGetValueData<TValue>(out IData<TValue> data) =>
            Instance.TryGetValueData(out data);

        public static TData? SGetData<TData>() =>
            Instance.GetData<TData>();

        public static bool STryGetData<TData>(out TData data) =>
            Instance.TryGetData(out data);

        public static int SGetIndex<TValue>(TValue value) => Instance.GetIndex(value);

        public static TValue? SGetValue<TValue, TId>(TId id) =>
            Instance.GetValue<TValue, TId>(id);

        public static bool STryGetValue<TValue, TId>(TId id, out TValue value) =>
            Instance.TryGetValue(id, out value);

        public static TValue? SGetValueByIndex<TValue>(int index, int idHashCode) =>
            Instance.GetValueByIndex<TValue>(index, idHashCode);

        public static bool STryGetValueByIndex<TValue>(int index, int idHashCode,
            out TValue value) => Instance.TryGetValueByIndex(index, idHashCode, out value);

        #endregion
        
        [IgnoreParent] [SerializeField] private ShowDictionary<Type, IData> datas = new();
        private Action<IContainerBuilder>? _addBuilder;

        private void Awake()
        {
            instance = this;
        }

        protected override void OnDestroy()
        {
            instance = null;
            base.OnDestroy();
        }

        public void Configure(IContainerBuilder builder) => _addBuilder?.Invoke(builder);

#if USE_UNITASK
        public async Cysharp.Threading.Tasks.UniTask Init()
        {
            DoRegisterDatas();
            await DoAddValues();
        }
#else
        public System.Collections.IEnumerator Init()
        {
            DoRegisterDatas();
            yield return StartCoroutine(DoAddValues());
        }
#endif

        protected abstract void DoRegisterDatas();

#if USE_UNITASK
        protected abstract Cysharp.Threading.Tasks.UniTask DoAddValues();
#else
        protected abstract System.Collections.IEnumerator DoAddValues();
#endif

        #region IValueRegister

        void IValueRegister.AddValue<TValue>(TValue value) => AddValue(value);

        void IValueRegister.AddValue<TValue>(IEnumerable<TValue> values) =>
            AddValue(values);

        #endregion

        #region Protected

        protected void RegisterData<TData, TValue>(TData data)
            where TData : IData<TValue>
        {
            data.Parent = this;
            datas.Add(typeof(TData), data);
            datas.Add(typeof(IData<TValue>), data);
            _addBuilder += builder => builder.Register(data).As<IData<TValue>>();
        }

        protected void RegisterData<TData, TValue, TId>(TData data)
            where TData : Data<TValue, TId> where TValue : IModel<TId>
        {
            ((IData)data).Parent = this;
            datas.Add(typeof(TData), data);
            datas.Add(typeof(IData<TValue>), data);
            datas.Add(typeof(Data<TValue, TId>), data);
            _addBuilder += builder => builder.Register(data).As<Data<TValue, TId>, IData<TValue>>();
        }

        protected void RegisterData<TData, TValue>()
            where TData : IData<TValue>, new()
        {
            var data = new TData { Parent = this };
            datas.Add(typeof(TData), data);
            datas.Add(typeof(IData<TValue>), data);
            _addBuilder += builder => builder.Register(data).As<IData<TValue>>();
        }

        protected void RegisterData<TData, TValue, TId>()
            where TData : Data<TValue, TId>, new() where TValue : IModel<TId>
        {
            var data = new TData();
            ((IData)data).Parent = this;
            datas.Add(typeof(TData), data);
            datas.Add(typeof(IData<TValue>), data);
            datas.Add(typeof(Data<TValue, TId>), data);
            _addBuilder += builder => builder.Register(data).As<Data<TValue, TId>, IData<TValue>>();
        }

        protected void AddValue<TValue>(TValue value)
        {
            if (datas.TryGetValue(typeof(IData<TValue>), out var data))
            {
                var tData = (IData<TValue>)data;
                tData.AddData(value);
            }

            if (value is IRegisterOtherValues registerOtherValues)
            {
                registerOtherValues.DoRegister(this);
            }
        }

        protected void AddValue<TValue>(IEnumerable<TValue> values)
        {
            var readOnlyList =
                values as IReadOnlyList<TValue> ?? values.ToArray();

            if (datas.TryGetValue(typeof(IData<TValue>), out var data))
            {
                var tData = (IData<TValue>)data;
                tData.AddData(readOnlyList);
            }

            foreach (var registerOtherValues in readOnlyList.OfType<IRegisterOtherValues>())
            {
                registerOtherValues.DoRegister(this);
            }
        }

        protected void AddValue<TBase, TValue>(IEnumerable<TValue> values) where TValue : TBase
        {
            var readOnlyList = values.OfType<TBase>().ToArray();

            if (datas.TryGetValue(typeof(IData<TBase>), out var data))
            {
                var tData = (IData<TBase>)data;
                tData.AddData(readOnlyList);
            }

            foreach (var registerOtherValues in readOnlyList.OfType<IRegisterOtherValues>())
            {
                registerOtherValues.DoRegister(this);
            }
        }

        #endregion

        #region Public

        public TData? GetData<TData>()
        {
            if (!datas.TryGetValue(typeof(TData), out var data))
            {
                Log($"无法找到对应Value{typeof(TData)}的Data!", ELogType.Warning);
                return default;
            }

            return (TData)data;
        }

        public bool TryGetData<TData>(out TData data)
        {
            if (!datas.TryGetValue(typeof(TData), out var data1))
            {
                Log($"无法找到对应Value{typeof(TData)}的Data!", ELogType.Warning);
                data = default!;
                return false;
            }

            data = (TData)data1;
            return true;
        }

        public IData<TValue>? GetValueData<TValue>()
        {
            if (!datas.TryGetValue(typeof(IData<TValue>), out var data))
            {
                Log($"无法找到对应Value{typeof(IData<TValue>)}的Data!", ELogType.Warning);
                return default;
            }

            return (IData<TValue>)data;
        }

        public bool TryGetValueData<TValue>(out IData<TValue> data)
        {
            if (!datas.TryGetValue(typeof(IData<TValue>), out var data1))
            {
                Log($"无法找到对应Value{typeof(IData<TValue>)}的Data!", ELogType.Warning);
                data = default!;
                return false;
            }

            data = (IData<TValue>)data1;
            return true;
        }

        public int GetIndex<TValue>(TValue value) =>
            TryGetValueData<TValue>(out var data) ? data.GetIndex(value) : -1;

        public TValue? GetValue<TValue, TId>(TId id) =>
            TryGetValueData<TValue>(out var data) ? data.Get(id) : default!;

        public bool TryGetValue<TValue, TId>(TId id, out TValue value)
        {
            if (TryGetValueData<TValue>(out var data))
                return data.TryGet(id, out value);
            value = default!;
            return false;
        }

        public TValue? GetValueByIndex<TValue>(int index, int idHashCode) =>
            TryGetValueData<TValue>(out var data)
                ? data.GetByIndex(index, idHashCode)
                : default!;

        public bool TryGetValueByIndex<TValue>(int index, int idHashCode, out TValue value)
        {
            if (TryGetValueData<TValue>(out var data))
                return data.TryGetByIndex(index, idHashCode, out value);
            value = default!;
            return false;
        }

        #endregion
    }
}