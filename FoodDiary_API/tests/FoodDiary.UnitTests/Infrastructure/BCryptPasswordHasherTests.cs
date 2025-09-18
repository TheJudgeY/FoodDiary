using Xunit;
using FoodDiary.Infrastructure.Password;

namespace FoodDiary.UnitTests.Infrastructure;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _passwordHasher;

    public BCryptPasswordHasherTests()
    {
        _passwordHasher = new BCryptPasswordHasher();
    }

    [Fact]
    public void HashPassword_ShouldReturnDifferentHashForSamePassword()
    {
        var password = "testPassword123";

        var hash1 = _passwordHasher.HashPassword(password);
        var hash2 = _passwordHasher.HashPassword(password);

        Assert.NotEqual(hash1, hash2);
        Assert.NotNull(hash1);
        Assert.NotNull(hash2);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        var password = "testPassword123";
        var hash = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword(hash, password);

        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        var password = "testPassword123";
        var wrongPassword = "wrongPassword123";
        var hash = _passwordHasher.HashPassword(password);

        var result = _passwordHasher.VerifyPassword(hash, wrongPassword);

        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("short")]
    [InlineData("veryLongPasswordWithSpecialCharacters!@#$%^&*()")]
    public void HashPassword_WithVariousPasswords_ShouldWork(string password)
    {
        var hash = _passwordHasher.HashPassword(password);

        Assert.NotNull(hash);
        Assert.NotEmpty(hash);
        Assert.True(_passwordHasher.VerifyPassword(hash, password));
    }
} 