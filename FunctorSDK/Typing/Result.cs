using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FunctorSDK.Typing;

/// <summary>
/// Represents a successful result.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <param name="Value">The success value.</param>
public record Ok<T>(T Value) where T : notnull;

/// <summary>
/// Represents an error result.
/// </summary>
/// <typeparam name="T">The type of the error value.</typeparam>
/// <param name="Value">The error value.</param>
public record Err<T>(T Error) where T : notnull;

/// <summary>
/// Represents a result that can either be a success (Ok) or an error (Err).
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="K">The type of the error value.</typeparam>
public readonly union Result<T, K>(Ok<T>, Err<K>) : 
    IUnwrapable<T>
    where T : notnull 
    where K : notnull
{
    /// <summary>
    /// Indicates whether the Result is a success (Ok) or an error (Err).
    /// </summary>
    public bool IsOk => this is Ok<T>;

    /// <summary>
    /// Indicates whether the Result is an error (Err) or a success (Ok).
    /// </summary>
    public bool IsErr => this is Err<K>;

    /// <summary>
    /// Unwraps the Result and returns the success value if it is Ok; otherwise, 
    /// throws an InvalidOperationException if it is Err.
    /// </summary>
    /// <returns>The success value if the Result is Ok.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Result is Err.</exception>
    public T Unwrap()
    {
        if (this is Ok<T> ok)
        {
            return ok.Value;
        }
        throw new InvalidOperationException("Cannot unwrap an Err value.");
    }

    /// <summary>
    /// Unwraps the Result and returns the success value if it is Ok; otherwise, 
    /// returns the provided default value if it is Err.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the Result is Err.</param>
    /// <returns>The success value if the Result is Ok; otherwise, the provided default value.</returns>
    public T UnwrapOr(T defaultValue)
    {
        if (this is Ok<T> ok)
        {
            return ok.Value;
        }
        return defaultValue;
    }

    /// <summary>
    /// Unwraps the Result and returns the success value if it is Ok; otherwise, 
    /// returns the value produced by the provided function if it is Err.
    /// </summary>
    /// <param name="defaultValueFunc">The function to produce a default value if the Result is Err.</param>
    /// <returns>The success value if the Result is Ok; otherwise, the value produced by the provided function.</returns>
    public T UnwrapOrElse(Func<T> defaultValueFunc)
    {
        if (this is Ok<T> ok)
        {
            return ok.Value;
        }
        return defaultValueFunc();
    }

    /// <summary>
    /// Matches the Result and executes the appropriate action based on whether it is Ok or Err.
    /// </summary>
    /// <param name="okAction">The action to execute if the Result is Ok.</param>
    /// <param name="errAction">The action to execute if the Result is Err.</param>
    public void Match(Action<Ok<T>> okAction, Action<Err<K>> errAction)
    {
        switch (this)
        {
            case Ok<T> ok:
                okAction(ok);
                break;
            case Err<K> err:
                errAction(err);
                break;
        }
    }
}