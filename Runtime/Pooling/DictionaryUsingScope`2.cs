using System;
using System.Collections.Generic;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用字典范围。
    /// </summary>
    /// <typeparam name="TKey">字典中的键的类型。</typeparam>
    /// <typeparam name="TValue">字典中的值的类型。</typeparam>
    public sealed class DictionaryUsingScope<TKey, TValue> : IDisposable
    {
        private Dictionary<TKey, TValue> _dictionary;

        /// <summary>
        /// 初始化 <see cref="DictionaryUsingScope{TKey,TValue}"/> 类的新实例。
        /// </summary>
        /// <param name="dictionary">此输出参数将被赋值为一个空字典。</param>
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
