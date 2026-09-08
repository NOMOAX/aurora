using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// Provides utility methods for the specified enumeration type.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    public static class EnumUtility<TEnum> where TEnum : struct, Enum
    {
        /// <summary>
        /// The names of the members in this enumeration type.
        /// </summary>
        public static readonly string[] Names;

        /// <summary>
        /// The values of the members in this enumeration type.
        /// </summary>
        public static readonly TEnum[] Values;

        /// <summary>
        /// The number of members in this enumeration type.
        /// </summary>
        public static readonly int Count;

        /// <summary>
        /// The underlying type of this enumeration type.
        /// </summary>
        public static readonly Type UnderlyingType;

        /// <summary>
        /// Whether this enumeration type is defined as bit flags.
        /// </summary>
        public static readonly bool IsBitwise;

        static EnumUtility()
        {
            Names          = typeof(TEnum).GetEnumNames();
            Values         = (TEnum[])typeof(TEnum).GetEnumValues();
            Count          = Values.Length;
            UnderlyingType = typeof(TEnum).GetEnumUnderlyingType();
            IsBitwise      = typeof(TEnum).GetCustomAttributes(typeof(FlagsAttribute), false).Length == 1;
        }

        /// <summary>
        /// Determines whether the specified member is defined in this enumeration type.
        /// </summary>
        /// <param name="value">The enumeration member.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> is defined in the <typeparamref name="TEnum"/> enumeration type; otherwise, <see langword="false"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDefined(TEnum value)
        {
            return Array.IndexOf(Values, value) >= 0;
        }

        /// <summary>
        /// Gets the field information that represents an enumeration member.
        /// </summary>
        /// <param name="value">The enumeration member.</param>
        /// <returns>The field information that represents <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not a member defined in the <typeparamref name="TEnum"/> enumeration type.</exception>
        /// <remarks>If another enumeration member has the same value as <paramref name="value"/>, use <see cref="GetFieldInfo(string)"/> instead to ensure an accurate return value.</remarks>
        public static FieldInfo GetFieldInfo(TEnum value)
        {
            if (!IsDefined(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            var name = value.ToString();
            return typeof(TEnum).GetField(name, BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
        }

        /// <summary>
        /// Gets the field information that represents the member with the specified name in this enumeration type.
        /// </summary>
        /// <param name="name">The name of the enumeration member.</param>
        /// <returns>The field information that represents the member named <paramref name="name"/> in this enumeration type.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="name"/> is not the name of any <typeparamref name="TEnum"/> enumeration member.</exception>
        public static FieldInfo GetFieldInfo(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            return typeof(TEnum).GetField(
                       name,
                       BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public
                   ) ?? throw new ArgumentOutOfRangeException(nameof(name), name, null);
        }

        /// <summary>
        /// Gets whether the specified enum value has the <see cref="ObsoleteAttribute"/> attribute.
        /// </summary>
        /// <param name="value">The enumeration member.</param>
        /// <returns><see langword="true"/> if <paramref name="value"/> has the <see cref="ObsoleteAttribute"/> attribute; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="value"/> is not a member defined in the <typeparamref name="TEnum"/> enumeration type.</exception>
        public static bool IsObsolete(TEnum value)
        {
            if (!IsDefined(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            var fieldInfo = GetFieldInfo(value);
            return fieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 1;
        }

        /// <summary>
        /// Gets whether the enumeration member with the specified name has the <see cref="ObsoleteAttribute"/> attribute.
        /// </summary>
        /// <param name="name">The name of the enumeration member.</param>
        /// <returns><see langword="true"/> if the enumeration member named <paramref name="name"/> has the <see cref="ObsoleteAttribute"/> attribute; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="name"/> is not the name of any <typeparamref name="TEnum"/> enumeration member.</exception>
        public static bool IsObsolete(string name)
        {
            var fieldInfo = GetFieldInfo(name);
            return fieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 1;
        }
    }
}
