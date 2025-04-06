using System;
using Aurora.Diagnostics;
using Aurora.Pooling;

namespace Aurora
{
    /// <summary>
    /// 临时 ID 生成器。
    /// </summary>
    /// <remarks>生成的临时 ID 将具有指定的前缀（可为 <see langword="null"/>）和后缀（可为 <see langword="null"/>），中间部分为 <see cref="Guid.NewGuid"/> 的以 "N" 为格式的字符串表现形式。</remarks>
    public sealed class TempIdGenerator
    {
        private readonly string _prefix;

        private readonly string _postfix;

        private const string GuidFormat = "N";

        /// <summary>
        /// 初始化 <see cref="TempIdGenerator"/> 类的新实例。
        /// </summary>
        /// <param name="prefix">临时 ID 的前缀。</param>
        /// <param name="postfix">临时 ID 的后缀。</param>
        public TempIdGenerator(string prefix, string postfix)
        {
            _prefix  = prefix;
            _postfix = postfix;
        }

        /// <summary>
        /// 生成并获取新的临时 ID。
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
        /// 判断指定的 ID 是否从格式上匹配此 <see cref="TempIdGenerator"/>。
        /// </summary>
        /// <param name="id">要进行格式检测的 ID。</param>
        /// <returns>如果 <paramref name="id"/> 从格式上匹配此实例，则为 <see langword="true"/>；否则为 <see langword="false"/>。</returns>
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
