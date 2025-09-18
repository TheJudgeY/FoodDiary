using FoodDiary.Core.Interfaces;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.Core.UserAggregate;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FoodDiary.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    private const string WATER_REMINDER_TITLE = "💧 Time to Hydrate!";
    private const string WATER_REMINDER_MESSAGE = "Don't forget to drink water. Staying hydrated is important for your health and fitness goals.";
    private const string CALORIE_WARNING_TITLE = "⚠️ Calorie Limit Warning";
    private const string CALORIE_WARNING_MESSAGE = "You're approaching your daily calorie limit. Consider adjusting your portions.";
    private const string GOAL_ACHIEVEMENT_TITLE = "🎉 Goal Achieved!";
    private const string GOAL_ACHIEVEMENT_MESSAGE = "Congratulations! You've achieved one of your fitness goals. Keep up the great work!";
    private const string WEEKLY_PROGRESS_TITLE = "📊 Weekly Progress Summary";
    private const string WEEKLY_PROGRESS_MESSAGE = "Your weekly nutrition and fitness progress summary is ready. Check it out to see how you're doing!";
    private const string DAILY_SUMMARY_TITLE = "📋 Daily Summary";
    private const string DAILY_SUMMARY_MESSAGE = "Your daily nutrition summary is ready. See how you did today and get personalized recommendations!";

    private const int BREAKFAST_START_HOUR = 7;
    private const int BREAKFAST_END_HOUR = 9;
    private const int LUNCH_START_HOUR = 12;
    private const int LUNCH_END_HOUR = 14;
    private const int DINNER_START_HOUR = 18;
    private const int DINNER_END_HOUR = 20;

    private const int DEFAULT_PAGE_SIZE = 20;

    private const int DAYS_TO_KEEP_READ_NOTIFICATIONS = 3;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    #region Core Notification Operations

    public async Task<Notification> CreateNotificationAsync(Guid userId, string title, string message, NotificationType type, NotificationPriority priority = NotificationPriority.Medium, string? contextData = null, string? imageUrl = null)
    {
        var notification = new Notification(userId, title, message, type, priority, contextData, imageUrl);

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return notification;
    }

    public async Task<List<Notification>> GetUserNotificationsAsync(Guid userId, int page = 1, int pageSize = DEFAULT_PAGE_SIZE, bool includeRead = false)
    {
        var query = _context.Notifications
            .Where(n => n.UserId == userId);

        if (!includeRead)
        {
            query = query.Where(n => n.Status == NotificationStatus.Unread);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Notification?> GetNotificationAsync(Guid notificationId, Guid userId)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
    }

    public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId, Guid userId)
    {
        var notification = await GetNotificationAsync(notificationId, userId);
        if (notification == null) return false;

        notification.MarkAsRead();
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && n.Status == NotificationStatus.Unread);
    }

    public async Task<bool> MarkAllNotificationsAsReadAsync(Guid userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.Status == NotificationStatus.Unread)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(Guid notificationId, Guid userId)
    {
        var notification = await GetNotificationAsync(notificationId, userId);
        if (notification == null) return false;

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> CleanupOldReadNotificationsAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-DAYS_TO_KEEP_READ_NOTIFICATIONS);
        
        var oldReadNotifications = await _context.Notifications
            .Where(n => n.Status == NotificationStatus.Read && n.ReadAt < cutoffDate)
            .ToListAsync();

        if (oldReadNotifications.Any())
        {
            _context.Notifications.RemoveRange(oldReadNotifications);
            await _context.SaveChangesAsync();
        }

        return oldReadNotifications.Count;
    }

    public async Task<int> CleanupOldReadNotificationsForUserAsync(Guid userId)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-DAYS_TO_KEEP_READ_NOTIFICATIONS);
        
        var oldReadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && n.Status == NotificationStatus.Read && n.ReadAt < cutoffDate)
            .ToListAsync();

        if (oldReadNotifications.Any())
        {
            _context.Notifications.RemoveRange(oldReadNotifications);
            await _context.SaveChangesAsync();
        }

        return oldReadNotifications.Count;
    }

    #endregion

    #region Smart Notification Creation

    public async Task<Notification?> CreateWaterReminderAsync(Guid userId)
    {
        var preferences = await GetUserPreferencesAsync(userId);
        if (!preferences.WaterRemindersEnabled) return null;

        var contextData = JsonSerializer.Serialize(new
        {
            action = "navigate",
            target = "dashboard",
            tab = "hydration",
            section = "water-tracking"
        });

        return await CreateNotificationAsync(
            userId,
            WATER_REMINDER_TITLE,
            WATER_REMINDER_MESSAGE,
            NotificationType.WaterReminder,
            NotificationPriority.Low,
            contextData
        );
    }

    public async Task<Notification?> CreateMealReminderAsync(Guid userId, DateTime localTime)
    {
        var preferences = await GetUserPreferencesAsync(userId);
        if (!preferences.MealRemindersEnabled) return null;

        var (title, message) = GetMealReminderContent(localTime);
        var mealType = GetMealType(localTime);

        var contextData = JsonSerializer.Serialize(new
        {
            action = "navigate",
            target = "food-entry",
            mealType = mealType.ToLower(),
            suggested = mealType.ToLower(),
            prefill = true
        });

        return await CreateNotificationAsync(
            userId,
            title,
            message,
            NotificationType.MealReminder,
            NotificationPriority.Medium,
            contextData
        );
    }

    public async Task<Notification?> CreateCalorieLimitWarningAsync(Guid userId)
    {
        var preferences = await GetUserPreferencesAsync(userId);
        if (!preferences.CalorieLimitWarningsEnabled) return null;

        var contextData = JsonSerializer.Serialize(new
        {
            action = "navigate",
            target = "analytics",
            view = "daily",
            focus = "calories",
            showRecommendations = true
        });

        return await CreateNotificationAsync(
            userId,
            CALORIE_WARNING_TITLE,
            CALORIE_WARNING_MESSAGE,
            NotificationType.CalorieLimitWarning,
            NotificationPriority.High,
            contextData
        );
    }

    public async Task<Notification?> CreateGoalAchievementNotificationAsync(Guid userId)
    {
        var preferences = await GetUserPreferencesAsync(userId);
        if (!preferences.GoalAchievementsEnabled) return null;

        var contextData = JsonSerializer.Serialize(new
        {
            action = "navigate",
            target = "goals",
            showAchievement = true,
            celebrate = true
        });

        return await CreateNotificationAsync(
            userId,
            GOAL_ACHIEVEMENT_TITLE,
            GOAL_ACHIEVEMENT_MESSAGE,
            NotificationType.GoalAchievement,
            NotificationPriority.Medium,
            contextData
        );
    }



    public async Task<Notification?> CreateWeeklyProgressNotificationAsync(Guid userId)
    {
        var preferences = await GetUserPreferencesAsync(userId);
        if (!preferences.WeeklyProgressEnabled) return null;

        var contextData = JsonSerializer.Serialize(new
        {
            action = "navigate",
            target = "analytics",
            view = "trends",
            period = "week",
            showInsights = true
        });

        return await CreateNotificationAsync(
            userId,
            WEEKLY_PROGRESS_TITLE,
            WEEKLY_PROGRESS_MESSAGE,
            NotificationType.WeeklyProgress,
            NotificationPriority.Low,
            contextData
        );
    }

    public async Task<Notification?> CreateDailySummaryNotificationAsync(Guid userId)
    {
        var preferences = await GetUserPreferencesAsync(userId);
        if (!preferences.DailySummaryEnabled) return null;

        var contextData = JsonSerializer.Serialize(new
        {
            action = "navigate",
            target = "analytics",
            view = "daily",
            showRecommendations = true,
            focus = "nextDay"
        });

        return await CreateNotificationAsync(
            userId,
            DAILY_SUMMARY_TITLE,
            DAILY_SUMMARY_MESSAGE,
            NotificationType.DailySummary,
            NotificationPriority.Low,
            contextData
        );
    }

    #endregion

    #region Private Helper Methods

    private async Task<NotificationPreferences> GetUserPreferencesAsync(Guid userId)
    {
        var preferences = await _context.NotificationPreferences
            .FirstOrDefaultAsync(np => np.UserId == userId);

        if (preferences == null)
        {
            preferences = CreateDefaultPreferences(userId);
            _context.NotificationPreferences.Add(preferences);
            await _context.SaveChangesAsync();
        }

        return preferences;
    }

    private static NotificationPreferences CreateDefaultPreferences(Guid userId)
    {
        return new NotificationPreferences(userId);
    }

    private static (string title, string message) GetMealReminderContent(DateTime localTime)
    {
        var mealType = GetMealType(localTime);
        var title = $"🍽️ {mealType} Time!";
        var message = $"It's time for {mealType.ToLower()}. Don't forget to log your meal to stay on track with your nutrition goals!";

        return (title, message);
    }

    private static string GetMealType(DateTime localTime)
    {
        var hour = localTime.Hour;

        if (hour >= BREAKFAST_START_HOUR && hour <= BREAKFAST_END_HOUR)
            return "Breakfast";
        else if (hour >= LUNCH_START_HOUR && hour <= LUNCH_END_HOUR)
            return "Lunch";
        else if (hour >= DINNER_START_HOUR && hour <= DINNER_END_HOUR)
            return "Dinner";
        else
            return "Snack";
    }

    #endregion
} 