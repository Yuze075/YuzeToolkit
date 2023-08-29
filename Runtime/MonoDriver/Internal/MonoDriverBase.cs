using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 更新驱动器的基类, 用于派生不同unity默认生命周期更新的子类
    /// </summary>
    internal abstract class MonoDriverBase : MonoBehaviour
    {
        #region Static

        private const int WrapperListsDefaultCapacity = 64;
        private const int WrapperListDefaultCapacity = 1;

        internal static float DeltaTime;
        internal static float FixedDeltaTime;

        #endregion

        /// <summary>
        /// 内部添加到对应具体更新节点的方法
        /// </summary>
        /// <param name="monoBase"></param>
        internal void AddMonoBase(IMonoBase monoBase)
        {
            if (monoBase is IMonoOnUpdate monoOnUpdate)
            {
                _updateToAdd.Add(monoOnUpdate);
            }

            if (monoBase is IMonoOnFixedUpdate monoOnFixedUpdate)
            {
                _fixedUpdateToAdd.Add(monoOnFixedUpdate);
            }

            if (monoBase is IMonoOnLateUpdate monoOnLateUpdate)
            {
                _lateUpdateToAdd.Add(monoOnLateUpdate);
            }
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        internal void Initialize()
        {
            _updateToAdd = new List<IMonoOnUpdate>();
            _updateWrapperLists = new List<MonoBaseWrapperList<IMonoOnUpdate>>(WrapperListsDefaultCapacity);
            _updateWrapperListToRemove = new Stack<int>();

            _fixedUpdateToAdd = new List<IMonoOnFixedUpdate>();
            _fixedUpdateWrapperLists = new List<MonoBaseWrapperList<IMonoOnFixedUpdate>>(WrapperListsDefaultCapacity);
            _fixedUpdateWrapperListToRemove = new Stack<int>();

            _lateUpdateToAdd = new List<IMonoOnLateUpdate>();
            _lateUpdateWrapperLists = new List<MonoBaseWrapperList<IMonoOnLateUpdate>>();
            _lateUpdateWrapperListToRemove = new Stack<int>();
        }

        #region Update

        // 初始化参数
        private List<IMonoOnUpdate> _updateToAdd;
        private List<MonoBaseWrapperList<IMonoOnUpdate>> _updateWrapperLists;
        private Stack<int> _updateWrapperListToRemove;
        private MonoBaseWrapperList<IMonoOnUpdate>.Comparer _updateComparer;

        // 缓存参数
        private List<MonoBaseWrapperList<IMonoOnUpdate>.MonoBaseWrapper> _updateWrappersCache;

        private void Update()
        {
            var addCount = _updateToAdd.Count;
            for (var index = 0; index < addCount; index++)
            {
                AddToList(_updateToAdd[index]);
            }

            _updateToAdd.Clear();


            DeltaTime = Time.deltaTime;

            var listsCount = _updateWrapperLists.Count;
            for (var i = 0; i < listsCount; i++)
            {
                var wrapperList = _updateWrapperLists[i];
                if (wrapperList.Count == 0)
                {
                    _updateWrapperListToRemove.Push(i);
                    continue;
                }

                _updateWrappersCache = wrapperList.Wrappers;

                var wrappersCount = _updateWrappersCache.Count;
                for (var index = 0; index < wrappersCount; index++)
                {
                    var wrapper = _updateWrappersCache[index];
                    if (wrapper.Type != WrapperType.Enable) continue;
                    wrapper.MonoBase.OnUpdate();
                }
            }

            _updateWrappersCache = null;

            while (_updateWrapperListToRemove.Count > 0)
            {
                _updateWrapperLists.RemoveAt(_updateWrapperListToRemove.Pop());
            }

            _updateWrapperListToRemove.Clear();
        }

        private void AddToList(IMonoOnUpdate monoOnUpdate)
        {
            var priority = monoOnUpdate.Priority;
            var index = _updateWrapperLists.BinarySearch(new MonoBaseWrapperList<IMonoOnUpdate>(priority),
                _updateComparer);

            MonoBaseWrapperList<IMonoOnUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _updateWrapperLists[index];
                monoOnUpdate.UpdateWrappers = wrapperList.Wrappers;
                monoOnUpdate.UpdateIndex = wrapperList.Add(monoOnUpdate);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<IMonoOnUpdate>(priority, WrapperListDefaultCapacity);
                monoOnUpdate.UpdateWrappers = wrapperList.Wrappers;
                monoOnUpdate.UpdateIndex = wrapperList.Add(monoOnUpdate);
                _updateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion

        #region FixedUpdate

        // 初始化参数
        private List<IMonoOnFixedUpdate> _fixedUpdateToAdd;
        private List<MonoBaseWrapperList<IMonoOnFixedUpdate>> _fixedUpdateWrapperLists;
        private Stack<int> _fixedUpdateWrapperListToRemove;
        private MonoBaseWrapperList<IMonoOnFixedUpdate>.Comparer _fixedUpdateComparer;

        // 缓存参数
        private List<MonoBaseWrapperList<IMonoOnFixedUpdate>.MonoBaseWrapper> _fixedUpdateWrappersCache;

        private void FixedUpdate()
        {
            var addCount = _fixedUpdateToAdd.Count;
            for (var index = 0; index < addCount; index++)
            {
                AddToList(_fixedUpdateToAdd[index]);
            }

            _fixedUpdateToAdd.Clear();


            FixedDeltaTime = Time.fixedDeltaTime;

            var listsCount = _fixedUpdateWrapperLists.Count;
            for (var i = 0; i < listsCount; i++)
            {
                var wrapperList = _fixedUpdateWrapperLists[i];
                if (wrapperList.Count == 0)
                {
                    _fixedUpdateWrapperListToRemove.Push(i);
                    continue;
                }

                _fixedUpdateWrappersCache = wrapperList.Wrappers;

                var wrappersCount = _fixedUpdateWrappersCache.Count;
                for (var index = 0; index < wrappersCount; index++)
                {
                    var wrapper = _fixedUpdateWrappersCache[index];
                    if (wrapper.Type != WrapperType.Enable) continue;

                    wrapper.MonoBase.OnFixedUpdate();
                }
            }

            _fixedUpdateWrappersCache = null;

            while (_fixedUpdateWrapperListToRemove.Count > 0)
            {
                _fixedUpdateWrapperLists.RemoveAt(_fixedUpdateWrapperListToRemove.Pop());
            }

            _fixedUpdateWrapperListToRemove.Clear();
        }

        private void AddToList(IMonoOnFixedUpdate monoOnFixedUpdate)
        {
            var priority = monoOnFixedUpdate.Priority;
            var index = _fixedUpdateWrapperLists.BinarySearch(new MonoBaseWrapperList<IMonoOnFixedUpdate>(priority),
                _fixedUpdateComparer);

            MonoBaseWrapperList<IMonoOnFixedUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _fixedUpdateWrapperLists[index];
                monoOnFixedUpdate.FixedUpdateWrappers = wrapperList.Wrappers;
                monoOnFixedUpdate.FixedUpdateIndex = wrapperList.Add(monoOnFixedUpdate);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<IMonoOnFixedUpdate>(priority, WrapperListDefaultCapacity);
                monoOnFixedUpdate.FixedUpdateWrappers = wrapperList.Wrappers;
                monoOnFixedUpdate.FixedUpdateIndex = wrapperList.Add(monoOnFixedUpdate);
                _fixedUpdateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion

        #region LateUpdate

        // 初始化参数
        private List<IMonoOnLateUpdate> _lateUpdateToAdd;
        private List<MonoBaseWrapperList<IMonoOnLateUpdate>> _lateUpdateWrapperLists;
        private Stack<int> _lateUpdateWrapperListToRemove;
        private MonoBaseWrapperList<IMonoOnLateUpdate>.Comparer _lateUpdateComparer;


        // 缓存参数
        private List<MonoBaseWrapperList<IMonoOnLateUpdate>.MonoBaseWrapper> _lateUpdateWrappersCache;

        public void LateUpdate()
        {
            var addCount = _lateUpdateToAdd.Count;
            for (var index = 0; index < addCount; index++)
            {
                AddToList(_lateUpdateToAdd[index]);
            }

            _lateUpdateToAdd.Clear();


            DeltaTime = Time.deltaTime;

            var listsCount = _lateUpdateWrapperLists.Count;
            for (var i = 0; i < listsCount; i++)
            {
                var wrapperList = _lateUpdateWrapperLists[i];
                if (wrapperList.Count == 0)
                {
                    _lateUpdateWrapperListToRemove.Push(i);
                    continue;
                }

                _lateUpdateWrappersCache = wrapperList.Wrappers;

                var wrappersCount = _lateUpdateWrappersCache.Count;
                for (var index = 0; index < wrappersCount; index++)
                {
                    var wrapper = _lateUpdateWrappersCache[index];
                    if (wrapper.Type != WrapperType.Enable) continue;

                    wrapper.MonoBase.OnLateUpdate();
                }
            }

            _lateUpdateWrappersCache = null;

            while (_lateUpdateWrapperListToRemove.Count > 0)
            {
                _lateUpdateWrapperLists.RemoveAt(_lateUpdateWrapperListToRemove.Pop());
            }

            _lateUpdateWrapperListToRemove.Clear();
        }

        private void AddToList(IMonoOnLateUpdate monoOnLateUpdate)
        {
            var priority = monoOnLateUpdate.Priority;
            var index = _lateUpdateWrapperLists.BinarySearch(new MonoBaseWrapperList<IMonoOnLateUpdate>(priority),
                _lateUpdateComparer);

            MonoBaseWrapperList<IMonoOnLateUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _lateUpdateWrapperLists[index];
                monoOnLateUpdate.LateUpdateWrappers = wrapperList.Wrappers;
                monoOnLateUpdate.LateUpdateIndex = wrapperList.Add(monoOnLateUpdate);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<IMonoOnLateUpdate>(priority, WrapperListDefaultCapacity);
                monoOnLateUpdate.LateUpdateWrappers = wrapperList.Wrappers;
                monoOnLateUpdate.LateUpdateIndex = wrapperList.Add(monoOnLateUpdate);
                _lateUpdateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion
    }
}