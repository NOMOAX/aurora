using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// An object pool.
    /// </summary>
    /// <typeparam name="T">The type of the members in the object pool.</typeparam>
    public sealed class Pool<T> : IPool<T> where T : class
    {
        private readonly Func<T> _createFunc;

        private readonly Action<T> _getAction;

        private readonly Func<T, bool> _returnFunc;

        private readonly Action<T> _disposeAction;

        private readonly int _capacity;

        private T _fastItem;

        private readonly ConcurrentQueue<T> _items = new();

        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool{T}"/> class using the specified policy.
        /// </summary>
        /// <param name="policy">The policy to use.</param>
        /// <exception cref="ArgumentNullException"><paramref name="policy"/> is <see langword="null"/>.</exception>
        public Pool(IPooledObjectPolicy<T> policy) : this(policy, System.Environment.ProcessorCount * 2)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pool{T}"/> class using the specified policy and maximum retained capacity.
        /// </summary>
        /// <param name="policy">The policy to use.</param>
        /// <param name="maximumRetained">The maximum number of cached objects.</param>
        /// <exception cref="ArgumentNullException"><paramref name="policy"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maximumRetained"/> is less than 1.</exception>
        public Pool(IPooledObjectPolicy<T> policy, int maximumRetained)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }
            if (maximumRetained < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumRetained), maximumRetained, null);
            }
            // Cache the target interface methods to avoid interface lookup overhead
            _createFunc    = policy.Create;
            _getAction     = policy.Get;
            _returnFunc    = policy.Return;
            _disposeAction = policy.Dispose;
            _capacity      = maximumRetained - 1; // -1 because _fastItem also counts as a cached object
        }

        /// <inheritdoc />
        public T Get()
        {
            var item = _fastItem;
            if (item == null || Interlocked.CompareExchange(ref _fastItem, null, item) != item)
            {
                if (_items.TryDequeue(out item))
                {
                    Interlocked.Decrement(ref _count);
                }
                else
                {
                    item = _createFunc();
                }
            }
            _getAction(item);
            return item;
        }

        /// <inheritdoc />
        public void Return(T obj)
        {
            if (_returnFunc(obj))
            {
                if (_fastItem == null && Interlocked.CompareExchange(ref _fastItem, obj, null) == null)
                {
                    return;
                }
                if (Interlocked.Increment(ref _count) <= _capacity)
                {
                    _items.Enqueue(obj);
                }
                else
                {
                    Interlocked.Decrement(ref _count); // Revert the increment above
                    _disposeAction(obj);
                }
            }
            else
            {
                _disposeAction(obj);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            var item = _fastItem;
            if (item != null && Interlocked.CompareExchange(ref _fastItem, null, item) == item)
            {
                _disposeAction(item);
            }
            while (_items.TryDequeue(out item))
            {
                Interlocked.Decrement(ref _count);
                _disposeAction(item);
            }
        }
    }
}
