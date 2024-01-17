#nullable enable
using System;
using System.Collections.Generic;
using YuzeToolkit.LogTool;
using YuzeToolkit.PoolTool;

namespace YuzeToolkit.DriverTool
{
    internal sealed class MonoWrapper<T> : IDisposable where T : class, IMonoBase
    {
        public static MonoWrapper<T> Get(List<int> toRemoveIndex)
        {
            var monoWrapper = GenericPool<MonoWrapper<T>>.Get();
            monoWrapper.OnGet(toRemoveIndex);
            return monoWrapper;
        }

        public static void Release(MonoWrapper<T> monoWrapper)
        {
            monoWrapper.OnRelease();
            GenericPool<MonoWrapper<T>>.Release(monoWrapper);
        }

        private List<int>? _toRemoveIndex;
        public int Index { get; private set; }
        public bool IsActive { get; private set; }


        public void ReSetActive() => IsActive = true;

        /// <summary>
        /// 获取时进行的初始化操作
        /// </summary>
        private void OnGet(List<int> toRemoveIndex)
        {
            _toRemoveIndex = toRemoveIndex;
            Index = -1;
            IsActive = true;
        }

        /// <summary>
        /// 对应新创建的<see cref="MonoWrapper{T}"/>绑定时设置其对应Index
        /// </summary>
        public void ReSetIndex(int index) => Index = index;

        /// <summary>
        /// 调用<see cref="Dispose"/>结算当前<see cref="MonoWrapper{T}"/>的更新周期
        /// </summary>
        public void Dispose()
        {
#if UNITY_EDITOR
            if (!IsActive) LogSys.LogError($"当前的{typeof(MonoWrapper<T>)}已经被释放过了, 无法重新释放!");
#endif
            IsActive = false;


            if (_toRemoveIndex == null)
            {
#if UNITY_EDITOR
                LogSys.LogError($"当前的{typeof(MonoWrapper<T>)}在释放时,{nameof(_toRemoveIndex)}为空!");
#endif
                return;
            }

            if (Index >= 0) _toRemoveIndex.Add(Index);
        }

        #region Pool

        private void OnRelease()
        {
#if UNITY_EDITOR
            if (IsActive)
                LogSys.LogError($"当前的{typeof(MonoWrapper<T>)}还绑定着Index{Index}的值, 错误的OnRelease回收!");
#endif
            _toRemoveIndex = null;
        }

        #endregion
    }
}