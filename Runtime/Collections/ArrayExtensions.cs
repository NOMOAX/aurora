using System;

namespace Aurora.Collections
{
    /// <summary>
    /// 为 <see cref="Array"/> 提供扩展方法。
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// 打乱当前数组中元素的顺序。
        /// </summary>
        /// <param name="array">要被打乱的数组。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <see langword="null"/>。</exception>
        public static void ShuffleInPlace<T>(this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            array.ShuffleInPlace(0, array.Length);
        }

        /// <summary>
        /// 打乱当前数组的指定范围中元素的顺序。
        /// </summary>
        /// <param name="array">要被打乱的数组。</param>
        /// <param name="index">打乱顺序范围的起始索引。</param>
        /// <param name="count">打乱顺序范围内的元素数。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或 <paramref name="count"/> 小于 0。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 和 <paramref name="count"/> 不能指定 <paramref name="array"/> 中的合理范围。</exception>
        public static void ShuffleInPlace<T>(this T[] array, int index, int count)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (array.Length - index < count)
            {
                throw new ArgumentException();
            }
            for (var i = index + count - 1; i > index; i--)
            {
                var j = RandomUtility.Shared.Next(index, i + 1);
                if (i == j)
                {
                    continue;
                }
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前数组中第一个匹配元素。
        /// </summary>
        /// <param name="array">要搜索的数组。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">数组中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的第一个元素，则为该元素；否则为类型 <typeparamref name="TSource"/> 的默认值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static TSource Find<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            foreach (var item in array)
            {
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前数组中第一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="array">要搜索的数组。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">数组中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的第一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static int FindIndex<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = 0; i < array.Length; i++)
            {
                if (match(array[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前数组中最后一个匹配元素。
        /// </summary>
        /// <param name="array">要搜索的数组。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">数组中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的最后一个元素，则为该元素；否则为类型 <typeparamref name="TSource"/> 的默认值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static TSource FindLast<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = array.Length - 1; i >= 0; i--)
            {
                var item = array[i];
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前数组中最后一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="array">要搜索的数组。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">数组中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的最后一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static int FindLastIndex<TSource, TState>(
            this TSource[]                          array,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = array.Length - 1; i >= 0; i--)
            {
                if (match(array[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

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
