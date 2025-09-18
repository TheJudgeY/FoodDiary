using Xunit;
using MediatR;
using NSubstitute;
using Ardalis.Result;
using FoodDiary.UseCases.Auth;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace FoodDiary.UnitTests.UseCases.Auth;

public class RegisterUserCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailConfirmationTokenGenerator _emailConfirmationTokenGenerator;
    private readonly IEmailSender _emailSender;
    private readonly EmailConfiguration _emailConfiguration;
    private readonly ILogger<RegisterUserCommandHandler> _logger;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _emailConfirmationTokenGenerator = Substitute.For<IEmailConfirmationTokenGenerator>();
        _emailSender = Substitute.For<IEmailSender>();
        var configuration = Substitute.For<IConfiguration>();
        configuration["FrontendUrl"].Returns("http://localhost:4173");
        _emailConfiguration = new EmailConfiguration(configuration);
        _logger = Substitute.For<ILogger<RegisterUserCommandHandler>>();
        
        _handler = new RegisterUserCommandHandler(_userRepository, _passwordHasher, _jwtTokenGenerator, _emailConfirmationTokenGenerator, _emailSender, _emailConfiguration, _logger);
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnSuccess()
    {
        var command = new RegisterUserCommand
        {
            Email = "test@example.com",
            Password = "password123",
            Name = "Test User"
        };

        var existingUsers = new List<User>();
        var hashedPassword = "hashedPassword123";
        var token = "jwt.token.here";
        var emailConfirmationToken = "email-confirmation-token";
        var tokenExpiration = DateTime.UtcNow.AddHours(24);

        _userRepository.ListAsync().Returns(Result<List<User>>.Success(existingUsers));
        _passwordHasher.HashPassword(command.Password).Returns(hashedPassword);
        _jwtTokenGenerator.GenerateToken(Arg.Any<string>(), command.Email, command.Name).Returns(token);
        _emailConfirmationTokenGenerator.GenerateToken().Returns(emailConfirmationToken);
        _emailConfirmationTokenGenerator.GetExpirationTime().Returns(tokenExpiration);
        _userRepository.AddAsync(Arg.Any<User>()).Returns(Result<User>.Success(new User("test@example.com", "hashedPassword", "Test User")));
        _emailSender.SendEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(command.Email, result.Value.Email);
        Assert.Equal(command.Name, result.Value.Name);
        Assert.Equal(token, result.Value.Token);
        Assert.NotEqual(Guid.Empty, result.Value.UserId);
        Assert.True(result.Value.EmailConfirmationRequired);
        Assert.Contains("check your email", result.Value.Message);
    }

    [Fact]
    public async Task Handle_WithExistingEmail_ShouldReturnError()
    {
        var command = new RegisterUserCommand
        {
            Email = "existing@example.com",
            Password = "password123",
            Name = "Test User"
        };

        var existingUsers = new List<User>
        {
            new User("existing@example.com", "hashedPassword", "Existing User")
        };

        _userRepository.ListAsync().Returns(Result<List<User>>.Success(existingUsers));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("already exists", result.Errors.First());
    }

    [Fact]
    public async Task Handle_WhenRepositoryFails_ShouldReturnError()
    {
        var command = new RegisterUserCommand
        {
            Email = "test@example.com",
            Password = "password123",
            Name = "Test User"
        };

        var existingUsers = new List<User>();
        var hashedPassword = "hashedPassword123";
        var emailConfirmationToken = "email-confirmation-token";
        var tokenExpiration = DateTime.UtcNow.AddHours(24);

        _userRepository.ListAsync().Returns(Result<List<User>>.Success(existingUsers));
        _passwordHasher.HashPassword(command.Password).Returns(hashedPassword);
        _emailConfirmationTokenGenerator.GenerateToken().Returns(emailConfirmationToken);
        _emailConfirmationTokenGenerator.GetExpirationTime().Returns(tokenExpiration);
        _userRepository.AddAsync(Arg.Any<User>()).Returns(Result<User>.Error("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Database error", result.Errors);
    }
} 