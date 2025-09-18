using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FoodDiary.UseCases.Auth;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<RegisterUserResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailConfirmationTokenGenerator _emailConfirmationTokenGenerator;
    private readonly IEmailSender _emailSender;
    private readonly EmailConfiguration _emailConfiguration;
    private readonly ILogger<RegisterUserCommandHandler> _logger;

    public RegisterUserCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailConfirmationTokenGenerator emailConfirmationTokenGenerator,
        IEmailSender emailSender,
        EmailConfiguration emailConfiguration,
        ILogger<RegisterUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailConfirmationTokenGenerator = emailConfirmationTokenGenerator;
        _emailSender = emailSender;
        _emailConfiguration = emailConfiguration;
        _logger = logger;
    }

    public async Task<Result<RegisterUserResponse>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var userExistsResult = await CheckUserExistsAsync(request.Email);
        if (!userExistsResult.IsSuccess)
        {
            return Result<RegisterUserResponse>.Error(string.Join("; ", userExistsResult.Errors));
        }

        var user = await CreateUserAsync(request);
        var addResult = await _userRepository.AddAsync(user);
        if (!addResult.IsSuccess)
        {
            return Result<RegisterUserResponse>.Error(string.Join("; ", addResult.Errors));
        }

        await SendConfirmationEmailAsync(user);

        var response = CreateSuccessResponse(user);
        return Result<RegisterUserResponse>.Success(response);
    }

    private async Task<Result> CheckUserExistsAsync(string email)
    {
        var existingUsers = await _userRepository.ListAsync();
        if (existingUsers.IsSuccess && existingUsers.Value.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            return Result.Error(AuthConstants.ErrorMessages.UserAlreadyExists);
        }
        return Result.Success();
    }

    private Task<User> CreateUserAsync(RegisterUserCommand request)
    {
        var emailConfirmationToken = _emailConfirmationTokenGenerator.GenerateToken();
        var tokenExpiration = _emailConfirmationTokenGenerator.GetExpirationTime();
        var hashedPassword = _passwordHasher.HashPassword(request.Password);

        var user = new User(request.Email, hashedPassword, request.Name);
        user.SetEmailConfirmationToken(emailConfirmationToken, tokenExpiration);
        
        return Task.FromResult(user);
    }

    private async Task SendConfirmationEmailAsync(User user)
    {
        try
        {
            var confirmationUrl = _emailConfiguration.BuildConfirmationUrl(user.Email, user.EmailConfirmationToken!);
            var emailBody = _emailConfiguration.BuildConfirmationEmailBody(user.Name, confirmationUrl);

            _logger.LogInformation(AuthConstants.Logging.AttemptingSendEmail, user.Email);
            _logger.LogInformation(AuthConstants.Logging.ConfirmationUrlLog, confirmationUrl);
            
            await _emailSender.SendEmailAsync(
                user.Email,
                _emailConfiguration.SenderEmail,
                _emailConfiguration.ConfirmationSubject,
                emailBody.Trim());
                
            _logger.LogInformation(AuthConstants.Logging.EmailSentSuccessfully, user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, AuthConstants.Logging.EmailSendFailed, user.Email, ex.Message);
        }
    }

    private RegisterUserResponse CreateSuccessResponse(User user)
    {
        var token = _jwtTokenGenerator.GenerateToken(user.Id.ToString(), user.Email, user.Name);

        return new RegisterUserResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Token = token,
            EmailConfirmationRequired = true,
            Message = AuthConstants.SuccessMessages.RegistrationSuccessful
        };
    }
} 