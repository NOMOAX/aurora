using System;

namespace Aurora
{
    /// <summary>
    /// Explicitly represents an empty result.
    /// </summary>
    public readonly struct VoidResult : IEquatable<VoidResult>
    {
        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="VoidResult"/>.
        /// </summary>
        /// <param name="obj">The object to compare with the current <see cref="VoidResult"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="VoidResult"/>; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            return obj is VoidResult;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current <see cref="VoidResult"/>.</returns>
        public override int GetHashCode()
        {
            return 0;
        }

        /// <summary>
        /// Determines whether the specified <see cref="VoidResult"/> is equal to the current <see cref="VoidResult"/>.
        /// </summary>
        /// <param name="other">The <see cref="VoidResult"/> to compare with the current <see cref="VoidResult"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="other"/> is a <see cref="VoidResult"/>; otherwise, <see langword="false"/>.</returns>
        public bool Equals(VoidResult other)
        {
            return true;
        }
    }
}
