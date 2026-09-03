using System;
using System.Collections.Generic;
using System.Text;
using FunctorSDK.Typing;

namespace FunctorSDKTests;

public class OptionTests
{
    [Fact]
    public void Option_ShouldBeSome_WhenCreatedWithValue()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act & Assert
        Assert.True(option.IsSome);
    }

    [Fact]
    public void Option_ShouldBeNone_WhenCreatedWithoutValue()
    {
        // Arrange
        Option<int> option = new None();
        // Act & Assert
        Assert.True(option.IsNone);
    }

    [Fact]
    public void Option_ShouldNotBeSome_WhenCreatedWithoutValue()
    {
        // Arrange
        Option<int> option = new None();
        // Act & Assert
        Assert.False(option.IsSome);
    }

    [Fact]
    public void Option_ShouldNotBeNone_WhenCreatedWithValue()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act & Assert
        Assert.False(option.IsNone);
    }

    [Fact]
    public void Option_MatchShouldInvokeOnlySomeAction_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        bool someActionInvoked = false;
        bool noneActionInvoked = false;
        // Act
        option.Match(
            some => someActionInvoked = true,
            none => noneActionInvoked = true
        );
        // Assert
        Assert.True(someActionInvoked);
        Assert.False(noneActionInvoked);
    }

    [Fact]
    public void Option_MatchShouldInvokeOnlyNoneAction_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        bool someActionInvoked = false;
        bool noneActionInvoked = false;
        // Act
        option.Match(
            some => someActionInvoked = true,
            none => noneActionInvoked = true
        );
        // Assert
        Assert.False(someActionInvoked);
        Assert.True(noneActionInvoked);
    }

    [Fact]
    public void Option_UnwrapShouldReturnValue_WhenOptionIsSome()
    {
        // Arrange
        int data = 42;
        Option<int> option = new Some<int>(data);
        // Act
        int value = option.Unwrap();
        // Assert
        Assert.Equal(data, value);
    }

    [Fact]
    public void Option_UnwrapShouldThrowInvalidOperationException_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => option.Unwrap());
    }

    [Fact]
    public void Option_UnwrapOrShouldReturnValue_WhenOptionIsSome()
    {
        // Arrange
        int data = 42;
        Option<int> option = new Some<int>(data);
        // Act
        int value = option.UnwrapOr(0);
        // Assert
        Assert.Equal(data, value);
    }

    [Fact]
    public void Option_UnwrapOrShouldReturnDefaultValue_WhenOptionIsNone()
    {
        // Arrange
        int defaultValue = 0;
        Option<int> option = new None();
        // Act
        int value = option.UnwrapOr(defaultValue);
        // Assert
        Assert.Equal(defaultValue, value);
    }

    [Fact]
    public void Option_UnwrapOrElseShouldReturnValue_WhenOptionIsSome()
    {
        // Arrange
        int data = 42;
        Option<int> option = new Some<int>(data);
        // Act
        int value = option.UnwrapOrElse(() => 0);
        // Assert
        Assert.Equal(data, value);
    }

    [Fact]
    public void Option_UnwrapOrElseShouldInvokeFunc_WhenOptionIsNone()
    {
        // Arrange
        int defaultValue = 0;
        Option<int> option = new None();
        // Act
        int value = option.UnwrapOrElse(() => defaultValue);
        // Assert
        Assert.Equal(defaultValue, value);
    }

    [Fact]
    public void Option_FromRefTypeShouldReturnSome_WhenValueIsNotNull()
    {
        // Arrange
        int data = 42;
        // Act
        Option<int> option = Option<int>.FromRefType(data);
        // Assert
        Assert.True(option.IsSome);
    }

    [Fact]
    public void Option_FromRefTypeShouldReturnNone_WhenValueIsNull()
    {
        // Arrange
        String? data = null; // Use string refernece type.
        // Act
        Option<string> option = Option<string>.FromRefType(data);
        // Assert
        Assert.True(option.IsNone);
    }

    [Fact]
    public void Option_FromRefTypeShouldThrowInvalidCastException_WhenValueIsOfDifferentType()
    {
        // Arrange
        string data = "Hello"; // Use string type.
        // Act & Assert
        // Attempt to create an int option with string
        Assert.Throws<InvalidCastException>(() => Option<int>.FromRefType(data));
    }
}
