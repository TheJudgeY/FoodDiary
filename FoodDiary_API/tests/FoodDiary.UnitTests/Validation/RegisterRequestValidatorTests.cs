using Xunit;
using FluentValidation;
using FoodDiary.Web.Validation;
using FoodDiary.Web.Auth;

namespace FoodDiary.UnitTests.Validation;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator;

    public RegisterRequestValidatorTests()
    {
        _validator = new RegisterRequestValidator();
    }

    [Fact]
    public void Should_Pass_When_Valid_Request()
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_Email_Is_Empty(string email)
    {
        var request = new RegisterRequest
        {
            Email = email,
            Password = "ValidPass123!",
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Email));
    }
    [Fact]
    public void Should_Fail_When_Email_Is_Null()
    {
        var request = new RegisterRequest
        {
            Email = null!,
            Password = "ValidPass123!",
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Email));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("test@")]
    [InlineData("@example.com")]
    [InlineData("test.example.com")]
    public void Should_Fail_When_Email_Is_Invalid(string email)
    {
        var request = new RegisterRequest
        {
            Email = email,
            Password = "ValidPass123!",
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Email));
    }

    [Fact]
    public void Should_Fail_When_Email_Too_Long()
    {
        var longEmail = new string('a', 250) + "@example.com";
        var request = new RegisterRequest
        {
            Email = longEmail,
            Password = "ValidPass123!",
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_Password_Is_Empty(string password)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = password,
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Password));
    }
    [Fact]
    public void Should_Fail_When_Password_Is_Null()
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = null!,
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Theory]
    [InlineData("short")]
    [InlineData("1234567")]
    public void Should_Fail_When_Password_Too_Short(string password)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = password,
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Theory]
    [InlineData("nouppercase123!")]
    [InlineData("NOLOWERCASE123!")]
    [InlineData("NoNumbers!")]
    [InlineData("NoSpecial123")]
    public void Should_Fail_When_Password_Missing_Requirements(string password)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = password,
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Fact]
    public void Should_Fail_When_Password_Too_Long()
    {
        var longPassword = new string('a', 129);
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = longPassword,
            Name = "John Doe"
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Password));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_Name_Is_Empty(string name)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = name
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Name));
    }
    [Fact]
    public void Should_Fail_When_Name_Is_Null()
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = null!
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Name));
    }

    [Theory]
    [InlineData("A")]
    [InlineData("")]
    public void Should_Fail_When_Name_Too_Short(string name)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = name
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Name));
    }

    [Theory]
    [InlineData("John123")]
    [InlineData("John@Doe")]
    [InlineData("John_Doe")]
    public void Should_Fail_When_Name_Contains_Invalid_Characters(string name)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = name
        };

        var result = _validator.Validate(request);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Name));
    }

    [Theory]
    [InlineData("John Doe")]
    [InlineData("Mary-Jane")]
    [InlineData("O'Connor")]
    [InlineData("Dr. Smith")]
    public void Should_Pass_When_Name_Contains_Valid_Characters(string name)
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "ValidPass123!",
            Name = name
        };

        var result = _validator.Validate(request);

        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(RegisterRequest.Name));
    }
} 