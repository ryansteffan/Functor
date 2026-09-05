using System;
using System.Collections.Generic;
using System.Text;
using FunctorSDK.Typing;

namespace FunctorSDKTests;


/// <summary>
/// Mutable number for testing reference type behavior.
/// </summary>
class MutableNumber
{
    public int Value { get; set; }
    public MutableNumber(int value)
    {
        Value = value;
    }
};

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
    public void Option_OrShouldReturnOption_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        Option<int> result = option.Or(new Some<int>(0));
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Option_OrShouldReturnOtherOption_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        // Act
        Option<int> result = option.Or(new Some<int>(0));
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(0, result.Unwrap());
    }

    [Fact]
    public void Option_OrElseShouldReturnOption_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        Option<int> result = option.OrElse(() => new Some<int>(0));
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Option_OrElseShouldInvokeFuncAndReturnResult_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        // Act
        Option<int> result = option.OrElse(() => new Some<int>(0));
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(0, result.Unwrap());
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
            () => noneActionInvoked = true
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
            () => noneActionInvoked = true
        );
        // Assert
        Assert.False(someActionInvoked);
        Assert.True(noneActionInvoked);
    }

    [Fact]
    public void Option_MatchShouldReturnValueFromSomeFunc_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        int result = option.Match(
            some => some * 2,
            () => 0
        );
        // Assert
        Assert.Equal(84, result);
    }

    [Fact]
    public void Option_MatchShouldReturnValueFromNoneFunc_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        // Act
        int result = option.Match(
            some => some * 2,
            () => 0
        );
        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Option_EffectShouldInvokeAction_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        bool actionInvoked = false;
        // Act
        option.Effect(value => actionInvoked = true);
        // Assert
        Assert.True(actionInvoked);
    }

    [Fact]
    public void Option_EffectShouldNotInvokeAction_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        bool actionInvoked = false;
        // Act
        option.Effect(value => actionInvoked = true);
        // Assert
        Assert.False(actionInvoked);
    }

    [Fact]
    public void Option_EffectShouldReturnSameOption_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        Option<int> result = option.Effect(value => { });
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Option_EffectShouldMutateExternalState_WhenOptionIsSome()
    {
        // Arrange
        int value = 42;
        Option<int> option = new Some<int>(value);
        // Act
        option.Effect(v => value = v + 1);
        // Assert
        Assert.Equal(43, value);
    }

    [Fact]
    public void Option_EffectShouldNotMutateInternalState_WhenOptionIsSomeValueType()
    {
        // Arrange
        int value = 42;
        Option<int> option = new Some<int>(value);
        // Act
        option.Effect(v => v++);
        // Assert
        Assert.Equal(value, option.Unwrap());
    }

    [Fact]
    public void Option_EffectShouldMutateInternalState_WhenOptionIsSomeReferenceType()
    {
        // Arrange
        MutableNumber number = new MutableNumber(42);
        Option<MutableNumber> option = new Some<MutableNumber>(number);
        // Act
        option.Effect(n => n.Value++);
        // Assert
        Assert.Equal(43, option.Unwrap().Value);
    }

    [Fact]
    public void Option_MapShouldReturnMappedOption_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        Option<string> result = option.Map(value => $"Value is {value}");
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal("Value is 42", result.Unwrap());
    }

    [Fact]
    public void Option_MapShouldReturnNone_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        // Act
        Option<string> result = option.Map(value => $"Value is {value}");
        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Option_MapShouldNotMutateInternalState_WhenOptionIsSomeValueType()
    {
        // Arrange
        int value = 42;
        Option<int> option = new Some<int>(value);
        // Act
        option.Map(v => v + 1);
        // Assert
        Assert.Equal(value, option.Unwrap());
    } 

    [Fact]
    public void Option_MapShouldNotMutateInternalState_WhenOptionIsSomeReferenceType()
    {
        // Arrange
        MutableNumber number = new MutableNumber(42);
        Option<MutableNumber> option = new Some<MutableNumber>(number);
        // Act
        option.Map(n => n.Value + 1);
        // Assert
        Assert.Equal(42, option.Unwrap().Value);
    }

    [Fact]
    public void Option_BindShouldReturnBoundOption_WhenOptionIsSome()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        Option<string> result = option
            .Bind<string>(value => new Some<string>($"Value is {value}"));
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal("Value is 42", result.Unwrap());
    }

    [Fact]
    public void Option_BindShouldReturnNone_WhenOptionIsNone()
    {
        // Arrange
        Option<int> option = new None();
        // Act
        Option<string> result = option
            .Bind<string>(value => new Some<string>($"Value is {value}"));
        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Option_BindShouldNotMutateInternalState_WhenOptionIsSomeValueType()
    {
        // Arrange
        int value = 42;
        Option<int> option = new Some<int>(value);
        // Act
        option.Bind<int>(v => new Some<int>(v + 1));
        // Assert
        Assert.Equal(value, option.Unwrap());
    }

    [Fact]
    public void Option_BindShouldNotMutateInternalState_WhenOptionIsSomeReferenceType()
    {
        // Arrange
        MutableNumber number = new MutableNumber(42);
        Option<MutableNumber> option = new Some<MutableNumber>(number);
        // Act
        option.Bind<MutableNumber>(n =>
        {
            return new Some<MutableNumber>(new MutableNumber(n.Value + 1));
        });
        // Assert
        Assert.Equal(42, option.Unwrap().Value);
    }

    [Fact]
    public void Option_FilterShouldReturnSome_WhenPredicateIsTrue()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        Option<int> result = option.Filter(value => value > 0);
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Option_FilterShouldReturnNone_WhenPredicateIsFalse()
    {
        // Arrange
        Option<int> option = new Some<int>(42);
        // Act
        Option<int> result = option.Filter(value => value < 0);
        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Option_FromNullableValue_ShouldReturnSome_WhenValueIsNotNull()
    {
        // Arrange
        int? nullableValue = 42;
        // Act
        Option<int> result = Option<int>.FromNullableValue(nullableValue);
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(42, result.Unwrap());
    }

    [Fact]
    public void Option_FromNullableValue_ShouldReturnNone_WhenValueIsNull()
    {
        // Arrange
        int? nullableValue = null;
        // Act
        Option<int> result = Option<int>.FromNullableValue(nullableValue);
        // Assert
        Assert.True(result.IsNone);
    }

    [Fact]
    public void Option_FromNullableReference_ShouldReturnSome_WhenReferenceTypeIsNotNull()
    {
        // Arrange
        MutableNumber? nullableValue = new MutableNumber(42);
        // Act
        Option<MutableNumber> result = Option<MutableNumber>.FromNullableReference(nullableValue);
        // Assert
        Assert.True(result.IsSome);
        Assert.Equal(42, result.Unwrap().Value);
    }

    [Fact]
    public void Option_FromNullableReference_ShouldReturnNone_WhenReferenceTypeIsNull()
    {
        // Arrange
        MutableNumber? nullableValue = null;
        // Act
        Option<MutableNumber> result = 
            Option<MutableNumber>.FromNullableReference(nullableValue);
        // Assert
        Assert.True(result.IsNone);
    }
}
