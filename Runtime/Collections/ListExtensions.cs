using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 为 <see cref="IList{T}"/> 接口提供扩展方法。
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// 打乱当前 <see cref="IList{T}"/> 中元素的顺序。
        /// </summary>
        /// <param name="collection">要被打乱的集合。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
        public static void ShuffleInPlace<T>(this IList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            ShuffleInPlace(collection, 0, collection.Count);
        }

        /// <summary>
        /// 打乱当前 <see cref="IList{T}"/> 的指定范围中元素的顺序。
        /// </summary>
        /// <param name="collection">要被打乱的集合。</param>
        /// <param name="index">打乱顺序范围的起始索引。</param>
        /// <param name="count">打乱顺序范围内的元素数。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或 <paramref name="count"/> 小于 0。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 和 <paramref name="count"/> 不能指定 <paramref name="collection"/> 中的合理范围。</exception>
        public static void ShuffleInPlace<T>(this IList<T> collection, int index, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (collection.Count - index < count)
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
                (collection[i], collection[j]) = (collection[j], collection[i]);
            }
        }

        /// <summary>
        /// 反转当前 <see cref="IList{T}"/> 中元素的顺序。
        /// </summary>
        /// <param name="collection">要被反转的集合。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
        public static void ReverseInPlace<T>(this IList<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            ReverseInPlace(collection, 0, collection.Count);
        }

        /// <summary>
        /// 反转当前 <see cref="IList{T}"/> 的指定范围中元素的顺序。
        /// </summary>
        /// <param name="collection">要被反转的集合。</param>
        /// <param name="index">反转范围的起始索引。</param>
        /// <param name="count">反转范围内的元素数。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或 <paramref name="count"/> 小于 0。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 和 <paramref name="count"/> 不能指定 <paramref name="collection"/> 中的合理范围。</exception>
        public static void ReverseInPlace<T>(this IList<T> collection, int index, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (collection.Count - index < count)
            {
                throw new ArgumentException();
            }
            for (int i = index, j = index + count - 1; i < j; i++, j--)
            {
                (collection[i], collection[j]) = (collection[j], collection[i]);
            }
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前 <see cref="IList{T}"/> 中第一个匹配元素。
        /// </summary>
        /// <param name="collection">要搜索的集合。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">集合中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的第一个元素，则为该元素；否则为类型 <typeparamref name="TSource"/> 的默认值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static TSource Find<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = 0; i < collection.Count; i++)
            {
                var item = collection[i];
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前 <see cref="IList{T}"/> 中第一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="collection">要搜索的集合。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">集合中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的第一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static int FindIndex<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = 0; i < collection.Count; i++)
            {
                if (match(collection[i], state))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前 <see cref="IList{T}"/> 中最后一个匹配元素。
        /// </summary>
        /// <param name="collection">要搜索的集合。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">集合中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的最后一个元素，则为该元素；否则为类型 <typeparamref name="TSource"/> 的默认值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static TSource FindLast<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                var item = collection[i];
                if (match(item, state))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// 搜索与指定条件相匹配的元素，返回当前 <see cref="IList{T}"/> 中最后一个匹配元素的从零开始的索引。
        /// </summary>
        /// <param name="collection">要搜索的集合。</param>
        /// <param name="match">条件。</param>
        /// <param name="state">将传递给 <paramref name="match"/> 的第二个形参。</param>
        /// <typeparam name="TSource">集合中元素的类型。</typeparam>
        /// <typeparam name="TState"><paramref name="state"/> 的类型。</typeparam>
        /// <returns>如果找到与 <paramref name="match"/> 定义的条件相匹配的最后一个元素，则为该元素的从零开始的索引；否则为 -1。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static int FindLastIndex<TSource, TState>(
            this IList<TSource>                     collection,
            ParameterizedPredicate<TSource, TState> match,
            TState                                  state)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            for (var i = collection.Count - 1; i >= 0; i--)
            {
                if (match(collection[i], state))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
