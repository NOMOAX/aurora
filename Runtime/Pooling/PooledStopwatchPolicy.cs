using System.Diagnostics;

namespace Aurora.Pooling
{
    /// <summary>
    /// A strategy for managing stopwatches in the pool.
    /// </summary>
    public class PooledStopwatchPolicy : IPooledObjectPolicy<Stopwatch>
    {
        /// <inheritdoc />
        public Stopwatch Create()
        {
            return new Stopwatch();
        }

        /// <inheritdoc />
        public void Get(Stopwatch obj)
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
