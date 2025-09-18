using System;
using System.Collections.Generic;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Core.NotificationAggregate;

public enum NotificationType
{
    WaterReminder,
    MealReminder,
    CalorieLimitWarning,
    GoalAchievement,
    
    WeeklyProgress,
    DailySummary
}

public enum NotificationPriority
{
    Low,
    Medium,
    High,
    Urgent
}

public enum NotificationStatus
{
    Unread,
    Read
}

public class Notification
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public NotificationStatus Status { get; private set; } = NotificationStatus.Unread;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; private set; }
    
    public string? ContextData { get; private set; }
    public string? ImageUrl { get; private set; }
    
    public User User { get; private set; } = null!;

    public Notification(Guid userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Medium, string? contextData = null, string? imageUrl = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        Priority = priority;
        ContextData = contextData;
        ImageUrl = imageUrl;
        Status = NotificationStatus.Unread;
        CreatedAt = DateTime.UtcNow;
    }

    private Notification() { }
    
    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Read)
            return;
            
        Status = NotificationStatus.Read;
        ReadAt = DateTime.UtcNow;
    }
} 