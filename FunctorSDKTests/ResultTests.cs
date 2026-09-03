using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FunctorSDK.Typing;
using FunctorSDK;

namespace FunctorSDKTests;

public class ResultTests
{
    /// <summary>
    /// Custom exception class for testing purposes.
    /// </summary>
    /// <seealso cref="Result_TryShouldReturnErrResult_WhenFuncThrowsCustomException"/>
    private sealed class CustomException(string message) : Exception(message);

    [Fact]
    public void Result_ShouldBeOk_WhenCreatedWithOkValue()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(42);
        // Act & Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public void Result_ShouldBeErr_WhenCreatedWithErrValue()
    {
        // Arrange
        Result<int, string> result = new Err<string>("Error");
        // Act & Assert
        Assert.True(result.IsErr);
    }

    [Fact]
    public void Result_ShouldNotBeOk_WhenCreatedWithErrValue()
    {
        // Arrange
        Result<int, string> result = new Err<string>("Error");
        // Act & Assert
        Assert.False(result.IsOk);
    }

    [Fact]
    public void Result_ShouldNotBeErr_WhenCreatedWithOkValue()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(42);
        // Act & Assert
        Assert.False(result.IsErr);
    }

    [Fact]
    public void Result_UnwrapShouldReturnValue_WhenResultIsOk()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(42);
        // Act
        int value = result.Unwrap();
        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void Result_UnwrapShouldThrowException_WhenResultIsErr()
    {
        // Arrange
        Result<int, string> result = new Err<string>("Error");
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public void Result_UnwrapOrShouldReturnValue_WhenResultIsOk()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(42);
        // Act
        int value = result.UnwrapOr(0);
        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void Result_UnwrapOrShouldReturnDefaultValue_WhenResultIsErr()
    {
        // Arrange
        Result<int, string> result = new Err<string>("Error");
        // Act
        int value = result.UnwrapOr(0);
        // Assert
        Assert.Equal(0, value);
    }

    [Fact]
    public void Result_UnwrapOrElseShouldReturnValue_WhenResultIsOk()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(42);
        // Act
        int value = result.UnwrapOrElse(() => 0);
        // Assert
        Assert.Equal(42, value);
    }

    [Fact]
    public void Result_UnwrapOrElseShouldInvokeFunc_WhenResultIsErr()
    {
        // Arrange
        Result<int, string> result = new Err<string>("Error");
        // Act
        int value = result.UnwrapOrElse(() => 0);
        // Assert
        Assert.Equal(0, value);
    }

    [Fact]
    public void Result_MatchShouldInvokeOnlyOkAction_WhenResultIsOk()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(42);
        bool okActionInvoked = false;
        bool errActionInvoked = false;
        // Act
        result.Match(
            ok => { okActionInvoked = true; },
            err => { errActionInvoked = true; }
        );
        // Assert
        Assert.True(okActionInvoked);
        Assert.False(errActionInvoked);
    }

    [Fact]
    public void Result_MatchShouldInvokeOnlyErrAction_WhenResultIsErr()
    {
        // Arrange
        Result<int, string> result = new Err<string>("Error");
        bool okActionInvoked = false;
        bool errActionInvoked = false;
        // Act
        result.Match(
            ok => { okActionInvoked = true; },
            err => { errActionInvoked = true; }
        );
        // Assert
        Assert.False(okActionInvoked);
        Assert.True(errActionInvoked);
    }

    [Fact]
    public void Result_MatchShouldProvideOkValue_WhenResultIsOk()
    {
        // Arrange
        int intialValue = 43;
        Result<int, string> result = new Ok<int>(intialValue);
        int updateValue = 0;
        // Act
        result.Match(
            ok => { updateValue = ok.Value; },
            err => { }
            );
        // Assert
        Assert.Equal(intialValue, updateValue);
    }

    [Fact]
    public void Result_MatchShouldProvideErrValue_WhenResultIsErr()
    {
        // Arrange
        string initialValue = "Error";
        Result<int, string> result = new Err<string>(initialValue);
        string updateValue = string.Empty;
        // Act
        result.Match(
            ok => { },
            err => { updateValue = err.Error; }
            );
        // Assert
        Assert.Equal(initialValue, updateValue);
    }
}