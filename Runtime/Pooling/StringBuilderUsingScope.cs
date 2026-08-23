using System;
using System.Text;
using System.Threading;

namespace Aurora.Pooling
{
    /// <summary>
    /// 使用可变字符串范围。
    /// </summary>
    public sealed class StringBuilderUsingScope : IDisposable
    {
        private StringBuilder _stringBuilder;

        /// <summary>
        /// 初始化 <see cref="StringBuilderUsingScope"/> 类的新实例。
        /// </summary>
        /// <param name="stringBuilder">此输出参数将被赋值为一个空可变字符串。</param>
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
