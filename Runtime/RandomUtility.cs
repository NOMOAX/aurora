using System;
using System.Collections.Generic;

namespace Aurora
{
    /// <summary>
    /// 用于生成全局伪随机数。
    /// </summary>
    public static class RandomUtility
    {
        /// <summary>
        /// 获取可从任何线程并发使用的线程安全的 <see cref="Random"/> 实例。
        /// </summary>
        public static Random Shared => ThreadSafeRandom.Instance;

        /// <summary>
        /// 获取一个 <see cref="bool"/> 值，它有 <paramref name="probability"/> 的概率为 <see langword="true"/>，有 1 - <paramref name="probability"/> 的概率为 <see langword="false"/>。
        /// </summary>
        /// <param name="probability">概率。</param>
        /// <returns>一个 <see cref="bool"/> 值，有 <paramref name="probability"/> 的概率为 <see langword="true"/>，有 1 - <paramref name="probability"/> 的概率为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="probability"/> 不是范围 [0, 1] 中的数。</exception>
        public static bool P(double probability)
        {
            if (probability == 0d)
            {
                return false;
            }
            if (probability == 1d)
            {
                return true;
            }
            if (probability > 0d && probability < 1d)
            {
                return probability > ThreadSafeRandom.Instance.NextDouble();
            }
            throw new ArgumentOutOfRangeException(nameof(probability), probability, null);
        }

        /// <summary>
        /// 指定各元素的权重，从集合中随机选取一个元素。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <param name="weights">集合中各元素的权重。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <returns>如果选取到了元素，则为该元素；否则为 <typeparamref name="T"/> 的默认值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="weights"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> 中包含的元素数小于 <paramref name="weights"/>，或者 <paramref name="weights"/> 中至少有一个元素小于 0 或者为非数字。</exception>
        /// <exception cref="NotSupportedException">将 <paramref name="weights"/> 从后往前依次累加，在某一步得到正无穷大。</exception>
        public static T Choose<T>(IList<T> collection, IList<double> weights)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            return Choose(collection, weights, 0, collection.Count);
        }

