using FastEndpoints;
using MediatR;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using Microsoft.AspNetCore.Authorization;

namespace FoodDiary.Web.Auth;

[AllowAnonymous]
public class GetEmailConfirmationTokenEndpoint : Endpoint<GetEmailConfirmationTokenRequest, GetEmailConfirmationTokenResponse>
{
    private readonly IRepository<User> _userRepository;
    private readonly ILogger<GetEmailConfirmationTokenEndpoint> _logger;

    public GetEmailConfirmationTokenEndpoint(
        IRepository<User> userRepository,
        ILogger<GetEmailConfirmationTokenEndpoint> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/auth/dev/email-confirmation-token");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get email confirmation token (Development Only)";
            s.Description = "Development endpoint to get email confirmation token for testing purposes";
        });
    }

    public override async Task HandleAsync(GetEmailConfirmationTokenRequest request, CancellationToken ct)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Development")
        {
            await SendErrorsAsync(404, ct);
            return;
        }

        var usersResult = await _userRepository.ListAsync();
        if (!usersResult.IsSuccess)
        {
            await SendErrorsAsync(500, ct);
            return;
        }

        var user = usersResult.Value.FirstOrDefault(u => 
            u.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            await SendErrorsAsync(404, ct);
            return;
        }

        if (user.EmailConfirmed)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var response = new GetEmailConfirmationTokenResponse
        {
            Email = user.Email,
            Token = user.EmailConfirmationToken ?? string.Empty,
            ExpiresAt = user.EmailConfirmationTokenExpiresAt ?? DateTime.UtcNow
        };

        await SendAsync(response, 200, ct);
    }
}

public class GetEmailConfirmationTokenRequest
{
    public string Email { get; set; } = string.Empty;
}

public class GetEmailConfirmationTokenResponse
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
} 