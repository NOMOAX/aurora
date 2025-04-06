using System;

namespace Aurora
{
    /// <summary>
    /// 明确表示结果为空。
    /// </summary>
    public readonly struct VoidResult : IEquatable<VoidResult>
    {
        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is VoidResult;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 0;
        }

        /// <inheritdoc />
        public bool Equals(VoidResult other)
        {
            return true;
        }
    }
}