        /// <summary>
        /// 指定各元素的权重，从集合中随机选取一个元素。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <param name="weights">集合中各元素的权重。</param>
        /// <param name="index">选取范围的开始索引。</param>
        /// <param name="count">选取范围内的元素数。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <returns>如果选取到了元素，则为该元素；否则为 <typeparamref name="T"/> 的默认值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="weights"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者 <paramref name="count"/> 小于 1。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="collection"/> 中包含的元素数，或者 <paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="weights"/> 中包含的元素数，或者 <paramref name="weights"/> 中至少有一个元素小于 0 或者为非数字。</exception>
        /// <exception cref="NotSupportedException">将 <paramref name="weights"/> 从后往前依次累加，在某一步得到正无穷大。</exception>
        public static T Choose<T>(IList<T> collection, IList<double> weights, int index, int count)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > collection.Count)
            {
                throw new ArgumentException();
            }
            if (index + count > weights.Count)
            {
                throw new ArgumentException();
            }
            var chosenIndex = GetChosenIndex(weights, index, count);
            return chosenIndex >= 0 ? collection[chosenIndex] : default;
        }

        /// <summary>
        /// 指定各元素的权重，尝试从集合中随机选取一个元素，并通过输出参数返回选取到的元素。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <param name="weights">集合中各元素的权重。</param>
        /// <param name="chosen">如果选取到了元素，则为该元素；否则为 <typeparamref name="T"/> 的默认值。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <returns>如果选取到了元素，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="weights"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> 中包含的元素数小于 <paramref name="weights"/>，或者 <paramref name="weights"/> 中至少有一个元素小于 0 或者为非数字。</exception>
        /// <exception cref="NotSupportedException">将 <paramref name="weights"/> 从后往前依次累加，在某一步得到正无穷大。</exception>
        public static bool TryChoose<T>(IList<T> collection, IList<double> weights, out T chosen)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            return TryChoose(collection, weights, 0, collection.Count, out chosen);
        }

        /// <summary>
        /// 指定各元素的权重，尝试从集合中随机选取一个元素，并通过输出参数返回选取到的元素。
        /// </summary>
        /// <param name="collection">集合。</param>
        /// <param name="weights">集合中各元素的权重。</param>
        /// <param name="index">选取范围的开始索引。</param>
        /// <param name="count">选取范围内的元素数。</param>
        /// <param name="chosen">如果选取到了元素，则为该元素；否则为 <typeparamref name="T"/> 的默认值。</param>
        /// <typeparam name="T">集合中元素的类型。</typeparam>
        /// <returns>如果选取到了元素，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> 或 <paramref name="weights"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者 <paramref name="count"/> 小于 1。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="collection"/> 中包含的元素数，或者 <paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="weights"/> 中包含的元素数，或者 <paramref name="weights"/> 中至少有一个元素小于 0 或者为非数字。</exception>
        /// <exception cref="NotSupportedException">将 <paramref name="weights"/> 从后往前依次累加，在某一步得到正无穷大。</exception>
        public static bool TryChoose<T>(IList<T> collection, IList<double> weights, int index, int count, out T chosen)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > collection.Count)
            {
                throw new ArgumentException();
            }
            if (index + count > weights.Count)
            {
                throw new ArgumentException();
            }
            var chosenIndex = GetChosenIndex(weights, index, count);
            var result      = chosenIndex >= 0;
            chosen = result ? collection[chosenIndex] : default;
            return result;
        }

        /// <summary>
        /// 在一组指定的选中值中按权重进行随机选取，返回选取到的权重值的索引。
        /// </summary>
        /// <param name="weights">权重值集合。</param>
        /// <returns>如果选取到了权重值，则为该权重值在权重值集合中的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weights"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="NotSupportedException">将 <paramref name="weights"/> 从后往前依次累加，在某一步得到正无穷大。</exception>
        public static int GetChosenIndex(IList<double> weights)
        {
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            return GetChosenIndex(weights, 0, weights.Count);
        }

        /// <summary>
        /// 在一组指定的选中值中按权重进行随机选取，返回选取到的权重值的索引。
        /// </summary>
        /// <param name="weights">权重值集合。</param>
        /// <param name="index">选取范围的开始索引。</param>
        /// <param name="count">选取范围内的元素数。</param>
        /// <returns>如果选取到了权重值，则为该权重值在权重值集合中的索引；否则为负数。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="weights"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> 小于 0，或者 <paramref name="count"/> 小于 1。</exception>
        /// <exception cref="ArgumentException"><paramref name="index"/> 加上 <paramref name="count"/> 大于 <paramref name="weights"/> 中包含的元素数，或者 <paramref name="weights"/> 中至少有一个元素小于 0 或者为非数字。</exception>
        /// <exception cref="NotSupportedException">将 <paramref name="weights"/> 从后往前依次累加，在某一步得到正无穷大。</exception>
        public static int GetChosenIndex(IList<double> weights, int index, int count)
        {
            if (weights == null)
            {
                throw new ArgumentNullException(nameof(weights));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, null);
            }
            if (count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, null);
            }
            if (index + count > weights.Count)
            {
                throw new ArgumentException();
            }
            var last = weights[index + count - 1];
            if (!(last >= 0d))
            {
                throw new ArgumentException($"{nameof(weights)} 中至少有一个元素小于 0 或者为非数字", nameof(weights));
            }
            var sum  = last;
            var sums = new double[count - 1];
            for (var sumIndex = count - 2; sumIndex >= 0; sumIndex--)
            {
                var weightIndex = index + sumIndex;
                var weight      = weights[weightIndex];
                if (!(weight >= 0d))
                {
                    throw new ArgumentException($"{nameof(weights)} 中至少有一个元素小于 0 或者为非数字", nameof(weights));
                }
                sum += weight;
                if (double.IsPositiveInfinity(sum))
                {
                    throw new NotSupportedException($"{nameof(weights)} 从后往前依次累加会在某一步得到正无穷大，无法进行计算");
                }
                sums[sumIndex] = sum;
            }
            for (var sumIndex = 0; sumIndex < count - 1; sumIndex++)
            {
                var weightIndex = index + sumIndex;
                var weight      = weights[weightIndex];
                if (weight == 0d)
                {
                    continue;
                }
                if (P(weight / sums[sumIndex]))
                {
                    return sumIndex;
                }
            }
            return last != 0d ? index + count - 1 : -1;
        }
    }
}
