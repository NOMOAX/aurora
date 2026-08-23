using System;

namespace Aurora
{
    /// <summary>
    /// Provides a set of methods that directly operate on memory.
    /// </summary>
    public static class Memory
    {
        /// <summary>
        /// Copies a specified number of bytes from one address in memory to another. If the two memory regions overlap, the result may be unexpected; use <see cref="Move"/> instead.
        /// </summary>
        /// <param name="destination">A pointer to the memory that stores the copied content.</param>
        /// <param name="source">A pointer to the source of data to copy in memory.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
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
            CopyImpl((byte*)destination, (byte*)source, length);
        }

        private static unsafe void CopyImpl(byte* destination, byte* source, ulong length)
        {
            while (length-- > 0UL)
            {
                *destination++ = *source++;
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from one address in memory to another.
        /// </summary>
        /// <param name="destination">A pointer to the memory that stores the copied content.</param>
        /// <param name="source">A pointer to the source of data to copy in memory.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
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
            MoveImpl((byte*)destination, (byte*)source, length);
        }

        private static unsafe void MoveImpl(byte* destination, byte* source, ulong length)
        {
            if ((ulong)destination <= (ulong)source || (ulong)destination >= (ulong)source + length)
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
        /// Sets every byte in a region of memory to the specified 8-bit unsigned integer.
        /// </summary>
        /// <param name="destination">A pointer to the memory to set values on.</param>
        /// <param name="value">The 8-bit unsigned integer to set on every byte.</param>
        /// <param name="length">The number of bytes to set.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
        public static unsafe void Set(void* destination, byte value, ulong length)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            SetImpl((byte*)destination, value, length);
        }

        private static unsafe void SetImpl(byte* destination, byte value, ulong length)
        {
            while (length-- > 0UL)
            {
                *destination++ = value;
            }
        }

        /// <summary>
        /// Clears a region of memory.
        /// </summary>
        /// <param name="destination">A pointer to the memory to be cleared.</param>
        /// <param name="length">The number of bytes to clear.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="destination"/> is <see langword="null"/>.</exception>
        public static unsafe void Clear(void* destination, ulong length)
        {
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            SetImpl((byte*)destination, 0, length);
        }

        /// <summary>
        /// Compares two regions of memory.
        /// </summary>
        /// <param name="pointer1">A pointer to the first region of memory.</param>
        /// <param name="pointer2">A pointer to the second region of memory.</param>
        /// <param name="length">The number of bytes to compare.</param>
        /// <returns>Compares each byte of the two memory regions in order; if they are not equal, returns the difference; if they are equal, continues to the next byte; if the last byte pair is still equal, returns 0.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="pointer1"/> or <paramref name="pointer2"/> is <see langword="null"/>.</exception>
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
            return CompareImpl((byte*)pointer1, (byte*)pointer2, length);
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
