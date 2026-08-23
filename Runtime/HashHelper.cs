using System;
using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// 提供一组与哈希值有关的工具方法。
    /// </summary>
    public static class HashHelper
    {
        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <returns>两个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2)
        {
            return (h1 << 5 | BitUtility.UnsignedRightShift(h1, 27)) + h1 ^ h2;
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <returns>三个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <returns>四个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3), h4);
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <returns>五个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <returns>六个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5), h6);
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <returns>七个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6), h7);
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <returns>八个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7), h8);
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <returns>九个哈希值的合并结果。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8, int h9)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7, h8), h9);
        }

        /// <summary>
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <param name="h10">参与合并的第十个哈希值。</param>
        /// <returns>十个哈希值的合并结果。</returns>
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
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <param name="h10">参与合并的第十个哈希值。</param>
        /// <param name="h11">参与合并的第十一个哈希值。</param>
        /// <returns>十一个哈希值的合并结果。</returns>
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
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <param name="h10">参与合并的第十个哈希值。</param>
        /// <param name="h11">参与合并的第十一个哈希值。</param>
        /// <param name="h12">参与合并的第十二个哈希值。</param>
        /// <returns>十二个哈希值的合并结果。</returns>
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
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <param name="h10">参与合并的第十个哈希值。</param>
        /// <param name="h11">参与合并的第十一个哈希值。</param>
        /// <param name="h12">参与合并的第十二个哈希值。</param>
        /// <param name="h13">参与合并的第十三个哈希值。</param>
        /// <returns>十三个哈希值的合并结果。</returns>
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
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <param name="h10">参与合并的第十个哈希值。</param>
        /// <param name="h11">参与合并的第十一个哈希值。</param>
        /// <param name="h12">参与合并的第十二个哈希值。</param>
        /// <param name="h13">参与合并的第十三个哈希值。</param>
        /// <param name="h14">参与合并的第十四个哈希值。</param>
        /// <returns>十四个哈希值的合并结果。</returns>
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
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <param name="h10">参与合并的第十个哈希值。</param>
        /// <param name="h11">参与合并的第十一个哈希值。</param>
        /// <param name="h12">参与合并的第十二个哈希值。</param>
        /// <param name="h13">参与合并的第十三个哈希值。</param>
        /// <param name="h14">参与合并的第十四个哈希值。</param>
        /// <param name="h15">参与合并的第十五个哈希值。</param>
        /// <returns>十五个哈希值的合并结果。</returns>
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
        /// 合并哈希值。
        /// </summary>
        /// <param name="h1">参与合并的第一个哈希值。</param>
        /// <param name="h2">参与合并的第二个哈希值。</param>
        /// <param name="h3">参与合并的第三个哈希值。</param>
        /// <param name="h4">参与合并的第四个哈希值。</param>
        /// <param name="h5">参与合并的第五个哈希值。</param>
        /// <param name="h6">参与合并的第六个哈希值。</param>
        /// <param name="h7">参与合并的第七个哈希值。</param>
        /// <param name="h8">参与合并的第八个哈希值。</param>
        /// <param name="h9">参与合并的第九个哈希值。</param>
        /// <param name="h10">参与合并的第十个哈希值。</param>
        /// <param name="h11">参与合并的第十一个哈希值。</param>
        /// <param name="h12">参与合并的第十二个哈希值。</param>
        /// <param name="h13">参与合并的第十三个哈希值。</param>
        /// <param name="h14">参与合并的第十四个哈希值。</param>
        /// <param name="h15">参与合并的第十五个哈希值。</param>
        /// <param name="h16">参与合并的第十六个哈希值。</param>
        /// <returns>十六个哈希值的合并结果。</returns>
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
        /// 合并哈希值。
        /// </summary>
        /// <param name="hashCodes">含有多个哈希值的可变长度的数组。</param>
        /// <returns>多个哈希值的合并结果。特别地，如果 <paramref name="hashCodes"/> 为空数组，则为 0。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="hashCodes"/> 为 <see langword="null"/>。</exception>
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
