using FunctorSDK.Typing;

namespace FunctorSDKTests;

public class ResultTests
{
    [Fact]
    public void Result_IsOk_WhenResultIsOk()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(1);
        // Act and Assert
        Assert.True(result.IsOk);
    }

    [Fact]
    public void Result_IsNotOk_WhenResultIsNotOk()
    {
        // Arrange
        Result<int, string> result = new Err<string>("error");
        // Act and Assert
        Assert.False(result.IsOk);
    }

    [Fact]
    public void Result_IsErr_WhenResultIsErr()
    {
        // Arrange
        Result<int, string> result = new Err<string>("error");
        // Act and Assert
        Assert.True(result.IsErr);
    }

    [Fact]
    public void Result_IsNotErr_WhenResultIsNotErr()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(1);
        // Act and Assert
        Assert.False(result.IsErr);
    }

    [Fact]
    public void Unwrap_WhenOk_ReturnsOkValue()
    {
        // Arrange
        const int initialValue = 1;
        Result<int, string> result = new Ok<int>(initialValue);
        // Act
        var value = result.Unwrap();
        // Assert
        Assert.Equal(initialValue, value);
    }

    [Fact]
    public void Unwrap_WhenErr_ThrowsInvalidOperationException()
    {
        // Arrange
        Result<int, string> result = new Err<string>("error");

        //  Act and Assert
        Assert.Throws<InvalidOperationException>(() => result.Unwrap());
    }

    [Fact]
    public void UnwrapErr_WhenOk_ThrowsInvalidOperationException()
    {
        // Arrange
        Result<int, string> result = new Ok<int>(1);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(result.UnwrapErr);
    }

    [Fact]
    public void UnwrapErr_WhenErr_ReturnsErrValue()
    {
        // Arrange
        const string initialErrValue = "error";
        Result<int, string> result = new Err<string>(initialErrValue);

        // Act
        var value = result.UnwrapErr();
        Assert.Equal(initialErrValue, value);
    }

    [Fact]
    public void UnwrapOr_WhenOk_ReturnsOkValue()
    {
        // Arrange
        const int realValue = 1;
        const int defaultValue = 2;
        Result<int, string> result = new Ok<int>(realValue);
        
        // Act
        var output = result.UnwrapOr(defaultValue);
        
        // Assert
        Assert.Equal(realValue, output);
    }

    [Fact]
    public void UnwrapOr_WhenErr_ReturnsDefaultValue()
    {
        // Arrange
        const int defaultValue = 2;
        Result<int, string> result = new Err<string>("error");

        // Act
        var output = result.UnwrapOr(defaultValue);
        
        // Assert
        Assert.Equal(defaultValue, output);
    }

    [Fact]
    public void UnwrapOrElse_WhenOk_ReturnsOkValue()
    {
        // Arrange
        const int realValue = 1;
        const int defaultValue = 2;
        Result<int, string> result = new Ok<int>(realValue);
        
        // Act
        var output = result.UnwrapOrElse(() => defaultValue);
        
        // Assert
        Assert.Equal(realValue, output);
    }

    [Fact]
    public void UnwrapOrElse_WhenErr_ReturnsValueFromDefaultFunction()
    {
        // Arrange
        const string initialValue = "error";
        Result<int, string> result = new Err<string>(initialValue);
        const int defaultValue = 2;

        // Act
        var output = result.UnwrapOrElse(()=> defaultValue);

        // Assert
        Assert.Equal(defaultValue, output);
    }

    [Fact]
    public void UnwrapOrElse_WhenOk_DoesNotExecuteErrHandler()
    {
        // Arrange
        var defaultHandlerExec = false;
        const int initialValue = 1;
        const int defaultValue = 2;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.UnwrapOrElse(() =>
        {
            defaultHandlerExec = true;
            return defaultValue;
        }); 

        // Assert
        Assert.Equal(initialValue, output);
        Assert.False(defaultHandlerExec);
    }
    
    [Fact]
    public void Match_WhenOk_ExecuteOkHandler()
    {
        // Arrange
        const int initialValue = 1;
        const int defaultValue = 2;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.Match(
            ok => ok, 
            _ => defaultValue
        );

        // Assert
        Assert.Equal(initialValue, output);
    }

    [Fact]
    public void Match_WhenErr_ExecuteErrHandler()
    {
        // Arrange
        const string initialValue = "error";
        const string defaultValue = "default error";
        Result<int, string> result = new Err<string>(initialValue);
        
        // Act
        var output = result.Match(
            _ => defaultValue,
            err => err
            );
        
        // Assert
        Assert.Equal(initialValue, output);
    }

    [Fact]
    public void Match_WhenOk_OnlyExecutesOkHandler()
    {
        // Arrange
        var okExec = false;
        var errExec = false;
        Result<int, string> result = new Ok<int>(1);

        // Act
        result.Match(
            _ => okExec = true,
            _ => errExec = true
        );
        
        // Assert
        Assert.True(okExec);
        Assert.False(errExec);
    }

    [Fact]
    public void Match_WhenErr_OnlyExecutesErrHandler()
    {
        // Arrange
        var errExec = false;
        var okExec = false;
        Result<int, string> result = new Err<string>("error");
        
        // Act
        result.Match(
            _ => okExec = true,
            _ => errExec = true
            );
        
        // Assert
        Assert.False(okExec);
        Assert.True(errExec);
    }

    [Fact]
    public void Map_WhenOk_ReturnsNewOkResult()
    {
        // Arrange
        const int initialValue = 1;
        const int toValue = 2;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.Map(value => toValue);

        // Assert
        Assert.True(output.IsOk);
        Assert.NotSame(result, output);
        Assert.Equal(toValue, output.Unwrap());
    }
    
    [Fact]
    public void Map_WhenOk_ExecutesMapFunction()
    {
        // Arrange
        const int initialValue = 1;
        const int toValue = 2;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.Map(value => toValue);

        // Assert
        Assert.Equal(toValue, output.Unwrap());
    }

    [Fact]
    public void Map_WhenErr_DoesNotExecuteOkMapFunc()
    {
        // Arrange
        var okExec = false;
        Result<int, string> result = new Err<string>("error");

        // Act
        var output = result.Map(value =>
        {
            okExec = true;
            return value;
        });

        // Assert
        Assert.False(okExec);
    }

    [Fact]
    public void Map_WhenErr_ReturnsNewErrWithExistingErrValue()
    {
        // Arrange
        const string initialError = "error";
        Result<int, string> result = new Err<string>(initialError);

        // Act
        var output = result.Map(value => value);

        // Assert
        Assert.True(output.IsErr);
        Assert.NotSame(result, output);
        Assert.Equal(initialError, output.UnwrapErr());
    }

    [Fact]
    public void MapErr_WhenErr_ReturnsNewErrResult ()
    {
        // Arrange
        const string  initialError = "error";
        const string toError = "toError";
        Result<int, string> result = new Err<string>(initialError);
        
        // Act
        var output  = result.MapErr(err=> toError);

        // Assert
        Assert.True(output.IsErr);
        Assert.NotSame(result, output);
        Assert.Equal(toError, output.UnwrapErr());
    }

    [Fact]
    public void MapErr_WhenErr_ExecutesMapFunction()
    {
        // Arrange
        var hasExec = false;
        const string initialValue = "error";
        const string toError = "toError";
        Result<int, string> result = new Err<string>(initialValue);

        // Act
        var output = result.MapErr(err =>
        {
            hasExec = true;
            return toError;
        });

        // Assert
        Assert.True(hasExec);
    }

    [Fact]
    public void MapErr_WhenOk_DoesNotExecuteMapFunc()
    {
        // Arrange
        const int initialValue = 1;
        var hasExec = false;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.MapErr(err =>
        {
            hasExec = true;
            return err;
        });

        // Assert
        Assert.False(hasExec);
    }

    [Fact]
    public void MapErr_WhenOk_ReturnsNewOkWithExistingOkValue()
    {
        // Arrange
        const int initialValue = 1;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.MapErr(ok => ok);
            
        // Assert
        Assert.True(output.IsOk);
        Assert.NotSame(result, output);
        Assert.Equal(initialValue, output.Unwrap());
    }
    
    [Fact]
    public void Bind_WhenOk_ReturnsNewOkResult()
    {
        // Arrange
        const int initialValue = 1;
        const int toValue = 2;
        Result<int, string> result = new Ok<int>(initialValue);
        
        // Act
        var output = result.Bind<int>(value => new Ok<int>(toValue));
        

        // Assert
        Assert.True(output.IsOk);
        Assert.NotSame(result, output);
        Assert.Equal(toValue, output.Unwrap());
    }

    [Fact]
    public void Bind_WhenOk_ExecutesBindFunction()
    {
        // Arrange
        const int initialValue = 1;
        var hasExec = false;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.Bind<int>(value =>
        {
            hasExec = true;
            return new  Ok<int>(value);
        });

        // Assert
        Assert.True(hasExec);
    }

    [Fact]
    public void Bind_WhenErr_DoesNotExecuteOkBindFunc()
    {
        // Arrange
        const int initialValue = 1;
        var hasExec = false;
        Result<int, string> result = new Err<string>("error");

        // Act
        var output = result.Bind<int>(value =>
        {
            hasExec = true;
            return new  Ok<int>(value);
        });

        // Assert
        Assert.False(hasExec);
    }

    [Fact]
    public void Bind_WhenErr_ReturnsNewErrWithExistingErrValue()
    {
        // Arrange
        const string initialError = "error";
        Result<int, string> result = new Err<string>(initialError);

        // Act
        var output = result.Bind<int>(value => new Ok<int>(value));

        // Assert
        Assert.True(output.IsErr);
        Assert.NotSame(result, output);
        Assert.Equal(initialError, output.UnwrapErr());
    }

    [Fact]
    public void BindErr_WhenErr_ReturnsNewErrResult()
    {
        // Arrange
        const string initialError = "error";
        const string toError = "toError";
        Result<int, string> result = new Err<string>(initialError);

        // Act
        var output = result.BindErr<string>(value => new Err<string>(toError));

        // Assert
        Assert.True(output.IsErr);
        Assert.NotSame(result, output);
        Assert.Equal(toError, output.UnwrapErr());
    }

    [Fact]
    public void BindErr_WhenErr_ExecutesBindFunction()
    {
        // Arrange
        var  hasExec = false;
        const string initialError = "error";
        Result<int, string> result = new Err<string>(initialError);

        // Act
        var output = result.BindErr<string>(value =>
        {
            hasExec = true;
            return new Err<string>(value);
        });

        // Assert
        Assert.True(hasExec);
    }

    [Fact]
    public void BindErr_WhenOk_DoesNotExecuteErrBindFunc()
    {
        // Arrange
        const int initialValue = 1;
        var hasExec = false;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.BindErr<string>(value =>
        {
            hasExec = true;
            return new Err<string>(value);
        });

        // Assert
        Assert.False(hasExec);
    }

    [Fact]
    public void BindErr_WhenOk_ReturnsNewErrWithExistingErrValue()
    {
        // Arrange
        const int initialValue = 1;
        Result<int, string> result = new Ok<int>(initialValue);
        
        // Act
        var output = result.BindErr<string>(value => new Err<string>(value));
        
        // Assert
        Assert.True(output.IsOk);
        Assert.NotSame(result, output);
        Assert.Equal(initialValue, output.Unwrap());
    }

    [Fact]
    public void Effect_WhenOk_ExecutesFunc()
    {
        // Arrange
        var hasExec = false;
        Result<int, string> result = new Ok<int>(1);

        // Act
        result.Effect(_ => hasExec = true);

        // Assert
        Assert.True(hasExec);
    }

    [Fact]
    public void Bind_WhenErr_DoesNotExecuteFunc()
    {
        // Arrange
        var hasExec = false;
        Result<int, string> result = new Err<string>("error");

        // Act
        result.Effect(_ => hasExec = true);

        // Assert
        Assert.False(hasExec);
    }

    [Fact]
    public void Effect_WhenErr_ExecutesFunc()
    {
        // Arrange
        var hasExec = false;
        Result<int, string> result = new Err<string>("error");

        // Act
        result.EffectErr(_ => hasExec = true);

        // Assert
        Assert.True(hasExec);
    }

    [Fact]
    public void Bind_WhenOk_DoesNotExecuteFunc()
    {
        // Arrange
        var hasExec = false;
        Result<int, string> result = new Ok<int>(1);

        // Act
        result.EffectErr(_ => hasExec = true);

        // Assert
        Assert.False(hasExec);
    }
    
    [Fact]
    public void ToOption_WhenOk_ReturnsSomeOfOkValue()
    {
        // Arrange
        const int initialValue = 1;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.ToOption();

        // Assert
        Assert.True(output is Some<int>);
        Assert.Equal(initialValue, output.Unwrap());
    }

    [Fact]
    public void ToOption_WhenErr_ReturnsNone()
    {
        // Arrange
        const int initialValue = 1;
        Result<int, string> result = new Err<string>("error");

        // Act
        var output = result.ToOption();

        // Assert
        Assert.True(output is None);
    }

    [Fact]
    public void ToErrOption_WhenErr_ReturnsSomeWithErrValue()
    {
        // Arrange
        const string initialValue = "error";
        Result<int, string> result = new Err<string>(initialValue);

        // Act
        var output = result.ToErrOption();

        // Assert
        Assert.True(output is Some<string>);
        Assert.Equal(initialValue, output.Unwrap());
    }

    [Fact]
    public void ToErrOption_WhenOk_ReturnsNone()
    {
        // Arrange
        const int initialValue = 1;
        Result<int, string> result = new Ok<int>(initialValue);

        // Act
        var output = result.ToErrOption();

        // Assert
        Assert.True(output is None);
    }
}