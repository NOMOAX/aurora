using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用队列范围。
    /// </summary>
    /// <typeparam name="T">队列的成员的类型。</typeparam>
    public sealed class QueueUsingScope<T> : IDisposable
    {
        private Queue<T> _queue;

        /// <summary>
        /// 初始化 <see cref="QueueUsingScope{T}"/> 类的新实例。
        /// </summary>
        /// <param name="queue">此输出参数将被赋值为一个空队列。</param>
        public QueueUsingScope(out Queue<T> queue)
        {
            _queue = PredefinedPools<T>.Queue.Get();
            queue  = _queue;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var queue = _queue;
            if (queue != null && Interlocked.CompareExchange(ref _queue, null, queue) == queue)
            {
                PredefinedPools<T>.Queue.Return(queue);
            }
        }
    }
}
