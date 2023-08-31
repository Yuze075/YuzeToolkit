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
        /// <param name="lifeCycle"></param>
        internal void AddMonoBase(ILifeCycle lifeCycle)
        {
            if (lifeCycle is IULifeCycle uLifeCycle)
            {
                _updateToAdd.Add(uLifeCycle);
            }

            if (lifeCycle is IFLifeCycle fLifeCycle)
            {
                _fixedUpdateToAdd.Add(fLifeCycle);
            }

            if (lifeCycle is ILLifeCycle lLifeCycle)
            {
                _lateUpdateToAdd.Add(lLifeCycle);
            }
        }

        /// <summary>
        /// 初始化方法
        /// </summary>
        internal void Initialize()
        {
            _updateToAdd = new List<IULifeCycle>();
            _updateWrapperLists = new List<MonoBaseWrapperList<IUpdate>>(WrapperListsDefaultCapacity);
            _updateWrapperListToRemove = new Stack<int>();

            _fixedUpdateToAdd = new List<IFLifeCycle>();
            _fixedUpdateWrapperLists = new List<MonoBaseWrapperList<IFixedUpdate>>(WrapperListsDefaultCapacity);
            _fixedUpdateWrapperListToRemove = new Stack<int>();

            _lateUpdateToAdd = new List<ILLifeCycle>();
            _lateUpdateWrapperLists = new List<MonoBaseWrapperList<ILateUpdate>>();
            _lateUpdateWrapperListToRemove = new Stack<int>();
        }

        #region Update

        // 初始化参数
        private List<IULifeCycle> _updateToAdd;
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

        private void AddToList(IULifeCycle uLifeCycle)
        {
            var update = uLifeCycle.Update;
            var priority = update.Priority;
            var index = _updateWrapperLists.BinarySearch(new MonoBaseWrapperList<IUpdate>(priority),
                _updateComparer);

            MonoBaseWrapperList<IUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _updateWrapperLists[index];
                uLifeCycle.Wrappers = wrapperList.Wrappers;
                uLifeCycle.Index = wrapperList.Add(update, uLifeCycle.Enable);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<IUpdate>(priority, WrapperListDefaultCapacity);
                uLifeCycle.Wrappers = wrapperList.Wrappers;
                uLifeCycle.Index = wrapperList.Add(update, uLifeCycle.Enable);
                _updateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion

        #region FixedUpdate

        // 初始化参数
        private List<IFLifeCycle> _fixedUpdateToAdd;
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

        private void AddToList(IFLifeCycle fLifeCycle)
        {
            var fixedUpdate = fLifeCycle.FixedUpdate;
            var priority = fixedUpdate.Priority;
            var index = _fixedUpdateWrapperLists.BinarySearch(new MonoBaseWrapperList<IFixedUpdate>(priority),
                _fixedUpdateComparer);

            MonoBaseWrapperList<IFixedUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _fixedUpdateWrapperLists[index];
                fLifeCycle.Wrappers = wrapperList.Wrappers;
                fLifeCycle.Index = wrapperList.Add(fixedUpdate, fLifeCycle.Enable);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<IFixedUpdate>(priority, WrapperListDefaultCapacity);
                fLifeCycle.Wrappers = wrapperList.Wrappers;
                fLifeCycle.Index = wrapperList.Add(fixedUpdate, fLifeCycle.Enable);
                _fixedUpdateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion

        #region LateUpdate

        // 初始化参数
        private List<ILLifeCycle> _lateUpdateToAdd;
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

        private void AddToList(ILLifeCycle lLifeCycle)
        {
            var lateUpdate = lLifeCycle.LateUpdate;
            var priority = lateUpdate.Priority;
            var index = _lateUpdateWrapperLists.BinarySearch(new MonoBaseWrapperList<ILateUpdate>(priority),
                _lateUpdateComparer);

            MonoBaseWrapperList<ILateUpdate> wrapperList;
            if (index >= 0)
            {
                wrapperList = _lateUpdateWrapperLists[index];
                lLifeCycle.Wrappers = wrapperList.Wrappers;
                lLifeCycle.Index = wrapperList.Add(lateUpdate,lLifeCycle.Enable);
            }
            else
            {
                wrapperList = new MonoBaseWrapperList<ILateUpdate>(priority, WrapperListDefaultCapacity);
                lLifeCycle.Wrappers = wrapperList.Wrappers;
                lLifeCycle.Index = wrapperList.Add(lateUpdate,lLifeCycle.Enable);
                _lateUpdateWrapperLists.Insert(~index, wrapperList);
            }
        }

        #endregion
    }
}