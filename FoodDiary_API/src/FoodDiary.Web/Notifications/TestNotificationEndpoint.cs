using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class TestNotificationEndpoint : Endpoint<TestNotificationRequest, TestNotificationResponse>
{
    private readonly INotificationSchedulerService _notificationScheduler;
    private readonly INotificationService _notificationService;
    private readonly ILogger<TestNotificationEndpoint> _logger;

    public TestNotificationEndpoint(
        INotificationSchedulerService notificationScheduler,
        INotificationService notificationService,
        ILogger<TestNotificationEndpoint> logger)
    {
        _notificationScheduler = notificationScheduler;
        _notificationService = notificationService;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/notifications/test");
        Summary(s =>
        {
            s.Summary = "Test notification generation";
            s.Description = "Manually triggers notification generation for testing purposes";
        });
    }

    public override async Task HandleAsync(TestNotificationRequest request, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        try
        {
            _logger.LogInformation("Manually triggering notification tests for user {UserId}", userId);

            await _notificationService.CreateWaterReminderAsync(userId);
            await _notificationService.CreateMealReminderAsync(userId, DateTime.UtcNow);
            await _notificationService.CreateCalorieLimitWarningAsync(userId);
            await _notificationService.CreateWeeklyProgressNotificationAsync(userId);
            await _notificationService.CreateGoalAchievementNotificationAsync(userId);

            var response = new TestNotificationResponse
            {
                Success = true,
                Message = "Notification tests completed successfully - 5 notifications created",
                Timestamp = DateTime.UtcNow
            };

            await SendAsync(response, 200, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during notification test for user {UserId}", userId);
            
            var response = new TestNotificationResponse
            {
                Success = false,
                Message = $"Error occurred during notification test: {ex.Message}",
                Timestamp = DateTime.UtcNow
            };

            await SendAsync(response, 500, ct);
        }
    }
}

public class TestNotificationRequest
{
    public string TestParameter { get; set; } = string.Empty;
}

public class TestNotificationResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
} 