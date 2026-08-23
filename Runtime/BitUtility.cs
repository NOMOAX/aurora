using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// 提供一组位运算方法。
    /// </summary>
    public static class BitUtility
    {
        /// <summary>
        /// 执行无符号右移运算。
        /// </summary>
        /// <param name="value">要执行无符号右移运算的值。</param>
        /// <param name="shiftCount">移动的位数。</param>
        /// <returns>无符号右移运算的结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int UnsignedRightShift(int value, int shiftCount)
        {
            return (int)InternalUnsignedRightShift((uint)value, shiftCount);
        }

        /// <summary>
        /// 执行无符号右移运算。
        /// </summary>
        /// <param name="value">要执行无符号右移运算的值。</param>
        /// <param name="shiftCount">移动的位数。</param>
        /// <returns>无符号右移运算的结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint UnsignedRightShift(uint value, int shiftCount)
        {
            return InternalUnsignedRightShift(value, shiftCount);
        }

        /// <summary>
        /// 执行无符号右移运算。
        /// </summary>
        /// <param name="value">要执行无符号右移运算的值。</param>
        /// <param name="shiftCount">移动的位数。</param>
        /// <returns>无符号右移运算的结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long UnsignedRightShift(long value, int shiftCount)
        {
            return (long)InternalUnsignedRightShift((ulong)value, shiftCount);
        }

        /// <summary>
        /// 执行无符号右移运算。
        /// </summary>
        /// <param name="value">要执行无符号右移运算的值。</param>
        /// <param name="shiftCount">移动的位数。</param>
        /// <returns>无符号右移运算的结果。</returns>
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
