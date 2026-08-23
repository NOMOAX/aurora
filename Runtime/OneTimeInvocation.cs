using System;
using System.Threading;

namespace Aurora
{
    /// <summary>
    /// 对另一个 <see cref="Invocation"/> 进行包装，确保仅第一次的对 <see cref="Invoke"/> 的调用会实际执行。
    /// </summary>
    public sealed class OneTimeInvocation : Invocation
    {
        private Invocation _invocation;

        /// <summary>
        /// 初始化 <see cref="OneTimeInvocation"/> 类的新实例。
        /// </summary>
        /// <param name="invocation">一个 <see cref="Invocation"/>。</param>
        /// <exception cref="ArgumentNullException"><paramref name="invocation"/> 为 <see langword="null"/>。</exception>
        public OneTimeInvocation(Invocation invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            _invocation = invocation;
        }

        /// <inheritdoc />
        public override void Invoke()
        {
            var invocation = _invocation;
            if (invocation != null && Interlocked.CompareExchange(ref _invocation, null, invocation) == invocation)
            {
                invocation.Invoke();
            }
        }
    }
}
