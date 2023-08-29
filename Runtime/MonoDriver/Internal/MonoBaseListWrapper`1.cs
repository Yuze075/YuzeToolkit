using System.Collections;
using System.Collections.Generic;

namespace YuzeToolkit.Utility
{
    /// <summary>
    /// 用于封装管理不同优先级更新的list
    /// </summary>
    internal readonly struct MonoBaseWrapperList<T> where T : class, IMonoBase
    {
        /// <summary>
        /// 用于封装<see cref="MonoBase"/>的结构体, 记录自身<see cref="WrapperType"/>
        /// </summary>
        internal struct MonoBaseWrapper
        {
            /// <summary>
            /// 默认构造函数, 用于绑定对应的空栈和自身Index
            /// </summary>
            /// <param name="nullStack"></param>
            internal MonoBaseWrapper(Stack<int> nullStack)
            {
                Type = WrapperType.Null;
                MonoBase = null;
                _nullStack = nullStack;
            }

            internal WrapperType Type;
            internal T MonoBase;
            private readonly Stack<int> _nullStack;


            /// <summary>
            /// 绑定新的<see cref="MonoBase"/>
            /// </summary>
            /// <param name="monoBase"></param>
            internal void SetMonoBase(T monoBase)
            {
                Type = monoBase.IsEnable ? WrapperType.Enable : WrapperType.Disable;
                MonoBase = monoBase;
            }

            internal void Enable()
            {
                // 启动
                Type = WrapperType.Enable;
            }

            internal void Disable()
            {
                // 关闭
                Type = WrapperType.Disable;
            }

            internal void Destroy(int index)
            {
                // 将空位添加带栈中
                _nullStack.Push(index);

                // 重设参数
                MonoBase = null;
                Type = WrapperType.Null;
            }
        }

        internal struct Comparer : IComparer<MonoBaseWrapperList<T>>
        {
            public int Compare(MonoBaseWrapperList<T> x, MonoBaseWrapperList<T> y)
            {
                if (x._priority == y._priority) return 0;
                if (x._priority > y._priority) return 1;
                return -1;
            }
        }

        internal MonoBaseWrapperList(int priority, int capacity)
        {
            _priority = priority;
            Wrappers = new List<MonoBaseWrapper>(capacity);
            _nullStack = new Stack<int>();
        }

        internal MonoBaseWrapperList(int priority)
        {
            _priority = priority;
            Wrappers = null;
            _nullStack = null;
        }

        internal readonly List<MonoBaseWrapper> Wrappers;
        private readonly int _priority;
        private readonly Stack<int> _nullStack;

        /// <summary>
        /// 当前正在使用的<see cref="MonoBaseWrapper"/>数量
        /// </summary>
        internal int Count => Wrappers.Count - _nullStack.Count;

        /// <summary>
        /// 添加函数, 用于向更新队列中添加元素
        /// </summary>
        /// <param name="monoBase"></param>
        internal int Add(T monoBase)
        {
            var wrapper = new MonoBaseWrapper(_nullStack);
            wrapper.SetMonoBase(monoBase);
            if (_nullStack.Count > 0)
            {
                var index = _nullStack.Pop();
                Wrappers[index] = wrapper;
                return index;
            }

            Wrappers.Add(wrapper);
            return Wrappers.Count - 1;
        }
    }
}