using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Aurora.Collections
{
    /// <summary>
    /// 黑板。
    /// </summary>
    [DebuggerTypeProxy(typeof(BlackboardDebugView))]
    public class Blackboard
    {
        private readonly Dictionary<string, object> _dictionary = new();

        internal IDictionary<string, object> Dictionary => _dictionary;

        /// <summary>
        /// 设置与指定的键关联的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/>。</exception>
        public void SetValue(string key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _dictionary[key] = value;
        }

        /// <summary>
        /// 获取与指定的键关联的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="TValue">值的类型。</typeparam>
        /// <returns>与指定的键关联的值。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="KeyNotFoundException">未找到指定的键。</exception>
        /// <exception cref="InvalidCastException">与指定的键关联的值不能转换为 <typeparamref name="TValue"/> 类型。</exception>
        public TValue GetValue<TValue>(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var value = _dictionary[key];
            return (TValue) value;
        }

        /// <summary>
        /// 获取与指定的键关联的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">当此方法返回时，如果找到指定的键，则为与该键关联的值；否则为 <typeparamref name="TValue"/> 类型的默认值。</param>
        /// <typeparam name="TValue">值的类型。</typeparam>
        /// <returns>如果找到指定的键，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/>。</exception>
        /// <exception cref="InvalidCastException">与指定的键关联的值不能转换为 <typeparamref name="TValue"/> 类型。</exception>
        public bool TryGetValue<TValue>(string key, out TValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (_dictionary.TryGetValue(key, out var value1))
            {
                value = (TValue) value1;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// 将具有指定的键的键值对从 <see cref="Blackboard"/> 移除。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>如果找到指定的键并移除键值对，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> 为 <see langword="null"/>。</exception>
        public bool Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            return _dictionary.Remove(key);
        }

        /// <summary>
        /// 将所有键值对从 <see cref="Blackboard"/> 中移除。
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}
