using System;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Core.NotificationAggregate;

public static class NotificationDefaults
{
    public static readonly TimeSpan DefaultWaterReminderTime = new(9, 0, 0);
    public static readonly TimeSpan DefaultBreakfastReminderTime = new(8, 0, 0);
    public static readonly TimeSpan DefaultLunchReminderTime = new(12, 0, 0);
    public static readonly TimeSpan DefaultDinnerReminderTime = new(18, 0, 0);
    public static readonly int DefaultWaterReminderFrequencyHours = 2;
    public static readonly string DefaultTimeZoneId = "UTC";
}

public class NotificationPreferences
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    
    public bool WaterRemindersEnabled { get; private set; } = true;
    public bool MealRemindersEnabled { get; private set; } = true;
    public bool CalorieLimitWarningsEnabled { get; private set; } = true;
    public bool GoalAchievementsEnabled { get; private set; } = true;
    
    public bool WeeklyProgressEnabled { get; private set; } = true;
    public bool DailySummaryEnabled { get; private set; } = true;
    
    public TimeSpan? WaterReminderTime { get; private set; } = NotificationDefaults.DefaultWaterReminderTime;
    public TimeSpan? BreakfastReminderTime { get; private set; } = NotificationDefaults.DefaultBreakfastReminderTime;
    public TimeSpan? LunchReminderTime { get; private set; } = NotificationDefaults.DefaultLunchReminderTime;
    public TimeSpan? DinnerReminderTime { get; private set; } = NotificationDefaults.DefaultDinnerReminderTime;
    
    public int WaterReminderFrequencyHours { get; private set; } = NotificationDefaults.DefaultWaterReminderFrequencyHours;
    public bool SendNotificationsOnWeekends { get; private set; } = true;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
    
    public User User { get; private set; } = null!;

    public NotificationPreferences(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private NotificationPreferences() { }
    
    public bool IsNotificationTypeEnabled(NotificationType type)
    {
        return type switch
        {
            NotificationType.WaterReminder => WaterRemindersEnabled,
            NotificationType.MealReminder => MealRemindersEnabled,
            NotificationType.CalorieLimitWarning => CalorieLimitWarningsEnabled,
            NotificationType.GoalAchievement => GoalAchievementsEnabled,

            NotificationType.WeeklyProgress => WeeklyProgressEnabled,
            NotificationType.DailySummary => DailySummaryEnabled,
            _ => true
        };
    }
    
    public void UpdatePreferences(NotificationPreferencesUpdate update)
    {
        if (update.WaterReminders.HasValue) WaterRemindersEnabled = update.WaterReminders.Value;
        if (update.MealReminders.HasValue) MealRemindersEnabled = update.MealReminders.Value;
        if (update.CalorieWarnings.HasValue) CalorieLimitWarningsEnabled = update.CalorieWarnings.Value;
        if (update.GoalAchievements.HasValue) GoalAchievementsEnabled = update.GoalAchievements.Value;
        
        if (update.WeeklyProgress.HasValue) WeeklyProgressEnabled = update.WeeklyProgress.Value;
        if (update.DailySummary.HasValue) DailySummaryEnabled = update.DailySummary.Value;
        
        if (update.WaterTime.HasValue) WaterReminderTime = update.WaterTime.Value;
        if (update.BreakfastTime.HasValue) BreakfastReminderTime = update.BreakfastTime.Value;
        if (update.LunchTime.HasValue) LunchReminderTime = update.LunchTime.Value;
        if (update.DinnerTime.HasValue) DinnerReminderTime = update.DinnerTime.Value;
        
        if (update.WaterFrequency.HasValue) WaterReminderFrequencyHours = update.WaterFrequency.Value;
        if (update.WeekendNotifications.HasValue) SendNotificationsOnWeekends = update.WeekendNotifications.Value;
        
        UpdatedAt = DateTime.UtcNow;
    }
    
    public bool ShouldSendNotificationToday()
    {
        var today = DateTime.UtcNow.DayOfWeek;
        return SendNotificationsOnWeekends || (today != DayOfWeek.Saturday && today != DayOfWeek.Sunday);
    }
}

public class NotificationPreferencesUpdate
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