#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using YuzeToolkit.IoCTool;

namespace YuzeToolkit.DataTool
{
    public abstract class DataManager : Container, IValueRegister
    {
        #region Static

        private static DataManager? _s_instance;

        /// <summary>
        /// <see cref="DataManager"/>需要在一个单独的场景(StartUp)中完成单例的加载<br/>
        /// 此单例在<see cref="Awake"/>的时候绑定单例, 在<see cref="OnDestroy"/>解除单例的绑定<br/>
        /// 在<see cref="DataManager"/>单例存在时创建其他<see cref="DataManager"/>, 会销毁新创建的<see cref="DataManager"/>的<see cref="DataManager.gameObject"/><br/>
        /// 在单独场景(StartUp)完成加载之后, 会在整个游戏的生命周期中一直存在
        /// </summary>
        public static DataManager S_Instance
        {
            get
            {
                if (_s_instance == null) throw new NullReferenceException($"当前的{nameof(DataManager)}实例不存在！");
                return _s_instance;
            }
        }

        #endregion

        protected override void Awake()
        {
            base.Awake();
            if (_s_instance != null)
            {
                Destroy(gameObject);
                throw new InvalidOperationException($"当前的{nameof(DataManager)}实例已经存在, 不应该再次进行创建！");
            }

            _s_instance = this;
        }

        protected override void OnDestroy()
        {
            if (_s_instance == this)
                _s_instance = null;
            base.OnDestroy();
        }

#if YUZE_USE_UNITASK
        public Cysharp.Threading.Tasks.UniTask InitData(CancellationToken token) => DoInitData(this, token);
        public Cysharp.Threading.Tasks.UniTask InitData() => InitData(destroyCancellationToken);
#else
        public System.Collections.IEnumerator InitData()
        {
            yield return StartCoroutine(DoInitData(this));
        }
#endif

#if YUZE_USE_UNITASK
        protected virtual Cysharp.Threading.Tasks.UniTask DoInitData(IValueRegister valueRegister,
            CancellationToken token) => default;
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

        public bool TryGetIndex<TValue>(TValue value, out int index)
        {
            if (Get<IData<TValue>>() is { } data)
                return data.TryGetHashInData(value, out index);

            index = -1;
            return false;
        }

        public TValue? GetValueByIndex<TValue>(int index, int idHashCode) => TryGet<IData<TValue>>(out var data)
            ? data.GetValueByHash(index, idHashCode)
            : default;

        public bool TryGetValueByIndex<TValue>(int index, int idHashCode, [MaybeNullWhen(false)] out TValue value)
        {
            if (Get<IData<TValue>>() is { } data)
                return data.TryGetValueByHash(index, idHashCode, out value);

            value = default;
            return false;
        }

        public bool TryGetIndex<TValue, TId>(TId id, out int index) where TValue : IModel<TId>
        {
            if (Get<Data<TValue, TId>>() is { } data)
                return data.TryGetHashInData(id, out index);

            index = -1;
            return false;
        }

        public TValue? GetValue<TValue, TId>(TId id) where TValue : IModel<TId> =>
            Get<Data<TValue, TId>>() is { } data ? data.Get(id) : default;

        public bool TryGetValue<TValue, TId>(TId id, [MaybeNullWhen(false)] out TValue value)
            where TValue : IModel<TId> where TId : IEquatable<TId>
        {
            if (Get<Data<TValue, TId>>() is { } data)
                return data.TryGet(id, out value);

            value = default;
            return false;
        }

        public Data<TValue, TId>? GetData<TValue, TId>() where TValue : IModel<TId> =>
            Get<Data<TValue, TId>>();

        public bool TryGetData<TValue, TId>([MaybeNullWhen(false)] out Data<TValue, TId> data)
            where TValue : IModel<TId> => TryGet(out data);

        #endregion
    }

    public interface IValueRegister
    {
        void RegisterValue<TValue>(TValue value);
        void RegisterValues<TValue>(IEnumerable<TValue> values);
    }
}