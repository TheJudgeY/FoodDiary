using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class GetNotificationPreferencesEndpoint : EndpointWithoutRequest<GetNotificationPreferencesResponse>
{
    private readonly AppDbContext _context;
    private readonly ILogger<GetNotificationPreferencesEndpoint> _logger;

    public GetNotificationPreferencesEndpoint(
        AppDbContext context,
        ILogger<GetNotificationPreferencesEndpoint> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override void Configure()
    {
        Get("/api/notifications/preferences");
        Summary(s =>
        {
            s.Summary = "Get notification preferences";
            s.Description = "Retrieves the current user's notification preferences. If no preferences exist, default preferences will be created and returned.";
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
            var preferences = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId, ct);

            if (preferences == null)
            {
                preferences = new NotificationPreferences(userId);
                _context.NotificationPreferences.Add(preferences);
                await _context.SaveChangesAsync(ct);
                
                _logger.LogInformation("Created default notification preferences for user {UserId}", userId);
            }

            var response = new GetNotificationPreferencesResponse
            {
                Success = true,
                Message = "Notification preferences retrieved successfully",
                Preferences = new NotificationPreferencesDto
                {
                    WaterRemindersEnabled = preferences.WaterRemindersEnabled,
                    MealRemindersEnabled = preferences.MealRemindersEnabled,
                    CalorieLimitWarningsEnabled = preferences.CalorieLimitWarningsEnabled,
                    GoalAchievementsEnabled = preferences.GoalAchievementsEnabled,
    
                    WeeklyProgressEnabled = preferences.WeeklyProgressEnabled,
                    DailySummaryEnabled = preferences.DailySummaryEnabled,
                    WaterReminderTime = preferences.WaterReminderTime?.ToString(@"hh\:mm"),
                    BreakfastReminderTime = preferences.BreakfastReminderTime?.ToString(@"hh\:mm"),
                    LunchReminderTime = preferences.LunchReminderTime?.ToString(@"hh\:mm"),
                    DinnerReminderTime = preferences.DinnerReminderTime?.ToString(@"hh\:mm"),
                    WaterReminderFrequencyHours = preferences.WaterReminderFrequencyHours,
                    SendNotificationsOnWeekends = preferences.SendNotificationsOnWeekends,
                    CreatedAt = preferences.CreatedAt,
                    UpdatedAt = preferences.UpdatedAt
                }
            };

            await SendAsync(response, 200, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification preferences for user {UserId}", userId);
            
            await SendAsync(new GetNotificationPreferencesResponse
            {
                Success = false,
                Message = $"Error retrieving notification preferences: {ex.Message}"
            }, 500, ct);
        }
    }
}

public class GetNotificationPreferencesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationPreferencesDto? Preferences { get; set; }
}

public class NotificationPreferencesDto
{
    public bool WaterRemindersEnabled { get; set; }
    public bool MealRemindersEnabled { get; set; }
    public bool CalorieLimitWarningsEnabled { get; set; }
    public bool GoalAchievementsEnabled { get; set; }
    
    public bool WeeklyProgressEnabled { get; set; }
    public bool DailySummaryEnabled { get; set; }
    public string? WaterReminderTime { get; set; }
    public string? BreakfastReminderTime { get; set; }
    public string? LunchReminderTime { get; set; }
    public string? DinnerReminderTime { get; set; }
    public int WaterReminderFrequencyHours { get; set; }
    public bool SendNotificationsOnWeekends { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
