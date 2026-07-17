using System;
using Xunit;
using task05;

namespace task05tests;

public class UserServiceTests
{
    private readonly UserService _userService = new();

    [Fact]
    public void RegisterUser_ValidUser_ReturnsTrue()
    {
        var user = new User { Username = "validUser", Password = "password123", Age = 20 };
        var result = _userService.RegisterUser(user);
        Assert.True(result);
    }

    [Fact]
    public void RegisterUser_ShortPassword_ThrowsUserValidationException()
    {
        var user = new User { Username = "user", Password = "123", Age = 20 };
        
        var exception = Assert.Throws<UserValidationException>(() => _userService.RegisterUser(user));
        Assert.Equal("Пароль должен содержать минимум 6 символов", exception.Message);
    }

    [Fact]
    public void RegisterUser_Underage_ThrowsUserValidationException()
    {
        var user = new User { Username = "junior", Password = "password123", Age = 16 };
        
        var exception = Assert.Throws<UserValidationException>(() => _userService.RegisterUser(user));
        Assert.Equal("Регистрация доступна только лицам старше 18 лет", exception.Message);
    }
}
