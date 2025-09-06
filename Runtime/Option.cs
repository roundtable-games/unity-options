using System;
using UnityEngine;

namespace Roundtable.Utilities
{
    /// <summary>
    ///     Untyped <see cref="Option{T}"/> helper. Used as a shorthand for writing
    ///     <c>Option.None</c> or <c>Option.Some(T)</c> as substitutions for more
    ///     verbose <c>Option{T}.None</c> or <c>Option{T}.Some(T)</c> expressions.
    /// </summary>
    public readonly struct Option
    {
        /// <summary>
        ///     Get a default <see cref="Option"/> that can be implicitly
        ///     converted to any <see cref="Option{T}.None"/> value.
        /// </summary>
        public static Option None => new();

        /// <inheritdoc cref="Option{T}.Some(T)"/>
        public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    }

    /// <summary>
    ///     Generic "Option" type as used in functional programming languages.
    /// </summary>
    [Serializable]
    public struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
    {
        [SerializeField]
        private T _value;
        
        [SerializeField]
        private bool _hasValue;

        private Option(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        private Option(T value) : this(value, true)
        {
            _value = value;
            _hasValue = true;
        }

        /// <summary>
        ///     Create an <see cref="Option{T}"/> with no value.
        /// </summary>
        /// <returns>
        ///     A new Option object that encapsulates no value.
        /// </returns>
        public static Option<T> None
        {
            get
            {
                var option = new Option<T>(default, hasValue: false);
                return option;
            }
        }

        /// <summary>
        ///     Whether the Option wraps a value.
        /// </summary>
        /// <returns>
        ///     True if the Option is not None.
        /// </returns>
        public readonly bool IsSome => _hasValue;

        /// <summary>
        ///     Whether the Option does not wrap a value.
        /// </summary>
        /// <returns>
        ///     True if the Option is not Some.
        /// </returns>
        public readonly bool IsNone => !_hasValue;

        /// <summary>
        ///     Create an <see cref="Option{T}"/> wrapping <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        ///     The value wrapped by the option.
        /// </param>
        /// <returns>
        ///     A new Option object that encapsulates <paramref name="value"/>.
        /// </returns>
        public static Option<T> Some(T value)
        {
            var option = new Option<T>(value);
            return option;
        }

        /// <summary>
        ///     Get the wrapped value of the option unsafely. If 
        ///     <see cref="IsNone"/>, this throws an
        ///     <see cref="InvalidOperationException"/> with
        ///     <paramref name="message"/>.
        /// </summary>
        /// <returns>
        ///     The <typeparamref name="T"/> if <see cref="IsSome"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if <see cref="IsNone"/>.
        /// </exception>
        public readonly T Expect(string message)
        {
            if (!TryUnwrap(out T value))
                throw new InvalidOperationException(message);

            return value;
        }
        /// <summary>
        ///     Check that the option <see cref="IsSome"/> and the value is
        ///     matched by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">
        ///     A condition to evaluate for the value of the option, if the
        ///     option is not <see cref="None"/>.
        /// </param>
        /// <returns>
        ///     True if the option <see cref="IsSome"/> and
        ///     <paramref name="predicate"/> is true.
        /// </returns>
        public readonly bool IsSomeAnd(Predicate<T> predicate)
        {
            if (!TryUnwrap(out var value))
                return false;

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            bool result = predicate(value);
            return result;
        }

        /// <summary>
        ///     Check whether the value <see cref="IsNone"/>, or that the
        ///     value is matched by <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">
        ///     A condition to evaluate for the value of the option, if the
        ///     option is not <see cref="None"/>.
        /// </param>
        /// <returns>
        ///     True if <see cref="IsNone"/> or <see cref="IsSome"/> and
        ///     <paramref name="predicate"/> evaluates to true.
        /// </returns>
        public readonly bool IsNoneOr(Predicate<T> predicate)
        {
            if (TryUnwrap(out var value))
                return false;

            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            bool result = predicate(value);
            return result;
        }

        /// <summary>
        ///     Map the value of this Option to a <see cref="Option{U}"/>
        ///     value using <paramref name="mapper"/>, returning a
        ///     <see cref="Option{T}.None"/> if the value is None.
        /// </summary>
        /// <typeparam name="U">
        ///     The value type of the returned option.
        /// </typeparam>
        /// <param name="mapper">
        ///     The mapper function used to convert the value.
        /// </param>
        /// <returns>
        ///     A new Option mapped from this value using <paramref name="mapper"/>.
        /// </returns>
        public readonly Option<U> Map<U>(Func<T, U> mapper)
        {
            if (IsNone)
                return Option<U>.None;

            if (mapper is null)
                throw new ArgumentNullException(nameof(mapper));

            return Option<U>.Some(mapper(_value));
        }

        /// <summary>
        ///     Map the value of this Option to a <see cref="Option{U}"/>
        ///     value using <paramref name="mapper"/>, returning
        ///     <paramref name="value"/> if the value is None.
        /// </summary>
        /// <typeparam name="U">
        ///     The value type of the returned option.
        /// </typeparam>
        /// <param name="mapper">
        ///     The mapper function used to convert the value.
        /// </param>
        /// <param name="value">
        ///     The value to be used if the current value is None.
        /// </param>
        /// <returns>
        ///     A new Option mapped from this value using <paramref name="mapper"/>.
        /// </returns>
        public readonly Option<U> MapOr<U>(Func<T, U> mapper, U value)
        {
            if (IsNone)
                return Option<U>.Some(value);

            if (mapper is null)
                throw new ArgumentNullException(nameof(mapper));

            return Option<U>.Some(mapper(_value));
        }

        /// <summary>
        ///     Map the value of this Option to a <see cref="Option{U}"/>
        ///     value using <paramref name="mapper"/>, returning the result of
        ///     <paramref name="func"/> if the value is None.
        /// </summary>
        /// <typeparam name="U">
        ///     The value type of the returned option.
        /// </typeparam>
        /// <param name="mapper">
        ///     The mapper function used to convert the value.
        /// </param>
        /// <param name="func">
        ///     The expression to be used if the current value is None.
        /// </param>
        /// <returns>
        ///     A new Option mapped from this value using <paramref name="mapper"/>.
        /// </returns>
        public readonly Option<U> MapOrElse<U>(Func<T, U> mapper, Func<U> func)
        {
            if (IsNone)
            {
                if (func is null)
                    throw new ArgumentNullException(nameof(func));

                return Option<U>.Some(func());
            }

            if (mapper is null)
                throw new ArgumentNullException(nameof(mapper));

            return Option<U>.Some(mapper(_value));
        }

        /// <summary>
        ///     Try to unwrap the option.
        /// </summary>
        /// <param name="value">
        ///     The value contained in the option, if not None.
        /// </param>
        /// <returns>
        ///     True if <see cref="IsSome"/>.
        /// </returns>
        public readonly bool TryUnwrap(out T value)
        {
            value = _value;
            return _hasValue;
        }

        /// <summary>
        ///     Get the wrapped value of the option unsafely. If 
        ///     <see cref="IsNone"/>, this throws an
        ///     <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <returns>
        ///     The <typeparamref name="T"/> if <see cref="IsSome"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if <see cref="IsNone"/>.
        /// </exception>
        public readonly T Unwrap()
        {
            if (!TryUnwrap(out T value))
                throw new InvalidOperationException("Failed to unwrap Option; value is 'None'.");

            return value;
        }

        /// <summary>
        ///     If <see cref="IsSome"/>, then return the wrapped value, otherwise
        ///     return <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to be returned if the Option is None.
        /// </param>
        /// <returns>
        ///     The wrapped value, or <paramref name="value"/> if
        ///     <see cref="IsNone"/>.
        /// </returns>
        public readonly T UnwrapOr(T value)
        {
            if (IsNone)
                return value;

            return _value;
        }

        /// <summary>
        ///     If <see cref="IsSome"/>, returns the wrapped value, otherwise
        ///     returns the <see langword="default"/> value of
        ///     <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        ///     The wrapped value of the option, otherwise the
        ///     <see langword="default"/> value of <typeparamref name="T"/> 
        /// </returns>
        public readonly T UnwrapOrDefault()
        {
            if (IsNone)
                return default;

            return _value;
        }

        /// <summary>
        ///     If the Option is Some, then return the wrapped value, otherwise
        ///     return a value calculated by <paramref name="func"/>.
        /// </summary>
        /// <remarks>
        ///     Compared to <see cref="UnwrapOr(T)"/>, this function does not
        ///     eagerly evaluate the return expression.
        /// </remarks>
        /// <param name="func">
        ///     An expression resulting in a <typeparamref name="T"/> value.
        /// </param>
        /// <returns>
        ///     The wrapped value, or the value returned by
        ///     <paramref name="func"/> if the Option is None.
        /// </returns>
        public readonly T UnwrapOrElse(Func<T> func)
        {
            if (IsNone)
            {
                if (func is null)
                    throw new ArgumentNullException(nameof(func));

                return func();
            }

            return _value;
        }

        public readonly override int GetHashCode()
        {
            unchecked
            {
                int hashCode = HashCode.Combine(_value, _hasValue);
                return hashCode;
            }
        }

        public readonly override bool Equals(object obj)
        {
            bool result = obj switch
            {
                Option<T> other => Equals(other),
                T value => Equals(value),
                _ => false
            };

            return result;
        }

        public readonly bool Equals(Option<T> other)
        {
            if (IsNone)
                return other.IsNone;

            if (IsSome && !other.IsSome)
                return false;

            bool isEqual = _value.Equals(other._value);
            return isEqual;
        }

        public readonly bool Equals(T value)
        {
            if (IsNone)
                return false;

            bool isEqual = _value.Equals(value);
            return isEqual;
        }

        public readonly override string ToString()
        {
            string result = string.Format("(Option) {0}", IsNone ? "None" : _value.ToString());
            return result;
        }

        public static implicit operator Option<T>(T value) => Some(value);
        public static implicit operator Option<T>(Option _) => None;

        public static bool operator ==(Option<T> a, Option<T> b) => a.Equals(b);
        public static bool operator !=(Option<T> a, Option<T> b) => !a.Equals(b);
        public static bool operator ==(T a, Option<T> b) => b.Equals(a);
        public static bool operator !=(T a, Option<T> b) => !b.Equals(a);
        public static bool operator ==(Option<T> a, T b) => a.Equals(b);
        public static bool operator !=(Option<T> a, T b) => !a.Equals(b);
    }
}
