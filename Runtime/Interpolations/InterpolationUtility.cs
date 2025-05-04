using System;
using System.Runtime.CompilerServices;

namespace Aurora.Interpolations
{
    /// <summary>
    /// 提供一组与插值有关的方法。
    /// </summary>
    public static class InterpolationUtility
    {
        /// <summary>
        /// 根据指定的插值方式和权重在指定的开始值和结束值之间执行插值。
        /// </summary>
        /// <param name="interpolation">插值类型。</param>
        /// <param name="begin">开始值。</param>
        /// <param name="end">结束值。</param>
        /// <param name="weight">一个介于 0 和 1 之间的值，指示插值的权重。</param>
        /// <returns>插值。</returns>
        public static double Interpolate(Interpolation interpolation, double begin, double end, double weight)
        {
            return InternalLinearInterpolate(begin, end, InternalTransform(interpolation, weight));
        }

        /// <summary>
        /// 根据指定的权重在开始值和结束值之间执行线性插值。
        /// </summary>
        /// <param name="begin">开始值。</param>
        /// <param name="end">结束值。</param>
        /// <param name="weight">一个介于 0 和 1 之间的值，指示插值的权重。</param>
        /// <returns>插值。</returns>
        public static double LinearInterpolate(double begin, double end, double weight)
        {
            return InternalLinearInterpolate(begin, end, weight);
        }

