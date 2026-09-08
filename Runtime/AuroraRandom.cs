using System;

namespace Aurora
{
    /// <summary>
    /// Extends <see cref="Random"/> with additional methods for random number generation.
    /// </summary>
    public sealed class AuroraRandom : Random
    {
        [ThreadStatic]
        private static AuroraRandom _instance;

        /// <summary>
        /// Gets the <see cref="AuroraRandom"/> instance for the current thread.
        /// </summary>
        /// <remarks>The instance is unique for each thread.</remarks>
        public static AuroraRandom Instance => _instance ??= new AuroraRandom();

        /// <summary>
        /// Returns a random floating-point number greater than or equal to 0 and less than or equal to 1.
        /// </summary>
        /// <returns>A random floating-point number greater than or equal to 0 and less than or equal to 1.</returns>
        /// <remarks>The probability of returning 1 is 2^-53, and the probability of returning any other specific value (0 or a non-endpoint grid point) is 2^-53 - 2^-106 (almost the same as the probability of returning 1).</remarks>
        public double NextDoubleIncludingOne()
        {
            return NextDouble() == 0 ? 1 : NextDouble();
        }

#if !NET6_0_OR_GREATER
        /// <summary>
        /// Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.
        /// </summary>
        /// <returns>A single-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
        /// <remarks>The actual upper bound of the random number returned by this method is 0.99999994.</remarks>
        public float NextSingle()
        {
            return (Next() & 0xFFFFFF) * (1f / (1 << 24));
        }
#endif

        /// <summary>
        /// Returns a random floating-point number greater than or equal to 0 and less than or equal to 1.
        /// </summary>
        /// <returns>A random floating-point number greater than or equal to 0 and less than or equal to 1.</returns>
        /// <remarks>The probability of returning 1 is 2^-24, and the probability of returning any other specific value (0 or a non-endpoint grid point) is 2^-24 - 2^-48 (almost the same as the probability of returning 1).</remarks>
        public float NextSingleIncludingOne()
        {
            return NextSingle() == 0 ? 1 : NextSingle();
        }
    }
}
