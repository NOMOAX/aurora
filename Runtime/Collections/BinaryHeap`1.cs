using System;
using System.Collections;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// A binary heap.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the binary heap.</typeparam>
    public sealed class BinaryHeap<T> : IEnumerable<T>
    {
        private readonly IComparer<T> _comparer;

        private T[] _array;

        private int _size;

        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"/> class.
        /// </summary>
        public BinaryHeap() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"/> class with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public BinaryHeap(int capacity) : this(capacity, Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"/> class with the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
        public BinaryHeap(IComparer<T> comparer) : this(0, comparer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryHeap{T}"/> class with the specified initial capacity and comparer.
        /// </summary>
        /// <param name="capacity">The initial capacity.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comparer"/> is <see langword="null"/>.</exception>
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
        /// Gets the number of members in this binary heap.
        /// </summary>
        public int Count => _size;

        /// <summary>
        /// Adds an element to this binary heap.
        /// </summary>
        /// <param name="t">The object to add to this binary heap.</param>
        /// <exception cref="OverflowException">The capacity of this binary heap is not enough to hold the object to add and needs to grow, but it cannot grow (the capacity has reached the maximum value of <see cref="int"/>).</exception>
        public void Add(T t)
        {
            var length = _array.Length;
            if (_size == length)
            {
                if (length == int.MaxValue)
                {
                    throw new OverflowException();
                }
                var newLength = (int)Math.Min((uint)length * 2 + 1, int.MaxValue);
                var newArray  = new T[newLength];
                Array.Copy(_array, 0, newArray, 0, length);
                _array = newArray;
            }
            _array[_size++] = t;
            ShiftUp(_size - 1);
            ++_version;
        }

        /// <summary>
        /// Adds all elements of the specified sequence to the <see cref="BinaryHeap{T}"/>.
        /// </summary>
        /// <param name="enumerable">The sequence whose elements are all added to this binary heap.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> is <see langword="null"/>.</exception>
        /// <exception cref="OverflowException">The capacity of this binary heap is not enough to hold the object to add and needs to grow, but it cannot grow (the capacity has reached the maximum value of <see cref="int"/>).</exception>
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
        /// Removes and returns the smallest element (per the comparer) in this binary heap.
        /// </summary>
        /// <returns>The smallest member (per the comparer) in this binary heap.</returns>
        /// <exception cref="InvalidOperationException">This binary heap has no elements.</exception>
        public T Take()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("This binary heap has no members");
            }
            var t = _array[0];
            _array[0] = _array[--_size];
            ShiftDown(0);
            ++_version;
            return t;
        }

        /// <summary>
        /// Returns the smallest element (per the comparer) in this binary heap.
        /// </summary>
        /// <returns>The smallest member (per the comparer) in this binary heap.</returns>
        /// <exception cref="InvalidOperationException">This binary heap has no elements.</exception>
        public T Peek()
        {
            if (_size == 0)
            {
                throw new InvalidOperationException("This binary heap has no members.");
            }
            return _array[0];
        }

        /// <summary>
        /// Determines whether an object is in this binary heap.
        /// </summary>
        /// <param name="t">The object to determine whether it is in this binary heap.</param>
        /// <returns><see langword="true"/> if <paramref name="t"/> is found in this binary heap; otherwise, <see langword="false"/>.</returns>
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
        /// Removes an element from this binary heap.
        /// </summary>
        /// <param name="t">The object to remove from this binary heap.</param>
        /// <returns><see langword="true"/> if <paramref name="t"/> is found in this binary heap; otherwise, <see langword="false"/>.</returns>
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
        /// Removes all elements from this binary heap.
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
        /// Gets an enumerator that can enumerate the elements of this binary heap.
        /// </summary>
        /// <returns>An enumerator that can enumerate the elements of this binary heap.</returns>
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
                    throw new InvalidOperationException("The binary heap was modified during enumeration");
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
            public readonly T Current
            {
                get
                {
                    if (_disposed)
                    {
                        throw new ObjectDisposedException(typeof(Enumerator).FullName);
                    }
                    return _index switch
                    {
                        -2 => throw new InvalidOperationException(
                                  "The enumerator is positioned before the first member of the binary heap"
                              ),
                        -1 => throw new InvalidOperationException(
                                  "The enumerator is positioned after the last member of the binary heap"
                              ),
                        _ => _currentElement
                    };
                }
            }

            readonly object IEnumerator.Current => Current;

            void IEnumerator.Reset()
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(typeof(Enumerator).FullName);
                }
                if (_version != _binaryHeap._version)
                {
                    throw new InvalidOperationException("The binary heap was modified during enumeration");
                }
                _index          = -1;
                _currentElement = default;
            }
        }
    }
}
