using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class UpdateNotificationPreferencesEndpoint : Endpoint<UpdateNotificationPreferencesRequest, UpdateNotificationPreferencesResponse>
{
    private readonly AppDbContext _context;
    private readonly ILogger<UpdateNotificationPreferencesEndpoint> _logger;

    public UpdateNotificationPreferencesEndpoint(
        AppDbContext context,
        ILogger<UpdateNotificationPreferencesEndpoint> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/api/notifications/preferences");
        Summary(s =>
        {
            s.Summary = "Update notification preferences";
            s.Description = "Updates the current user's notification preferences. All fields are optional - only provided fields will be updated.";
        });
    }

    public override async Task HandleAsync(UpdateNotificationPreferencesRequest request, CancellationToken ct)
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
            }

            var update = new NotificationPreferencesUpdate
            {
                WaterReminders = request.WaterReminders,
                MealReminders = request.MealReminders,
                CalorieWarnings = request.CalorieWarnings,
                GoalAchievements = request.GoalAchievements,

                WeeklyProgress = request.WeeklyProgress,
                DailySummary = request.DailySummary,
                WaterTime = request.WaterTime,
                BreakfastTime = request.BreakfastTime,
                LunchTime = request.LunchTime,
                DinnerTime = request.DinnerTime,
                WaterFrequency = request.WaterFrequency,
                WeekendNotifications = request.WeekendNotifications
            };

            preferences.UpdatePreferences(update);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Updated notification preferences for user {UserId}", userId);

            await SendAsync(new UpdateNotificationPreferencesResponse
            {
                Success = true,
                Message = "Notification preferences updated successfully",
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
                    UpdatedAt = preferences.UpdatedAt
                }
            }, 200, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating notification preferences for user {UserId}", userId);
            
            await SendAsync(new UpdateNotificationPreferencesResponse
            {
                Success = false,
                Message = $"Error updating notification preferences: {ex.Message}"
            }, 500, ct);
        }
    }
}

public class UpdateNotificationPreferencesRequest
{
    public bool? WaterReminders { get; set; }
    public bool? MealReminders { get; set; }
    public bool? CalorieWarnings { get; set; }
    public bool? GoalAchievements { get; set; }
    
    public bool? WeeklyProgress { get; set; }
    public bool? DailySummary { get; set; }
    
    public TimeSpan? WaterTime { get; set; }
    public TimeSpan? BreakfastTime { get; set; }
    public TimeSpan? LunchTime { get; set; }
    public TimeSpan? DinnerTime { get; set; }
    
    public int? WaterFrequency { get; set; }
    public bool? WeekendNotifications { get; set; }
}

public class UpdateNotificationPreferencesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationPreferencesDto? Preferences { get; set; }
}


