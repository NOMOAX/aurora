using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 二叉堆。
    /// </summary>
    /// <typeparam name="T">二叉堆中的成员的类型。</typeparam>
    public sealed class BinaryHeap<T> : IEnumerable<T>
    {
        private readonly IComparer<T> _comparer;

        private T[] _array;

        private int _size;

        private int _version;

        /// <summary>
        /// 初始化 <see cref="BinaryHeap{T}"/> 类的新实例。
        /// </summary>
        public BinaryHeap() : this(0)
        {
        }

        /// <summary>
        /// 使用指定的初始容量初始化 <see cref="BinaryHeap{T}"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">初始容量。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 0。</exception>
        public BinaryHeap(int capacity) : this(capacity, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// 使用指定的比较器初始化 <see cref="BinaryHeap{T}"/> 类的新实例。
        /// </summary>
        /// <param name="comparer">比较器。</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> 为 <see langword="null"/>。</exception>
        public BinaryHeap(IComparer<T> comparer) : this(0, comparer)
        {
        }

        /// <summary>
        /// 使用指定的初始容量和指定的比较器初始化 <see cref="BinaryHeap{T}"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">初始容量。</param>
        /// <param name="comparer">比较器。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 0。</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> 为 <see langword="null"/>。</exception>
        public BinaryHeap(int capacity, IComparer<T> comparer)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null);
            }
            _array    = new T[capacity];
            _size     = 0;
            _comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));
            _version  = 0;
        }

        /// <summary>
        /// 获取这个二叉堆中的成员的数量。
        /// </summary>
        public int Count => _size;

        /// <summary>
        /// 向这个二叉堆添加一个成员。
        /// </summary>
        /// <param name="t">要添加到这个二叉堆的对象。</param>
        /// <exception cref="OverflowException">这个二叉堆的容量不足以容纳将要添加的对象，需要进行扩容，但是又无法进行扩容（容量已经达到了 <see cref="int"/> 的最大值）。</exception>
        public void Add(T t)
        {
            var length = _array.Length;
            if (_size == length)
            {
                if (length == int.MaxValue)
                {
                    throw new OverflowException();
                }
                var newLength = (int) System.Math.Min((uint) length * 2 + 1, int.MaxValue);
                var newArray  = new T[newLength];
                Array.Copy(_array, 0, newArray, 0, length);
                _array = newArray;
            }
            _array[_size++] = t;
            ShiftUp(_size - 1);
            ++_version;
        }

        /// <summary>
        /// 向 <see cref="BinaryHeap{T}"/> 添加指定的序列中的所有成员。
        /// </summary>
        /// <param name="enumerable">要将其中所有成员添加到这个二叉堆的序列。</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="OverflowException">这个二叉堆的容量不足以容纳将要添加的对象，需要进行扩容，但是又无法进行扩容（容量已经达到了 <see cref="int"/> 的最大值）。</exception>
        public void AddRange(IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }
            foreach (var element in enumerable)
            {
                Add(element);
            }
        }

        /// <summary>
        /// 移除并返回这个二叉堆中（根据比较器比较的结果）最小的成员。
        /// </summary>
        /// <returns>这个二叉堆中（根据比较器比较的结果）最小的成员。</returns>
        /// <exception cref="InvalidOperationException">这个二叉堆的成员数为 0。</exception>
        public T Take()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("二叉堆的成员数为 0");
            }
            var t = _array[0];
            _array[0] = _array[--_size];
            ShiftDown(0);
            ++_version;
            return t;
        }

        /// <summary>
        /// 返回这个二叉堆中（根据比较器比较的结果）最小的成员。
        /// </summary>
        /// <returns>这个二叉堆中（根据比较器比较的结果）最小的成员。</returns>
        /// <exception cref="InvalidOperationException">这个二叉堆的成员数为 0。</exception>
        public T Peek()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("二叉堆的成员数为 0 .");
            }
            return _array[0];
        }

        /// <summary>
        /// 确定某个对象是否在这个二叉堆中。
        /// </summary>
        /// <param name="t">要确定是否在这个二叉堆中的对象。</param>
        /// <returns>如果在这个二叉堆中找到了 <paramref name="t"/>。则返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
        public bool Contains(T t)
        {
            return IndexOf(t) >= 0;
        }

        private int IndexOf(T t)
        {
            var equalityComparer = EqualityComparer<T>.Default;
            for (var i = 0; i < _size; i++)
            {
                if (equalityComparer.Equals(_array[i], t))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 从这个二叉堆中移除某个成员。
        /// </summary>
        /// <param name="t">要从这个二叉堆中移除的对象。</param>
        /// <returns>如果在这个二叉堆中找到了 <paramref name="t"/>，则为 <see langword="true"/> ；否则为 <see langword="false"/>。</returns>
        public bool Remove(T t)
        {
            var index = IndexOf(t);
            if (index < 0)
            {
                return false;
            }
            if (index == --_size)
            {
                ++_version;
                return true;
            }
            _array[index] = _array[_size];
            ShiftDown(index);
            ++_version;
            return true;
        }

        /// <summary>
        /// 移除这个二叉堆中的所有成员。
        /// </summary>
        public void Clear()
        {
            if (_size == 0)
            {
                return;
            }
            Array.Clear(_array, 0, _size);
            _size = 0;
            ++_version;
        }

        private void ShiftUp(int index)
        {
            while (index > 0)
            {
                var parentIndex = (index - 1) >> 1;
                var compare     = _comparer.Compare(_array[parentIndex], _array[index]);
                if (compare <= 0)
                {
                    break;
                }
                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void ShiftDown(int index)
        {
            var half = _size >> 1;
            while (index < half)
            {
                var mostIndex      = index;
                var leftChildIndex = (index << 1) + 1;
                var compare        = _comparer.Compare(_array[leftChildIndex], _array[mostIndex]);
                if (compare < 0)
                {
                    mostIndex = leftChildIndex;
                }
                var rightChildIndex = leftChildIndex + 1;
                if (rightChildIndex < _size)
                {
                    compare = _comparer.Compare(_array[rightChildIndex], _array[mostIndex]);
                    if (compare < 0)
                    {
                        mostIndex = rightChildIndex;
                    }
                }
                if (mostIndex == index)
                {
                    break;
                }
                Swap(index, mostIndex);
                index = mostIndex;
            }
        }

        private void Swap(int index1, int index2)
        {
            if (index1 == index2)
            {
                return;
            }
            (_array[index1], _array[index2]) = (_array[index2], _array[index1]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 获取可用于枚举这个二叉堆的成员的枚举器。
        /// </summary>
        /// <returns>可用于枚举这个二叉堆的成员的枚举器。</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        private struct Enumerator : IEnumerator<T>
        {
            private readonly BinaryHeap<T> _binaryHeap;

            private readonly int _version;

            private T _currentElement;

            private int _index;

            private bool _disposed;

            internal Enumerator(BinaryHeap<T> binaryHeap)
            {
                _binaryHeap     = binaryHeap;
                _version        = binaryHeap._version;
                _currentElement = default;
                _index          = -1;
                _disposed       = false;
            }

            void IDisposable.Dispose()
            {
                if (_disposed)
                {
                    return;
                }
                _index          = -2;
                _currentElement = default;
                _disposed       = true;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(typeof(Enumerator).FullName);
                }
                if (_version != _binaryHeap._version)
                {
                    throw new InvalidOperationException("在枚举期间修改了二叉堆");
                }
                if (_index == -2)
                {
                    return false;
                }
                ++_index;
                if (_index == _binaryHeap._size)
                {
                    _index          = -2;
                    _currentElement = default;
                    return false;
                }
                _currentElement = _binaryHeap._array[_index];
                return true;
            }

            /// <inheritdoc />
            public T Current
            {
                get
                {
                    if (_disposed)
                    {
                        throw new ObjectDisposedException(typeof(Enumerator).FullName);
                    }
                    if (_index == -2)
                    {
                        throw new InvalidOperationException("枚举器位于二叉堆的第一个成员之前");
                    }
                    if (_index == -1)
                    {
                        throw new InvalidOperationException("枚举器位于二叉堆的最后一个成员之后");
                    }
                    return _currentElement;
                }
            }

            object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(typeof(Enumerator).FullName);
                }
                if (_version != _binaryHeap._version)
                {
                    throw new InvalidOperationException("在枚举期间修改了二叉堆");
                }
                _index          = -1;
                _currentElement = default;
            }
        }
    }
}
