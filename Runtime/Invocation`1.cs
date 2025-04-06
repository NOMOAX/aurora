using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// 返回值的调用。
    /// </summary>
    /// <typeparam name="TResult">返回值的类型。</typeparam>
    public abstract class Invocation<TResult>
    {
        /// <summary>
        /// 调用。
        /// </summary>
        /// <returns>返回的值。</returns>
        public abstract TResult Invoke();

        /// <summary>
        /// 创建一个 <see cref="Invocation{TResult}"/> 实例，调用它的 <see cref="Invoke"/> 方法将直接返回指定的值。
        /// </summary>
        /// <param name="result"><see cref="Invocation{TResult}"/> 实例执行 <see cref="Invoke"/> 方法的返回值。</param>
        /// <returns>调用 <see cref="Invoke"/> 方法直接返回指定值的 <see cref="Invocation{TResult}"/> 实例。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Invocation<TResult> FromResult(TResult result)
        {
            return new InvocationResult<TResult>(result);
        }
    }
}
