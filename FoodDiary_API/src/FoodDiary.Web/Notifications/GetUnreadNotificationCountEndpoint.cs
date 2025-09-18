using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class GetUnreadNotificationCountEndpoint : EndpointWithoutRequest<GetUnreadNotificationCountResponse>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<GetUnreadNotificationCountEndpoint> _logger;

    public GetUnreadNotificationCountEndpoint(
        INotificationService notificationService,
        ILogger<GetUnreadNotificationCountEndpoint> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/notifications/count");
        Summary(s =>
        {
            s.Summary = "Get unread notification count";
            s.Description = "Retrieves the count of unread notifications for the authenticated user.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        try
        {
            var count = await _notificationService.GetUnreadNotificationCountAsync(userId);

            _logger.LogDebug("Retrieved unread notification count {Count} for user {UserId}", count, userId);

            await SendAsync(new GetUnreadNotificationCountResponse
            {
                Success = true,
                Count = count,
                Message = "Unread notification count retrieved successfully"
            }, 200, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unread notification count for user {UserId}", userId);
            
            await SendAsync(new GetUnreadNotificationCountResponse
            {
                Success = false,
                Count = 0,
                Message = $"Error retrieving unread notification count: {ex.Message}"
            }, 500, ct);
        }
    }
}

public class GetUnreadNotificationCountResponse
{
    public bool Success { get; set; }
    public int Count { get; set; }
    public string Message { get; set; } = string.Empty;
}
