using System.Collections.Generic;

namespace Aurora.Pooling
{
    /// <summary>
    /// 管理池中的字典的策略。
    /// </summary>
    /// <typeparam name="TKey">字典的键的类型。</typeparam>
    /// <typeparam name="TValue">字典的值的类型。</typeparam>
    public class PooledDictionaryPolicy<TKey, TValue> : IPooledObjectPolicy<Dictionary<TKey, TValue>>
    {
        /// <summary>
        /// 获取或设置池化的字典的初始容量。
        /// </summary>
        public int InitialCapacity { get; set; } = 13;

        /// <summary>
        /// 获取或设置允许被放入池的字典的最大长度。
        /// </summary>
        public int MaximumRetainedCount { get; set; } = 251;

        /// <inheritdoc />
        public Dictionary<TKey, TValue> Create()
        {
            return new Dictionary<TKey, TValue>(InitialCapacity, EqualityComparer<TKey>.Default);
        }

        /// <inheritdoc />
        public void OnGet(Dictionary<TKey, TValue> obj)
        {
        }

        /// <inheritdoc />
        public bool Return(Dictionary<TKey, TValue> obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.Count > MaximumRetainedCount)
            {
                return false;
            }
            if (obj.Comparer != (object) EqualityComparer<TKey>.Default)
            {
                return false;
            }
            obj.Clear();
            return true;
        }

        /// <inheritdoc />
        public void Dispose(Dictionary<TKey, TValue> obj)
        {
            obj?.Clear();
        }
    }
}
