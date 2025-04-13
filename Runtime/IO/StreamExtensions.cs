using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Pooling;

namespace Aurora.IO
{
    /// <summary>
    /// 为 <see cref="Stream"/> 类提供扩展方法。
    /// </summary>
    public static class StreamExtensions
    {
        /// <seealso cref="Stream.CopyTo(System.IO.Stream)"/>
        /// <remarks>此方法将重用缓冲区，以帮助减少频繁分配内存的行为。</remarks>
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

        /// <seealso cref="Stream.CopyToAsync(System.IO.Stream)"/>
        /// <remarks>此方法将重用缓冲区，以帮助减少频繁分配内存的行为。</remarks>
        public static Task CopyToFrugallyAsync(this Stream stream, Stream destination)
        {
            return InternalCopyToFrugallyAsync(stream, destination, CancellationToken.None);
        }

        /// <seealso cref="Stream.CopyToAsync(System.IO.Stream)"/>
        /// <remarks>此方法将重用缓冲区，以帮助减少频繁分配内存的行为。</remarks>
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