        /// <summary>
        /// 将线性插值权重值转换为另一权重值。
        /// </summary>
        /// <param name="interpolation">要转换到的插值类型。</param>
        /// <param name="weight">线性插值的权重值。</param>
        /// <returns>新权重值。使用此权重值在开始值和结束值之间执行线性插值，等同于在开始值和结束值之间执行 <paramref name="interpolation"/> 插值。</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interpolation"/> 不是在 <see cref="Interpolation"/> 枚举中定义的成员。</exception>
        public static double Transform(Interpolation interpolation, double weight)
        {
            return InternalTransform(interpolation, weight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double InternalLinearInterpolate(double begin, double end, double weight)
        {
            return begin * (1 - weight) + end * weight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double InternalTransform(Interpolation interpolation, double weight)
        {
            return interpolation switch
            {
                Interpolation.Linear => TransformToLinear(weight),
                Interpolation.InSine => TransformToInSine(weight),
                Interpolation.OutSine => TransformToOutSine(weight),
                Interpolation.InOutSine => TransformToInOutSine(weight),
                Interpolation.InQuad => TransformToInQuad(weight),
                Interpolation.OutQuad => TransformToOutQuad(weight),
                Interpolation.InOutQuad => TransformToInOutQuad(weight),
                Interpolation.InCubic => TransformToInCubic(weight),
                Interpolation.OutCubic => TransformToOutCubic(weight),
                Interpolation.InOutCubic => TransformToInOutCubic(weight),
                Interpolation.InQuart => TransformToInQuart(weight),
                Interpolation.OutQuart => TransformToOutQuart(weight),
                Interpolation.InOutQuart => TransformToInOutQuart(weight),
                Interpolation.InQuint => TransformToInQuint(weight),
                Interpolation.OutQuint => TransformToOutQuint(weight),
                Interpolation.InOutQuint => TransformToInOutQuint(weight),
                Interpolation.InExpo => TransformToInExpo(weight),
                Interpolation.OutExpo => TransformToOutExpo(weight),
                Interpolation.InOutExpo => TransformToInOutExpo(weight),
                Interpolation.InCirc => TransformToInCirc(weight),
                Interpolation.OutCirc => TransformToOutCirc(weight),
                Interpolation.InOutCirc => TransformToInOutCirc(weight),
                Interpolation.InBack => TransformToInBack(weight),
                Interpolation.OutBack => TransformToOutBack(weight),
                Interpolation.InOutBack => TransformToInOutBack(weight),
                Interpolation.InElastic => TransformToInElastic(weight),
                Interpolation.OutElastic => TransformToOutElastic(weight),
                Interpolation.InOutElastic => TransformToInOutElastic(weight),
                Interpolation.InBounce => TransformToInBounce(weight),
                Interpolation.OutBounce => TransformToOutBounce(weight),
                Interpolation.InOutBounce => TransformToInOutBounce(weight),
                _ => throw new ArgumentOutOfRangeException(nameof(interpolation), interpolation, null)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToLinear(double amount)
        {
            return amount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInSine(double amount)
        {
            return 1 - TransformToOutSine(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutSine(double amount)
        {
            return Math.Sin(amount * (Math.PI / 2));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutSine(double amount)
        {
            return amount < 0.5 ? TransformToInSine(amount * 2) * 0.5 : TransformToOutSine(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInQuad(double amount)
        {
            return amount * amount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutQuad(double amount)
        {
            return 1 - TransformToInQuad(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutQuad(double amount)
        {
            return amount < 0.5 ? TransformToInQuad(amount * 2) * 0.5 : TransformToOutQuad(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInCubic(double amount)
        {
            return amount * amount * amount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutCubic(double amount)
        {
            return 1 - TransformToInCubic(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutCubic(double amount)
        {
            return amount < 0.5
                       ? TransformToInCubic(amount * 2) * 0.5
                       : TransformToOutCubic(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInQuart(double amount)
        {
            return amount * amount * amount * amount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutQuart(double amount)
        {
            return 1 - TransformToInQuart(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutQuart(double amount)
        {
            return amount < 0.5
                       ? TransformToInQuart(amount * 2) * 0.5
                       : TransformToOutQuart(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInQuint(double amount)
        {
            return amount * amount * amount * amount * amount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutQuint(double amount)
        {
            return 1 - TransformToInQuint(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutQuint(double amount)
        {
            return amount < 0.5
                       ? TransformToInQuint(amount * 2) * 0.5
                       : TransformToOutQuint(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInExpo(double amount)
        {
            return amount == 0 ? 0 : Math.Pow(2, amount * 10 - 10);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutExpo(double amount)
        {
            return 1 - TransformToInExpo(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutExpo(double amount)
        {
            return amount < 0.5 ? TransformToInExpo(amount * 2) * 0.5 : TransformToOutExpo(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInCirc(double amount)
        {
            return 1 - Math.Sqrt(1 - amount * amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutCirc(double amount)
        {
            return 1 - TransformToInCirc(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutCirc(double amount)
        {
            return amount < 0.5 ? TransformToInCirc(amount * 2) * 0.5 : TransformToOutCirc(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInBack(double amount)
        {
            const double c1 = 1.7015802317654063;
            const double c3 = c1 + 1;

            return (c3 * amount - c1) * amount * amount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutBack(double amount)
        {
            return 1 - TransformToInBack(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutBack(double amount)
        {
            return amount < 0.5 ? TransformToInBack(amount * 2) * 0.5 : TransformToOutBack(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInElastic(double amount)
        {
            const double c4 = Math.PI * 2 / 3;

            return amount switch
            {
                0 => 0,
                1 => 1,
                _ => -Math.Pow(2, 10 * amount - 10) * Math.Sin((amount * 10 - 10.75) * c4)
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutElastic(double amount)
        {
            return 1 - TransformToInElastic(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutElastic(double amount)
        {
            return amount < 0.5
                       ? TransformToInElastic(amount * 2) * 0.5
                       : TransformToOutElastic(amount * 2 - 1) * 0.5 + 0.5;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInBounce(double amount)
        {
            return 1 - TransformToOutBounce(1 - amount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToOutBounce(double amount)
        {
            const double n1 = 7.5625;
            const double d1 = 2.75;

            if (amount < 1 / d1)
            {
                return n1 * amount * amount;
            }
            if (amount < 2 / d1)
            {
                return n1 * (amount -= 1.5 / d1) * amount + 0.75;
            }
            if (amount < 2.5 / d1)
            {
                return n1 * (amount -= 2.25 / d1) * amount + 0.9375;
            }
            return n1 * (amount -= 2.625 / d1) * amount + 0.984375;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double TransformToInOutBounce(double amount)
        {
            return amount < 0.5
                       ? TransformToInBounce(amount * 2) * 0.5
                       : TransformToOutBounce(amount * 2 - 1) * 0.5 + 0.5;
        }
    }
}
