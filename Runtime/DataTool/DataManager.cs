#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using YuzeToolkit.IoCTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DataTool
{
    public abstract class DataManager : Container, IValueRegister
    {
        #region Static

        private static DataManager? _s_instance;

        public static DataManager S_Instance
        {
            get
            {
                LogSys.IsNotNull(_s_instance != null, nameof(_s_instance));
                return _s_instance;
            }
        }

        public static TData? S_GetData<TData>() where TData : IData => S_Instance.Get<TData>();

        public static bool S_TryGetData<TData>([MaybeNullWhen(false)] out TData data) where TData : IData =>
            S_Instance.TryGet(out data);

        #region IData

        public static int S_GetIndex<TValue>(TValue value) => S_Instance.Get<IData<TValue>>()?.GetIndex(value) ?? -1;

        public static bool S_TryGetIndex<TValue>(TValue value, out int index)
        {
            index = -1;
            return S_Instance.Get<IData<TValue>>()?.TryGetIndex(value, out index) ?? false;
        }

        public static TValue? S_GetValueByIndex<TValue>(int index, int idHashCode) =>
            S_Instance.TryGet<IData<TValue>>(out var data)
                ? data.GetByIndex(index, idHashCode)
                : default!;

        public static bool S_TryGetValueByIndex<TValue>(int index, int idHashCode,
            [MaybeNullWhen(false)] out TValue value)
        {
            value = default;
            return S_Instance.Get<IData<TValue>>()?.TryGetByIndex(index, idHashCode, out value)?? false;
        }

        #endregion

        #region Data

        public static int S_GetIndex<TValue, TId>(TId id) where TValue : IModel<TId> where TId : unmanaged =>
            S_Instance.Get<Data<TValue, TId>>()?.GetIndex(id) ?? -1;

        public static bool S_TryGetIndex<TValue, TId>(TId id, out int index)
            where TValue : IModel<TId> where TId : unmanaged
        {
            index = -1;
            return S_Instance.Get<Data<TValue, TId>>()?.TryGetIndex(id, out index) ?? false;
        }

        public static Data<TValue, TId>? S_GetDataByValue<TValue, TId>()
            where TValue : IModel<TId> where TId : unmanaged =>
            S_Instance.Get<Data<TValue, TId>>();

        public static bool S_TryGetDataByValue<TValue, TId>([MaybeNullWhen(false)] out Data<TValue, TId> data)
            where TValue : IModel<TId> where TId : unmanaged => S_Instance.TryGet(out data);

        public static TValue? S_GetValue<TValue, TId>(TId id) where TValue : IModel<TId> where TId : unmanaged
        {
            var data = S_Instance.Get<Data<TValue, TId>>();
            return data == null ? default : data.Get(id);
        }

        public static bool S_TryGetValue<TValue, TId>(TId id, [MaybeNullWhen(false)] out TValue value)
            where TValue : IModel<TId> where TId : unmanaged
        {
            value = default;
            return S_Instance.Get<Data<TValue, TId>>()?.TryGet(id, out value) ?? false;
        }

        #endregion

        #region StringData

        public static int S_GetIndex<TValue>(string id) where TValue : IModel<string> =>
            S_Instance.Get<StringData<TValue>>()?.GetIndex(id) ?? -1;

        public static bool S_TryGetIndex<TValue>(string id, out int index) where TValue : IModel<string>
        {
            index = -1;
            return S_Instance.Get<StringData<TValue>>()?.TryGetIndex(id, out index) ?? false;
        }

        public static StringData<TValue>? S_GetDataByValue<TValue>() where TValue : IModel<string> =>
            S_Instance.Get<StringData<TValue>>();

        public static bool S_TryGetDataByValue<TValue>([MaybeNullWhen(false)] out StringData<TValue> data)
            where TValue : IModel<string> => S_Instance.TryGet(out data);

        public static TValue? S_GetValue<TValue>(string id) where TValue : IModel<string>
        {
            var data = S_Instance.Get<StringData<TValue>>();
            return data == null ? default : data.Get(id);
        }

        public static bool S_TryGetValue<TValue>(string id, [MaybeNullWhen(false)] out TValue value)
            where TValue : IModel<string>
        {
            value = default;
            return S_Instance.Get<StringData<TValue>>()?.TryGet(id, out value) ?? false;
        }

        #endregion

        #endregion

        protected override void Awake()
        {
            base.Awake();
            if (_s_instance != null)
                LogError($"已经存在对应的{nameof(DataManager)}！");

            _s_instance = this;
        }

        protected override void OnDestroy()
        {
            if (_s_instance == this)
                _s_instance = null;
            base.OnDestroy();
        }

#if YUZE_TOOLKIT_USE_UNITASK
        public Cysharp.Threading.Tasks.UniTask InitData(CancellationToken token) => DoInitData(this, token);
        public Cysharp.Threading.Tasks.UniTask InitData() => InitData(destroyCancellationToken);
#else
        public System.Collections.IEnumerator InitData()
        {
            yield return StartCoroutine(DoInitData(this));
        }
#endif

#if YUZE_TOOLKIT_USE_UNITASK
#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        protected virtual async Cysharp.Threading.Tasks.UniTask DoInitData(IValueRegister valueRegister,
            CancellationToken token)
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
        }
