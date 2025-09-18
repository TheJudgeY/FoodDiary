using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class CleanupNotificationsEndpoint : EndpointWithoutRequest<CleanupNotificationsResponse>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<CleanupNotificationsEndpoint> _logger;

    public CleanupNotificationsEndpoint(
        INotificationService notificationService,
        ILogger<CleanupNotificationsEndpoint> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/notifications/cleanup");
        Summary(s =>
        {
            s.Summary = "Cleanup old read notifications";
            s.Description = "Automatically deletes read notifications older than 3 days to save database space.";
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
            var deletedCount = await _notificationService.CleanupOldReadNotificationsForUserAsync(userId);

            _logger.LogInformation("Cleaned up {DeletedCount} old read notifications for user {UserId}", deletedCount, userId);

            await SendAsync(new CleanupNotificationsResponse
            {
                Success = true,
                Message = $"Cleaned up {deletedCount} old read notifications",
                DeletedCount = deletedCount
            }, 200, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up notifications for user {UserId}", userId);
            
            await SendAsync(new CleanupNotificationsResponse
            {
                Success = false,
                Message = $"Error cleaning up notifications: {ex.Message}",
                DeletedCount = 0
            }, 500, ct);
        }
    }
}

public class CleanupNotificationsResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int DeletedCount { get; set; }
}
