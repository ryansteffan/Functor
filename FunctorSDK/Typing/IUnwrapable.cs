using System;
using System.Collections.Generic;
using System.Text;

namespace FunctorSDK.Typing;

/// <summary>
/// Represents an object that can be unwrapped to retrieve its underlying value of type T.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
public interface IUnwrapable<T>
{
    /// <summary>
    /// Unwraps the value of the object, throwing an exception if the value is not present.
    /// </summary>
    /// <returns>The unwrapped value.</returns>
    public T Unwrap();

    /// <summary>
    /// Unwraps the value of the object, returning the provided default value if the value is not present.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the value is not present.</param>
    /// <returns>The unwrapped value or the default value.</returns>
    public T UnwrapOr(T defaultValue);

    /// <summary>
    /// Unwraps the value of the object, returning the result of the provided function if the value is not present.
    /// </summary>
    /// <param name="defaultValueFunc">A function that provides the default value if the value is not present.</param>
    /// <returns>The unwrapped value or the result of the default value function.</returns>
    public T UnwrapOrElse(Func<T> defaultValueFunc);
}

