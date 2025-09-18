using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FoodDiary.Web.Auth;

[Authorize]
public class TestAuthEndpoint : EndpointWithoutRequest<TestAuthResponse>
{
    public override void Configure()
    {
        Get("/api/auth/test");
        Summary(s =>
        {
            s.Summary = "Test authenticated endpoint";
            s.Description = "Returns user information from JWT token";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var name = User.FindFirst(ClaimTypes.Name)?.Value;

        var response = new TestAuthResponse
        {
            UserId = userId ?? "Unknown",
            Email = email ?? "Unknown",
            Name = name ?? "Unknown",
            Message = "Authentication successful!"
        };

        await SendAsync(response, 200, ct);
    }
}

public class TestAuthResponse
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
} 