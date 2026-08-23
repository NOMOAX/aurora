using System;
using System.Runtime.CompilerServices;

namespace Aurora.Interpolations
{
    /// <summary>
    /// Provides a set of methods related to interpolation.
    /// </summary>
    public static class InterpolationUtility
    {
        /// <summary>
        /// Interpolates between the specified begin and end values using the specified interpolation mode and weight.
        /// </summary>
        /// <param name="begin">The begin value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="weight">A value between 0 and 1 that indicates the weight of the interpolation.</param>
        /// <param name="interpolation">The interpolation type.</param>
        /// <returns>The interpolation.</returns>
        public static double Interpolate(double begin, double end, double weight, Interpolation interpolation)
        {
            return InternalLinearInterpolate(begin, end, InternalTransform(weight, interpolation));
        }

        /// <summary>
        /// Performs a linear interpolation between the begin and end values using the specified weight.
        /// </summary>
        /// <param name="begin">The begin value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="weight">A value between 0 and 1 that indicates the weight of the interpolation.</param>
        /// <returns>The interpolation.</returns>
        public static double LinearInterpolate(double begin, double end, double weight)
        {
            return InternalLinearInterpolate(begin, end, weight);
        }

        /// <summary>
        /// Computes the linear weight of a specified value between the begin and end values.
        /// </summary>
        /// <param name="begin">The begin value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The target value.</param>
        /// <returns>If <paramref name="begin"/> and <paramref name="end"/> are not equal, <c>(value - begin) / (end - begin)</c>; otherwise, 0.</returns>
        public static double InverseLinearInterpolate(double begin, double end, double value)
        {
            return InternalInverseLinearInterpolate(begin, end, value, 0);
        }

        /// <summary>
        /// Computes the linear weight of a specified value between the begin and end values.
        /// </summary>
        /// <param name="begin">The begin value.</param>
        /// <param name="end">The end value.</param>
        /// <param name="value">The target value.</param>
        /// <param name="defaultWeightWhenBeginAndEndAreEqual">When <paramref name="begin"/> and <paramref name="end"/> are equal, this value is returned.</param>
        /// <returns>If <paramref name="begin"/> and <paramref name="end"/> are not equal, <c>(value - begin) / (end - begin)</c>; otherwise, <paramref name="defaultWeightWhenBeginAndEndAreEqual"/>.</returns>
        public static double InverseLinearInterpolate(
            double begin,
            double end,
            double value,
            double defaultWeightWhenBeginAndEndAreEqual)
        {
            return InternalInverseLinearInterpolate(begin, end, value, defaultWeightWhenBeginAndEndAreEqual);
        }

        /// <summary>
        /// Converts a linear-interpolation weight value to another weight value.
        /// </summary>
        /// <param name="weight">The weight value of the linear interpolation.</param>
        /// <param name="interpolation">The interpolation type to convert to.</param>
        /// <returns>The new weight value. Using this weight value to perform a linear interpolation between the begin and end values is equivalent to performing an <paramref name="interpolation"/> interpolation between the begin and end values.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="interpolation"/> is not a member defined in the <see cref="Interpolation"/> enum.</exception>
        public static double Transform(double weight, Interpolation interpolation)
        {
            return InternalTransform(weight, interpolation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double InternalLinearInterpolate(double begin, double end, double weight)
        {
            return begin * (1 - weight) + end * weight;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double InternalInverseLinearInterpolate(
            double begin,
            double end,
            double value,
            double defaultWeightWhenBeginAndEndAreEqual)
        {
            if (begin == end)
            {
                return defaultWeightWhenBeginAndEndAreEqual;
            }
            return (value - begin) / (end - begin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double InternalTransform(double weight, Interpolation interpolation)
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

            return amount switch
            {
                < 1 / d1   => n1 * amount * amount,
                < 2 / d1   => n1 * (amount -= 1.5 / d1) * amount + 0.75,
                < 2.5 / d1 => n1 * (amount -= 2.25 / d1) * amount + 0.9375,
                _          => n1 * (amount -= 2.625 / d1) * amount + 0.984375
            };
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
