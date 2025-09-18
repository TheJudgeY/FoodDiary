using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class DeleteNotificationEndpoint : Endpoint<EmptyRequest, DeleteNotificationResponse>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<DeleteNotificationEndpoint> _logger;

    public DeleteNotificationEndpoint(
        INotificationService notificationService,
        ILogger<DeleteNotificationEndpoint> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Delete("/api/notifications/{id}");
        Summary(s =>
        {
            s.Summary = "Delete notification";
            s.Description = "Deletes a specific notification for the authenticated user.";
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
            var result = await _notificationService.DeleteNotificationAsync(notificationId, userId);

            if (result)
            {
                _logger.LogInformation("Notification {NotificationId} deleted for user {UserId}", notificationId, userId);

                await SendAsync(new DeleteNotificationResponse
                {
                    Success = true,
                    Message = "Notification deleted successfully"
                }, 200, ct);
            }
            else
            {
                _logger.LogWarning("Failed to delete notification {NotificationId} for user {UserId} - notification not found or access denied", notificationId, userId);

                await SendAsync(new DeleteNotificationResponse
                {
                    Success = false,
                    Message = "Notification not found or access denied"
                }, 404, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId} for user {UserId}", notificationId, userId);
            
            await SendAsync(new DeleteNotificationResponse
            {
                Success = false,
                Message = $"Error deleting notification: {ex.Message}"
            }, 500, ct);
        }
    }
}



public class DeleteNotificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
