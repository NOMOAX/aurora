using System.Collections;

namespace Aurora.Collections
{
    /// <summary>
    /// Implements <see cref="IEnumerator"/> using the null-object pattern.
    /// </summary>
    public sealed class NullEnumerator : IEnumerator
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NullEnumerator Instance { get; } = new();

        private NullEnumerator()
        {
        }

        bool IEnumerator.MoveNext()
        {
            return false;
        }

        object IEnumerator.Current => null;

        void IEnumerator.Reset()
        {
        }
    }
}
