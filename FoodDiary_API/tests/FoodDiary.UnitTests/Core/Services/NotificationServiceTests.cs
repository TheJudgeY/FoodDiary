using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.Infrastructure.Services;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;
using Microsoft.EntityFrameworkCore.InMemory;
using System.Reflection;

namespace FoodDiary.UnitTests.Core.Services;

public class NotificationServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly INotificationService _notificationService;

    public NotificationServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new AppDbContext(options);
        _notificationService = new NotificationService(_context);
    }

    private void SetNavigationProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, value);
    }

    [Fact]
    public async Task CreateNotificationAsync_WithValidData_CreatesNotification()
    {
        var userId = Guid.NewGuid();
        var title = "Test Notification";
        var message = "This is a test notification";
        var type = NotificationType.WaterReminder;

        var result = await _notificationService.CreateNotificationAsync(userId, title, message, type);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(title, result.Title);
        Assert.Equal(message, result.Message);
        Assert.Equal(type, result.Type);
        Assert.Equal(NotificationStatus.Unread, result.Status);
        Assert.NotEqual(default(DateTime), result.CreatedAt);
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithValidUserId_ReturnsNotifications()
    {
        var userId = Guid.NewGuid();
        var notification1 = new Notification(userId, "Test 1", "Message 1", NotificationType.WaterReminder);
        var notification2 = new Notification(userId, "Test 2", "Message 2", NotificationType.MealReminder);

        await _context.Notifications.AddRangeAsync(notification1, notification2);
        await _context.SaveChangesAsync();

        var result = await _notificationService.GetUserNotificationsAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, n => Assert.Equal(userId, n.UserId));
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithPagination_ReturnsCorrectPage()
    {
        var userId = Guid.NewGuid();
        var notifications = Enumerable.Range(1, 10)
            .Select(i => new Notification(userId, $"Test {i}", $"Message {i}", NotificationType.WaterReminder))
            .ToList();

        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();

        var result = await _notificationService.GetUserNotificationsAsync(userId, page: 1, pageSize: 3);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task GetUserNotificationsAsync_WithIncludeReadFalse_ExcludesReadNotifications()
    {
        var userId = Guid.NewGuid();
        var unreadNotification = new Notification(userId, "Unread", "Message", NotificationType.WaterReminder);
        var readNotification = new Notification(userId, "Read", "Message", NotificationType.WaterReminder);
        readNotification.MarkAsRead();

        await _context.Notifications.AddRangeAsync(unreadNotification, readNotification);
        await _context.SaveChangesAsync();

        var result = await _notificationService.GetUserNotificationsAsync(userId, includeRead: false);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Unread", result.First().Title);
    }

    [Fact]
    public async Task GetNotificationAsync_WithValidId_ReturnsNotification()
    {
        var userId = Guid.NewGuid();
        var notification = new Notification(userId, "Test", "Message", NotificationType.WaterReminder);

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        var result = await _notificationService.GetNotificationAsync(notification.Id, userId);

        Assert.NotNull(result);
        Assert.Equal(notification.Id, result.Id);
        Assert.Equal(userId, result.UserId);
    }

    [Fact]
    public async Task GetNotificationAsync_WithInvalidId_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var invalidId = Guid.NewGuid();

        var result = await _notificationService.GetNotificationAsync(invalidId, userId);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetNotificationAsync_WithWrongUserId_ReturnsNull()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var notification = new Notification(userId1, "Test", "Message", NotificationType.WaterReminder);

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        var result = await _notificationService.GetNotificationAsync(notification.Id, userId2);

        Assert.Null(result);
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_WithValidNotification_MarksAsRead()
    {
        var userId = Guid.NewGuid();
        var notification = new Notification(userId, "Test", "Message", NotificationType.WaterReminder);

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        var result = await _notificationService.MarkNotificationAsReadAsync(notification.Id, userId);

        Assert.True(result);
        
        var updatedNotification = await _context.Notifications.FindAsync(notification.Id);
        Assert.NotNull(updatedNotification);
        Assert.Equal(NotificationStatus.Read, updatedNotification.Status);
        Assert.NotNull(updatedNotification.ReadAt);
    }

    [Fact]
    public async Task MarkNotificationAsReadAsync_WithInvalidId_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        var invalidId = Guid.NewGuid();

        var result = await _notificationService.MarkNotificationAsReadAsync(invalidId, userId);

        Assert.False(result);
    }

    [Fact]
    public async Task GetUnreadNotificationCountAsync_WithValidUserId_ReturnsCorrectCount()
    {
        var userId = Guid.NewGuid();
        var unread1 = new Notification(userId, "Unread 1", "Message", NotificationType.WaterReminder);
        var unread2 = new Notification(userId, "Unread 2", "Message", NotificationType.WaterReminder);
        var read = new Notification(userId, "Read", "Message", NotificationType.WaterReminder);
        read.MarkAsRead();
        var read2 = new Notification(userId, "Read 2", "Message", NotificationType.WaterReminder);
        read2.MarkAsRead();

        await _context.Notifications.AddRangeAsync(unread1, unread2, read, read2);
        await _context.SaveChangesAsync();

        var count = await _notificationService.GetUnreadNotificationCountAsync(userId);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task MarkAllNotificationsAsReadAsync_WithValidUserId_MarksAllAsRead()
    {
        var userId = Guid.NewGuid();
        var unread1 = new Notification(userId, "Unread 1", "Message", NotificationType.WaterReminder);
        var unread2 = new Notification(userId, "Unread 2", "Message", NotificationType.WaterReminder);
        var read = new Notification(userId, "Read", "Message", NotificationType.WaterReminder);
        read.MarkAsRead();

        await _context.Notifications.AddRangeAsync(unread1, unread2, read);
        await _context.SaveChangesAsync();

        var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);

        Assert.True(result);
        
        var updatedNotifications = await _context.Notifications.Where(n => n.UserId == userId).ToListAsync();
        Assert.All(updatedNotifications.Where(n => n.Title.StartsWith("Unread")), n => 
        {
            Assert.Equal(NotificationStatus.Read, n.Status);
            Assert.NotNull(n.ReadAt);
        });
    }

    [Fact]
    public async Task DeleteNotificationAsync_WithValidNotification_DeletesNotification()
    {
        var userId = Guid.NewGuid();
        var notification = new Notification(userId, "Test", "Message", NotificationType.WaterReminder);

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();

        var result = await _notificationService.DeleteNotificationAsync(notification.Id, userId);

        Assert.True(result);
        
        var deletedNotification = await _context.Notifications.FindAsync(notification.Id);
        Assert.Null(deletedNotification);
    }

    [Fact]
    public async Task CreateWaterReminderAsync_WithValidUserId_CreatesWaterReminder()
    {
        var userId = Guid.NewGuid();

        var result = await _notificationService.CreateWaterReminderAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(NotificationType.WaterReminder, result.Type);
        Assert.Equal(NotificationStatus.Unread, result.Status);
    }

    [Fact]
    public async Task CreateMealReminderAsync_WithValidData_CreatesMealReminder()
    {
        var userId = Guid.NewGuid();
        var localTime = DateTime.UtcNow;

        var result = await _notificationService.CreateMealReminderAsync(userId, localTime);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(NotificationType.MealReminder, result.Type);
        Assert.Contains("time", result.Title.ToLower());
    }

    [Fact]
    public async Task CreateCalorieLimitWarningAsync_WithValidUserId_CreatesWarning()
    {
        var userId = Guid.NewGuid();

        var result = await _notificationService.CreateCalorieLimitWarningAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(NotificationType.CalorieLimitWarning, result.Type);
        Assert.Equal(NotificationPriority.High, result.Priority);
    }

    [Fact]
    public async Task CreateGoalAchievementNotificationAsync_WithValidUserId_CreatesAchievementNotification()
    {
        var userId = Guid.NewGuid();

        var result = await _notificationService.CreateGoalAchievementNotificationAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(NotificationType.GoalAchievement, result.Type);
        Assert.Equal(NotificationPriority.Medium, result.Priority);
    }



    [Fact]
    public async Task CreateWeeklyProgressNotificationAsync_WithValidUserId_CreatesProgressNotification()
    {
        var userId = Guid.NewGuid();

        var result = await _notificationService.CreateWeeklyProgressNotificationAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(NotificationType.WeeklyProgress, result.Type);
        Assert.Contains("weekly", result.Title.ToLower());
    }

    [Fact]
    public async Task CreateDailySummaryNotificationAsync_WithValidUserId_CreatesSummaryNotification()
    {
        var userId = Guid.NewGuid();

        var result = await _notificationService.CreateDailySummaryNotificationAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(NotificationType.DailySummary, result.Type);
        Assert.Contains("daily", result.Title.ToLower());
    }

    public void Dispose()
    {
        _context.Dispose();
    }
} 