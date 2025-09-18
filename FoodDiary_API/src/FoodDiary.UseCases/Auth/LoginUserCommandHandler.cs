using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Auth;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginUserResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var usersResult = await _userRepository.ListAsync();
        if (!usersResult.IsSuccess)
        {
            return Result<LoginUserResponse>.Error("Failed to retrieve users");
        }

        var user = usersResult.Value.FirstOrDefault(u => 
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return Result<LoginUserResponse>.Error("Invalid email or password");
        }

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            return Result<LoginUserResponse>.Error("Invalid email or password");
        }

        if (!user.EmailConfirmed)
        {
            return Result<LoginUserResponse>.Error("Please confirm your email address before logging in. Check your inbox for a confirmation link or request a new one.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user.Id.ToString(), user.Email, user.Name);

        var response = new LoginUserResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Token = token
        };

        return Result<LoginUserResponse>.Success(response);
    }
} 