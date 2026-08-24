using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aurora.Collections
{
    /// <summary>
    /// A deque.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the deque.</typeparam>
    [DebuggerTypeProxy(typeof(DequeDebugView<>))]
    [DebuggerDisplay(nameof(Count) + " = {" + nameof(Count) + "}")]
    public class Deque<T> : IList<T>, IReadOnlyList<T>
    {
        /// <summary>
        /// The array that stores the elements.
        /// </summary>
        /// <remarks>It is treated as circular (the element before the first is the last, and the element after the last is the first).</remarks>
        private T[] _array;

        /// <summary>
        /// The index of the element at the head.
        /// </summary>
        private int _head;

        /// <summary>
        /// The index of the element after the tail (if it is the last, it wraps to the head).
        /// </summary>
        private int _tail;

        /// <summary>
        /// The number of elements.
        /// </summary>
        private int _size;

        /// <summary>
        /// A version number used to ensure the <see cref="Deque{T}"/> is not modified during enumeration.
        /// </summary>
        private int _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class with capacity 0.
        /// </summary>
        public Deque()
        {
            _array = Array.Empty<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class with the specified capacity.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public Deque(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity, null);
            }
            _array = capacity == 0 ? Array.Empty<T>() : new T[capacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class that contains all elements of the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements fill the initial elements of the deque.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
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
        /// Gets the total number of elements the <see cref="Deque{T}"/> internal data structure can hold without resizing.
        /// </summary>
        public int Capacity => _array.Length;

        /// <summary>
        /// Gets the number of elements in the <see cref="Deque{T}"/>.
        /// </summary>
        public int Count => _size;

        bool ICollection<T>.IsReadOnly => false;

        /// <summary>
        /// Gets or sets the element at the specified index in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or is greater than or equal to the number of elements in the <see cref="Deque{T}"/>.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                return _array[ToIndex(index)];
            }
            set
            {
                if (index < 0 || index >= _size)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _array[ToIndex(index)] = value;
                _version++;
            }
        }

        /// <summary>
        /// Returns the object at the head of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The object at the head of the <see cref="Deque{T}"/>.</returns>
        public T PeekFirst()
        {
            if (_size == 0)
            {
                ThrowForEmptyDeque();
            }
            return _array[_head];
        }

        /// <summary>
        /// Assigns the object at the head of the <see cref="Deque{T}"/> to the <paramref name="result"/> parameter.
        /// </summary>
        /// <param name="result">The object at the head of the <see cref="Deque{T}"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Deque{T}"/> is not empty; otherwise, <see langword="false"/>.</returns>
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
        /// Returns the object at the tail of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The object at the tail of the <see cref="Deque{T}"/>.</returns>
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
        /// Assigns the object at the tail of the <see cref="Deque{T}"/> to the <paramref name="result"/> parameter.
        /// </summary>
        /// <param name="result">The object at the tail of the <see cref="Deque{T}"/>.</param>
        /// <returns><see langword="true"/> if the <see cref="Deque{T}"/> is not empty; otherwise, <see langword="false"/>.</returns>
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
        /// Adds an object to the head of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the head of the <see cref="Deque{T}"/>.</param>
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
        /// Adds an object to the tail of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="item">The object to add to the tail of the <see cref="Deque{T}"/>.</param>
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

        void ICollection<T>.Add(T item)
        {
            EnqueueLast(item);
        }

        /// <summary>
        /// Inserts an object at the specified index in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="item"/>.</param>
        /// <param name="item">The object to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or is greater than the number of elements in the <see cref="Deque{T}"/>.</exception>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > _size)
            {
                throw new ArgumentOutOfRangeException();
            }
            if (_size == _array.Length)
            {
                Grow(_size + 1);
            }
            InternalInsert(ToIndexCapacityIsValid(index), item);
        }

        private void InternalInsert(int index, T item)
        {
            var array = _array;
            var head  = _head;
            var tail  = _tail;
            if (head > tail && index >= head)
            {
                Array.Copy(array, head, array, head - 1, index - head);
                array[index - 1] = item;

                _head--;
            }
            else
            {
                Array.Copy(array, index, array, index + 1, tail - index);
                array[index] = item;

                MoveNext(ref _tail);
            }
            _size++;
            _version++;
        }

        /// <summary>
        /// Removes all elements from the <see cref="Deque{T}"/>.
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
        /// Removes the element at the specified index in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than 0, or is greater than or equal to the number of elements in the <see cref="Deque{T}"/>.</exception>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _size)
            {
                throw new ArgumentOutOfRangeException();
            }
            InternalRemoveAt(ToIndex(index));
        }

        private void InternalRemoveAt(int index)
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
        /// Removes the first occurrence of the specified object from the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        /// <returns><see langword="true"/> if removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool Remove(T item)
        {
            var index = InternalIndexOf(item);
            if (index < 0)
            {
                return false;
            }
            InternalRemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes the last occurrence of the specified object from the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="item">The object to remove.</param>
        /// <returns><see langword="true"/> if removed successfully; otherwise, <see langword="false"/>.</returns>
        public bool RemoveLast(T item)
        {
            var index = InternalLastIndexOf(item);
            if (index < 0)
            {
                return false;
            }
            InternalRemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes and returns the object at the head of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The object removed from the head of the <see cref="Deque{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
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
        /// Removes the object at the head of the <see cref="Deque{T}"/> and assigns it to the <paramref name="result"/> parameter.
        /// </summary>
        /// <param name="result">The object removed from the head of the <see cref="Deque{T}"/>.</param>
        /// <returns><see langword="true"/> if removed successfully; otherwise, <see langword="false"/>.</returns>
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
        /// Removes and returns the object at the tail of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The object removed from the tail of the <see cref="Deque{T}"/>.</returns>
        /// <exception cref="InvalidOperationException">The <see cref="Deque{T}"/> is empty.</exception>
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
        /// Removes the object at the tail of the <see cref="Deque{T}"/> and assigns it to the <paramref name="result"/> parameter.
        /// </summary>
        /// <param name="result">The object removed from the tail of the <see cref="Deque{T}"/>.</param>
        /// <returns><see langword="true"/> if removed successfully; otherwise, <see langword="false"/>.</returns>
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
        /// Copies all elements from the <see cref="Deque{T}"/> into the specified array.
        /// </summary>
        /// <param name="array">The array into which the elements of the <see cref="Deque{T}"/> are copied.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0 or greater than the length of <paramref name="array"/>.</exception>
        /// <exception cref="ArgumentException">The number of elements in the <see cref="Deque{T}"/> is greater than the available space in <paramref name="array"/> from <paramref name="arrayIndex"/> to the end.</exception>
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
                throw new ArgumentException(
                    $"The number of elements in the source deque is greater than the available space from {nameof(arrayIndex)} to the end of the target {nameof(array)}"
                );
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

        /// <summary>
        /// Determines whether an element is in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="Deque{T}"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="item"/> is found in the <see cref="Deque{T}"/>; otherwise, <see langword="false"/>.</returns>
        public bool Contains(T item)
        {
            return InternalIndexOf(item) >= 0;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first match in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="Deque{T}"/>.</param>
        /// <returns>The zero-based index of the first match in the <see cref="Deque{T}"/>, if found; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            var index = InternalIndexOf(item);
            return index >= 0 ? ToLogicalIndex(index) : index;
        }

        private int InternalIndexOf(T item)
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

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the first matching element in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="match">The condition.</param>
        /// <returns>The zero-based index of the first element matching <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        public int FindIndex(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            var index = InternalFindIndex(match);
            return index >= 0 ? ToLogicalIndex(index) : index;
        }

        private int InternalFindIndex(Predicate<T> match)
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

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the first matching element in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="match">The condition.</param>
        /// <param name="state">The state parameter passed by the user.</param>
        /// <typeparam name="TState">The type of the state parameter.</typeparam>
        /// <returns>The zero-based index of the first element matching <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        public int FindIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            var index = InternalFindIndex(match, state);
            return index >= 0 ? ToLogicalIndex(index) : index;
        }

        private int InternalFindIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
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

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the last match in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="Deque{T}"/>.</param>
        /// <returns>The zero-based index of the last match in the <see cref="Deque{T}"/>, if found; otherwise, -1.</returns>
        public int LastIndexOf(T item)
        {
            var index = InternalLastIndexOf(item);
            return index >= 0 ? ToLogicalIndex(index) : index;
        }

        private int InternalLastIndexOf(T item)
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

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the last matching element in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="match">The condition.</param>
        /// <returns>The zero-based index of the last element matching <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        public int FindLastIndex(Predicate<T> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            var index = InternalFindLastIndex(match);
            return index >= 0 ? ToLogicalIndex(index) : index;
        }

        private int InternalFindLastIndex(Predicate<T> match)
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

        /// <summary>
        /// Searches for an element matching the specified condition and returns the zero-based index of the last matching element in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="match">The condition.</param>
        /// <param name="state">The state parameter passed by the user.</param>
        /// <typeparam name="TState">The type of the state parameter.</typeparam>
        /// <returns>The zero-based index of the last element matching <paramref name="match"/>, if found; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="match"/> is <see langword="null"/>.</exception>
        public int FindLastIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
        {
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            var index = InternalFindLastIndex(match, state);
            return index >= 0 ? ToLogicalIndex(index) : index;
        }

        private int InternalFindLastIndex<TState>(ParameterizedPredicate<T, TState> match, TState state)
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

        /// <summary>
        /// Ensures that the capacity of the <see cref="Deque{T}"/> is at least the specified value.
        /// </summary>
        /// <param name="capacity">The minimum capacity to ensure.</param>
        /// <returns>The new capacity of the <see cref="Deque{T}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
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

        /// <summary>
        /// Sets the capacity of the <see cref="Deque{T}"/> to the number of elements in the <see cref="Deque{T}"/> (if that number is less than 90% of the current capacity).
        /// </summary>
        public void TrimExcess()
        {
            var threshold = (int)(_array.Length * 0.9);
            if (_size < threshold)
            {
                SetCapacity(_size);
            }
        }

        /// <summary>
        /// Sets the capacity of the <see cref="Deque{T}"/> to the specified value.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than the number of elements in the <see cref="Deque{T}"/>.</exception>
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

        /// <summary>
        /// Copies all elements of the <see cref="Deque{T}"/> into a new array.
        /// </summary>
        /// <returns>A new array containing all elements copied from the <see cref="Deque{T}"/>.</returns>
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

        private void MovePrevious(ref int index)
        {
            index = index == 0 ? _array.Length - 1 : index - 1;
        }

        private void MoveNext(ref int index)
        {
            index = index == _array.Length - 1 ? 0 : index + 1;
        }

        /// <summary>
        /// Reverses the order of the elements in the <see cref="Deque{T}"/>.
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
            return (uint)head + (uint)logicalIndex < (uint)capacity
                       ? head + logicalIndex
                       : head + logicalIndex - capacity;
        }

        private int ToIndexCapacityIsValid(int logicalIndex)
        {
            var head     = _head;
            var capacity = _array.Length;
            return (uint)head + (uint)logicalIndex <= (uint)capacity
                       ? head + logicalIndex
                       : head + logicalIndex - capacity;
        }

        private static void ThrowForEmptyDeque()
        {
            throw new InvalidOperationException("The deque is empty");
        }

        /// <summary>
        /// Gets an enumerator that enumerates all elements of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>An enumerator that enumerates all elements of the <see cref="Deque{T}"/>.</returns>
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
        /// An enumerator used to enumerate the <see cref="Deque{T}"/>.
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
                throw new InvalidOperationException(
                    _index == -1 ? "Enumeration has not started" : "Enumeration has ended"
                );
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
    }
}
