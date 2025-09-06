using System;

namespace PillowFortress
{
    public readonly struct Option
    {
        public static Option None { get; } = new();
    }

    /// <summary>
    ///     Generic "Option" type as used in functional programming languages.
    /// </summary>
    public readonly struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

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
        public static Option<T> None()
        {
            var option = new Option<T>(default, hasValue: false);
            return option;
        }

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
        ///     Whether the Option wraps a value.
        /// </summary>
        /// <returns>
        ///     True if the Option is not None.
        /// </returns>
        public bool IsSome() => _hasValue;

        /// <summary>
        ///     Whether the Option does not wrap a value.
        /// </summary>
        /// <returns>
        ///     True if the Option is not Some.
        /// </returns>
        public bool IsNone() => !_hasValue;

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
        public Option<U> Map<U>(Func<T, U> mapper)
        {
            if (mapper is null)
                throw new ArgumentNullException(nameof(mapper));

            if (IsNone())
                return Option<U>.None();

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
        /// <param name="value">
        ///     The value to be used if the current value is None.
        /// </param>
        /// <param name="mapper">
        ///     The mapper function used to convert the value.
        /// </param>
        /// <returns>
        ///     A new Option mapped from this value using <paramref name="mapper"/>.
        /// </returns>
        public Option<U> MapOr<U>(U value, Func<T, U> mapper)
        {
            if (mapper is null)
                throw new ArgumentNullException(nameof(mapper));

            if (IsNone())
                return Option<U>.Some(value);

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
        /// <param name="func">
        ///     The expression to be used if the current value is None.
        /// </param>
        /// <param name="mapper">
        ///     The mapper function used to convert the value.
        /// </param>
        /// <returns>
        ///     A new Option mapped from this value using <paramref name="mapper"/>.
        /// </returns>
        public Option<U> MapOrElse<U>(Func<U> func, Func<T, U> mapper)
        {
            if (func is null)
                throw new ArgumentNullException(nameof(func));

            if (mapper is null)
                throw new ArgumentNullException(nameof(mapper));

            if (IsNone())
                return Option<U>.Some(func());

            return Option<U>.Some(mapper(_value));
        }

        /// <summary>
        ///     Try to unwrap the option.
        /// </summary>
        /// <param name="value">
        ///     The value contained in the Option, if not None.
        /// </param>
        /// <returns>
        ///     True if the Option contains a value.
        /// </returns>
        public bool TryUnwrap(out T value)
        {
            value = _value;
            return _hasValue;
        }

        /// <summary>
        ///     If the option is Some, then return the wrapped value, otherwise
        ///     return <paramref name="value"/>.
        /// </summary>
        /// <param name="value">
        ///     The value to be returned if the Option is None.
        /// </param>
        /// <returns>
        ///     The wrapped value, or <paramref name="value"/> if the Option
        ///     is None.
        /// </returns>
        public T UnwrapOr(T value)
        {
            if (IsNone())
                return value;

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
        public T UnwrapOrElse(Func<T> func)
        {
            if (IsNone())
            {
                if (func is null)
                    throw new ArgumentNullException(nameof(func));

                return func();
            }

            return _value;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = HashCode.Combine(_value, _hasValue);
                return hashCode;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Option<T> other)
            {
                return Equals(other);
            }
            else if (obj is T value)
            {
                return Equals(value);
            }

            return false;
        }

        public bool Equals(Option<T> other)
        {
            if (IsNone())
                return other.IsNone();

            if (IsSome() && !other.IsSome())
                return false;

            return _value.Equals(other._value);
        }

        public bool Equals(T value)
        {
            if (IsNone())
                return false;

            return _value.Equals(value);
        }

        public override string ToString()
        {
            string result = string.Concat("[Option] ", IsNone() ? "None" : _value.ToString());
            return result;
        }

        public static implicit operator Option<T>(T value) => Some(value);
        public static implicit operator Option<T>(Option _) => None();

        public static bool operator ==(Option<T> a, Option<T> b) => a.Equals(b);
        public static bool operator !=(Option<T> a, Option<T> b) => !a.Equals(b);
        public static bool operator ==(T a, Option<T> b) => b.Equals(a);
        public static bool operator !=(T a, Option<T> b) => !b.Equals(a);
        public static bool operator ==(Option<T> a, T b) => a.Equals(b);
        public static bool operator !=(Option<T> a, T b) => !a.Equals(b);
    }
}
