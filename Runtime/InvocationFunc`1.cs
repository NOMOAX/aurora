using System;

namespace Aurora
{
    /// <summary>
    /// 无参数且返回值的调用。
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public sealed class InvocationFunc<TResult> : Invocation<TResult>
    {
        private readonly Func<TResult> _func;

        /// <summary>
        /// 初始化 <see cref="InvocationFunc{TResult}"/> 类的新实例。
        /// </summary>
        /// <param name="func">无参数且返回值的方法。</param>
        /// <exception cref="ArgumentNullException"><paramref name="func"/> 为 <see langword="null"/>。</exception>
        public InvocationFunc(Func<TResult> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <inheritdoc />
        public override TResult Invoke()
        {
            return _func();
        }
    }
}
