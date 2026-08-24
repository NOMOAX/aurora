using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Pooling;

namespace Aurora.IO
{
    /// <summary>
    /// Provides extension methods for the <see cref="Stream"/> class.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads the bytes from the current stream and writes them to the destination stream.
        /// </summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">The current stream does not support reading, or <paramref name="destination"/> does not support writing.</exception>
        /// <exception cref="ObjectDisposedException">Either the current stream or <paramref name="destination"/> was closed before the copy operation was called.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <seealso cref="Stream.CopyTo(System.IO.Stream)"/>
        /// <remarks>This method reuses a buffer to help reduce frequent memory allocations.</remarks>
        public static void CopyToFrugally(this Stream stream, Stream destination)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (!stream.CanRead && !stream.CanWrite)
            {
                throw new ObjectDisposedException(stream.GetType().FullName);
            }
            if (!destination.CanRead && !destination.CanWrite)
            {
                throw new ObjectDisposedException(destination.GetType().FullName);
            }
            if (!stream.CanRead)
            {
                throw new NotSupportedException();
            }
            if (!destination.CanWrite)
            {
                throw new NotSupportedException();
            }
            var buffer = PredefinedPools.ByteArrayLength4096.Get();
            try
            {
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destination.Write(buffer, 0, count);
                }
            }
            finally
            {
                PredefinedPools.ByteArrayLength4096.Return(buffer);
            }
        }

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to the destination stream.
        /// </summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">The current stream does not support reading, or <paramref name="destination"/> does not support writing.</exception>
        /// <exception cref="ObjectDisposedException">Either the current stream or <paramref name="destination"/> was closed before the copy operation was called.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <seealso cref="Stream.CopyToAsync(System.IO.Stream)"/>
        /// <remarks>This method reuses a buffer to help reduce frequent memory allocations.</remarks>
        public static Task CopyToFrugallyAsync(this Stream stream, Stream destination)
        {
            return InternalCopyToFrugallyAsync(stream, destination, CancellationToken.None);
        }

        /// <summary>
        /// Asynchronously reads the bytes from the current stream and writes them to the destination stream, using the specified cancellation token to cancel the operation.
        /// </summary>
        /// <param name="stream">The stream from which to read.</param>
        /// <param name="destination">The stream to which the contents of the current stream will be copied.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous copy operation.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> or <paramref name="destination"/> is <see langword="null"/>.</exception>
        /// <exception cref="NotSupportedException">The current stream does not support reading, or <paramref name="destination"/> does not support writing.</exception>
        /// <exception cref="ObjectDisposedException">Either the current stream or <paramref name="destination"/> was closed before the copy operation was called.</exception>
        /// <exception cref="OperationCanceledException">The token was canceled.</exception>
        /// <exception cref="IOException">An I/O error occurred.</exception>
        /// <seealso cref="Stream.CopyToAsync(System.IO.Stream)"/>
        /// <remarks>This method reuses a buffer to help reduce frequent memory allocations.</remarks>
        public static Task CopyToFrugallyAsync(
            this Stream       stream,
            Stream            destination,
            CancellationToken cancellationToken)
        {
            return InternalCopyToFrugallyAsync(stream, destination, cancellationToken);
        }

        private static async Task InternalCopyToFrugallyAsync(
            Stream            stream,
            Stream            destination,
            CancellationToken cancellationToken)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }
            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }
            if (!stream.CanRead && !stream.CanWrite)
            {
                throw new ObjectDisposedException(stream.GetType().FullName);
            }
            if (!destination.CanRead && !destination.CanWrite)
            {
                throw new ObjectDisposedException(destination.GetType().FullName);
            }
            if (!stream.CanRead)
            {
                throw new NotSupportedException();
            }
            if (!destination.CanWrite)
            {
                throw new NotSupportedException();
            }
            var buffer = PredefinedPools.ByteArrayLength4096.Get();
            try
            {
                int count;
                while ((count = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)
                                            .ConfigureAwait(false)) > 0)
                {
                    await destination.WriteAsync(buffer, 0, count, cancellationToken).ConfigureAwait(false);
                }
            }
            finally
            {
                PredefinedPools.ByteArrayLength4096.Return(buffer);
            }
        }
    }
}
