using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// 提供一组与类型相关的工具方法。
    /// </summary>
    public static class ClassUtility
    {
        /// <summary>
        /// 执行指定类型的静态构造函数（如果未执行过）。
        /// </summary>
        /// <typeparam name="T">要执行静态构造函数的类型。</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ExecuteStaticConstructor<T>()
        {
        }
    }
}
