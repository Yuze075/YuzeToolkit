using System.Collections.Generic;

namespace YuzeToolkit.DriverTool
{
    /// <summary>
    /// 用于封装管理不同优先级更新的list
    /// </summary>
    internal readonly struct MonoBaseWrapperList<T> where T : class, IMonoBase
    {
        /// <summary>
        /// 用于封装<see cref="MonoBase"/>的结构体
        /// </summary>
        public struct MonoBaseWrapper
        {
            /// <summary>
            /// 默认构造函数, 用于绑定对应的空栈和自身Index
            /// </summary>
            /// <param name="nullStack"></param>
            public MonoBaseWrapper(Stack<int> nullStack)
            {
                IsNull = true;
                MonoBase = null;
                _nullStack = nullStack;
            }

            public bool IsNull;
            public T MonoBase;
            private readonly Stack<int> _nullStack;


            /// <summary>
            /// 绑定新的<see cref="MonoBase"/>
            /// </summary>
            public void SetMonoBase(T monoBase)
            {
                IsNull = false;
                MonoBase = monoBase;
            }

            public void Dispose(int index)
            {
                // 将空位添加带栈中
                _nullStack.Push(index);

                // 重设参数
                MonoBase = null;
                IsNull = true;
            }
        }

        public struct Comparer : IComparer<MonoBaseWrapperList<T>>
        {
            public int Compare(MonoBaseWrapperList<T> x, MonoBaseWrapperList<T> y)
            {
                if (x._priority == y._priority) return 0;
                if (x._priority > y._priority) return 1;
                return -1;
            }
        }

        public MonoBaseWrapperList(int priority, int capacity)
        {
            _priority = priority;
            Wrappers = new List<MonoBaseWrapper>(capacity);
            _nullStack = new Stack<int>();
        }

        public MonoBaseWrapperList(int priority)
        {
            _priority = priority;
            Wrappers = null;
            _nullStack = null;
        }

        public readonly List<MonoBaseWrapper> Wrappers;
        private readonly int _priority;
        private readonly Stack<int> _nullStack;

        /// <summary>
        /// 当前正在使用的<see cref="MonoBaseWrapper"/>数量
        /// </summary>
        public int Count => Wrappers.Count - _nullStack.Count;

        /// <summary>
        /// 添加函数, 用于向更新队列中添加元素
        /// </summary>
        public int Add(T monoBase)
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