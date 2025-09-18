using System;
using System.Collections.Generic;

namespace FoodDiary.UseCases.Notifications;

public class NotificationDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ContextData { get; set; }
    public string? ImageUrl { get; set; }
    
    public bool IsUnread { get; set; }
    public bool IsRead { get; set; }
    public TimeSpan Age { get; set; }
    public bool IsRecent { get; set; }
    public string PriorityColor { get; set; } = string.Empty;
    public string TypeIcon { get; set; } = string.Empty;
}

public class NotificationPreferencesDTO
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

public class NotificationSummaryDTO
{
    public int TotalCount { get; set; }
    public int UnreadCount { get; set; }
    public int ReadCount { get; set; }
} 