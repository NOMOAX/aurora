using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aurora.Collections
{
    /// <summary>
    /// 双端队列。
    /// </summary>
    /// <typeparam name="T">双端队列中元素的类型。</typeparam>
    /// <remarks>为保证安全性，<see cref="Deque{T}"/> 自身未提供全部功能。要使用额外功能，请通过 <see cref="GetAccessor"/> 获取访问器。</remarks>
    [DebuggerTypeProxy(typeof(DequeDebugView<>))]
    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class Deque<T> : ICollection<T>, ICollection, IReadOnlyList<T>
    {
        /// <summary>
        /// 存放元素的数组。
        /// </summary>
        /// <remarks>它被视为是首尾循环的（第一个元素的上一个元素是最后一个元素，最后一个元素的下一个元素是第一个元素）。</remarks>
        private T[] _array;

        /// <summary>
        /// 开头处元素的索引。
        /// </summary>
        private int _head;

        /// <summary>
        /// 结尾处元素的下一个（如果是最后一个，则回到开头）元素的索引。
        /// </summary>
        private int _tail;

        /// <summary>
        /// 元素的数量。
        /// </summary>
        private int _size;

        /// <summary>
        /// 版本号，用于在枚举 <see cref="Deque{T}"/> 时确保 <see cref="Deque{T}"/> 没有被修改。
        /// </summary>
        private int _version;

        /// <summary>
        /// 初始化容量为 0 的 <see cref="Deque{T}"/> 类的新实例。
        /// </summary>
        public Deque()
        {
            _array = Array.Empty<T>();
        }

        /// <summary>
        /// 初始化具有指定容量的 <see cref="Deque{T}"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">容量。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 0。</exception>
        public Deque(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null);
            }
            _array = new T[capacity];
        }

        /// <summary>
        /// 初始化拥有指定集合中所有元素的 <see cref="Deque{T}"/> 类的新实例。
        /// </summary>
        /// <param name="collection">要向双端列表填充初始元素的集合。</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
        public Deque(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            _array = EnumerableHelpers.ToArray(collection, out _size);
            if (_size != _array.Length)
            {
                _tail = _size;
            }
        }

        /// <summary>
        /// 获取 <see cref="Deque{T}"/> 中元素的数量。
        /// </summary>
        public int Count => _size;

        /// <summary>
        /// 获取 <see cref="Deque{T}"/> 的内部数据结构在不调整大小的情况下能够容纳的元素总数。
        /// </summary>
        public int Capacity => _array.Length;

        bool ICollection<T>.IsReadOnly => false;

        void ICollection<T>.Add(T item)
        {
            EnqueueLast(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            var index = IndexOf(item);
            if (index < 0)
            {
                return false;
            }
            RemoveAt(index);
            return true;
        }

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        T IReadOnlyList<T>.this[int index] => GetAccessor()[index];

        /// <summary>
        /// 移除 <see cref="Deque{T}"/> 中的所有元素。
        /// </summary>
        public void Clear()
        {
            var size = _size;
            if (size != 0)
            {
                var array = _array;
                var head  = _head;
                var tail  = _tail;
                if (tail == 0 || head < tail)
                {
                    Array.Clear(array, head, size);
                }
                else
                {
                    Array.Clear(array, head, array.Length - head);
                    Array.Clear(array, 0,    tail);
                }
                _size = 0;
            }
            _head = 0;
            _tail = 0;
            _version++;
        }

        /// <summary>
        /// 将 <see cref="Deque{T}"/> 中的所有元素复制到指定数组中。
        /// </summary>
        /// <param name="array">要将 <see cref="Deque{T}"/> 中的元素复制到的数组。</param>
        /// <param name="arrayIndex"><paramref name="array"/> 中从 0 开始的索引，从此处开始复制。</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> 小于 0 或大于 <paramref name="array"/> 的长度。</exception>
        /// <exception cref="ArgumentException"><see cref="Deque{T}"/> 中的元素个数大于 <paramref name="array"/> 从 <paramref name="arrayIndex"/> 到末尾之间的可用空间。</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, null);
            }
            if (array.Length - arrayIndex < _size)
            {
                throw new ArgumentException($"源双端队列中的元素个数大于目标 {nameof(array)} 从 {nameof(arrayIndex)} 到末尾之间的可用空间");
            }
            var numToCopy = _size;
            if (numToCopy == 0)
            {
                return;
            }
            var firstPart = _array.Length - _head < numToCopy ? _array.Length - _head : numToCopy;
            Array.Copy(_array, _head, array, arrayIndex, firstPart);
            numToCopy -= firstPart;
            if (numToCopy > 0)
            {
                Array.Copy(_array, 0, array, arrayIndex + _array.Length - _head, numToCopy);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException($"{nameof(array)} 是多维的");
            }
            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException($"{nameof(array)} 的起始索引不为 0");
            }
            var arrayLen = array.Length;
            if (index < 0 || index > arrayLen)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (arrayLen - index < _size)
            {
                throw new ArgumentException($"源双端队列中的元素个数大于目标 {nameof(array)} 从 {nameof(index)} 到末尾之间的可用空间");
            }
            var numToCopy = _size;
            if (numToCopy == 0)
            {
                return;
            }
            try
            {
                var firstPart = _array.Length - _head < numToCopy ? _array.Length - _head : numToCopy;
                Array.Copy(_array, _head, array, index, firstPart);
                numToCopy -= firstPart;
                if (numToCopy > 0)
                {
                    Array.Copy(_array, 0, array, index + _array.Length - _head, numToCopy);
                }
            }
            catch (ArrayTypeMismatchException e)
            {
                throw new ArgumentException($"源双端队列中的元素类型与目标 {nameof(array)} 的元素类型不匹配", nameof(array), e);
            }
        }

        /// <summary>
        /// 将对象添加到 <see cref="Deque{T}"/> 的开头处。
        /// </summary>
        /// <param name="item">要添加的元素。</param>
        public void EnqueueFirst(T item)
        {
            if (_size == _array.Length)
            {
                Grow(_size + 1);
            }
            MovePrevious(ref _head);
            _array[_head] = item;
            _size++;
            _version++;
        }

        /// <summary>
        /// 将对象添加到 <see cref="Deque{T}"/> 的结尾处。
        /// </summary>
        /// <param name="item">要添加的元素。</param>
        public void EnqueueLast(T item)
        {
            if (_size == _array.Length)
            {
                Grow(_size + 1);
            }
            _array[_tail] = item;
            MoveNext(ref _tail);
            _size++;
            _version++;
        }

        /// <summary>
        /// 获取枚举 <see cref="Deque{T}"/> 所有元素的枚举器。
        /// </summary>
        /// <returns>枚举 <see cref="Deque{T}"/> 所有元素的枚举器。</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 移除并返回 <see cref="Deque{T}"/> 开头处的对象。
        /// </summary>
        /// <returns>从 <see cref="Deque{T}"/> 开头处移除的对象。</returns>
        /// <exception cref="InvalidOperationException"><see cref="Deque{T}"/> 为空。</exception>
        public T DequeueFirst()
        {
            if (_size == 0)
            {
                ThrowForEmptyDeque();
            }

            var head  = _head;
            var array = _array;

            var removed = array[head];
            array[head] = default;

            MoveNext(ref _head);

            _size--;
            _version++;
            return removed;
        }

        /// <summary>
        /// 移除 <see cref="Deque{T}"/> 开头处的对象，并将它赋值给 <paramref name="result"/> 参数。
        /// </summary>
        /// <param name="result">从 <see cref="Deque{T}"/> 开头处移除的对象。</param>
        /// <returns>如果成功移除，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public bool TryDequeueFirst(out T result)
        {
            if (_size == 0)
            {
                result = default;
                return false;
            }

            var head  = _head;
            var array = _array;

            result      = array[head];
            array[head] = default;

            MoveNext(ref _head);

            _size--;
            _version++;
            return true;
        }

        /// <summary>
        /// 移除并返回 <see cref="Deque{T}"/> 结尾处的对象。
        /// </summary>
        /// <returns>从 <see cref="Deque{T}"/> 结尾处移除的对象。</returns>
        /// <exception cref="InvalidOperationException"><see cref="Deque{T}"/> 为空。</exception>
        public T DequeueLast()
        {
            if (_size == 0)
            {
                ThrowForEmptyDeque();
            }

            MovePrevious(ref _tail);

            var tail  = _tail;
            var array = _array;

            var removed = array[tail];
            array[tail] = default;

            _size--;
            _version++;
            return removed;
        }

        /// <summary>
        /// 移除 <see cref="Deque{T}"/> 结尾处的对象，并将它赋值给 <paramref name="result"/> 参数。
        /// </summary>
        /// <param name="result">从 <see cref="Deque{T}"/> 结尾处移除的对象。</param>
        /// <returns>如果成功移除，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public bool TryDequeueLast(out T result)
        {
            if (_size == 0)
            {
                result = default;
                return false;
            }

            MovePrevious(ref _tail);

            var tail  = _tail;
            var array = _array;

            result      = array[tail];
            array[tail] = default;

            _size--;
            _version++;
            return true;
        }

        /// <summary>
        /// 返回 <see cref="Deque{T}"/> 开头处的对象。
        /// </summary>
        /// <returns><see cref="Deque{T}"/> 开头处的对象。</returns>
        public T PeekFirst()
        {
            if (_size == 0)
            {
                ThrowForEmptyDeque();
            }
            return _array[_head];
        }

        /// <summary>
        /// 将 <see cref="Deque{T}"/> 开头处的对象赋值给 <paramref name="result"/> 参数。
        /// </summary>
        /// <param name="result"><see cref="Deque{T}"/> 开头处的对象。</param>
        /// <returns>如果 <see cref="Deque{T}"/> 不为空，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public bool TryPeekFirst(out T result)
        {
            if (_size == 0)
            {
                result = default;
                return false;
            }
            result = _array[_head];
            return true;
        }

        /// <summary>
        /// 返回 <see cref="Deque{T}"/> 结尾处的对象。
        /// </summary>
        /// <returns><see cref="Deque{T}"/> 结尾处的对象。</returns>
        public T PeekLast()
        {
            if (_size == 0)
            {
                ThrowForEmptyDeque();
            }
            var tail = _tail;
            MovePrevious(ref tail);
            return _array[tail];
        }

        /// <summary>
        /// 将 <see cref="Deque{T}"/> 结尾处的对象赋值给 <paramref name="result"/> 参数。
        /// </summary>
        /// <param name="result"><see cref="Deque{T}"/> 结尾处的对象。</param>
        /// <returns>如果 <see cref="Deque{T}"/> 不为空，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public bool TryPeekLast(out T result)
        {
            if (_size == 0)
            {
                result = default;
                return false;
            }
            var tail = _tail;
            MovePrevious(ref tail);
            result = _array[tail];
            return true;
        }

        /// <summary>
        /// 确定某元素是否在 <see cref="Deque{T}"/> 中。
        /// </summary>
        /// <param name="item">要在 <see cref="Deque{T}"/> 中定位的对象。</param>
        /// <returns>如果在 <see cref="Deque{T}"/> 中找到 <paramref name="item"/>，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        /// <summary>
        /// 将 <see cref="Deque{T}"/> 的所有元素复制到新数组。
        /// </summary>
        /// <returns>包含从 <see cref="Deque{T}"/> 复制的所有元素的新数组。</returns>
        public T[] ToArray()
        {
            var size = _size;
            if (size == 0)
            {
                return Array.Empty<T>();
            }
            var array  = _array;
            var head   = _head;
            var tail   = _tail;
            var array1 = new T[size];
            if (tail == 0 || head < tail)
            {
                Array.Copy(array, head, array1, 0, size);
            }
            else
            {
                Array.Copy(array, head, array1, 0,                   array.Length - head);
                Array.Copy(array, 0,    array1, array.Length - head, tail);
            }
            return array1;
        }

        private void SetCapacity(int capacity)
        {
            var array    = _array;
            var head     = _head;
            var tail     = _tail;
            var size     = _size;
            var newArray = new T[capacity];
            if (size != 0)
            {
                if (tail == 0 || head < tail)
                {
                    Array.Copy(array, head, newArray, 0, size);
                }
                else
                {
                    Array.Copy(array, head, newArray, 0,                   array.Length - head);
                    Array.Copy(array, 0,    newArray, array.Length - head, tail);
                }
            }
            _array = newArray;
            _head  = 0;
            _tail  = size == capacity ? 0 : size;
            _version++;
        }

        private void MovePrevious(ref int index)
        {
            index = index == 0 ? _array.Length - 1 : index - 1;
        }

        private void MoveNext(ref int index)
        {
            index = index == _array.Length - 1 ? 0 : index + 1;
        }

        private static void ThrowForEmptyDeque()
        {
            throw new InvalidOperationException("双端队列为空");
        }

        /// <summary>
        /// 将 <see cref="Deque{T}"/> 的容量设置为 <see cref="Deque{T}"/> 中元素的数量（如果该数字小于当前容量的 90%）。
        /// </summary>
        public void TrimExcess()
        {
            var threshold = (int) (_array.Length * 0.9);
            if (_size < threshold)
            {
                SetCapacity(_size);
            }
        }

        /// <summary>
        /// 将 <see cref="Deque{T}"/> 的容量设置为指定值。
        /// </summary>
        /// <param name="capacity">新容量。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 <see cref="Deque{T}"/> 中元素的数量。</exception>
        public void TrimExcess(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null);
            }
            if (capacity < _size)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null);
            }
            SetCapacity(capacity);
        }

        /// <summary>
        /// 确保 <see cref="Deque{T}"/> 的容量至少为指定值。
        /// </summary>
        /// <param name="capacity">要确保的最小容量。</param>
        /// <returns><see cref="Deque{T}"/> 的新容量。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 0。</exception>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null);
            }
            if (_array.Length < capacity)
            {
                Grow(capacity);
            }
            return _array.Length;
        }

        private void Grow(int capacity)
        {
            const int growFactor  = 2;
            const int minimumGrow = 4;

            var newCapacity = checked(_array.Length * growFactor);
            newCapacity = Math.Max(newCapacity, checked(_array.Length + minimumGrow));
            if (newCapacity < capacity)
            {
                newCapacity = capacity;
            }
            SetCapacity(newCapacity);
        }

        private int IndexOf(T item)
        {
            var size = _size;
            if (size == 0)
            {
                return -1;
            }
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                return Array.IndexOf(array, item, head, size);
            }
            var index = Array.IndexOf(array, item, head, array.Length - head);
            return index >= 0 ? index : Array.IndexOf(array, item, 0, tail);
        }

        private int FindIndex(Predicate<T> match)
        {
            var size = _size;
            if (size == 0)
            {
                return -1;
            }
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                Array.FindIndex(array, head, size, match);
            }
            var index = Array.FindIndex(array, head, array.Length - head, match);
            return index >= 0 ? index : Array.FindIndex(array, 0, tail, match);
        }

        private int FindIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
        {
            var size = _size;
            if (size == 0)
            {
                return -1;
            }
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                FindIndex(array, head, size, match, state);
            }
            var index = FindIndex(array, head, array.Length - head, match, state);
            return index >= 0 ? index : FindIndex(array, 0, tail, match, state);
        }

        private static int FindIndex<TState>(
            T[]                               array,
            int                               startIndex,
            int                               count,
            ParameterizedPredicate<T, TState> match,
            TState                            state)
        {
            var endIndex = startIndex + count;
            for (var i = startIndex; i < endIndex; i++)
            {
                if (match(array[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        private int LastIndexOf(T item)
        {
            var size = _size;
            if (size == 0)
            {
                return -1;
            }
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                return Array.LastIndexOf(array, item, tail - 1, size);
            }
            var index = Array.LastIndexOf(array, item, tail - 1, tail);
            return index >= 0 ? index : Array.LastIndexOf(array, item, array.Length - 1, array.Length - head);
        }

        private int FindLastIndex(Predicate<T> match)
        {
            var size = _size;
            if (size == 0)
            {
                return -1;
            }
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                return Array.FindLastIndex(array, tail - 1, size, match);
            }
            var index = Array.FindLastIndex(array, tail - 1, tail, match);
            return index >= 0 ? index : Array.FindLastIndex(array, array.Length - 1, array.Length - head, match);
        }

        private int FindLastIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
        {
            var size = _size;
            if (size == 0)
            {
                return -1;
            }
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                return FindLastIndex(array, tail - 1, size, match, state);
            }
            var index = FindLastIndex(array, tail - 1, tail, match, state);
            return index >= 0 ? index : FindLastIndex(array, array.Length - 1, array.Length - head, match, state);
        }

        private static int FindLastIndex<TState>(
            T[]                               array,
            int                               startIndex,
            int                               count,
            ParameterizedPredicate<T, TState> match,
            TState                            state)
        {
            var endIndex = startIndex - count;
            for (var i = startIndex; i > endIndex; i--)
            {
                if (match(array[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        private void RemoveAt(int index)
        {
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                if (index < head + (_size >> 1))
                {
                    Array.Copy(array, head, array, head + 1, index - head);
                    array[head] = default;

                    _head++;
                }
                else
                {
                    MovePrevious(ref _tail);

                    Array.Copy(array, index + 1, array, index, tail - index);
                    array[tail] = default;
                }
            }
            else
            {
                if (index >= head)
                {
                    Array.Copy(array, head, array, head + 1, index - head);
                    array[head] = default;

                    MoveNext(ref _head);
                }
                else
                {
                    MovePrevious(ref _tail);

                    Array.Copy(array, index + 1, array, index, tail - index);
                    array[tail] = default;
                }
            }
            _size--;
            _version++;
        }

        /// <summary>
        /// 反转 <see cref="Deque{T}"/> 中元素的顺序。
        /// </summary>
        public void Reverse()
        {
            var size = _size;
            if (size < 2)
            {
                return;
            }
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (tail == 0 || head < tail)
            {
                Array.Reverse(array, head, size);
            }
            else
            {
                var capacity = array.Length;
                Array.Reverse(array, head, capacity - head);
                Array.Reverse(array, 0,    tail);
                if (head == tail)
                {
                    _head = 0;
                    _tail = 0;
                }
                else if (tail >= capacity - head)
                {
                    Array.Copy(array, head, array, tail, capacity - head);
                    Array.Clear(array, size, capacity - size);
                    _head = 0;
                    _tail = size;
                }
                else
                {
                    Array.Copy(array, 0, array, capacity - size, tail);
                    Array.Clear(array, 0, capacity - size);
                    _head = head - tail;
                    _tail = 0;
                }
            }
            _version++;
        }

        /// <summary>
        /// 获取访问器。
        /// </summary>
        /// <returns>访问此 <see cref="Deque{T}"/> 的访问器。</returns>
        public Accessor GetAccessor()
        {
            return new Accessor(this);
        }

        private int ToLogicalIndex(int index)
        {
            var head     = _head;
            var capacity = _array.Length;
            return index - head >= 0 ? index - head : index - head + capacity;
        }

        private int ToIndex(int logicalIndex)
        {
            var head     = _head;
            var capacity = _array.Length;
            return head + logicalIndex < (uint) capacity ? head + logicalIndex : head + logicalIndex - capacity;
        }

        /// <summary>
        /// 用于枚举 <see cref="Deque{T}"/> 的枚举器。
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            private readonly Deque<T> _deque;

            private readonly int _version;

            private int _index;

            private T _current;

            internal Enumerator(Deque<T> deque)
            {
                _deque   = deque;
                _version = deque._version;
                _index   = -1;
                _current = default;
            }

            /// <inheritdoc />
            public void Dispose()
            {
                _index   = -2;
                _current = default;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_version != _deque._version)
                {
                    throw new InvalidOperationException();
                }
                if (_index == -2)
                {
                    return false;
                }
                _index++;
                if (_index == _deque._size)
                {
                    _index   = -2;
                    _current = default;
                    return false;
                }
                _current = _deque._array[_deque.ToIndex(_index)];
                return true;
            }

            /// <inheritdoc />
            public readonly T Current
            {
                get
                {
                    if (_index < 0)
                    {
                        ThrowEnumerationNotStartedOrEnded();
                    }
                    return _current;
                }
            }

            private readonly void ThrowEnumerationNotStartedOrEnded()
            {
                throw new InvalidOperationException(_index == -1 ? "枚举未开始" : "枚举已结束");
            }

            readonly object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                if (_version != _deque._version)
                {
                    throw new InvalidOperationException();
                }
                _index   = -1;
                _current = default;
            }
        }

        /// <summary>
        /// 用于访问 <see cref="Deque{T}"/> 的访问器。
        /// </summary>
        public readonly struct Accessor
        {
            private readonly Deque<T> _deque;

            internal Accessor(Deque<T> deque)
            {
                _deque = deque;
            }

            /// <summary>
            /// 获取 <see cref="Deque{T}"/> 中元素的数量。
            /// </summary>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            public int Count
            {
                get
                {
                    if (_deque == null)
                    {
                        throw new InvalidOperationException();
                    }
                    return _deque._size;
                }
            }

            /// <summary>
            /// 获取或设置 <see cref="Deque{T}"/> 中位于指定索引处的元素。
            /// </summary>
            /// <param name="index">索引。</param>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者大于或等于 <see cref="Deque{T}"/> 中元素的数量。</exception>
            public T this[int index]
            {
                get
                {
                    if (_deque == null)
                    {
                        throw new InvalidOperationException();
                    }
                    if (index < 0 || index >= _deque._size)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    return _deque._array[_deque.ToIndex(index)];
                }
                set
                {
                    if (index < 0 || index >= _deque._size)
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                    _deque._array[_deque.ToIndex(index)] = value;
                }
            }

            /// <summary>
            /// 搜索指定的对象，返回在 <see cref="Deque{T}"/> 中第一个匹配项的从零开始的索引。
            /// </summary>
            /// <param name="item">要在 <see cref="Deque{T}"/> 中定位的对象。</param>
            /// <returns>如果找到，则为 <see cref="Deque{T}"/> 中第一个匹配项的从零开始的索引；否则为 -1。</returns>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            public int IndexOf(T item)
            {
                if (_deque == null)
                {
                    throw new InvalidOperationException();
                }
                var index = _deque.IndexOf(item);
                return index >= 0 ? _deque.ToLogicalIndex(index) : index;
            }

            /// <summary>
            /// 搜索与指定条件相匹配的元素，返回在 <see cref="Deque{T}"/> 中第一个匹配元素的从零开始的索引。
            /// </summary>
            /// <param name="match">条件。</param>
            /// <returns>如果找到与 <paramref name="match"/> 相匹配的第一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            /// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <see langword="null"/>。</exception>
            public int FindIndex(Predicate<T> match)
            {
                if (_deque == null)
                {
                    throw new InvalidOperationException();
                }
                if (match == null)
                {
                    throw new ArgumentNullException(nameof(match));
                }
                var index = _deque.FindIndex(match);
                return index >= 0 ? _deque.ToLogicalIndex(index) : index;
            }

            /// <summary>
            /// 搜索与指定条件相匹配的元素，返回在 <see cref="Deque{T}"/> 中第一个匹配元素的从零开始的索引。
            /// </summary>
            /// <param name="match">条件。</param>
            /// <param name="state">由用户传入的状态参数。</param>
            /// <typeparam name="TState">状态参数的类型。</typeparam>
            /// <returns>如果找到与 <paramref name="match"/> 相匹配的第一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            /// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <see langword="null"/>。</exception>
            public int FindIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
            {
                if (_deque == null)
                {
                    throw new InvalidOperationException();
                }
                if (match == null)
                {
                    throw new ArgumentNullException(nameof(match));
                }
                var index = _deque.FindIndex(match, state);
                return index >= 0 ? _deque.ToLogicalIndex(index) : index;
            }

            /// <summary>
            /// 搜索指定的对象，返回在 <see cref="Deque{T}"/> 中最后一个匹配项的从零开始的索引。
            /// </summary>
            /// <param name="item">要在 <see cref="Deque{T}"/> 中定位的对象。</param>
            /// <returns>如果找到，则为 <see cref="Deque{T}"/> 中最后一个匹配项的从零开始的索引；否则为 -1。</returns>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            public int LastIndexOf(T item)
            {
                if (_deque == null)
                {
                    throw new InvalidOperationException();
                }
                var index = _deque.LastIndexOf(item);
                return index >= 0 ? _deque.ToLogicalIndex(index) : index;
            }

            /// <summary>
            /// 搜索与指定条件相匹配的元素，返回在 <see cref="Deque{T}"/> 中最后一个匹配元素的从零开始的索引。
            /// </summary>
            /// <param name="match">条件。</param>
            /// <returns>如果找到与 <paramref name="match"/> 相匹配的最后一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            /// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <see langword="null"/>。</exception>
            public int FindLastIndex(Predicate<T> match)
            {
                if (_deque == null)
                {
                    throw new InvalidOperationException();
                }
                if (match == null)
                {
                    throw new ArgumentNullException(nameof(match));
                }
                var index = _deque.FindLastIndex(match);
                return index >= 0 ? _deque.ToLogicalIndex(index) : index;
            }

            /// <summary>
            /// 搜索与指定条件相匹配的元素，返回在 <see cref="Deque{T}"/> 中最后一个匹配元素的从零开始的索引。
            /// </summary>
            /// <param name="match">条件。</param>
            /// <param name="state">由用户传入的状态参数。</param>
            /// <typeparam name="TState">状态参数的类型。</typeparam>
            /// <returns>如果找到与 <paramref name="match"/> 相匹配的最后一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            /// <exception cref="ArgumentNullException"><paramref name="match"/> 为 <see langword="null"/>。</exception>
            public int FindLastIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
            {
                if (_deque == null)
                {
                    throw new InvalidOperationException();
                }
                if (match == null)
                {
                    throw new ArgumentNullException(nameof(match));
                }
                var index = _deque.FindLastIndex(match, state);
                return index >= 0 ? _deque.ToLogicalIndex(index) : index;
            }

            /// <summary>
            /// 移除 <see cref="Deque{T}"/> 指定索引处的元素。
            /// </summary>
            /// <param name="index">要移除的元素的从零开始的索引。</param>
            /// <exception cref="InvalidOperationException">此实例无效，请通过 <see cref="Deque{T}.GetAccessor"/> 获取有效的访问器实例。</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者大于或等于 <see cref="Deque{T}"/> 中元素的数量。</exception>
            public void RemoveAt(int index)
            {
                if (_deque == null)
                {
                    throw new InvalidOperationException();
                }
                if (index < 0 || index >= _deque._size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _deque.RemoveAt(_deque.ToIndex(index));
            }
        }
    }
}
