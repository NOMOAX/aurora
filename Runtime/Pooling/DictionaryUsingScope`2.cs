using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// A using scope for a dictionary.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public sealed class DictionaryUsingScope<TKey, TValue> : IDisposable
    {
        private Dictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryUsingScope{TKey,TValue}"/> class.
        /// </summary>
        /// <param name="dictionary">This output parameter is assigned an empty dictionary.</param>
        public DictionaryUsingScope(out Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = PredefinedPools<TKey, TValue>.Dictionary.Get();
            dictionary  = _dictionary;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            var dictionary = _dictionary;
            if (dictionary != null && Interlocked.CompareExchange(ref _dictionary, null, dictionary) == dictionary)
            {
                PredefinedPools<TKey, TValue>.Dictionary.Return(dictionary);
            }
        }
    }
}
