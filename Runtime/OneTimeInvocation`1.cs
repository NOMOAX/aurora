using System;
using System.Threading;

namespace Aurora
{
    /// <summary>
    /// 对另一个 <see cref="Invocation{TResult}"/> 进行包装，确保仅第一次的对 <see cref="Invoke"/> 的调用会实际执行。
    /// </summary>
    /// <typeparam name="TResult">返回值的类型。</typeparam>
    public sealed class OneTimeInvocation<TResult> : Invocation<TResult>
    {
        private Invocation<TResult> _invocation;

        private TResult _result;

        /// <summary>
        /// 初始化 <see cref="OneTimeInvocation{TResult}"/> 类的新实例。
        /// </summary>
        /// <param name="invocation">一个 <see cref="Invocation{TResult}"/>。</param>
        /// <exception cref="ArgumentNullException"><paramref name="invocation"/> 为 <see langword="null"/>。</exception>
        public OneTimeInvocation(Invocation<TResult> invocation)
        {
            if (invocation == null)
            {
                throw new ArgumentNullException(nameof(invocation));
            }
            _invocation = invocation;
        }

        /// <inheritdoc />
        public override TResult Invoke()
        {
            var invocation = _invocation;
            if (invocation != null && Interlocked.CompareExchange(ref _invocation, null, invocation) == invocation)
            {
                _result = invocation.Invoke();
            }
            return _result;
        }
    }
}
