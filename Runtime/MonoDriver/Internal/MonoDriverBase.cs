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
        /// <param name="disposable"></param>
        internal void AddMonoBase(IDisposable disposable)
        {
            if (disposable is IUpdateCycle uLifeCycle)
            {
                _updateToAdd.Add(uLifeCycle);
            }

            if (disposable is IFixedUpdateCycle fLifeCycle)
            {
                _fixedUpdateToAdd.Add(fLifeCycle);
            }

            if (disposable is ILateUpdateCycle lLifeCycle)
            {
                _lateUpdateToAdd.Add(lLifeCycle);
            }
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        internal void Initialize()
        {
            _updateToAdd = new List<IUpdateCycle>();
            _updateWrapperLists = new List<MonoBaseWrapperList<IUpdate>>(WrapperListsDefaultCapacity);
            _updateWrapperListToRemove = new Stack<int>();

            _fixedUpdateToAdd = new List<IFixedUpdateCycle>();
            _fixedUpdateWrapperLists = new List<MonoBaseWrapperList<IFixedUpdate>>(WrapperListsDefaultCapacity);
            _fixedUpdateWrapperListToRemove = new Stack<int>();

            _lateUpdateToAdd = new List<ILateUpdateCycle>();
            _lateUpdateWrapperLists = new List<MonoBaseWrapperList<ILateUpdate>>();
            _lateUpdateWrapperListToRemove = new Stack<int>();
        }

        #region Update

        // 初始化参数
        private List<IUpdateCycle> _updateToAdd;
        private List<MonoBaseWrapperList<IUpdate>> _updateWrapperLists;
        private Stack<int> _updateWrapperListToRemove;
        private MonoBaseWrapperList<IUpdate>.Comparer _updateComparer;

        // 缓存参数
        private List<MonoBaseWrapperList<IUpdate>.MonoBaseWrapper> _updateWrappersCache;

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
                    if (wrapper.IsNull) continue;
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

        private void AddToList(IUpdateCycle updateCycle)
        {
            var update = updateCycle.Update;
            var priority = update.Priority;
            var index = _updateWrapperLists.BinarySearch(new MonoBaseWrapperList<IUpdate>(priority),
                _updateComparer);

            MonoBaseWrapperList<IUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _updateWrapperLists[index];
                updateCycle.Wrappers = wrapperList.Wrappers;
                updateCycle.Index = wrapperList.Add(update);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<IUpdate>(priority, WrapperListDefaultCapacity);
                updateCycle.Wrappers = wrapperList.Wrappers;
                updateCycle.Index = wrapperList.Add(update);
                _updateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion

        #region FixedUpdate

        // 初始化参数
        private List<IFixedUpdateCycle> _fixedUpdateToAdd;
        private List<MonoBaseWrapperList<IFixedUpdate>> _fixedUpdateWrapperLists;
        private Stack<int> _fixedUpdateWrapperListToRemove;
        private MonoBaseWrapperList<IFixedUpdate>.Comparer _fixedUpdateComparer;

        // 缓存参数
        private List<MonoBaseWrapperList<IFixedUpdate>.MonoBaseWrapper> _fixedUpdateWrappersCache;

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
                    if (wrapper.IsNull) continue;

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

        private void AddToList(IFixedUpdateCycle fixedUpdateCycle)
        {
            var fixedUpdate = fixedUpdateCycle.FixedUpdate;
            var priority = fixedUpdate.Priority;
            var index = _fixedUpdateWrapperLists.BinarySearch(new MonoBaseWrapperList<IFixedUpdate>(priority),
                _fixedUpdateComparer);

            MonoBaseWrapperList<IFixedUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _fixedUpdateWrapperLists[index];
                fixedUpdateCycle.Wrappers = wrapperList.Wrappers;
                fixedUpdateCycle.Index = wrapperList.Add(fixedUpdate);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<IFixedUpdate>(priority, WrapperListDefaultCapacity);
                fixedUpdateCycle.Wrappers = wrapperList.Wrappers;
                fixedUpdateCycle.Index = wrapperList.Add(fixedUpdate);
                _fixedUpdateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion

        #region LateUpdate

        // 初始化参数
        private List<ILateUpdateCycle> _lateUpdateToAdd;
        private List<MonoBaseWrapperList<ILateUpdate>> _lateUpdateWrapperLists;
        private Stack<int> _lateUpdateWrapperListToRemove;
        private MonoBaseWrapperList<ILateUpdate>.Comparer _lateUpdateComparer;


        // 缓存参数
        private List<MonoBaseWrapperList<ILateUpdate>.MonoBaseWrapper> _lateUpdateWrappersCache;

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
                    if (wrapper.IsNull) continue;

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

        private void AddToList(ILateUpdateCycle lateUpdateCycle)
        {
            var lateUpdate = lateUpdateCycle.LateUpdate;
            var priority = lateUpdate.Priority;
            var index = _lateUpdateWrapperLists.BinarySearch(new MonoBaseWrapperList<ILateUpdate>(priority),
                _lateUpdateComparer);

            MonoBaseWrapperList<ILateUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _lateUpdateWrapperLists[index];
                lateUpdateCycle.Wrappers = wrapperList.Wrappers;
                lateUpdateCycle.Index = wrapperList.Add(lateUpdate);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<ILateUpdate>(priority, WrapperListDefaultCapacity);
                lateUpdateCycle.Wrappers = wrapperList.Wrappers;
                lateUpdateCycle.Index = wrapperList.Add(lateUpdate);
                _lateUpdateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion
    }
}