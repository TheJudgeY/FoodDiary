using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Auth;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<ConfirmEmailResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;

    public ConfirmEmailCommandHandler(FoodDiary.Core.Interfaces.IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<ConfirmEmailResponse>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var allUsers = await _userRepository.ListAsync();
            if (!allUsers.IsSuccess)
            {
                return Result<ConfirmEmailResponse>.Error("Failed to retrieve users");
            }

            var user = allUsers.Value.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return Result<ConfirmEmailResponse>.Error("User not found");
            }

            if (user.EmailConfirmed)
            {
                return Result<ConfirmEmailResponse>.Success(new ConfirmEmailResponse
                {
                    Success = true,
                    Message = "Email is already confirmed"
                });
            }

            if (string.IsNullOrEmpty(user.EmailConfirmationToken) || 
                !user.EmailConfirmationToken.Equals(request.Token, StringComparison.OrdinalIgnoreCase))
            {
                return Result<ConfirmEmailResponse>.Error("Invalid confirmation token");
            }

            if (user.EmailConfirmationTokenExpiresAt.HasValue && 
                user.EmailConfirmationTokenExpiresAt.Value < DateTime.UtcNow)
            {
                return Result<ConfirmEmailResponse>.Error("Confirmation token has expired");
            }

            user.ConfirmEmail();

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.IsSuccess)
            {
                return Result<ConfirmEmailResponse>.Error("Failed to confirm email");
            }

            return Result<ConfirmEmailResponse>.Success(new ConfirmEmailResponse
            {
                Success = true,
                Message = "Email confirmed successfully! You can now log in to your account."
            });
        }
        catch (Exception ex)
        {
            return Result<ConfirmEmailResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
} 