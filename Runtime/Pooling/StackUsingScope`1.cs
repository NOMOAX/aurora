using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用栈范围。
    /// </summary>
    /// <typeparam name="T">栈的成员的类型。</typeparam>
    public sealed class StackUsingScope<T> : IDisposable
    {
        private Stack<T> _stack;

        /// <summary>
        /// 初始化 <see cref="StackUsingScope{T}"/> 类的新实例。
        /// </summary>
        /// <param name="stack">此输出参数将被赋值为一个空栈。</param>
        public StackUsingScope(out Stack<T> stack)
        {
            _stack = PredefinedPools<T>.Stack.Get();
            stack  = _stack;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var stack = _stack;
            if (stack != null && Interlocked.CompareExchange(ref _stack, null, stack) == stack)
            {
                PredefinedPools<T>.Stack.Return(stack);
            }
        }
    }
}
