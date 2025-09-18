using Xunit;
using MediatR;
using NSubstitute;
using Ardalis.Result;
using FoodDiary.UseCases.Auth;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UnitTests.UseCases.Auth;

public class LoginUserCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _handler = new LoginUserCommandHandler(_userRepository, _passwordHasher, _jwtTokenGenerator);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnSuccess()
    {
        var command = new LoginUserCommand
        {
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User("test@example.com", "hashedPassword123", "Test User");
        user.ConfirmEmail();

        var users = new List<User> { user };
        var token = "jwt.token.here";

        _userRepository.ListAsync().Returns(Result<List<User>>.Success(users));
        _passwordHasher.VerifyPassword(user.PasswordHash, command.Password).Returns(true);
        _jwtTokenGenerator.GenerateToken(user.Id.ToString(), user.Email, user.Name).Returns(token);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal(user.Email, result.Value.Email);
        Assert.Equal(user.Name, result.Value.Name);
        Assert.Equal(token, result.Value.Token);
    }

    [Fact]
    public async Task Handle_WithNonExistentEmail_ShouldReturnError()
    {
        var command = new LoginUserCommand
        {
            Email = "nonexistent@example.com",
            Password = "password123"
        };

        var users = new List<User>();

        _userRepository.ListAsync().Returns(Result<List<User>>.Success(users));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid email or password", result.Errors.First());
    }

    [Fact]
    public async Task Handle_WithIncorrectPassword_ShouldReturnError()
    {
        var command = new LoginUserCommand
        {
            Email = "test@example.com",
            Password = "wrongpassword"
        };

        var user = new User("test@example.com", "hashedPassword123", "Test User");

        var users = new List<User> { user };

        _userRepository.ListAsync().Returns(Result<List<User>>.Success(users));
        _passwordHasher.VerifyPassword(user.PasswordHash, command.Password).Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid email or password", result.Errors.First());
    }

    [Fact]
    public async Task Handle_WhenRepositoryFails_ShouldReturnError()
    {
        var command = new LoginUserCommand
        {
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepository.ListAsync().Returns(Result<List<User>>.Error("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Failed to retrieve users", result.Errors.First());
    }
} 