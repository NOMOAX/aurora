using System;
using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 为 <see cref="IEnumerable{T}"/> 接口提供扩展方法。
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 使用默认的相等性比较器，在当前 <see cref="IEnumerable{T}"/> 中搜索指定对象，并返回其首个匹配项的索引。
        /// </summary>
        /// <param name="source">要搜索的序列。</param>
        /// <param name="value">要在 <paramref name="source"/> 中搜索的对象。</param>
        /// <typeparam name="TSource">序列中成员的类型。</typeparam>
        /// <returns>如果在整个 <paramref name="source"/> 中找到了 <paramref name="value"/> 的第一个匹配项，则为该项的从零开始的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <see langword="null"/>。</exception>
        public static int IndexOf<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case TSource[] array:
                    return Array.IndexOf(array, value);
                case IList<TSource> list:
                    return list.IndexOf(value);
                default:
                {
                    using var enumerator = source.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var equalityComparer = EqualityComparer<TSource>.Default;
                        var currentIndex     = -1;
                        var foundIndex       = -1;
                        do
                        {
                            checked
                            {
                                ++currentIndex;
                            }
                            var current = enumerator.Current;
                            if (equalityComparer.Equals(current, value))
                            {
                                foundIndex = currentIndex;
                                break;
                            }
                        } while (enumerator.MoveNext());
                        return foundIndex;
                    }
                    return -1;
                }
            }
        }

        /// <summary>
        /// 使用指定的相等性比较器，在当前 <see cref="IEnumerable{T}"/> 中搜索指定对象，并返回其首个匹配项的索引。
        /// </summary>
        /// <param name="source">要搜索的序列。</param>
        /// <param name="value">要在 <paramref name="source"/> 中搜索的对象。</param>
        /// <param name="equalityComparer">用于比较两个 <typeparamref name="TSource"/> 对象是否相等的比较器。</param>
        /// <typeparam name="TSource">序列中成员的类型。</typeparam>
        /// <returns>如果在整个 <paramref name="source"/> 中找到了 <paramref name="value"/> 的第一个匹配项，则为该项的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <see langword="null"/>。</exception>
        public static int IndexOf<TSource>(
            this IEnumerable<TSource>  source,
            TSource                    value,
            IEqualityComparer<TSource> equalityComparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                equalityComparer ??= EqualityComparer<TSource>.Default;
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (equalityComparer.Equals(current, value))
                    {
                        foundIndex = currentIndex;
                        break;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }

        /// <summary>
        /// 使用默认的相等性比较器，在当前 <see cref="IEnumerable{T}"/> 中搜索指定对象，并返回其最后一个匹配项的索引。
        /// </summary>
        /// <param name="source">要搜索的序列。</param>
        /// <param name="value">要在 <paramref name="source"/> 中搜索的对象。</param>
        /// <typeparam name="TSource">序列中成员的类型。</typeparam>
        /// <returns>如果在整个 <paramref name="source"/> 中找到了 <paramref name="value"/> 的最后一个匹配项，则为该项的从零开始的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <see langword="null"/>。</exception>
        public static int LastIndexOf<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case TSource[] array:
                    return Array.LastIndexOf(array, value);
                case List<TSource> list:
                    return list.LastIndexOf(value);
                default:
                {
                    using var enumerator = source.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var equalityComparer = EqualityComparer<TSource>.Default;
                        var currentIndex     = -1;
                        var foundIndex       = -1;
                        do
                        {
                            checked
                            {
                                ++currentIndex;
                            }
                            var current = enumerator.Current;
                            if (equalityComparer.Equals(current, value))
                            {
                                foundIndex = currentIndex;
                            }
                        } while (enumerator.MoveNext());
                        return foundIndex;
                    }
                    return -1;
                }
            }
        }

        /// <summary>
        /// 使用指定的相等性比较器，在当前 <see cref="IEnumerable{T}"/> 中搜索指定对象，并返回其最后一个匹配项的索引。
        /// </summary>
        /// <param name="source">要搜索的序列。</param>
        /// <param name="value">要在 <paramref name="source"/> 中搜索的对象。</param>
        /// <param name="equalityComparer">用于比较两个 <typeparamref name="TSource"/> 对象是否相等的比较器。</param>
        /// <typeparam name="TSource">序列中成员的类型。</typeparam>
        /// <returns>如果在整个 <paramref name="source"/> 中找到了 <paramref name="value"/> 的最后一个匹配项，则为该项的从零开始的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 为 <see langword="null"/>。</exception>
        public static int LastIndexOf<TSource>(
            this IEnumerable<TSource>  source,
            TSource                    value,
            IEqualityComparer<TSource> equalityComparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                equalityComparer ??= EqualityComparer<TSource>.Default;
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (equalityComparer.Equals(current, value))
                    {
                        foundIndex = currentIndex;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }

        /// <summary>
        /// 使用指定的条件，在当前 <see cref="IEnumerable{T}"/> 中搜索匹配的成员，并返回其首个匹配项的索引。
        /// </summary>
        /// <param name="source">要搜索的序列。</param>
        /// <param name="match">搜索 <paramref name="source"/> 中的成员的条件。</param>
        /// <typeparam name="TSource">序列中成员的类型。</typeparam>
        /// <returns>如果在整个 <paramref name="source"/> 中找到了满足 <paramref name="match"/> 的第一个匹配项，则为该项的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static int FindIndex<TSource>(this IEnumerable<TSource> source, Predicate<TSource> match)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (match(current))
                    {
                        foundIndex = currentIndex;
                        break;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }

        /// <summary>
        /// 使用指定的条件，在当前 <see cref="IEnumerable{T}"/> 中搜索匹配的成员，并返回其最后一个匹配项的索引。
        /// </summary>
        /// <param name="source">要搜索的序列。</param>
        /// <param name="match">搜索 <paramref name="source"/> 中的对象的条件。</param>
        /// <typeparam name="TSource">序列中成员的类型。</typeparam>
        /// <returns>如果在整个 <paramref name="source"/> 中找到了满足 <paramref name="match"/> 的最后一个匹配项，则为该项的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 或 <paramref name="match"/> 为 <see langword="null"/>。</exception>
        public static int FindLastIndex<TSource>(this IEnumerable<TSource> source, Predicate<TSource> match)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (match == null)
            {
                throw new ArgumentNullException(nameof(match));
            }
            using var enumerator = source.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var currentIndex = -1;
                var foundIndex   = -1;
                do
                {
                    checked
                    {
                        ++currentIndex;
                    }
                    var current = enumerator.Current;
                    if (match(current))
                    {
                        foundIndex = currentIndex;
                    }
                } while (enumerator.MoveNext());
                return foundIndex;
            }
            return -1;
        }
    }
}
