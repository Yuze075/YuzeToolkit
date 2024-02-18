#nullable enable
#if YUZE_USE_UNITY_NETCODE
using System;
using System.Collections;
using System.Collections.Generic;

namespace YuzeToolkit.Network.LagCompensation
{
    /// <summary>
    /// 固定大小的队列
    /// </summary>
    public sealed class FixedQueue<T> : IEnumerable<T>
    {
        public FixedQueue(int maxSize)
        {
            _queue = new T[maxSize];
            _queueStart = 0;
        }

        private readonly T[] _queue;
        private int _queueStart;
        private int _version;
        public int Count { get; private set; }

        private int GetTrueIndex(int virtualIndex) => (_queueStart + virtualIndex) % _queue.Length;

        /// <summary>
        /// 通过虚拟的Index获取到对应对象
        /// </summary>
        /// <param name="index">获取对象的虚拟Index, 即抽象的队列中的顺序</param>
        public T this[int index] => _queue[GetTrueIndex(index)];

        /// <summary>
        /// 将对象放入队列中
        /// </summary>
        /// <returns>返回放入队列是否替换的其他值, true代表替换的其他值, false代表没有替换其他值</returns>
        public bool Enqueue(T t)
        {
            _queue[GetTrueIndex(Count)] = t;
            ++_version;
            if (++Count <= _queue.Length) return false;
            --Count;
            return true;
        }

        /// <summary>
        /// 从队列中取出一个对象
        /// </summary>
        public T Dequeue()
        {
            if (--Count == -1)
            {
                ++Count;
                throw new IndexOutOfRangeException("无法从空队列中取出对象!");
            }

            var res = _queue[_queueStart];
            _queue[_queueStart] = default!;
            _queueStart = GetTrueIndex(1); // 将_queueStart移动到下一个位置
            ++_version;
            return res;
        }


        public Enumerator GetEnumerator() => new(this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public struct Enumerator : IEnumerator<T>
        {
#nullable disable
            private readonly FixedQueue<T> _fixedQueue;
            private readonly int _version;
            private int _index;
            private T _currentElement;

            public T Current
            {
                get
                {
                    if (_index < 0)
                        throw new InvalidOperationException(
                            $"当前{nameof(FixedQueue<T>)}的{nameof(Enumerator)}还未初始化或者已经释放!");
                    return _currentElement;
                }
            }

            object IEnumerator.Current => Current;

            internal Enumerator(FixedQueue<T> fixedQueue)
            {
                _fixedQueue = fixedQueue;
                _version = _fixedQueue._version;
                _index = -1;
                _currentElement = default;
            }

            public bool MoveNext()
            {
                if (_version != _fixedQueue._version)
                    throw new InvalidOperationException("在foreach的过程中不能改变对应集合的值!");

                if (_index == -2) return false;

                ++_index;

                if (_index == _fixedQueue.Count)
                {
                    Dispose();
                    return false;
                }

                _currentElement = _fixedQueue[_index];
                return true;
            }

            void IEnumerator.Reset()
            {
                if (_version != _fixedQueue._version)
                    throw new InvalidOperationException($"对应集合的值已经发生改变无法重新{nameof(IEnumerator.Reset)}!");
                _index = -1;
                _currentElement = default;
            }

            public void Dispose()
            {
                _index = -2;
                _currentElement = default;
            }

#nullable enable
        }
    }
}
#endif