using FoodDiary.Core.NotificationAggregate;

namespace FoodDiary.UseCases.Notifications;

public interface INotificationService
{
    bool IsUnread(Notification notification);
    bool IsRead(Notification notification);
    
    TimeSpan GetAge(Notification notification);
    bool IsRecent(Notification notification);
    
    string GetPriorityColor(Notification notification);
    string GetTypeIcon(Notification notification);
}

public class NotificationService : INotificationService
{
    private const int RecentHoursThreshold = 24;

    public bool IsUnread(Notification notification) =>
        notification.Status == NotificationStatus.Unread;

    public bool IsRead(Notification notification) =>
        notification.Status == NotificationStatus.Read;

    public TimeSpan GetAge(Notification notification) =>
        DateTime.UtcNow - notification.CreatedAt;

    public bool IsRecent(Notification notification) =>
        GetAge(notification).TotalHours < RecentHoursThreshold;

    public string GetPriorityColor(Notification notification) =>
        notification.Priority switch
        {
            NotificationPriority.Low => "blue",
            NotificationPriority.Medium => "yellow",
            NotificationPriority.High => "orange",
            NotificationPriority.Urgent => "red",
            _ => "gray"
        };

    public string GetTypeIcon(Notification notification) =>
        notification.Type switch
        {
            NotificationType.WaterReminder => "💧",
            NotificationType.MealReminder => "🍽️",
            NotificationType.CalorieLimitWarning => "⚠️",
            NotificationType.GoalAchievement => "🎉",

            NotificationType.WeeklyProgress => "📊",
            NotificationType.DailySummary => "📋",
            _ => "📢"
        };
} 