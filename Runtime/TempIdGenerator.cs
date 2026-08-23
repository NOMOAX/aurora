using System;
using Aurora.Diagnostics;
using Aurora.Pooling;

namespace Aurora
{
    /// <summary>
    /// A temporary ID generator.
    /// </summary>
    /// <remarks>The generated temporary ID has the specified prefix (which may be <see langword="null"/>) and postfix (which may be <see langword="null"/>); the middle part is the string representation of <see cref="Guid.NewGuid"/> formatted as "N".</remarks>
    public sealed class TempIdGenerator
    {
        private readonly string _prefix;

        private readonly string _postfix;

        private const string GuidFormat = "N";

        /// <summary>
        /// Initializes a new instance of the <see cref="TempIdGenerator"/> class.
        /// </summary>
        /// <param name="prefix">The prefix of the temporary ID.</param>
        /// <param name="postfix">The postfix of the temporary ID.</param>
        public TempIdGenerator(string prefix, string postfix)
        {
            _prefix  = prefix;
            _postfix = postfix;
        }

        /// <summary>
        /// Generates and gets a new temporary ID.
        /// </summary>
        public string NewTempId
        {
            get
            {
                var stringBuilder = PredefinedPools.StringBuilder.Get();
                try
                {
                    stringBuilder.Append(_prefix);
                    stringBuilder.Append(Guid.NewGuid().ToString(GuidFormat));
                    stringBuilder.Append(_postfix);
                    return stringBuilder.ToString();
                }
                finally
                {
                    PredefinedPools.StringBuilder.Return(stringBuilder);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified ID matches this <see cref="TempIdGenerator"/> in format.
        /// </summary>
        /// <param name="id">The ID whose format is to be checked.</param>
        /// <returns><see langword="true"/> if <paramref name="id"/> matches this instance in format; otherwise, <see langword="false"/>.</returns>
        public bool Match(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(_prefix) && !id.StartsWith(_prefix, StringComparison.Ordinal))
            {
                return false;
            }
            if (!string.IsNullOrEmpty(_postfix) && !id.EndsWith(_postfix, StringComparison.Ordinal))
            {
                return false;
            }
            string guidString;
            var    stringBuilder = PredefinedPools.StringBuilder.Get();
            try
            {
                stringBuilder.Append(id);
                stringBuilder.Length -= _postfix?.Length ?? 0;
                stringBuilder.Remove(0, _prefix?.Length ?? 0);
                guidString = stringBuilder.ToString();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Log.E(e);
                return false;
            }
            finally
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
            return Guid.TryParseExact(guidString, GuidFormat, out _);
        }
    }
}
