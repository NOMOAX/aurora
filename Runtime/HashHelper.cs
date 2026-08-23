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
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <returns>The combined result of 2 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2)
        {
            return (h1 << 5 | BitUtility.UnsignedRightShift(h1, 27)) + h1 ^ h2;
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <returns>The combined result of 3 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <returns>The combined result of 4 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3), h4);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <returns>The combined result of 5 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <returns>The combined result of 6 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5), h6);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <returns>The combined result of 7 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6), h7);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <returns>The combined result of 8 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7), h8);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <returns>The combined result of 9 hash codes.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8, int h9)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8), h9);
        }

        /// <summary>
        /// Combines hash codes.
        /// </summary>
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <param name="h10">The 10th hash code involved in the combination.</param>
        /// <returns>The combined result of 10 hash codes.</returns>
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
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <param name="h10">The 10th hash code involved in the combination.</param>
        /// <param name="h11">The 11th hash code involved in the combination.</param>
        /// <returns>The combined result of 11 hash codes.</returns>
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
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <param name="h10">The 10th hash code involved in the combination.</param>
        /// <param name="h11">The 11th hash code involved in the combination.</param>
        /// <param name="h12">The 12th hash code involved in the combination.</param>
        /// <returns>The combined result of 12 hash codes.</returns>
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
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <param name="h10">The 10th hash code involved in the combination.</param>
        /// <param name="h11">The 11th hash code involved in the combination.</param>
        /// <param name="h12">The 12th hash code involved in the combination.</param>
        /// <param name="h13">The 13th hash code involved in the combination.</param>
        /// <returns>The combined result of 13 hash codes.</returns>
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
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <param name="h10">The 10th hash code involved in the combination.</param>
        /// <param name="h11">The 11th hash code involved in the combination.</param>
        /// <param name="h12">The 12th hash code involved in the combination.</param>
        /// <param name="h13">The 13th hash code involved in the combination.</param>
        /// <param name="h14">The 14th hash code involved in the combination.</param>
        /// <returns>The combined result of 14 hash codes.</returns>
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
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <param name="h10">The 10th hash code involved in the combination.</param>
        /// <param name="h11">The 11th hash code involved in the combination.</param>
        /// <param name="h12">The 12th hash code involved in the combination.</param>
        /// <param name="h13">The 13th hash code involved in the combination.</param>
        /// <param name="h14">The 14th hash code involved in the combination.</param>
        /// <param name="h15">The 15th hash code involved in the combination.</param>
        /// <returns>The combined result of 15 hash codes.</returns>
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
        /// <param name="h1">The 1st hash code involved in the combination.</param>
        /// <param name="h2">The 2nd hash code involved in the combination.</param>
        /// <param name="h3">The 3rd hash code involved in the combination.</param>
        /// <param name="h4">The 4th hash code involved in the combination.</param>
        /// <param name="h5">The 5th hash code involved in the combination.</param>
        /// <param name="h6">The 6th hash code involved in the combination.</param>
        /// <param name="h7">The 7th hash code involved in the combination.</param>
        /// <param name="h8">The 8th hash code involved in the combination.</param>
        /// <param name="h9">The 9th hash code involved in the combination.</param>
        /// <param name="h10">The 10th hash code involved in the combination.</param>
        /// <param name="h11">The 11th hash code involved in the combination.</param>
        /// <param name="h12">The 12th hash code involved in the combination.</param>
        /// <param name="h13">The 13th hash code involved in the combination.</param>
        /// <param name="h14">The 14th hash code involved in the combination.</param>
        /// <param name="h15">The 15th hash code involved in the combination.</param>
        /// <param name="h16">The 16th hash code involved in the combination.</param>
        /// <returns>The combined result of 16 hash codes.</returns>
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
