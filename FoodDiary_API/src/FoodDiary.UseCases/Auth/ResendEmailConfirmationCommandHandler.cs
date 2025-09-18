using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Auth;

public class ResendEmailConfirmationCommandHandler : IRequestHandler<ResendEmailConfirmationCommand, Result<ResendEmailConfirmationResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly IEmailConfirmationTokenGenerator _emailConfirmationTokenGenerator;
    private readonly IEmailSender _emailSender;

    public ResendEmailConfirmationCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        IEmailConfirmationTokenGenerator emailConfirmationTokenGenerator,
        IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _emailConfirmationTokenGenerator = emailConfirmationTokenGenerator;
        _emailSender = emailSender;
    }

    public async Task<Result<ResendEmailConfirmationResponse>> Handle(ResendEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var allUsers = await _userRepository.ListAsync();
            if (!allUsers.IsSuccess)
            {
                return Result<ResendEmailConfirmationResponse>.Error("Failed to retrieve users");
            }

            var user = allUsers.Value.FirstOrDefault(u => u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return Result<ResendEmailConfirmationResponse>.Error("User not found");
            }

            if (user.EmailConfirmed)
            {
                return Result<ResendEmailConfirmationResponse>.Success(new ResendEmailConfirmationResponse
                {
                    Success = true,
                    Message = "Email is already confirmed"
                });
            }

            var newToken = _emailConfirmationTokenGenerator.GenerateToken();
            var tokenExpiration = _emailConfirmationTokenGenerator.GetExpirationTime();

            user.SetEmailConfirmationToken(newToken, tokenExpiration);

            var updateResult = await _userRepository.UpdateAsync(user);
            if (!updateResult.IsSuccess)
            {
                return Result<ResendEmailConfirmationResponse>.Error("Failed to update user");
            }

            try
            {
                var confirmationUrl = $"http://localhost:4173/confirm-email?email={Uri.EscapeDataString(user.Email)}&token={newToken}";
                var emailBody = $@"
Hello {user.Name},

You requested a new email confirmation link. Please confirm your email address by clicking the link below:

{confirmationUrl}

This link will expire in 24 hours.

If you didn't request this email, please ignore it.

Best regards,
The FoodDiary Team";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "noreply@fooddiary.com",
                    "Confirm your FoodDiary account",
                    emailBody.Trim());
            }
            catch (Exception ex)
            {
                return Result<ResendEmailConfirmationResponse>.Error($"Failed to send confirmation email: {ex.Message}");
            }

            return Result<ResendEmailConfirmationResponse>.Success(new ResendEmailConfirmationResponse
            {
                Success = true,
                Message = "Confirmation email sent successfully! Please check your inbox."
            });
        }
        catch (Exception ex)
        {
            return Result<ResendEmailConfirmationResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
} 