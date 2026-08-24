using System;
using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// Provides a set of utility methods related to hash codes.
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <returns>The combined result of two hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2)
        {
            return (h1 << 5 | BitUtility.UnsignedRightShift(h1, 27)) + h1 ^ h2;
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <returns>The combined result of three hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <returns>The combined result of four hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3), h4);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <returns>The combined result of five hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <returns>The combined result of six hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5), h6);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <returns>The combined result of seven hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6), h7);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <returns>The combined result of eight hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7), h8);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <returns>The combined result of nine hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8, int h9)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8), h9);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <param name="h10">The tenth hash code involved in the combination.</param>
        /// <returns>The combined result of ten hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8,
            int h9,
            int h10)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8, h9), h10);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <param name="h10">The tenth hash code involved in the combination.</param>
        /// <param name="h11">The eleventh hash code involved in the combination.</param>
        /// <returns>The combined result of eleven hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8,
            int h9,
            int h10,
            int h11)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8, h9, h10), h11);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <param name="h10">The tenth hash code involved in the combination.</param>
        /// <param name="h11">The eleventh hash code involved in the combination.</param>
        /// <param name="h12">The twelfth hash code involved in the combination.</param>
        /// <returns>The combined result of twelve hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8,
            int h9,
            int h10,
            int h11,
            int h12)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11), h12);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <param name="h10">The tenth hash code involved in the combination.</param>
        /// <param name="h11">The eleventh hash code involved in the combination.</param>
        /// <param name="h12">The twelfth hash code involved in the combination.</param>
        /// <param name="h13">The thirteenth hash code involved in the combination.</param>
        /// <returns>The combined result of thirteen hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8,
            int h9,
            int h10,
            int h11,
            int h12,
            int h13)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11, h12), h13);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <param name="h10">The tenth hash code involved in the combination.</param>
        /// <param name="h11">The eleventh hash code involved in the combination.</param>
        /// <param name="h12">The twelfth hash code involved in the combination.</param>
        /// <param name="h13">The thirteenth hash code involved in the combination.</param>
        /// <param name="h14">The fourteenth hash code involved in the combination.</param>
        /// <returns>The combined result of fourteen hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8,
            int h9,
            int h10,
            int h11,
            int h12,
            int h13,
            int h14)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11, h12, h13), h14);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <param name="h10">The tenth hash code involved in the combination.</param>
        /// <param name="h11">The eleventh hash code involved in the combination.</param>
        /// <param name="h12">The twelfth hash code involved in the combination.</param>
        /// <param name="h13">The thirteenth hash code involved in the combination.</param>
        /// <param name="h14">The fourteenth hash code involved in the combination.</param>
        /// <param name="h15">The fifteenth hash code involved in the combination.</param>
        /// <returns>The combined result of fifteen hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8,
            int h9,
            int h10,
            int h11,
            int h12,
            int h13,
            int h14,
            int h15)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11, h12, h13, h14), h15);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The first hash code involved in the combination.</param>
        /// <param name="h2">The second hash code involved in the combination.</param>
        /// <param name="h3">The third hash code involved in the combination.</param>
        /// <param name="h4">The fourth hash code involved in the combination.</param>
        /// <param name="h5">The fifth hash code involved in the combination.</param>
        /// <param name="h6">The sixth hash code involved in the combination.</param>
        /// <param name="h7">The seventh hash code involved in the combination.</param>
        /// <param name="h8">The eighth hash code involved in the combination.</param>
        /// <param name="h9">The ninth hash code involved in the combination.</param>
        /// <param name="h10">The tenth hash code involved in the combination.</param>
        /// <param name="h11">The eleventh hash code involved in the combination.</param>
        /// <param name="h12">The twelfth hash code involved in the combination.</param>
        /// <param name="h13">The thirteenth hash code involved in the combination.</param>
        /// <param name="h14">The fourteenth hash code involved in the combination.</param>
        /// <param name="h15">The fifteenth hash code involved in the combination.</param>
        /// <param name="h16">The sixteenth hash code involved in the combination.</param>
        /// <returns>The combined result of sixteen hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(
            int h1,
            int h2,
            int h3,
            int h4,
            int h5,
            int h6,
            int h7,
            int h8,
            int h9,
            int h10,
            int h11,
            int h12,
            int h13,
            int h14,
            int h15,
            int h16)
        {
            return CombineHashCodes(
                CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11, h12, h13, h14, h15),
                h16
            );
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="hashCodes">A variable-length array containing multiple hash codes.</param>
        /// <returns>The combined result of multiple hash codes. In particular, if <paramref name="hashCodes"/> is an empty array, it is 0.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hashCodes"/> is <see langword="null"/>.</exception>
        public static int CombineHashCodes(params int[] hashCodes)
        {
            if (hashCodes == null)
            {
                throw new ArgumentNullException(nameof(hashCodes));
            }
            var length = hashCodes.Length;
            switch (length)
            {
                case 0:
                    return 0;
                case 1:
                    return hashCodes[0];
                default:
                    var hashCode = CombineHashCodes(hashCodes[0], hashCodes[1]);
                    for (var i = 2; i < length; i++)
                    {
                        hashCode = CombineHashCodes(hashCode, hashCodes[i]);
                    }
                    return hashCode;
            }
        }
    }
}
