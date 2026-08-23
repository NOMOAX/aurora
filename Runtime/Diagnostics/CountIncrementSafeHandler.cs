using System;
using System.Threading;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// Used to keep the execution count within a reasonable range when executing loops or recursions whose number of executions cannot be accurately predicted.
    /// </summary>
    public sealed class CountIncrementSafeHandler
    {
        private readonly int _maxCount;

        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountIncrementSafeHandler"/> class.
        /// </summary>
        /// <param name="maxCount">The maximum execution count.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxCount"/> is less than 0.</exception>
        public CountIncrementSafeHandler(int maxCount)
        {
            if (maxCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }
            _maxCount = maxCount;
        }

        /// <summary>
        /// Increments the execution count and throws an exception if it exceeds the maximum execution count.
        /// </summary>
        /// <exception cref="UnexpectedException">The incremented execution count is greater than the maximum execution count.</exception>
        public void Increment()
        {
            if (Interlocked.Increment(ref _count) is var count && (count > _maxCount || count < 0))
            {
                throw new UnexpectedException();
            }
        }
    }
}
