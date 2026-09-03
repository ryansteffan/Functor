using System;
using System.Collections.Generic;
using System.Text;
using FunctorSDK.Typing;
using FunctorSDK;

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
        var handledResult = result.Catch<InvalidOperationException>(ex => new Ok<None>(new None()));
        // Assert
        Assert.True(handledResult.IsOk);
    }

    [Fact]
    public void Catch_DoesNotHandleException_WhenExceptionDoNotMatch()
    {
        // Arrange
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        // Act
        var handledResult = result.Catch<InvalidCastException>(ex => new Ok<None>(new None()));
        // Assert
        Assert.True(handledResult.IsErr);
    }

    [Fact]
    public void Catch_HandlesException_WhenExceptionMatchesBaseType()
    {
        // Arrange
        var result = Exceptable.Try<None>(() => throw new InvalidOperationException("error"));
        // Act
        var handledResult = result.Catch<Exception>(ex => new Ok<None>(new None()));
        // Assert
        Assert.True(handledResult.IsOk);
    }

    [Fact]
    public void Catch_ChainStops_When
}
