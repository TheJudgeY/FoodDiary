using FoodDiary.Core.NotificationAggregate;

namespace FoodDiary.Core.Interfaces;

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(Guid userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Medium, string? contextData = null, string? imageUrl = null);
    Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = 20, bool includeRead = false);
    Task<Notification?> GetNotificationAsync(Guid notificationId, Guid userId);
    Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId);
    Task<int> GetUnreadNotificationCountAsync(Guid userId);
    Task<bool> MarkAllNotificationsAsReadAsync(Guid userId);
    Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId);
    
    Task<Notification?> CreateWaterReminderAsync(Guid userId);
    Task<Notification?> CreateMealReminderAsync(Guid userId, DateTime localTime);
    Task<Notification?> CreateCalorieLimitWarningAsync(Guid userId);
    Task<Notification?> CreateGoalAchievementNotificationAsync(Guid userId);
    
    Task<Notification?> CreateWeeklyProgressNotificationAsync(Guid userId);
    Task<Notification?> CreateDailySummaryNotificationAsync(Guid userId);
    
    Task<int> CleanupOldReadNotificationsAsync();
    Task<int> CleanupOldReadNotificationsForUserAsync(Guid userId);
} 