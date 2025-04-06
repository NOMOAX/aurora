using System.Diagnostics;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的秒表的策略。
    /// </summary>
    public class PooledStopwatchPolicy : IPooledObjectPolicy<Stopwatch>
    {
        /// <inheritdoc />
        public Stopwatch Create()
        {
            return new Stopwatch();
        }

        /// <inheritdoc />
        public void OnGet(Stopwatch obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Stopwatch obj)
        {
            if (obj == null)
            {
                return false;
            }
            obj.Reset();
            return true;
        }

        /// <inheritdoc />
        public void Dispose(Stopwatch obj)
        {
            obj?.Reset();
        }
    }
}
