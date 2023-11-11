using System.Collections.Generic;
using System.Linq;
using System.Threading;
using YuzeToolkit.IoCTool;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DataTool
{
    public abstract class DataManager : Container, IValueRegister
    {
        #region Static

        private static DataManager? instance;

        public static DataManager Instance => instance.IsNotNull();

        public static IData<TValue>? SGetValueData<TValue>() =>
            Instance.GetValueData<TValue>();

        public static bool STryGetValueData<TValue>(out IData<TValue> data) =>
            Instance.TryGetValueData(out data);

        public static TData? SGetData<TData>() where TData : IData =>
            Instance.GetData<TData>();

        public static bool STryGetData<TData>(out TData data) where TData : IData =>
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

        protected override void Awake()
        {
            base.Awake();
            if (instance != null)
                Log($"已经存在对应的{nameof(DataManager)}！", ELogType.Error);

            instance = this;
        }

        protected override void OnDestroy()
        {
            if (instance == this)
                instance = null;
            base.OnDestroy();
        }

#if USE_UNITASK
        public Cysharp.Threading.Tasks.UniTask InitData(CancellationToken token) => DoInitData(this, token);
        public Cysharp.Threading.Tasks.UniTask InitData() => InitData(destroyCancellationToken);
#else
        public System.Collections.IEnumerator InitData()
        {
            yield return StartCoroutine(DoInitData(this));
        }
#endif

#if USE_UNITASK
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
            if (value is IRegisterOtherValues registerOtherValues) registerOtherValues.DoRegister(this);
        }

        void IValueRegister.RegisterValues<TValue>(IEnumerable<TValue> values)
        {
            var readOnlyValues =
                values as IReadOnlyList<TValue> ?? values.ToArray();

            if (TryGet<IData<TValue>>(out var data)) data.RegisterValues(readOnlyValues);
            foreach (var registerOtherValues in readOnlyValues.OfType<IRegisterOtherValues>())
                registerOtherValues.DoRegister(this);
        }

        #endregion

        #region Public

        public TData? GetData<TData>() where TData : IData => Get<TData>();
        public bool TryGetData<TData>(out TData data) where TData : IData => TryGet(out data);
        public IData<TValue>? GetValueData<TValue>() => Get<IData<TValue>>();
        public bool TryGetValueData<TValue>(out IData<TValue> data) => TryGet(out data);

        public int GetIndex<TValue>(TValue value) => GetValueData<TValue>()?.GetIndex(value) ?? -1;
        public int GetIndex<TValue, TId>(TId id) => GetValueData<TValue>()?.GetIndex(id) ?? -1;

        public TValue? GetValue<TValue, TId>(TId id) => TryGetValueData<TValue>(out var data) ? data.Get(id) : default;

        public bool TryGetValue<TValue, TId>(TId id, out TValue value)
        {
            if (TryGetValueData<TValue>(out var data))
                return data.TryGet(id, out value);

            value = default!;
            return false;
        }

        public TValue? GetValueByIndex<TValue>(int index, int idHashCode) => TryGetValueData<TValue>(out var data)
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