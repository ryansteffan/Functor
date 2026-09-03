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
    /// Executes the provided actions based on whether the Option is Some or None.
    /// </summary>
    /// <param name="someAction">The action to execute if the Option is Some.</param>
    /// <param name="noneAction">The action to execute if the Option is None.</param>
    public void Match(Action<Some<T>> someAction, Action<None> noneAction)
    {
        switch (this)
        {
            case Some<T> some:
                someAction(some);
                break;
            case None none:
                noneAction(none);
                break;
        }
    }

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
    /// Creates an Option<T> from a reference type value (ie. class). 
    /// If the value is null, it returns None; otherwise, it returns Some<T> with the value.
    /// </summary>
    /// <param name="value">The reference type value to convert to an Option<T>.</param>
    /// <returns>An Option<T> representing the value.</returns>
    /// <exception cref="InvalidCastException">Thrown if the value cannot be cast to the specified type T.</exception>
    public static Option<T> FromRefType(object? value) => value switch
    {
        null => new None(),
        T tValue => new Some<T>(tValue),
        _ => throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to Option<{typeof(T)}>"),
    };
}
