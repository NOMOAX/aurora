using System.Collections;

namespace Aurora.Collections
{
    /// <summary>
    /// 使用空对象模式实现 <see cref="IEnumerator"/>。
    /// </summary>
    public sealed class NullEnumerator : IEnumerator
    {
        /// <summary>
        /// 获取单一实例。
        /// </summary>
        public static NullEnumerator Instance { get; } = new NullEnumerator();

        private NullEnumerator()
        {
        }

        bool IEnumerator.MoveNext()
        {
            return false;
        }

        object IEnumerator.Current => null;

        void IEnumerator.Reset()
        {
        }
    }
}
