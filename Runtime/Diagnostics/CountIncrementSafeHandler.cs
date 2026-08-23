using System;
using System.Threading;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// 用于在执行无法准确预知执行次数的循环或递归时确保执行次数在一个合理的范围内。
    /// </summary>
    public sealed class CountIncrementSafeHandler
    {
        private readonly int _maxCount;

        private int _count;

        /// <summary>
        /// 初始化 <see cref="CountIncrementSafeHandler"/> 类的新实例。
        /// </summary>
        /// <param name="maxCount">最大执行次数。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxCount"/> 小于 0。</exception>
        public CountIncrementSafeHandler(int maxCount)
        {
            if (maxCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }
            _maxCount = maxCount;
        }

        /// <summary>
        /// 递增执行次数，如果超过最大执行次数则抛出异常。
        /// </summary>
        /// <exception cref="UnexpectedException">递增后的执行次数大于最大执行次数。</exception>
        public void Increment()
        {
            if (Interlocked.Increment(ref _count) is var count && (count > _maxCount || count < 0))
            {
                throw new UnexpectedException();
            }
        }
    }
}
