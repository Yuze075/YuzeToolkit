#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;
using YuzeToolkit.LogTool;

namespace YuzeToolkit.DriverTool
{
    public struct UpdateToken : IDisposable
    {
        public UpdateToken(IDisposable? update, IDisposable? fixedUpdate, IDisposable? lateUpdate)
        {
            _update = update;
            _fixedUpdate = fixedUpdate;
            _lateUpdate = lateUpdate;
        }

        private IDisposable? _update;
        private IDisposable? _fixedUpdate;
        private IDisposable? _lateUpdate;

        public void Dispose()
        {
            if (_update != null)
            {
                _update.Dispose();
                _update = null;
            }
            
            if (_fixedUpdate != null)
            {
                _fixedUpdate.Dispose();
                _fixedUpdate = null;
            }
            
            if (_lateUpdate != null)
            {
                _lateUpdate.Dispose();
                _lateUpdate = null;
            }
        }
    }

    /// <summary>
    /// 更新驱动器的基类, 用于派生不同unity默认生命周期更新的子类
    /// </summary>
    public abstract class MonoDriverBase : MonoBehaviour
    {
        public virtual float DeltaTime => Time.deltaTime;
        public virtual float FixedDeltaTime => Time.fixedDeltaTime;

        public UpdateToken Add(IMonoBase monoBase) =>
            new(AddUpdate(monoBase), AddFixedUpdate(monoBase), AddLateUpdate(monoBase));

        #region Update

        private readonly List<MonoWrapperList<IUpdate>> _updateWrapperListToAdd = new();
        private readonly Dictionary<int, MonoWrapperList<IUpdate>> _updateWrapperListMap = new();
        private readonly List<MonoWrapperList<IUpdate>> _updateWrapperLists = new();
        private readonly Stack<int> _updateWrapperListToRemoveIndex = new();

        private IDisposable? AddUpdate(IMonoBase monoBase)
        {
            if (monoBase is not IUpdate update) return null;
            var priority = update.UpdatePriority;
            if (_updateWrapperListMap.TryGetValue(priority, out var monoWrapperList))
                return monoWrapperList.Add(update);

            monoWrapperList = new MonoWrapperList<IUpdate>(priority);
            _updateWrapperListToAdd.Add(monoWrapperList);
            _updateWrapperListMap.Add(priority, monoWrapperList);
            return monoWrapperList.Add(update);
        }

        private void Update()
        {
            var addCount = _updateWrapperListToAdd.Count;
            for (var i = 0; i < addCount; i++)
            {
                var toAdd = _updateWrapperListToAdd[i];
                var index = _updateWrapperLists.BinarySearch(toAdd, MonoWrapperList<IUpdate>.Comparer);
                if (index >= 0) throw new SameWrapperListException(toAdd.Priority, toAdd.GetType());
                _updateWrapperLists.Insert(~index, toAdd);
            }

            _updateWrapperListToAdd.Clear();

            IMonoBase.S_DeltaTime = DeltaTime;
            var listCount = _updateWrapperLists.Count;
            for (var i = 0; i < listCount; i++)
            {
                var monoWrapperList = _updateWrapperLists[i];
                monoWrapperList.CheckChange();
                if (monoWrapperList.Count == 0)
                {
                    _updateWrapperListToRemoveIndex.Push(i);
                    continue;
                }

                var monoBases = monoWrapperList.MonoBases;
                var monoBasesCount = monoBases.Count;
                for (var j = 0; j < monoBasesCount; j++) monoBases[j]?.OnUpdate();
            }

            while (_updateWrapperListToRemoveIndex.Count > 0)
            {
                var index = _updateWrapperListToRemoveIndex.Pop();
                var wrapperList = _updateWrapperLists[index];
                _updateWrapperLists.RemoveAt(index);
                _updateWrapperListMap.Remove(wrapperList.Priority);
                wrapperList.Clear();
            }

            _updateWrapperListToRemoveIndex.Clear();
        }

        #endregion

        #region FixedUpdate

        private readonly List<MonoWrapperList<IFixedUpdate>> _fixedUpdateWrapperListToAdd = new();
        private readonly Dictionary<int, MonoWrapperList<IFixedUpdate>> _fixedUpdateWrapperListMap = new();
        private readonly List<MonoWrapperList<IFixedUpdate>> _fixedUpdateWrapperLists = new();
        private readonly Stack<int> _fixedUpdateWrapperListToRemoveIndex = new();

        private IDisposable? AddFixedUpdate(IMonoBase monoBase)
        {
            if (monoBase is not IFixedUpdate fixedUpdate) return null;
            var priority = fixedUpdate.UpdatePriority;
            if (_fixedUpdateWrapperListMap.TryGetValue(priority, out var monoWrapperList))
                return monoWrapperList.Add(fixedUpdate);

            monoWrapperList = new MonoWrapperList<IFixedUpdate>(priority);
            _fixedUpdateWrapperListToAdd.Add(monoWrapperList);
            _fixedUpdateWrapperListMap.Add(priority, monoWrapperList);
            return monoWrapperList.Add(fixedUpdate);
        }

