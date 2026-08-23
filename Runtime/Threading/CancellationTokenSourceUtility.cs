using System;
using System.Threading;

namespace Aurora.Threading
{
    /// <summary>
    /// Provides utility methods for the <see cref="CancellationTokenSource"/> class.
    /// </summary>
    public static class CancellationTokenSourceUtility
    {
        /// <summary>
        /// Determines whether the specified <see cref="CancellationTokenSource"/> has been disposed.
        /// </summary>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        /// <returns><see langword="true"/> if <paramref name="cancellationTokenSource"/> has been disposed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cancellationTokenSource"/> is <see langword="null"/>.</exception>
        public static bool IsDisposed(CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource == null)
            {
                throw new ArgumentNullException(nameof(cancellationTokenSource));
            }
            try
            {
                _ = cancellationTokenSource.Token;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }
    }
}