#else
        protected virtual System.Collections.IEnumerator DoInitData(IValueRegister valueRegister)
        {
            yield return null;
        }
#endif

        #region IValueRegister

        void IValueRegister.RegisterValue<TValue>(TValue value)
        {
            if (TryGet<IData<TValue>>(out var data)) data.RegisterValue(value);
        }

        void IValueRegister.RegisterValues<TValue>(IEnumerable<TValue> values)
        {
            if (TryGet<IData<TValue>>(out var data))
                data.RegisterValues(values as IReadOnlyList<TValue> ?? values.ToArray());
        }

        #endregion

        #region Public

        public TData? GetData<TData>() where TData : IData => Get<TData>();
        public bool TryGetData<TData>([MaybeNullWhen(false)] out TData data) where TData : IData => TryGet(out data);

        #region IData

        public int GetIndex<TValue>(TValue value) => Get<IData<TValue>>()?.GetIndex(value) ?? -1;

        public bool TryGetIndex<TValue>(TValue value, out int index)
        {
            index = -1;
            return Get<IData<TValue>>()?.TryGetIndex(value, out index) ?? false;
        }

        public TValue? GetValueByIndex<TValue>(int index, int idHashCode) => TryGet<IData<TValue>>(out var data)
            ? data.GetByIndex(index, idHashCode)
            : default;

        public bool TryGetValueByIndex<TValue>(int index, int idHashCode, [MaybeNullWhen(false)] out TValue value)
        {
            value = default;
            return Get<IData<TValue>>()?.TryGetByIndex(index, idHashCode, out value) ?? false;
        }

        #endregion

        #region Data

        public int GetIndex<TValue, TId>(TId id) where TValue : IModel<TId> where TId : unmanaged =>
            Get<Data<TValue, TId>>()?.GetIndex(id) ?? -1;

        public bool TryGetIndex<TValue, TId>(TId id, out int index) where TValue : IModel<TId> where TId : unmanaged
        {
            index = -1;
            return Get<Data<TValue, TId>>()?.TryGetIndex(id, out index) ?? false;
        }

        public Data<TValue, TId>? GetDataByValue<TValue, TId>() where TValue : IModel<TId> where TId : unmanaged =>
            Get<Data<TValue, TId>>();

        public bool TryGetDataByValue<TValue, TId>([MaybeNullWhen(false)] out Data<TValue, TId> data)
            where TValue : IModel<TId> where TId : unmanaged => TryGet(out data);

        public TValue? GetValue<TValue, TId>(TId id) where TValue : IModel<TId> where TId : unmanaged
        {
            var data = Get<Data<TValue, TId>>();
            return data == null ? default : data.Get(id);
        }

        public bool TryGetValue<TValue, TId>(TId id, [MaybeNullWhen(false)] out TValue value)
            where TValue : IModel<TId> where TId : unmanaged
        {
            value = default;
            return Get<Data<TValue, TId>>()?.TryGet(id, out value) ?? false;
        }

        #endregion

        #region StringData

        public int GetIndex<TValue>(string id) where TValue : IModel<string> =>
            Get<StringData<TValue>>()?.GetIndex(id) ?? -1;

        public bool TryGetIndex<TValue>(string id, out int index) where TValue : IModel<string>
        {
            index = -1;
            return Get<StringData<TValue>>()?.TryGetIndex(id, out index) ?? false;
        }

        public StringData<TValue>? GetDataByValue<TValue>() where TValue : IModel<string> =>
            Get<StringData<TValue>>();

        public bool TryGetDataByValue<TValue>([MaybeNullWhen(false)] out StringData<TValue> data)
            where TValue : IModel<string> => TryGet(out data);

        public TValue? GetValue<TValue>(string id) where TValue : IModel<string>
        {
            var data = Get<StringData<TValue>>();
            return data == null ? default : data.Get(id);
        }

        public bool TryGetValue<TValue>(string id, [MaybeNullWhen(false)] out TValue value)
            where TValue : IModel<string>
        {
            value = default;
            return Get<StringData<TValue>>()?.TryGet(id, out value) ?? false;
        }

        #endregion

        #endregion
    }
    
    public interface IValueRegister
    {
        void RegisterValue<TValue>(TValue value);
        void RegisterValues<TValue>(IEnumerable<TValue> values);
    }
}