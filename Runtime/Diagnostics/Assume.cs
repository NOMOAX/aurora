using System;
using System.Collections.Generic;
using System.Globalization;

namespace Aurora.Diagnostics
{
    /// <summary>
    /// 假设。
    /// </summary>
    /// <remarks>如果假设不成立，将调用 <see cref="Log.E"/> 记录错误信息。</remarks>
    public static class Assume
    {
        /// <summary>
        /// 假设实际值与预期值相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreEqual<T>(T actual, T expected)
        {
            return AreEqual(actual, expected, null, null);
        }

        /// <summary>
        /// 假设实际值与预期值相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <param name="comparer">比较器。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreEqual<T>(T actual, T expected, IEqualityComparer<T> comparer)
        {
            return AreEqual(actual, expected, comparer, null);
        }

        /// <summary>
        /// 假设实际值与预期值相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreEqual<T>(T actual, T expected, string message)
        {
            return AreEqual(actual, expected, null, message);
        }

        /// <summary>
        /// 假设实际值与预期值相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <param name="comparer">比较器。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreEqual<T>(T actual, T expected, IEqualityComparer<T> comparer, string message)
        {
            if ((comparer ?? EqualityComparer<T>.Default).Equals(actual, expected))
            {
                return true;
            }
            Log.E(
                message == null
                    ? $"Expected: {GetStringRepresentation(expected)}. But was: {GetStringRepresentation(actual)}."
                    : $"{message}{System.Environment.NewLine}Expected: {GetStringRepresentation(expected)}. But was: {GetStringRepresentation(actual)}."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值与预期值不相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreNotEqual<T>(T actual, T expected)
        {
            return AreNotEqual(actual, expected, null, null);
        }

        /// <summary>
        /// 假设实际值与预期值不相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <param name="comparer">比较器。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreNotEqual<T>(T actual, T expected, IEqualityComparer<T> comparer)
        {
            return AreNotEqual(actual, expected, comparer, null);
        }

        /// <summary>
        /// 假设实际值与预期值不相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreNotEqual<T>(T actual, T expected, string message)
        {
            return AreNotEqual(actual, expected, null, message);
        }

        /// <summary>
        /// 假设实际值与预期值不相等。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="expected">预期值。</param>
        /// <param name="comparer">比较器。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool AreNotEqual<T>(T actual, T expected, IEqualityComparer<T> comparer, string message)
        {
            if (!(comparer ?? EqualityComparer<T>.Default).Equals(actual, expected))
            {
                return true;
            }
            Log.E(
                message == null
                    ? $"Expected: not equal to {GetStringRepresentation(expected)}. But was: {GetStringRepresentation(actual)}."
                    : $"{message}{System.Environment.NewLine}Expected: not equal to {GetStringRepresentation(expected)}. But was: {GetStringRepresentation(actual)}."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <typeparam name="T">实际值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNull<T>(T actual) where T : class
        {
            return IsNull(actual, null);
        }

        /// <summary>
        /// 假设实际值为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">实际值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNull<T>(T actual, string message) where T : class
        {
            if (actual == null)
            {
                return true;
            }
            Log.E(
                message == null
                    ? $"Expected: null. But was: {GetStringRepresentation(actual)}."
                    : $"{message}{System.Environment.NewLine}Expected: null. But was: {GetStringRepresentation(actual)}."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <typeparam name="T">实际值的基础值类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNull<T>(T? actual) where T : struct
        {
            return IsNull(actual, null);
        }

        /// <summary>
        /// 假设实际值为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">实际值的基础值类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNull<T>(T? actual, string message) where T : struct
        {
            if (actual == null)
            {
                return true;
            }
            Log.E(
                message == null
                    ? $"Expected: null. But was: {GetStringRepresentation(actual)}."
                    : $"{message}{System.Environment.NewLine}Expected: null. But was: {GetStringRepresentation(actual)}."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值不为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <typeparam name="T">实际值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNotNull<T>(T actual) where T : class
        {
            return IsNotNull(actual, null);
        }

        /// <summary>
        /// 假设实际值不为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">实际值的类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNotNull<T>(T actual, string message) where T : class
        {
            if (actual != null)
            {
                return true;
            }
            Log.E(
                message == null
                    ? "Expected: not null. But was: null."
                    : $"{message}{System.Environment.NewLine}Expected: not null. But was: null."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值不为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <typeparam name="T">实际值的基础值类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNotNull<T>(T? actual) where T : struct
        {
            return IsNotNull(actual, null);
        }

        /// <summary>
        /// 假设实际值不为 <see langword="null"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <typeparam name="T">实际值的基础值类型。</typeparam>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsNotNull<T>(T? actual, string message) where T : struct
        {
            if (actual != null)
            {
                return true;
            }
            Log.E(
                message == null
                    ? "Expected: not null. But was: null."
                    : $"{message}{System.Environment.NewLine}Expected: not null. But was: null."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值为 <see langword="true"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsTrue(bool actual)
        {
            return IsTrue(actual, null);
        }

        /// <summary>
        /// 假设实际值为 <see langword="true"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsTrue(bool actual, string message)
        {
            if (actual)
            {
                return true;
            }
            Log.E(
                message == null
                    ? "Expected: True. But was: False."
                    : $"{message}{System.Environment.NewLine}Expected: True. But was: False."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值为 <see langword="true"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsTrue(bool? actual)
        {
            return IsTrue(actual, null);
        }

        /// <summary>
        /// 假设实际值为 <see langword="true"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsTrue(bool? actual, string message)
        {
            switch (actual)
            {
                case true:
                    return true;
                case false:
                    Log.E(
                        message == null
                            ? "Expected: True. But was: False."
                            : $"{message}{System.Environment.NewLine}Expected: True. But was: False."
                    );
                    return false;
                case null:
                    Log.E(
                        message == null
                            ? "Expected: True. But was: null."
                            : $"{message}{System.Environment.NewLine}Expected: True. But was: null."
                    );
                    return false;
            }
        }

        /// <summary>
        /// 假设实际值为 <see langword="false"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsFalse(bool actual)
        {
            return IsFalse(actual, null);
        }

        /// <summary>
        /// 假设实际值为 <see langword="false"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsFalse(bool actual, string message)
        {
            if (!actual)
            {
                return true;
            }
            Log.E(
                message == null
                    ? "Expected: False. But was: True."
                    : $"{message}{System.Environment.NewLine}Expected: False. But was: True."
            );
            return false;
        }

        /// <summary>
        /// 假设实际值为 <see langword="false"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsFalse(bool? actual)
        {
            return IsFalse(actual, null);
        }

        /// <summary>
        /// 假设实际值为 <see langword="false"/>。
        /// </summary>
        /// <param name="actual">实际值。</param>
        /// <param name="message">当假设不成立时记录的自定义消息。</param>
        /// <returns>如果假设成立，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        public static bool IsFalse(bool? actual, string message)
        {
            switch (actual)
            {
                case false:
                    return true;
                case true:
                    Log.E(
                        message == null
                            ? "Expected: False. But was: True."
                            : $"{message}{System.Environment.NewLine}Expected: False. But was: True."
                    );
                    return false;
                case null:
                    Log.E(
                        message == null
                            ? "Expected: False. But was: null."
                            : $"{message}{System.Environment.NewLine}Expected: False. But was: null."
                    );
                    return false;
            }
        }

        private static string GetStringRepresentation(object value)
        {
            return value switch
            {
                null                     => "null",
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _                        => value.ToString()
            };
        }
    }
}
