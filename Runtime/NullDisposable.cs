using System;

namespace Aurora
{
    /// <summary>
    /// Implements <see cref="IDisposable"/> using the null-object pattern.
    /// </summary>
    public sealed class NullDisposable : IDisposable
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NullDisposable Instance { get; } = new();

        private NullDisposable()
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