        private void FixedUpdate()
        {
            var addCount = _fixedUpdateWrapperListToAdd.Count;
            for (var i = 0; i < addCount; i++)
            {
                var toAdd = _fixedUpdateWrapperListToAdd[i];
                var index = _fixedUpdateWrapperLists.BinarySearch(toAdd, MonoWrapperList<IFixedUpdate>.Comparer);
                if (index >= 0) throw new SameWrapperListException(toAdd.Priority, toAdd.GetType());
                _fixedUpdateWrapperLists.Insert(~index, toAdd);
            }

            _fixedUpdateWrapperListToAdd.Clear();

            IMonoBase.S_FixedDeltaTime = FixedDeltaTime;
            var listCount = _fixedUpdateWrapperLists.Count;
            for (var i = 0; i < listCount; i++)
            {
                var monoWrapperList = _fixedUpdateWrapperLists[i];
                monoWrapperList.CheckChange();
                if (monoWrapperList.Count == 0)
                {
                    _fixedUpdateWrapperListToRemoveIndex.Push(i);
                    continue;
                }

                var monoBases = monoWrapperList.MonoBases;
                var monoBasesCount = monoBases.Count;
                for (var j = 0; j < monoBasesCount; j++) monoBases[j]?.OnFixedUpdate();
            }

            while (_fixedUpdateWrapperListToRemoveIndex.Count > 0)
            {
                var index = _fixedUpdateWrapperListToRemoveIndex.Pop();
                var wrapperList = _fixedUpdateWrapperLists[index];
                _fixedUpdateWrapperLists.RemoveAt(index);
                _fixedUpdateWrapperListMap.Remove(wrapperList.Priority);
                wrapperList.Clear();
            }

            _fixedUpdateWrapperListToRemoveIndex.Clear();
        }

        #endregion

        #region LateUpdate

        private readonly List<MonoWrapperList<ILateUpdate>> _lateUpdateWrapperListToAdd = new();
        private readonly Dictionary<int, MonoWrapperList<ILateUpdate>> _lateUpdateWrapperListMap = new();
        private readonly List<MonoWrapperList<ILateUpdate>> _lateUpdateWrapperLists = new();
        private readonly Stack<int> _lateUpdateWrapperListToRemoveIndex = new();

        private IDisposable? AddLateUpdate(IMonoBase monoBase)
        {
            if (monoBase is not ILateUpdate update) return null;
            var priority = update.UpdatePriority;
            if (_lateUpdateWrapperListMap.TryGetValue(priority, out var monoWrapperList))
                return monoWrapperList.Add(update);

            monoWrapperList = new MonoWrapperList<ILateUpdate>(priority);
            _lateUpdateWrapperListToAdd.Add(monoWrapperList);
            _lateUpdateWrapperListMap.Add(priority, monoWrapperList);
            return monoWrapperList.Add(update);
        }

        private void LateUpdate()
        {
            var addCount = _lateUpdateWrapperListToAdd.Count;
            for (var i = 0; i < addCount; i++)
            {
                var toAdd = _lateUpdateWrapperListToAdd[i];
                var index = _lateUpdateWrapperLists.BinarySearch(toAdd, MonoWrapperList<ILateUpdate>.Comparer);
                if (index >= 0) throw new SameWrapperListException(toAdd.Priority, toAdd.GetType());
                _lateUpdateWrapperLists.Insert(~index, toAdd);
            }

            _lateUpdateWrapperListToAdd.Clear();

            IMonoBase.S_DeltaTime = DeltaTime;
            var listCount = _lateUpdateWrapperLists.Count;
            for (var i = 0; i < listCount; i++)
            {
                var monoWrapperList = _lateUpdateWrapperLists[i];
                monoWrapperList.CheckChange();
                if (monoWrapperList.Count == 0)
                {
                    _lateUpdateWrapperListToRemoveIndex.Push(i);
                    continue;
                }

                var monoBases = monoWrapperList.MonoBases;
                var monoBasesCount = monoBases.Count;
                for (var j = 0; j < monoBasesCount; j++) monoBases[j]?.OnLateUpdate();
            }

            while (_lateUpdateWrapperListToRemoveIndex.Count > 0)
            {
                var index = _lateUpdateWrapperListToRemoveIndex.Pop();
                var wrapperList = _lateUpdateWrapperLists[index];
                _lateUpdateWrapperLists.RemoveAt(index);
                _lateUpdateWrapperListMap.Remove(wrapperList.Priority);
                wrapperList.Clear();
            }

            _lateUpdateWrapperListToRemoveIndex.Clear();
        }

        #endregion
    }
}