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
public record Err<T>(T Value) where T : notnull;

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
    public T Unwrap() => this switch
    {
        Ok<T> ok => ok.Value,
        Err<K> => throw new InvalidOperationException("Cannot unwrap an Err value.")
    };

    /// <summary>
    /// Unwraps the Result and returns the error value if it is Err; otherwise,
    /// </summary>
    /// <returns>The error value if the Result is Err.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Result is Ok.</exception>
    public K UnwrapErr() => this switch
    {
        Err<K> err => err.Value,
        Ok<T> => throw new InvalidOperationException("Cannot unwrap an Ok value.")
    };

    /// <summary>
    /// Unwraps the Result and returns the success value if it is Ok; otherwise, 
    /// returns the provided default value if it is Err.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the Result is Err.</param>
    /// <returns>The success value if the Result is Ok; otherwise, the provided default value.</returns>
    public T UnwrapOr(T defaultValue) => this switch
    {
        Ok<T> ok => ok.Value,
        Err<K> => defaultValue
    };

    /// <summary>
    /// Returns value T if present, else executes a provided function that
    /// returns a value of type T.
    /// </summary>
    /// <param name="defaultValueFunc">A function that returns a default value of type T.</param>
    /// <returns>Value of type T if present, else executes function that returns a default value of type T.</returns>
    public T UnwrapOrElse(Func<T> defaultValueFunc) => this switch
    {
        Ok<T> ok => ok.Value,
        Err<K> => defaultValueFunc()
    };

    /// <summary>
    /// Matches the Result and executes the appropriate function based on whether it is Ok or Err.
    /// </summary>
    /// <typeparam name="TOut">The type of the output value.</typeparam>
    /// <param name="onOk">The function to execute if the Result is Ok.</param>
    /// <param name="onErr">The function to execute if the Result is Err.</param>
    /// <returns>The result of the executed function.</returns>
    public TOut Match<TOut>(Func<T, TOut> onOk, Func<K, TOut> onErr) => this switch
    {
        Ok<T> ok => onOk(ok.Value),
        Err<K> err => onErr(err.Value)
    };

    /// <summary>
    /// Transforms the value of T into a value of type TOut, using the mapper function.
    /// </summary>
    /// <typeparam name="TOut">The type to return from the mapper function.</typeparam>
    /// <param name="mapper">The function that handles that transformation.</param>
    /// <returns>The new Result, with the value of type T transformed to a value of type TOut.</returns>
    public Result<TOut, K> Map<TOut>(Func<T, TOut> mapper) where TOut : notnull => this switch
    {
        Ok<T> ok => new Ok<TOut>(mapper(ok.Value)),
        Err<K> err => new Err<K>(err.Value),
    };

    /// <summary>
    /// Transforms the value of K to a value of type KOut, using the mapper function.
    /// </summary>
    /// <typeparam name="KOut">The type to return from the mapper function.</typeparam>
    /// <param name="mapper">The function that handles the transformation.</param>
    /// <returns>The new Result, with the value of type K transformed to a value of type KOut.</returns>
    public Result<T, KOut> MapErr<KOut>(Func<K, KOut> mapper) where KOut : notnull => this switch
    {
        Ok<T> ok => new Ok<T>(ok.Value),
        Err<K> err => new Err<KOut>(mapper(err.Value)),
    };

    /// <summary>
    /// Applies a function to the current result if the value is Ok,
    /// returning the Result returned from the applied function,
    /// or the current Err value if present.
    /// </summary>
    /// <param name="bindFunc">
    /// The function returning the Result to apply to the current Result.
    /// </param>
    /// <typeparam name="TOut">
    /// The type of the Ok value of the Result Returned from the applied function.
    /// </typeparam>
    /// <returns>
    /// The result returned from the applied function, or the current Err if it is present.
    /// </returns>
    public Result<TOut, K> Bind<TOut>(Func<T, Result<TOut, K>> bindFunc) where TOut : notnull => this switch
    {
        Ok<T> ok => bindFunc(ok.Value),
        Err<K> err => new Err<K>(err.Value),
    };

    /// <summary>
    /// Applies a function to the current result if the value is Err,
    /// returning the Result returned from the applied function,
    /// or the current Err value if present.
    /// </summary>
    /// <param name="bindFunc">
    /// The function returning the Result to apply to the current Result.
    /// </param>
    /// <typeparam name="KOut">
    /// The type of the Err value of the Result Returned from the applied function.
    /// </typeparam>
    /// <returns>
    /// The result returned from the applied function, or the current Ok value if it is present.
    /// </returns>
    public Result<T, KOut> BindErr<KOut>(Func<K, Result<T, KOut>> bindFunc) where KOut : notnull => this switch
    {
        Ok<T> ok => new Ok<T>(ok.Value),
        Err<K> err => bindFunc(err.Value),
    };

    /// <summary>
    /// Executes a side effect when the Result is Ok.
    /// </summary>
    /// <param name="action">
    /// The function that takes in an arg of the Ok value, and performs side effect(s).
    /// </param>
    /// <returns>The current result.</returns>
    public Result<T, K> Effect(Action<T> action)
    {
        if (this is Ok<T> ok)
        {
            action(ok.Value);
        }

        return this;
    }

    /// <summary>
    /// Executes a side effect when the result is an Err.
    /// </summary>
    /// <param name="action">
    /// The function that takes in the Err value, and performs side effect(s).
    /// </param>
    /// <returns>The current result.</returns>
    public Result<T, K> EffectErr(Action<K> action)
    {
        if (this is Err<K> err)
        {
            action(err.Value);
        }

        return this;
    }

    /// <summary>
    /// Converts the Result into an option where if Ok the value becomes Some(okValue),
    /// or if Err becomes None.
    /// </summary>
    /// <returns>Some if the Result is Ok, None if it is an Err.</returns>
    public Option<T> ToOption() => this switch
    {
        Ok<T> ok => new Some<T>(ok.Value),
        Err<K> => new None(),
    };

    /// <summary>
    /// Converts the Result into an option where the Err value becomes Some(errValue),
    /// or if Ok becomes None.
    /// </summary>
    /// <returns>None if the value is OK, else returns Some with the value of the Err.</returns>
    public Option<K> ToErrOption() => this switch
    {
        Ok<T> => new None(),
        Err<K> err => new Some<K>(err.Value)
    };
}