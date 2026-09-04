using FunctorSDK.Typing;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctorSDK;

public class Exceptable<T>
    : IUnwrapable<T>
    where T : notnull
{
    private Result<T, Exception> _InitialResult { get; init; }

    internal Exceptable(Result<T, Exception> result)
    {
        _InitialResult = result;
    }

    public bool IsOk => _InitialResult is Ok<T>;
    public bool IsErr => _InitialResult is Err<Exception>;
    public bool IsException<TException>() where TException : Exception
    {
        if (_InitialResult is Err<Exception> err && err.Error is TException)
        {
            return true;
        }
        return false;
    }

    public Option<TException> GetException<TException>() where TException : Exception
    {
        if (_InitialResult is Err<Exception> err && err.Error is TException ex)
        {
            return new Some<TException>(ex);
        }
        return new None();
    }
    
    public Exceptable<T> Catch<TException>(Func<TException, T> handler) where TException : Exception
    {
        if (_InitialResult is Err<Exception> err && err.Error is TException ex)
        {
            var newResult = handler(ex);
            return new Exceptable<T>(new Ok<T>(newResult));
        }
        return this;
    }

    public Exceptable<T> Finally(Action<Result<T, Exception>> action)
    {
        action(_InitialResult);
        return this;
    }

    public Option<T> ToValue()
    {
        return _InitialResult switch
        {
            Ok<T> ok => new Some<T>(ok.Value),
            Err<Exception> _ => new None(),
            _ => new None()
        };
    }

    public Result<T, Exception> ToResult()
    {
        return _InitialResult;
    }

    public T Unwrap()
    {
        return _InitialResult switch
        {
            Ok<T> ok => ok.Value,
            Err<Exception> err => throw err.Error,
        };
    }

    public T UnwrapOr(T defaultValue)
    {
        return _InitialResult switch
        {
            Ok<T> ok => ok.Value,
            Err<Exception> _ => defaultValue,
        };
    }

    public T UnwrapOrElse(Func<T> defaultValueFunc)
    {
        return _InitialResult switch
        {
            Ok<T> ok => ok.Value,
            Err<Exception> _ => defaultValueFunc(),
        };
    }
}

/// <summary>
/// Provides static methods for instantiating Exceptable objects.
/// </summary>
public static class Exceptable
{
    /// <summary>
    /// Attempts to execute the provided function and returns an Exceptable 
    /// object that encapsulates the result or any exception that may occur.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the function.</typeparam>
    /// <param name="func">The function to execute.</param>
    /// <returns>
    /// An Exceptable object encapsulating the result or any exception that may occur.
    /// </returns>
    public static Exceptable<T> Try<T>(Func<T> func) where T : notnull
    {
        try
        {
            var result = func();
            return new Exceptable<T>(new Ok<T>(result));
        }
        catch (Exception ex)
        {
            return new Exceptable<T>(new Err<Exception>(ex));
        }
    }
}
