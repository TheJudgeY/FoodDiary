using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class MarkNotificationAsReadEndpoint : Endpoint<EmptyRequest, MarkNotificationAsReadResponse>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<MarkNotificationAsReadEndpoint> _logger;

    public MarkNotificationAsReadEndpoint(
        INotificationService notificationService,
        ILogger<MarkNotificationAsReadEndpoint> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/api/notifications/{id}/read");
        Summary(s =>
        {
            s.Summary = "Mark notification as read";
            s.Description = "Marks a specific notification as read for the authenticated user.";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var idParam = Route<string>("id");
        if (string.IsNullOrEmpty(idParam) || !Guid.TryParse(idParam, out var notificationId))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        try
        {
            var result = await _notificationService.MarkNotificationAsReadAsync(notificationId, userId);

            if (result)
            {
                _logger.LogInformation("Notification {NotificationId} marked as read for user {UserId}", notificationId, userId);

                await SendAsync(new MarkNotificationAsReadResponse
                {
                    Success = true,
                    Message = "Notification marked as read successfully"
                }, 200, ct);
            }
            else
            {
                _logger.LogWarning("Failed to mark notification {NotificationId} as read for user {UserId} - notification not found or access denied", notificationId, userId);

                await SendAsync(new MarkNotificationAsReadResponse
                {
                    Success = false,
                    Message = "Notification not found or access denied"
                }, 404, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
            
            await SendAsync(new MarkNotificationAsReadResponse
            {
                Success = false,
                Message = $"Error marking notification as read: {ex.Message}"
            }, 500, ct);
        }
    }
}



public class MarkNotificationAsReadResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
