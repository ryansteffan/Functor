using System;
using System.Collections.Generic;
using System.Text;
using FunctorSDK.Typing;
using FunctorSDK;
using System.Text.RegularExpressions;

namespace FunctorSDKTests;

public class ExceptableTests
{
    [Fact]
    public void Try_ReturnsOk_WhenNoException()
    {
        var result = Exceptable.Try(() => 42);
        Assert.True(result.IsOk);
        Assert.Equal(42, result.ToValue().Unwrap());
    }

    [Fact]
    public void Try_ReturnsErr_WhenExceptionThrown()
    {
        var result = Exceptable.Try<int>(() => throw new InvalidOperationException("Test exception"));
        Assert.True(result.IsErr);
        Assert.True(result.IsException<InvalidOperationException>());
    }

    [Fact]
    public void IsOk_ReturnsTrue_WhenTryDoesNotThrow()
    {
        // Act & Assert
        var result = Exceptable.Try(() => 0);
        Assert.True(result.IsOk);
        Assert.False(result.IsErr);
    }

    [Fact]
    public void IsErr_ReturnsErr_WhenTryThrowsException()
    {
        // Act & Assert
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("Error"));
        Assert.True(result.IsErr);
        Assert.False(result.IsOk);
    }

    [Fact]
    public void IsException_ReturnsTrue_WhenExceptionMatches()
    {
        // Arrange & Act
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        var isException = result.IsException<InvalidOperationException>();
        // Assert
        Assert.True(isException);

    }

    [Fact]
    public void IsException_ReturnsTrue_WhenExceptionMatchesBaseType()
    {
        // Arrange & Act
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        var isException = result.IsException<Exception>(); // Test a catch all senerio
        // Assert
        Assert.True(isException);
    }

    [Fact]
    public void IsException_ReturnsFalse_WhenExceptionDoNotMatch()
    {
        // Arrange & Act
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        var isException = result.IsException<InvalidCastException>();
        // Assert
        Assert.False(isException);
    }

    [Fact]
    public void Catch_HandlesException_WhenExceptionMatches()
    {
        // Arrange
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        // Act
        var handledResult = result.Catch<InvalidOperationException>(ex => new None());
        // Assert
        Assert.True(handledResult.IsOk);
    }

    [Fact]
    public void Catch_DoesNotHandleException_WhenExceptionDoNotMatch()
    {
        // Arrange
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        // Act
        var handledResult = result.Catch<InvalidCastException>(ex => new None());
        // Assert
        Assert.True(handledResult.IsErr);
    }

    [Fact]
    public void Catch_HandlesException_WhenExceptionMatchesBaseType()
    {
        // Arrange
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        // Act
        var handledResult = result.Catch<Exception>(ex => new None());
        // Assert
        Assert.True(handledResult.IsOk);
    }

    [Fact]
    public void Catch_ChainStops_WhenExceptionHasBeenCaught()
    {
        // Arrange
        var isInvalid = false;
        var isException = false;
        // Act
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"))
            .Catch<InvalidOperationException>(ex => { isInvalid = true; return new None(); })
            .Catch<Exception>(ex => { isException = true; return new None(); });
        // Assert
        Assert.True(isInvalid);
        Assert.False(isException);
    }

    [Fact]
    public void Catch_ChainSkips_WhenExceptionDoesNotMatch()
    {
        // Arrange
        var isParser = false;
        var isInvalid = false;
        // Act
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"))
            .Catch<RegexParseException>(ex => { isParser = true; return new None(); })
            .Catch<InvalidOperationException>(ex => { isInvalid = true; return new None(); });
        // Assert
        Assert.True(isInvalid);
        Assert.False(isParser);
    }

    // TODO: Finish catch tests

    [Fact]
    public void Finally_IsRun_AferExceptionTakesPlace()
    {
        // Arrange
        var isFinallyRun = false;
        // Act
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"))
            .Catch<InvalidOperationException>(ex => { isFinallyRun = true; return new None(); })
            .Finally(result => { isFinallyRun = true; }); 
        // Assert
        Assert.True(isFinallyRun);
    }

    [Fact]
    public void Finally_IsRun_AferNoExceptionTakesPlace()
    {
        // Arrange
        var isFinallyRun = false;
        // Act
        var result = Exceptable.Try<None>(() => new None())
            .Finally(result => { isFinallyRun = true; });
        // Assert
        Assert.True(isFinallyRun);
    }

    [Fact]
    public void Finally_RevicesFinalResult_AfterNoCatchesTakePlaceAndExceptionThrown()
    {
        // Arrange
        var gotValue = false;
        var gotException = false;
        // Act
        var result = Exceptable.Try<None>(() => throw new InvalidCastException("error"))
            .Finally(result =>
            {
                var output = result switch
                {
                    Ok<None> => gotValue = true,
                    Err<Exception> => gotException = true,
                };
            });
        // Assert 
        Assert.False(gotValue);
        Assert.True(gotException);
    }

    [Fact]
    public void Finally_RevicesFinalResult_AfterCatchesTakePlaceAndExceptionNoThrown()
    {
        // Arrange
        var gotValue = false;
        var gotException = false;
        // Act
        var result = Exceptable.Try<None>(() => new None())
            .Finally(result =>
            {
                var output = result switch
                {
                    Ok<None> => gotValue = true,
                    Err<Exception> => gotException = true,
                };
            });
        // Assert 
        Assert.True(gotValue);
        Assert.False(gotException);
    }


    [Fact]
    public void ToValue_ReturnsSome_WhenExceptionHasBeenHandled()
    {
        // Arrange & Act
        var result = Exceptable.Try<None>(() => throw new InvalidCastException("error"))
            .Catch<InvalidCastException>(ex => new None())
            .ToValue();
        // Assert
        result.Match(
            value => { },
            err => { Assert.Fail(); }
            );
    }

    [Fact]
    public void ToValue_ReturnsNone_WhenExceptionHasNotBeenBeenHandled()
    {
        // Arrange & Act
        var result = Exceptable.Try<None>(() => throw new InvalidCastException("error"))
            .ToValue();
        // Assert
        result.Match(
            value => { Assert.Fail(); },
            err => { }
            );
    }

    // TODO: Finish tests afer update to Option/Result.
}
