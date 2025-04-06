using System;

namespace Aurora
{
    /// <summary>
    /// 提供一组数学方法。
    /// </summary>
    public static class AuroraMath
    {
        /// <summary>
        /// 判断一个有符号 32 位整数是否是质数。
        /// </summary>
        /// <param name="candidate">要进行判断的数。</param>
        /// <returns>如果 <paramref name="candidate"/> 是质数，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsPrime(int candidate)
        {
            if (candidate < 0)
            {
                return false;
            }
            if ((candidate & 1) == 0)
            {
                return candidate == 2;
            }
            var limit = (int) Math.Sqrt(candidate);
            for (var divisor = 3; divisor <= limit; divisor += 2)
            {
                if (candidate % divisor == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
