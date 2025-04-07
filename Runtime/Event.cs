using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// 管理被识别号标识的事件的注册、取消注册和发送。
    /// </summary>
    public static class Event
    {
        private static readonly ConcurrentDictionary<int, Delegate> Delegates =
            new ConcurrentDictionary<int, Delegate>();

        /// <remarks>表示 <c>System.Collections.Concurrent.ConcurrentDictionary&lt;int, Delegate&gt;</c> 类型的实例方法 <c>private bool TryRemoveInternal(int key, out Delegate value, bool match, Delegate oldValue)</c> 的签名</remarks>
        private delegate bool TryRemoveInternalMethodSignature(
            int          key,
            out Delegate value,
            bool         matchValue,
            Delegate     oldValue);

        /// <remarks>表示 <see cref="Delegates"/> 的实例方法 <c>private bool TryRemoveInternal(int key, out Delegate value, bool match, Delegate oldValue)</c></remarks>
        private static readonly TryRemoveInternalMethodSignature TryRemoveInternalCall =
            (TryRemoveInternalMethodSignature) Delegate.CreateDelegate(
                typeof(TryRemoveInternalMethodSignature),
                Delegates,
                typeof(ConcurrentDictionary<int, Delegate>).GetMethod(
                    "TryRemoveInternal",
                    BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    CallingConventions.Standard | CallingConventions.HasThis,
                    new[] { typeof(int), typeof(Delegate).MakeByRefType(), typeof(bool), typeof(Delegate) },
                    null
                )!,
                true
            );

        /// <summary>
        /// 向事件注册委托。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="delegate">要向事件注册的委托。</param>
        /// <exception cref="ArgumentException">被 <paramref name="id"/> 标识的事件和 <paramref name="delegate"/> 都不为 <see langword="null"/>，且他们不是相同委托类型的实例。</exception>
        public static void Subscribe(int id, Delegate @delegate)
        {
            if (@delegate == null)
            {
                return;
            }
            while (true)
            {
                if (Delegates.TryGetValue(id, out var oldDelegate))
                {
                    var newDelegate = Delegate.Combine(oldDelegate, @delegate);
                    if (Delegates.TryUpdate(id, newDelegate, oldDelegate))
                    {
                        return;
                    }
                }
                else
                {
                    if (Delegates.TryAdd(id, @delegate))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 从事件取消注册委托。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="delegate">要从事件取消注册的委托。</param>
        /// <exception cref="ArgumentException">被 <paramref name="id"/> 标识的事件和 <paramref name="delegate"/> 都不为 <see langword="null"/>，且他们不是相同委托类型的实例。</exception>
        public static void Unsubscribe(int id, Delegate @delegate)
        {
            if (@delegate == null)
            {
                return;
            }
            while (true)
            {
                if (!Delegates.TryGetValue(id, out var oldDelegate))
                {
                    return;
                }
                var newDelegate = Delegate.Remove(oldDelegate, @delegate);
                if (newDelegate != null)
                {
                    if (Delegates.TryUpdate(id, newDelegate, oldDelegate))
                    {
                        return;
                    }
                }
                else
                {
                    if (TryRemove(id, oldDelegate))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="args">参数。</param>
        /// <returns>如果存在被 <paramref name="id"/> 标识的事件，则为调用该事件的返回值；否则为 <see langword="null"/>。</returns>
        /// <exception cref="TargetParameterCountException"><paramref name="args"/> 中参数的数量不合法。</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/> 中参数的顺序或类型不合法。</exception>
        public static object Send(int id, params object[] args)
        {
            return Delegates.TryGetValue(id, out var value) ? Invoke(value, args) : null;
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="args">参数。</param>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <returns>如果存在被 <paramref name="id"/> 标识的事件，则为调用该事件的返回值转换为 <typeparamref name="T"/>（如果为 <see langword="null"/> 则为 <typeparamref name="T"/> 的默认值）后的结果；否则为 <typeparamref name="T"/> 的默认值。</returns>
        /// <exception cref="TargetParameterCountException"><paramref name="args"/> 中参数的数量不合法。</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/> 中参数的顺序或类型不合法。</exception>
        /// <exception cref="InvalidCastException">无法将返回值转换为 <typeparamref name="T"/>。</exception>
        public static T Send<T>(int id, params object[] args)
        {
            return Delegates.TryGetValue(id, out var value) ? Invoke<T>(value, args) : default;
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="args">参数。</param>
        /// <returns>如果存在被 <paramref name="id"/> 标识的事件，则为该事件的委托列表按顺序调用后再将各个返回值组合而成的数组；否则为长度为 0 的数组。</returns>
        /// <exception cref="TargetParameterCountException"><paramref name="args"/> 中参数的数量不合法。</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/> 中参数的顺序或类型不合法。</exception>
        public static object[] Sends(int id, params object[] args)
        {
            return Delegates.TryGetValue(id, out var value) ? Invokes(value, args) : Array.Empty<object>();
        }

        /// <summary>
        /// 发送事件。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="args">参数。</param>
        /// <typeparam name="T">返回值的类型。</typeparam>
        /// <returns>如果存在被 <paramref name="id"/> 标识的事件，则为该事件的委托列表按顺序调用后再将各个返回值转换为 <typeparamref name="T"/>（如果为 <see langword="null"/> 则为 <typeparamref name="T"/> 的默认值）后再组合而成的数组；否则为长度为 0 的数组。</returns>
        /// <exception cref="TargetParameterCountException"><paramref name="args"/> 中参数的数量不合法。</exception>
        /// <exception cref="ArgumentException"><paramref name="args"/> 中参数的顺序或类型不合法。</exception>
        /// <exception cref="InvalidCastException">在调用被 <paramref name="id"/> 标识的事件的委托列表时，无法将其中的某一个返回值转换为 <typeparamref name="T"/>。</exception>
        public static T[] Sends<T>(int id, params object[] args)
        {
            return Delegates.TryGetValue(id, out var value) ? Invokes<T>(value, args) : Array.Empty<T>();
        }

        /// <summary>
        /// 清除 <see cref="Event"/> 中的所有事件。
        /// </summary>
        public static void Clear()
        {
            Delegates.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object Invoke(Delegate @delegate, object[] args)
        {
            return @delegate.DynamicInvoke(args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T Invoke<T>(Delegate @delegate, object[] args)
        {
            return (T) Invoke(@delegate, args);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object[] Invokes(Delegate @delegate, object[] args)
        {
            var delegates    = @delegate.GetInvocationList();
            var length       = delegates.Length;
            var returnValues = new object[length];
            for (var i = 0; i < length; i++)
            {
                returnValues[i] = Invoke(delegates[i], args);
            }
            return returnValues;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static T[] Invokes<T>(Delegate @delegate, object[] args)
        {
            var delegates    = @delegate.GetInvocationList();
            var length       = delegates.Length;
            var returnValues = new T[length];
            for (var i = 0; i < length; i++)
            {
                returnValues[i] = Invoke<T>(delegates[i], args);
            }
            return returnValues;
        }

        /// <summary>
        /// 尝试从 <see cref="Delegates"/> 中移除指定的键，当且仅当存在该键，且与该键对应的值等于 <paramref name="oldValue"/>。
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryRemove(int key, Delegate oldValue)
        {
            return TryRemoveInternalCall(key, out _, true, oldValue);
        }
    }
}
