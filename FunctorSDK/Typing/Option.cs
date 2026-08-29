using System;
using System.Collections.Generic;
using System.Text;

namespace FunctorSDK.Typing;

public record Some<T>(T Value) where T : notnull;

public record None;

public readonly union Option<T>(Some<T>, None) :
    IUnwrapable<T>,
    IFrom<Option<T>, object?>
    where T : notnull
{
    public bool IsSome => this is Some<T>;
    public bool IsNone => this is None;

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

    public T Unwrap() => this switch
    {
        Some<T> some => some.Value,
        None => throw new InvalidOperationException("Cannot unwrap a None value."),
    };

    public T UnwrapOr(T defaultValue) => this switch
    {
        Some<T> some => some.Value,
        None => defaultValue,
    };

    public T UnwrapOrElse(Func<T> defaultValueFunc) => this switch
    {
        Some<T> some => some.Value,
        None => defaultValueFunc(),
    };

    public static Option<T> From(object? value) => value switch
    {
        null => new None(),
        T tValue => new Some<T>(tValue),
        _ => throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to Option<{typeof(T)}>"),
    };
}
