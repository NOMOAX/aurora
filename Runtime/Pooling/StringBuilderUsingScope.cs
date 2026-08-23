using System;
using System.Text;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a mutable string.
    /// </summary>
    public sealed class StringBuilderUsingScope : IDisposable
    {
        private StringBuilder _stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuilderUsingScope"/> class.
        /// </summary>
        /// <param name="stringBuilder">This output parameter is assigned an empty mutable string.</param>
        public StringBuilderUsingScope(out StringBuilder stringBuilder)
        {
            _stringBuilder = PredefinedPools.StringBuilder.Get();
            stringBuilder  = _stringBuilder;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var stringBuilder = _stringBuilder;
            if (stringBuilder != null &&
                Interlocked.CompareExchange(ref _stringBuilder, null, stringBuilder) == stringBuilder)
            {
                PredefinedPools.StringBuilder.Return(stringBuilder);
            }
        }
    }
}
