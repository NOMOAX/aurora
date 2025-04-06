using System;

namespace Aurora
{
    /// <summary>
    /// 为 <see cref="Array"/> 提供扩展方法。
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 将一种类型的数组转换为另一种类型的数组。
        /// </summary>
        /// <param name="array">要转换为目标类型的数组。</param>
        /// <param name="converter">转换器。</param>
        /// <param name="state">传入转换器的第二个参数。</param>
        /// <typeparam name="TInput">源数组元素的类型。</typeparam>
        /// <typeparam name="TOutput">目标数组元素的类型。</typeparam>
        /// <returns>目标类型的数组，包含从源数组转换而来的元素。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 或 <paramref name="converter"/> 为 <see langword="null"/>。</exception>
        /// <remarks>该方法是对 <see cref="Array.ConvertAll{TInput,TOutput}"/> 的扩展，允许传入由用户定义的状态信息，避免使用闭包。</remarks>
        public static TOutput[] ConvertAll<TInput, TOutput>(
            this TInput[]                 array,
            Func<TInput, object, TOutput> converter,
            object                        state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }
            var length      = array.Length;
            var outputArray = new TOutput[length];
            for (var i = 0; i < length; ++i)
            {
                outputArray[i] = converter(array[i], state);
            }
            return outputArray;
        }
    }
}
