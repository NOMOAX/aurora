using System;

namespace Aurora
{
    internal sealed class ThreadSafeRandom : Random
    {
        internal static ThreadSafeRandom Instance { get; } = new ThreadSafeRandom();

        private readonly object _lock = new object();

        private ThreadSafeRandom()
        {
        }

        protected override double Sample()
        {
            lock (_lock)
            {
                return base.Sample();
            }
        }

        public override int Next()
        {
            lock (_lock)
            {
                return base.Next();
            }
        }

        public override int Next(int minValue, int maxValue)
        {
            lock (_lock)
            {
                return base.Next(minValue, maxValue);
            }
        }

        public override int Next(int maxValue)
        {
            lock (_lock)
            {
                return base.Next(maxValue);
            }
        }

        public override double NextDouble()
        {
            lock (_lock)
            {
                return base.NextDouble();
            }
        }

        public override void NextBytes(byte[] buffer)
        {
            lock (_lock)
            {
                base.NextBytes(buffer);
            }
        }
    }
}
