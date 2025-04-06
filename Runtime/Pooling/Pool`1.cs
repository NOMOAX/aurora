using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 对象池。
    /// </summary>
    /// <typeparam name="T">对象池中成员的类型。</typeparam>
    [DebuggerTypeProxy(typeof(PoolDebugView<>))]
    public sealed class Pool<T> : IPool<T> where T : class
    {
        private readonly Func<T> _funcCreate;

        private readonly Action<T> _actionOnGet;

        private readonly Func<T, bool> _funcReturn;

        private readonly Action<T> _actionDispose;

        private readonly int _capacity;

        private int _count;

        private readonly ConcurrentQueue<T> _items = new ConcurrentQueue<T>();

        private T _fastItem;

        private volatile bool _isDisposed;

        /// <summary>
        /// 使用指定的策略初始化 <see cref="Pool{T}"/> 类的新实例。
        /// </summary>
        /// <param name="policy">要使用的策略。</param>
        /// <exception cref="ArgumentNullException"><paramref name="policy"/> 为 <see langword="null"/>。</exception>
        public Pool(IPooledObjectPolicy<T> policy) : this(policy, Environment.ProcessorCount * 2)
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
            // 缓存这些方法，避免接口查找开销
            _funcCreate    = policy.Create;
            _actionOnGet   = policy.OnGet;
            _funcReturn    = policy.Return;
            _actionDispose = policy.Dispose;
            _capacity      = maximumRetained - 1; // -1 是因为 _fastItem 也算一个缓存对象
        }

        internal T[] GetItems()
        {
            var items    = _items.ToArray();
            var fastItem = _fastItem;
            if (fastItem == null)
            {
                return items;
            }
            var length            = items.Length;
            var itemsWithFastItem = new T[length + 1];
            Array.Copy(items, 0, itemsWithFastItem, 1, length);
            itemsWithFastItem[0] = fastItem;
            return itemsWithFastItem;
        }

        /// <inheritdoc />
        public bool IsEmpty => _fastItem == null && _items.IsEmpty;

        /// <inheritdoc />
        public T Get()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(typeof(Pool<T>).FullName);
            }
            // 尝试将 _fastItem 取出
            var fastItem = _fastItem;
            if (fastItem != null && Interlocked.CompareExchange(ref _fastItem, null, fastItem) == fastItem)
            {
                _actionOnGet(fastItem);
                return fastItem;
            }
            // 尝试从 _items 中取出
            if (_items.TryDequeue(out var item))
            {
                Interlocked.Decrement(ref _count);
            }
            // 从 _items 中取出失败，所以创建一个
            else
            {
                item = _funcCreate();
            }
            _actionOnGet(item);
            return item;
        }

        /// <inheritdoc />
        public void Return(T obj)
        {
            if (_isDisposed || !_funcReturn(obj))
            {
                _actionDispose(obj);
                return;
            }
            // 尝试放入到 _fastItem
            if (_fastItem == null && Interlocked.CompareExchange(ref _fastItem, obj, null) == null)
            {
                return;
            }
            // 递增 _count 的值，然后判断它是否小于 _capacity，如果成功则可以放入到 _items 中
            if (Interlocked.Increment(ref _count) <= _capacity)
            {
                _items.Enqueue(obj);
                return;
            }
            // 失败了，所以递减 _count 的值（撤销上面的递增操作），然后释放 obj
            Interlocked.Decrement(ref _count);
            _actionDispose(obj);
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (_isDisposed)
            {
                return;
            }
            InternalClear();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            InternalClear();
        }

        private void InternalClear()
        {
            var fastItem = _fastItem;
            // 尝试释放 _fastItem
            if (fastItem != null && Interlocked.CompareExchange(ref _fastItem, null, fastItem) == fastItem)
            {
                _actionDispose(fastItem);
            }
            while (_items.TryDequeue(out var item))
            {
                Interlocked.Decrement(ref _count);
                _actionDispose(item);
            }
        }
    }
}
