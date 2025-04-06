using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// 为指定的枚举类型提供工具方法。
    /// </summary>
    /// <typeparam name="TEnum">枚举类型。</typeparam>
    public static class EnumUtility<TEnum> where TEnum : struct, Enum
    {
        /// <summary>
        /// 该枚举类型中的各个枚举成员的名称。
        /// </summary>
        public static readonly string[] Names;

        /// <summary>
        /// 该枚举类型中的各个枚举成员的值。
        /// </summary>
        public static readonly TEnum[] Values;

        /// <summary>
        /// 该枚举类型中的枚举成员的个数。
        /// </summary>
        public static readonly int Count;

        /// <summary>
        /// 该枚举类型的基础类型。
        /// </summary>
        public static readonly Type UnderlyingType;

        /// <summary>
        /// 该枚举类型是否是按位枚举类型。
        /// </summary>
        public static readonly bool IsBitwise;

        static EnumUtility()
        {
            Names          = typeof(TEnum).GetEnumNames();
            Values         = (TEnum[]) typeof(TEnum).GetEnumValues();
            Count          = Values.Length;
            UnderlyingType = typeof(TEnum).GetEnumUnderlyingType();
            IsBitwise      = typeof(TEnum).GetCustomAttributes(typeof(FlagsAttribute), false).Length == 1;
        }

        /// <summary>
        /// 判断该枚举类型中是否定义了指定的枚举成员。
        /// </summary>
        /// <param name="value">枚举成员。</param>
        /// <returns>如果 <typeparamref name="TEnum"/> 枚举类型中定义了 <paramref name="value"/>，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(TEnum value)
        {
            return Array.IndexOf(Values, value) >= 0;
        }

        /// <summary>
        /// 获取表示枚举成员的字段信息。
        /// </summary>
        /// <param name="value">枚举成员。</param>
        /// <returns>表示 <paramref name="value"/> 的字段信息。</returns>
        /// <remarks>如果存在其他枚举成员与 <paramref name="value"/> 具有相同的值，请改用 <see cref="GetFieldInfo(string)"/> 以确保获取准确的返回值。</remarks>
        public static FieldInfo GetFieldInfo(TEnum value)
        {
            var name = value.ToString();
            return typeof(TEnum).GetField(name, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// 获取表示该枚举类型中具有指定名称的枚举成员的字段信息。
        /// </summary>
        /// <param name="name">枚举成员的名称。</param>
        /// <returns>表示该枚举类型中名称为 <paramref name="name"/> 的枚举成员的字段信息。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> 为 <see langword="null"/>。</exception>
        public static FieldInfo GetFieldInfo(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return typeof(TEnum).GetField(name, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
        }
    }
}
