using System;
using Aurora.Pooling;

namespace Aurora
{
    /// <summary>
    /// A thread-safe <see cref="Random"/>.
    /// </summary>
    public sealed class ThreadSafeRandom : Random
    {
        internal static ThreadSafeRandom Instance { get; } = new();

        private readonly object _lock = new();

        private ThreadSafeRandom()
        {
        }

        /// <inheritdoc />
        protected override double Sample()
        {
            lock (_lock)
            {
                return base.Sample();
            }
        }

        /// <inheritdoc />
        public override int Next()
        {
            lock (_lock)
            {
                return base.Next();
            }
        }

        /// <inheritdoc />
        public override int Next(int minValue, int maxValue)
        {
            lock (_lock)
            {
                return base.Next(minValue, maxValue);
            }
        }

        /// <inheritdoc />
        public override int Next(int maxValue)
        {
            lock (_lock)
            {
                return base.Next(maxValue);
            }
        }

        /// <inheritdoc />
        public override double NextDouble()
        {
            lock (_lock)
            {
                return base.NextDouble();
            }
        }

        /// <summary>
        /// Returns a random floating-point number greater than or equal to 0 and less than or equal to 1.
        /// </summary>
        /// <returns>A random floating-point number greater than or equal to 0 and less than or equal to 1.</returns>
        /// <remarks>Unlike <see cref="Random.NextDouble"/>, the random number returned by this method may equal 1.</remarks>
        public double NextDoubleIncludingOne()
        {
            lock (_lock)
            {
                var array = PredefinedPools<byte>.ArrayLength4.Get();
                try
                {
                    base.NextBytes(array);
                    var uintValue = BitConverter.ToUInt32(array, 0);
                    return uintValue / (double)uint.MaxValue;
                }
                finally
                {
                    PredefinedPools<byte>.ArrayLength4.Return(array);
                }
            }
        }

        /// <inheritdoc />
        public override void NextBytes(byte[] buffer)
        {
            lock (_lock)
            {
                base.NextBytes(buffer);
            }
        }
    }
}
