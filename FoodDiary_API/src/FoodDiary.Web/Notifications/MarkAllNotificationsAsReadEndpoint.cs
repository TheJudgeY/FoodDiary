using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class MarkAllNotificationsAsReadEndpoint : EndpointWithoutRequest<MarkAllNotificationsAsReadResponse>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<MarkAllNotificationsAsReadEndpoint> _logger;

    public MarkAllNotificationsAsReadEndpoint(
        INotificationService notificationService,
        ILogger<MarkAllNotificationsAsReadEndpoint> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/api/notifications/mark-all-read");
        Summary(s =>
        {
            s.Summary = "Mark all notifications as read";
            s.Description = "Marks all unread notifications as read for the authenticated user.";
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
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

            if (result)
            {
                _logger.LogInformation("All notifications marked as read for user {UserId}", userId);

                await SendAsync(new MarkAllNotificationsAsReadResponse
                {
                    Success = true,
                    Message = "All notifications marked as read successfully"
                }, 200, ct);
            }
            else
            {
                _logger.LogWarning("Failed to mark all notifications as read for user {UserId}", userId);

                await SendAsync(new MarkAllNotificationsAsReadResponse
                {
                    Success = false,
                    Message = "Failed to mark all notifications as read"
                }, 500, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
            
            await SendAsync(new MarkAllNotificationsAsReadResponse
            {
                Success = false,
                Message = $"Error marking all notifications as read: {ex.Message}"
            }, 500, ct);
        }
    }
}

public class MarkAllNotificationsAsReadResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
