using System.Collections.Generic;

namespace Aurora.Collections
{
    /// <summary>
    /// 为 <see cref="KeyValuePair{TKey,TValue}"/> 结构提供扩展方法。
    /// </summary>
    public static class KeyValuePairExtensions
    {
        /// <summary>
        /// 析构此键值对。
        /// </summary>
        /// <param name="keyValuePair">键值对。</param>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <typeparam name="TKey">键的类型。</typeparam>
        /// <typeparam name="TValue">值的类型。</typeparam>
        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> keyValuePair,
            out  TKey                       key,
            out  TValue                     value)
        {
            key   = keyValuePair.Key;
            value = keyValuePair.Value;
        }
    }
}
