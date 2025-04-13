using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 对象池。
    /// </summary>
    /// <typeparam name="T">对象池中成员的类型。</typeparam>
    public sealed class Pool<T> : IPool<T> where T : class
    {
        private readonly Func<T> _createFunc;

        private readonly Action<T> _getAction;

        private readonly Func<T, bool> _returnFunc;

        private readonly Action<T> _disposeAction;

        private readonly int _capacity;

        private T _fastItem;

        private readonly ConcurrentQueue<T> _items = new ConcurrentQueue<T>();

        private int _count;

        /// <summary>
        /// 使用指定的策略初始化 <see cref="Pool{T}"/> 类的新实例。
        /// </summary>
        /// <param name="policy">要使用的策略。</param>
        /// <exception cref="ArgumentNullException"><paramref name="policy"/> 为 <see langword="null"/>。</exception>
        public Pool(IPooledObjectPolicy<T> policy) : this(policy, System.Environment.ProcessorCount * 2)
        {
        }

        /// <summary>
        /// 使用指定的策略和指定的最大缓存容量初始化 <see cref="Pool{T}"/> 类的新实例。
        /// </summary>
        /// <param name="policy">要使用的策略。</param>
        /// <param name="maximumRetained">缓存对象的最大数量。</param>
        /// <exception cref="ArgumentNullException"><paramref name="policy"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maximumRetained"/> 小于 1。</exception>
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
            // 缓存目标接口方法，避免接口查找开销
            _createFunc    = policy.Create;
            _getAction     = policy.Get;
            _returnFunc    = policy.Return;
            _disposeAction = policy.Dispose;
            _capacity      = maximumRetained - 1; // -1 是因为 _fastItem 也算一个缓存对象
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
                    Interlocked.Decrement(ref _count); // 撤销上面的递增操作
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
