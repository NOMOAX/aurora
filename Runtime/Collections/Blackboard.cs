using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aurora.Collections
{
    /// <summary>
    /// A blackboard.
    /// </summary>
    [DebuggerTypeProxy(typeof(BlackboardDebugView))]
    public class Blackboard
    {
        private readonly Dictionary<string, object> _dictionary = new();

        internal IDictionary<string, object> Dictionary => _dictionary;

        /// <summary>
        /// Sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public void SetValue(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _dictionary[key] = value;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>The value associated with the specified key.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="KeyNotFoundException">The specified key was not found.</exception>
        /// <exception cref="InvalidCastException">The value associated with the specified key cannot be converted to type <typeparamref name="TValue"/>.</exception>
        public TValue GetValue<TValue>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = _dictionary[key];
            return (TValue)value;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">When this method returns, if the specified key was found, the value associated with that key; otherwise, the default value of type <typeparamref name="TValue"/>.</param>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns><see langword="true"/> if the specified key was found; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidCastException">The value associated with the specified key cannot be converted to type <typeparamref name="TValue"/>.</exception>
        public bool TryGetValue<TValue>(string key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (_dictionary.TryGetValue(key, out var value1))
            {
                value = (TValue)value1;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Removes the key-value pair with the specified key from the <see cref="Blackboard"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><see langword="true"/> if the specified key was found and the key-value pair was removed; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _dictionary.Remove(key);
        }

        /// <summary>
        /// Removes all key-value pairs from the <see cref="Blackboard"/>.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}
