using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aurora.Pooling;

namespace Aurora
{
    /// <summary>
    /// Provides utility methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeUtility
    {
        [Flags]
        private enum FormatFlags
        {
            SearchDeclaringType = 1,

            SearchGenericArguments = 2,

            PrependNamespace = 4,

            All = SearchDeclaringType | SearchGenericArguments | PrependNamespace
        }

        private static readonly IReadOnlyDictionary<Type, string> BuildInTypeNames = new Dictionary<Type, string>
        {
            [typeof(void)]    = "void",
            [typeof(bool)]    = "bool",
            [typeof(char)]    = "char",
            [typeof(sbyte)]   = "sbyte",
            [typeof(byte)]    = "byte",
            [typeof(short)]   = "short",
            [typeof(ushort)]  = "ushort",
            [typeof(int)]     = "int",
            [typeof(uint)]    = "uint",
            [typeof(long)]    = "long",
            [typeof(ulong)]   = "ulong",
            [typeof(float)]   = "float",
            [typeof(double)]  = "double",
            [typeof(decimal)] = "decimal",
            [typeof(string)]  = "string",
            [typeof(object)]  = "object"
        };

        private static readonly Type[] ValueTupleTypeDefinitions =
        {
            typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>), typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>), typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>)
        };

        private static readonly ConditionalWeakTable<Type, string> NicelyFormattedTypeNames = new();

        private static readonly ConditionalWeakTable<Type, string>.CreateValueCallback
            CreateNicelyFormattedTypeNameCallback = CreateNicelyFormattedTypeName;

        /// <summary>
        /// Gets the string representation of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The string representation of <paramref name="type"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        public static string GetNicelyFormattedName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return InternalGetNicelyFormattedTypeName(type);
        }

        private static string InternalGetNicelyFormattedTypeName(Type type)
        {
            return NicelyFormattedTypeNames.GetValue(type, CreateNicelyFormattedTypeNameCallback);
        }

        private static string CreateNicelyFormattedTypeName(Type type)
        {
            return CreateNicelyFormattedTypeName(type, FormatFlags.All);
        }

        private static string CreateNicelyFormattedTypeName(Type type, FormatFlags formatFlags)
        {
            return BuildInTypeNames.TryGetValue(type, out var value) ||
                   TryHandleArrayType(type, formatFlags, out value) ||
                   TryHandleByReferenceType(type, formatFlags, out value) ||
                   TryHandlePointerType(type, formatFlags, out value) ||
                   TryHandleNestedType(type, formatFlags, out value) ||
                   TryHandleNullableType(type, formatFlags, out value) ||
                   TryHandleValueTupleType(type, true, out value) || TryHandleGenericType(type, formatFlags, out value)
                       ? value
                       : HandleIgnoreGenericPartType(type, formatFlags);
        }

        private static bool TryHandleArrayType(Type type, FormatFlags formatFlags, out string value)
        {
            if (!type.IsArray)
            {
                value = null;
                return false;
            }
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(CreateNicelyFormattedTypeName(type.GetElementType(), formatFlags));
                stringBuilder.Append('[');
                stringBuilder.Append(',', type.GetArrayRank() - 1);
                stringBuilder.Append(']');
                value = stringBuilder.ToString();
                return true;
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }

        private static bool TryHandleByReferenceType(Type type, FormatFlags formatFlags, out string value)
        {
            if (!type.IsByRef)
            {
                value = null;
                return false;
            }
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(CreateNicelyFormattedTypeName(type.GetElementType(), formatFlags));
                stringBuilder.Append('&');
                value = stringBuilder.ToString();
                return true;
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }

        private static bool TryHandlePointerType(Type type, FormatFlags formatFlags, out string value)
        {
            if (!type.IsPointer)
            {
                value = null;
                return false;
            }
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(CreateNicelyFormattedTypeName(type.GetElementType(), formatFlags));
                stringBuilder.Append('*');
                value = stringBuilder.ToString();
                return true;
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }

        private static bool TryHandleNestedType(Type type, FormatFlags formatFlags, out string value)
        {
            if ((formatFlags & FormatFlags.SearchDeclaringType) == 0 || !type.IsNested)
            {
                value = null;
                return false;
            }
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(CreateNicelyFormattedTypeName(type.DeclaringType, formatFlags));
                stringBuilder.Append('.');
                stringBuilder.Append(
                    CreateNicelyFormattedTypeName(
                        type,
                        formatFlags & ~FormatFlags.SearchDeclaringType & ~FormatFlags.PrependNamespace
                    )
                );
                value = stringBuilder.ToString();
                return true;
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }

        private static bool TryHandleNullableType(Type type, FormatFlags formatFlags, out string value)
        {
            var nullableUnderlyingType = Nullable.GetUnderlyingType(type);
            if (nullableUnderlyingType == null)
            {
                value = null;
                return false;
            }
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(CreateNicelyFormattedTypeName(nullableUnderlyingType, formatFlags));
                stringBuilder.Append('?');
                value = stringBuilder.ToString();
                return true;
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }

        private static bool TryHandleValueTupleType(Type type, bool surroundWithParentheses, out string value)
        {
            if (!type.IsGenericType)
            {
                value = null;
                return false;
            }
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var index                 = Array.IndexOf(ValueTupleTypeDefinitions, genericTypeDefinition);
            if (index >= 0)
            {
                var genericArguments = type.GetGenericArguments();
                var stringBuilder    = PredefinedPools.StringBuilder.Get();
                try
                {
                    if (surroundWithParentheses)
                    {
                        stringBuilder.Append('(');
                    }
                    for (var i = 0; i < genericArguments.Length; i++)
                    {
                        if (i > 0)
                        {
                            stringBuilder.Append(',');
                        }
                        var genericArgument = genericArguments[i];
                        if (genericArgument.IsGenericParameter)
                        {
                            continue;
                        }
                        if (i != 7)
                        {
                            stringBuilder.Append(InternalGetNicelyFormattedTypeName(genericArgument));
                        }
                        else if (TryHandleValueTupleType(genericArgument, false, out var tRestValue))
                        {
                            stringBuilder.Append(tRestValue);
                        }
                    }
                    if (surroundWithParentheses)
                    {
                        stringBuilder.Append(')');
                    }
                    value = stringBuilder.ToString();
                    return true;
                }
                finally
                {
                    PredefinedPools.StringBuilder.Return(stringBuilder);
                }
            }
            value = null;
            return false;
        }

        private static bool TryHandleGenericType(Type type, FormatFlags formatFlags, out string value)
        {
            if ((formatFlags & FormatFlags.SearchGenericArguments) == 0 || !type.IsGenericType)
            {
                value = null;
                return false;
            }
            var stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(
                    CreateNicelyFormattedTypeName(type, formatFlags & ~FormatFlags.SearchGenericArguments)
                );
                stringBuilder.Append('<');
                var genericArguments = type.GetGenericArguments();
                for (var i = 0; i < genericArguments.Length; i++)
                {
                    if (i > 0)
                    {
                        stringBuilder.Append(',');
                    }
                    var genericArgument = genericArguments[i];
                    if (genericArgument.IsGenericParameter)
                    {
                        continue;
                    }
                    stringBuilder.Append(InternalGetNicelyFormattedTypeName(genericArgument));
                }
                stringBuilder.Append('>');
                value = stringBuilder.ToString();
                return true;
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }

        private static string HandleIgnoreGenericPartType(Type type, FormatFlags formatFlags)
        {
            var name                   = type.Name;
            var index                  = name.IndexOf('`');
            var nameWithoutGenericPart = index < 0 ? name : name.Substring(0, index);
            if ((formatFlags & FormatFlags.PrependNamespace) != 0)
            {
                var @namespace = type.Namespace;
                if (@namespace != null)
                {
                    var stringBuilder = PredefinedPools.StringBuilder.Get();
                    try
                    {
                        stringBuilder.Append(@namespace);
                        stringBuilder.Append('.');
                        stringBuilder.Append(nameWithoutGenericPart);
                        return stringBuilder.ToString();
                    }
                    finally
                    {
                        PredefinedPools.StringBuilder.Return(stringBuilder);
                    }
                }
            }
            return nameWithoutGenericPart;
        }
    }
}
