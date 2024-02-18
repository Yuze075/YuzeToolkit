#nullable enable
using System;
using System.Collections.Generic;

namespace YuzeToolkit.DriverTool
{
    internal readonly struct MonoWrapperList<T> where T : class, IMonoBase
    {
        public static readonly IComparer<MonoWrapperList<T>> Comparer = new MonoWrapperListComparer();

        public MonoWrapperList(int priority, int capacity = 16)
        {
            Priority = priority;
            MonoBases = new List<T?>(capacity);
            _toAdd = new List<(T, MonoWrapper<T>)>();
            _toRemoveIndex = new List<int>();
            _monoWrappers = new List<MonoWrapper<T>>(capacity);
            _nullIndex = new Stack<int>();
        }

        public readonly int Priority;

        public readonly List<T?> MonoBases;
        private readonly List<MonoWrapper<T>> _monoWrappers;

        private readonly List<(T, MonoWrapper<T>)> _toAdd;
        private readonly List<int> _toRemoveIndex;

        private readonly Stack<int> _nullIndex;

        public int Count => MonoBases.Count - _nullIndex.Count;

        /// <summary>
        /// 将一个<see cref="T"/>类型的<see cref="IMonoBase"/>对象添加到预添加队列中
        /// </summary>
        public IDisposable Add(T monoBase)
        {
#if UNITY_EDITOR
            if (monoBase.UpdatePriority != Priority)
                MonoBaseExtensions.LogError($"传入的{monoBase}的优先级为:{monoBase.UpdatePriority}, 不是{this}对应的优先级: {Priority}!");
#endif
            // 检测是否存在空的MonoWrapper
            if (_nullIndex.Count > 0)
            {
                // 获取一个空的MonoWrapper
                var index = _nullIndex.Pop();
                var monoWrapper = _monoWrappers[index];
                
                // 设置MonoWrapper为激活状态
                monoWrapper.ReSetActive();
                
                // 添加到预添加队列中
                _toAdd.Add((monoBase, monoWrapper));
                return monoWrapper;
            }
            else
            {
                // 创建一个新的MonoWrapper
                var monoWrapper = MonoWrapper<T>.Get(_toRemoveIndex);
                
                // 添加到预添加队列中
                _toAdd.Add((monoBase, monoWrapper));
                return monoWrapper;
            }
        }

        /// <summary>
        /// 检测<see cref="MonoWrapperList{T}"/>是否有<see cref="MonoWrapper{T}"/>对象的添加或者删除
        /// </summary>
        public void CheckChange()
        {
            // 检测MonoWrapper的删除
            var toRemoveCount = _toRemoveIndex.Count;   
            for (var i = 0; i < toRemoveCount; i++)
            {
                var index = _toRemoveIndex[i];
#if UNITY_EDITOR
                if (MonoBases[index] == null)
                    MonoBaseExtensions.LogError($"优先级为: {Priority}的{this}, 移除的Index{index}, 类型为{typeof(T)}对象为空");
#endif
                // 将对应IMonoBase设置为空
                MonoBases[index] = null;
                // 将空的位置放入栈中, 待其他对象使用
                _nullIndex.Push(index);
            }

            _toRemoveIndex.Clear();

            // 检测MonoWrapper的添加
            var addCount = _toAdd.Count;
            for (var i = 0; i < addCount; i++)
            {
                var (update, updateWrapper) = _toAdd[i];

                // 如果是已经绑定Index, 说明是在列表中的Wrapper
                if (updateWrapper.Index >= 0)
                {
                    // 判断是否已经被释放
                    if (!updateWrapper.IsActive) continue;
#if UNITY_EDITOR
                    if (MonoBases[updateWrapper.Index] != null)
                        MonoBaseExtensions.LogError($"优先级为: {Priority}的{this}, 重设的Index为{updateWrapper.Index}, 类型为{typeof(T)}对象不为空");
#endif
                    // 将对于位置的IUpdate替换为目标update
                    MonoBases[updateWrapper.Index] = update;
                    continue;
                }

                // 如果Index小于0说明是新建的Wrapper
                
                // 如果IsActive为False, 则说明还没有更新, MonoWrapper已经被释放了, 直接由对象池回收
                if (!updateWrapper.IsActive)
                {
                    MonoWrapper<T>.Release(updateWrapper);
                    continue;
                }

                // 没有被释放, 设置对应的Index
                updateWrapper.ReSetIndex(MonoBases.Count);
                // 添加到_updates和_updateWrappers中
                MonoBases.Add(update);
                _monoWrappers.Add(updateWrapper);
            }

            _toAdd.Clear();

            // if (_nullIndex.Count > MonoBases.Count / 2)
            // {
            //     // todo 缩小数组大小
            // }
        }

        public void Clear()
        {
            MonoBases.Clear();
            _toAdd.Clear();
            _toRemoveIndex.Clear();
            var wrappersCount = _monoWrappers.Count;
            for (var i = 0; i < wrappersCount; i++) MonoWrapper<T>.Release(_monoWrappers[i]);
            _monoWrappers.Clear();
            _nullIndex.Clear();
        }

        #region Struct

        private class MonoWrapperListComparer : IComparer<MonoWrapperList<T>>
        {
            public int Compare(MonoWrapperList<T> x, MonoWrapperList<T> y)
            {
                if (x.Priority == y.Priority) return 0;
                if (x.Priority > y.Priority) return 1;
                return -1;
            }
        }

        #endregion
    }
}