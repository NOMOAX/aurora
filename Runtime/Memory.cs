using System;

namespace Aurora
{
    /// <summary>
    /// 提供一组直接操作内存的方法。
    /// </summary>
    public static class Memory
    {
        /// <summary>
        /// 将指定长度的一些字节从内存中的一个地址复制到另一个地址。如果两段内存有重叠部分，结果可能会不符合预期，请改用 <see cref="Move"/>。
        /// </summary>
        /// <param name="destination">指向用于存储复制内容的内存的指针。</param>
        /// <param name="source">指向内存上要复制的数据源的指针。</param>
        /// <param name="length">要复制的字节数。</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> 或 <paramref name="destination"/> 为 <see langword="null"/>。</exception>
        public static unsafe void Copy(void* destination, void* source, ulong length)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (length == 0UL)
            {
                return;
            }
            CopyImpl((byte*) destination, (byte*) source, length);
        }

        private static unsafe void CopyImpl(byte* destination, byte* source, ulong length)
        {
            while (length-- > 0UL)
            {
                *destination++ = *source++;
            }
        }

        /// <summary>
        /// 将指定长度的一些字节从内存中的一个地址复制到另一个地址。
        /// </summary>
        /// <param name="destination">指向用于存储复制内容的内存的指针。</param>
        /// <param name="source">指向内存上要复制的数据源的指针。</param>
        /// <param name="length">要复制的字节数。</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> 或 <paramref name="destination"/> 为 <see langword="null"/>。</exception>
        public static unsafe void Move(void* destination, void* source, ulong length)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (length == 0UL)
            {
                return;
            }
            MoveImpl((byte*) destination, (byte*) source, length);
        }

        private static unsafe void MoveImpl(byte* destination, byte* source, ulong length)
        {
            if ((ulong) destination <= (ulong) source || (ulong) destination >= (ulong) source + length)
            {
                CopyImpl(destination, source, length);
            }
            else
            {
                source      += length - 1;
                destination += length - 1;
                while (length-- > 0UL)
                {
                    *destination-- = *source--;
                }
            }
        }

        /// <summary>
        /// 将某一段内存上的每个字节设置为指定的 8 位无符号整数。
        /// </summary>
        /// <param name="destination">指向要设置值的内存的指针。</param>
        /// <param name="value">要给每个字节设置的 8 位无符号整数。</param>
        /// <param name="length">要设置的字节数。</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="destination"/> 为 <see langword="null"/>。</exception>
        public static unsafe void Set(void* destination, byte value, ulong length)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            SetImpl((byte*) destination, value, length);
        }

        private static unsafe void SetImpl(byte* destination, byte value, ulong length)
        {
            while (length-- > 0UL)
            {
                *destination++ = value;
            }
        }

        /// <summary>
        /// 将某一段内存清空。
        /// </summary>
        /// <param name="destination">指向要被清空的内存的指针。</param>
        /// <param name="length">要清空的字节数。</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="destination"/> 为 <see langword="null"/>。</exception>
        public static unsafe void Clear(void* destination, ulong length)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            SetImpl((byte*) destination, 0, length);
        }

        /// <summary>
        /// 比较两段内存。
        /// </summary>
        /// <param name="pointer1">指向第一段内存的指针。</param>
        /// <param name="pointer2">指向第二段内存的指针。</param>
        /// <param name="length">要比较的字节数。</param>
        /// <returns>依次比较两段内存的每个字节，若不相等则返回差值，若相等则继续比较下一组字节；若比较到最后一组字节时他们依然相等，则返回 0。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pointer1"/> 或 <paramref name="pointer2"/> 为 <see langword="null"/>。</exception>
        public static unsafe int Compare(void* pointer1, void* pointer2, ulong length)
        {
            if (pointer1 == null)
            {
                throw new ArgumentNullException(nameof(pointer1));
            }
            if (pointer2 == null)
            {
                throw new ArgumentNullException(nameof(pointer2));
            }
            return CompareImpl((byte*) pointer1, (byte*) pointer2, length);
        }

        private static unsafe int CompareImpl(byte* pointer1, byte* pointer2, ulong length)
        {
            while (length-- > 0UL)
            {
                var num = *pointer1++ - *pointer2++;
                if (num == 0)
                {
                    continue;
                }
                return num;
            }
            return 0;
        }
    }
}
