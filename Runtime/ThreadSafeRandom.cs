using System;
using Aurora.Pooling;

namespace Aurora
{
    /// <summary>
    /// 线程安全的 <see cref="Random"/>。
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
        /// 返回一个大于等于 0 并且小于等于 1 的随机浮点数。
        /// </summary>
        /// <returns>一个大于等于 0 并且小于等于 1 的随机浮点数。</returns>
        /// <remarks>与 <see cref="Random.NextDouble"/> 的行为不同，此方法返回的随机数可能等于 1。</remarks>
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
