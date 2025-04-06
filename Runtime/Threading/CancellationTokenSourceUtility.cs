using System;
using System.Threading;

namespace Aurora.Threading
{
    /// <summary>
    /// 为 <see cref="CancellationTokenSource"/> 提供工具方法。
    /// </summary>
    public static class CancellationTokenSourceUtility
    {
        /// <summary>
        /// 判断指定的 <see cref="CancellationTokenSource"/> 是否已释放。
        /// </summary>
        /// <param name="cancellationTokenSource">取消令牌源。</param>
        /// <returns>如果 <paramref name="cancellationTokenSource"/> 已释放，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="cancellationTokenSource"/> 为 <see langword="null"/>。</exception>
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
