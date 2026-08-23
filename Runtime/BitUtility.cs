using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// Provides a set of bit-operation methods.
    /// </summary>
    public static class BitUtility
    {
        /// <summary>
        /// Performs an unsigned right-shift operation.
        /// </summary>
        /// <param name="value">The value to shift right without sign extension.</param>
        /// <param name="shiftCount">The number of bits to shift.</param>
        /// <returns>The result of the unsigned right-shift operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnsignedRightShift(int value, int shiftCount)
        {
            return (int)InternalUnsignedRightShift((uint)value, shiftCount);
        }

        /// <summary>
        /// Performs an unsigned right-shift operation.
        /// </summary>
        /// <param name="value">The value to shift right without sign extension.</param>
        /// <param name="shiftCount">The number of bits to shift.</param>
        /// <returns>The result of the unsigned right-shift operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint UnsignedRightShift(uint value, int shiftCount)
        {
            return InternalUnsignedRightShift(value, shiftCount);
        }

        /// <summary>
        /// Performs an unsigned right-shift operation.
        /// </summary>
        /// <param name="value">The value to shift right without sign extension.</param>
        /// <param name="shiftCount">The number of bits to shift.</param>
        /// <returns>The result of the unsigned right-shift operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long UnsignedRightShift(long value, int shiftCount)
        {
            return (long)InternalUnsignedRightShift((ulong)value, shiftCount);
        }

        /// <summary>
        /// Performs an unsigned right-shift operation.
        /// </summary>
        /// <param name="value">The value to shift right without sign extension.</param>
        /// <param name="shiftCount">The number of bits to shift.</param>
        /// <returns>The result of the unsigned right-shift operation.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong UnsignedRightShift(ulong value, int shiftCount)
        {
            return InternalUnsignedRightShift(value, shiftCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint InternalUnsignedRightShift(uint value, int shiftCount)
        {
            return value >> shiftCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong InternalUnsignedRightShift(ulong value, int shiftCount)
        {
            return value >> shiftCount;
        }
    }
}
