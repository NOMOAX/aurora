using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Aurora
{
    /// <summary>
    /// 管理被识别号标识的事件的注册、取消注册和发送。
    /// </summary>
    public static class Event
    {
        private static readonly Dictionary<int, Delegate> Delegates = new Dictionary<int, Delegate>();

        private static readonly object Lock = new object();

        /// <summary>
        /// 向事件注册委托。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="subscribeDelegate">要向事件注册的委托。</param>
        /// <exception cref="ArgumentException">被 <paramref name="id"/> 标识的事件和 <paramref name="subscribeDelegate"/> 都不为 <see langword="null"/>，且他们不是相同委托类型的实例。</exception>
        public static void Subscribe(int id, Delegate subscribeDelegate)
        {
            if (subscribeDelegate == null)
            {
                return;
            }
            lock (Lock)
            {
                if (Delegates.TryGetValue(id, out var @delegate))
                {
                    Delegates[id] = Delegate.Combine(@delegate, subscribeDelegate);
                }
                else
                {
                    Delegates.Add(id, subscribeDelegate);
                }
            }
        }

        /// <summary>
        /// 从事件取消注册委托。
        /// </summary>
        /// <param name="id">用于标识事件的识别号。</param>
        /// <param name="unsubscribeDelegate">要从事件取消注册的委托。</param>
        /// <exception cref="ArgumentException">被 <paramref name="id"/> 标识的事件和 <paramref name="unsubscribeDelegate"/> 都不为 <see langword="null"/>，且他们不是相同委托类型的实例。</exception>
        public static void Unsubscribe(int id, Delegate unsubscribeDelegate)
        {
            if (unsubscribeDelegate == null)
            {
                return;
            }
            lock (Lock)
            {
                if (!Delegates.TryGetValue(id, out var @delegate))
                {
                    return;
                }
                var left = Delegate.Remove(@delegate, unsubscribeDelegate);
                if (left == null)
                {
                    Delegates.Remove(id);
                }
                else
                {
                    Delegates[id] = left;
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
            bool     got;
            Delegate @delegate;
            lock (Lock)
            {
                got = Delegates.TryGetValue(id, out @delegate);
            }
            return got switch
            {
                true  => Invoke(@delegate, args),
                false => null
            };
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
            bool     got;
            Delegate @delegate;
            lock (Lock)
            {
                got = Delegates.TryGetValue(id, out @delegate);
            }
            return got switch
            {
                true  => Invoke<T>(@delegate, args),
                false => default
            };
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
            bool     got;
            Delegate @delegate;
            lock (Lock)
            {
                got = Delegates.TryGetValue(id, out @delegate);
            }
            return got switch
            {
                true  => Invokes(@delegate, args),
                false => Array.Empty<object>()
            };
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
            bool     got;
            Delegate @delegate;
            lock (Lock)
            {
                got = Delegates.TryGetValue(id, out @delegate);
            }
            return got switch
            {
                true  => Invokes<T>(@delegate, args),
                false => Array.Empty<T>()
            };
        }

        /// <summary>
        /// 清除 <see cref="Event"/> 中的所有事件。
        /// </summary>
        public static void Clear()
        {
            lock (Lock)
            {
                Delegates.Clear();
            }
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
    }
}
