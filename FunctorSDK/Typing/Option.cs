using System;
using System.Collections.Generic;
using System.Text;

namespace FunctorSDK.Typing;

/// <summary>
/// Represents the presence of a value in an Option type.
/// </summary>
/// <typeparam name="T">The type of data that may be present.</typeparam>
/// <param name="Value">The value that is present.</param>
public record Some<T>(T Value) where T : notnull;

/// <summary>
/// Represents the absence of a value in an Option type.
/// </summary>
public record None;

/// <summary>
/// Represents an optional value that can either be present (Some) or absent (None).
/// </summary>
/// <typeparam name="T">The type of data that may be present.</typeparam>
public readonly union Option<T>(Some<T>, None) :
    IUnwrapable<T>
    where T : notnull
{
    /// <summary>
    /// Indicates whether the Option contains a value (Some) or not (None).
    /// </summary>
    public bool IsSome => this is Some<T>;
    /// <summary>
    /// Indicates whether the Option does not contain a value (None).
    /// </summary>
    public bool IsNone => this is None;

    /// <summary>
    /// Unwraps the Option and returns the contained value if it is Some; otherwise, throws an InvalidOperationException if it is None.
    /// </summary>
    /// <returns>The contained value if the Option is Some.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Option is None.</exception>
    public T Unwrap() => this switch
    {
        Some<T> some => some.Value,
        None => throw new InvalidOperationException("Cannot unwrap a None value."),
    };

    /// <summary>
    /// Unwraps the Option and returns the contained value if it is Some; otherwise, returns the provided default value if it is None.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the Option is None.</param>
    /// <returns>The contained value if the Option is Some; otherwise, the provided default value.</returns>
    public T UnwrapOr(T defaultValue) => this switch
    {
        Some<T> some => some.Value,
        None => defaultValue,
    };

    /// <summary>
    /// Unwraps the Option and returns the contained value if it is Some; otherwise, invokes the provided function and returns its result if it is None.
    /// </summary>
    /// <param name="defaultValueFunc">The function to invoke and return its result if the Option is None.</param>
    /// <returns>The contained value if the Option is Some; otherwise, the result of the provided function.</returns>
    public T UnwrapOrElse(Func<T> defaultValueFunc) => this switch
    {
        Some<T> some => some.Value,
        None => defaultValueFunc(),
    };

    /// <summary>
    /// Returns the current Option if it is Some; otherwise, returns the provided Option if it is None.
    /// </summary>
    /// <param name="other">The Option to return if the current Option is None.</param>
    /// <returns>The current Option if it is Some; otherwise, the provided Option.</returns>
    public Option<T> Or(Option<T> other) => this switch
    {
        Some<T> some => some,
        None => other,
    };

    /// <summary>
    /// Returns the current Option if it is Some; otherwise, 
    /// invokes the provided handler function and returns its result if it is None.
    /// </summary>
    /// <param name="handler">The function to invoke if the Option is None.</param>
    /// <returns>The current Option if it is Some; otherwise, the result of the provided function.</returns>
    public Option<T> OrElse(Func<Option<T>> handler) => this switch
    {
        Some<T> some => some,
        None => handler(),
    };

    /// <summary>
    /// Applies the provided functions based on whether the Option is Some or None, and returns the result of the corresponding function.
    /// </summary>
    /// <typeparam name="TOut">The type of the result.</typeparam>
    /// <param name="someFunc">The function to invoke if the Option is Some.</param>
    /// <param name="noneFunc">The function to invoke if the Option is None.</param>
    /// <returns>The result of the corresponding function.</returns>
    public TOut Match<TOut>(Func<T, TOut> someFunc, Func<TOut> noneFunc) => this switch
    {
        Some<T> some => someFunc(some.Value),
        None => noneFunc(),
    };

    /// <summary>
    /// Applies the provided action to the contained value if the Option is Some, and returns the same Option.
    /// </summary>
    /// <remarks>
    /// Notes:<br/>
    /// - Modifications of either T's state as well as any state from the external scope can lead to unintended side effects.<br/>
    /// - It is recommended that you do not modify state within the action as it can lead to behavior that is hard to test and debug.
    /// </remarks>
    /// <param name="action">The function to apply to the contained value if the Option is Some.</param>
    /// <returns>The same Option.</returns>
    public Option<T> Effect(Action<T> action)
    {
        if (this is Some<T> some)
        {
            action(some.Value);
        }
        return this;
    }

    /// <summary>
    /// Applies the provided mapping function to the contained value if the Option is Some, 
    /// and returns a new Option with the mapped value; otherwise, returns None if the Option is None.
    /// </summary>
    /// <remarks>
    /// Notes:<br/>
    /// - Modifications of either T's state as well as any state from the external scope can lead to unintended side effects.<br/>
    /// - It is recommended that you do not modify state within the action as it can lead to behavior that is hard to test and debug.
    /// </remarks>
    /// <param name="mapFunc">The function to apply to the contained value if the Option is Some.</param>
    /// <returns>A new Option with the mapped value or None.</returns>
    public Option<TOut> Map<TOut>(Func<T, TOut> mapFunc) where TOut: notnull => this switch
    {
        Some<T> some => new Some<TOut>(mapFunc(some.Value)),
        None => new None(),
    };

    /// <summary>
    /// Applies the provided binding function to the contained value if the Option is Some, 
    /// and returns the result; otherwise, returns None if the Option is None.
    /// </summary>
    /// <remarks>
    /// Notes:<br/>
    /// - Modifications of either T's state as well as any state from the external scope can lead to unintended side effects.<br/>
    /// - It is recommended that you do not modify state within the action as it can lead to behavior that is hard to test and debug.
    /// </remarks>
    /// <param name="bindFunc">The function to apply to the contained value if the Option is Some.</param>
    /// <returns>The result of the binding function or None.</returns>
    public Option<TOut> Bind<TOut>(Func<T, Option<TOut>> bindFunc) where TOut: notnull => this switch
    {
        Some<T> some => bindFunc(some.Value),
        None => new None(),
    };

    /// <summary>
    /// Filters the Option based on the provided predicate. If the Option is Some and the predicate returns true for its value, 
    /// it returns the same Some; otherwise, it returns None.
    /// </summary>
    /// <param name="predicate">The predicate to apply to the contained value.</param>
    /// <returns>The filtered Option.</returns>
    public Option<T> Filter(Func<T, bool> predicate) => this switch
    {
        Some<T> some when predicate(some.Value) => some,
        _ => new None(),
    };

    /// <summary>
    /// Creates an Option<TFrom> from a nullable value type (ie. struct, int, etc.).
    /// </summary>
    /// <typeparam name="TFrom">The type of the value.</typeparam>
    /// <param name="value">The nullable value to convert.</param>
    /// <returns>An Option<TFrom> representing the value.</returns>
    public static Option<TFrom> FromNullableValue<TFrom>(TFrom? value) where TFrom : struct
    {
        if (value is not null and TFrom tValue)
        {
            return new Some<TFrom>(tValue);
        }
        else
        {
            return new None();
        }
    }

    /// <summary>
    /// Creates an Option<TFrom> from a nullable reference type (ie. class).
    /// </summary>
    /// <typeparam name="TFrom">The type of the reference value.</typeparam>
    /// <param name="value">The nullable reference value to convert.</param>
    /// <returns>An Option<TFrom> representing the value.</returns>
    public static Option<TFrom> FromNullableReference<TFrom>(TFrom? value) where TFrom : class
    {
        if (value is not null and TFrom tValue)
        {
            return new Some<TFrom>(tValue);
        }
        else
        {
            return new None();
        }
    }
}
