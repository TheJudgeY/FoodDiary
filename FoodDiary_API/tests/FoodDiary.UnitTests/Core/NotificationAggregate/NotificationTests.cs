using System;
using System.Reflection;
using Xunit;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.UseCases.Notifications;

namespace FoodDiary.UnitTests.Core.NotificationAggregate;

public class NotificationTests
{
    private readonly INotificationService _notificationService;

    public NotificationTests()
    {
        _notificationService = new NotificationService();
    }

    private void SetNavigationProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, value);
    }

    [Fact]
    public void Constructor_WithValidData_CreatesNotificationWithDefaultValues()
    {
        var userId = Guid.NewGuid();
        var title = "Test Notification";
        var message = "This is a test notification";
        var type = NotificationType.WaterReminder;

        var notification = new Notification(userId, title, message, type);

        Assert.NotEqual(Guid.Empty, notification.Id);
        Assert.Equal(userId, notification.UserId);
        Assert.Equal(title, notification.Title);
        Assert.Equal(message, notification.Message);
        Assert.Equal(type, notification.Type);
        Assert.Equal(NotificationStatus.Unread, notification.Status);
        Assert.Equal(NotificationPriority.Medium, notification.Priority);
        Assert.NotEqual(default(DateTime), notification.CreatedAt);
        Assert.Null(notification.ReadAt);
        Assert.Null(notification.ContextData);
    }

    [Fact]
    public void MarkAsRead_WhenUnread_ChangesStatusToRead()
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);

        notification.MarkAsRead();

        Assert.Equal(NotificationStatus.Read, notification.Status);
        Assert.True(_notificationService.IsRead(notification));
        Assert.False(_notificationService.IsUnread(notification));
        Assert.NotNull(notification.ReadAt);
        Assert.True(notification.ReadAt <= DateTime.UtcNow);
        Assert.True(notification.ReadAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void MarkAsRead_WhenAlreadyRead_DoesNotChangeReadAt()
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);
        notification.MarkAsRead();
        var originalReadAt = notification.ReadAt;

        notification.MarkAsRead();

        Assert.Equal(NotificationStatus.Read, notification.Status);
        Assert.Equal(originalReadAt, notification.ReadAt);
    }

    [Fact]
    public void MarkAsRead_WhenUnread_UpdatesReadAt()
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);
        var originalReadAt = notification.ReadAt;

        notification.MarkAsRead();

        Assert.Equal(NotificationStatus.Read, notification.Status);
        Assert.NotEqual(originalReadAt, notification.ReadAt);
        Assert.NotNull(notification.ReadAt);
    }

    [Fact]
    public void Age_ReturnsCorrectTimeSpan()
    {
        var createdAt = DateTime.UtcNow.AddHours(-2);
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);
        var createdAtProperty = typeof(Notification).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance);
        createdAtProperty?.SetValue(notification, createdAt);

        var age = _notificationService.GetAge(notification);

        Assert.True(age.TotalHours >= 1.9 && age.TotalHours <= 2.1);
    }

    [Fact]
    public void IsRecent_WhenLessThan24Hours_ReturnsTrue()
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);

        var isRecent = _notificationService.IsRecent(notification);

        Assert.True(isRecent);
    }

    [Fact]
    public void IsRecent_WhenMoreThan24Hours_ReturnsFalse()
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);
        var createdAtProperty = typeof(Notification).GetProperty("CreatedAt", BindingFlags.Public | BindingFlags.Instance);
        createdAtProperty?.SetValue(notification, DateTime.UtcNow.AddDays(-2));

        var isRecent = _notificationService.IsRecent(notification);

        Assert.False(isRecent);
    }

    [Theory]
    [InlineData(NotificationPriority.Low, "blue")]
    [InlineData(NotificationPriority.Medium, "yellow")]
    [InlineData(NotificationPriority.High, "orange")]
    [InlineData(NotificationPriority.Urgent, "red")]
    public void GetPriorityColor_ReturnsCorrectColor(NotificationPriority priority, string expectedColor)
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);
        var priorityProperty = typeof(Notification).GetProperty("Priority", BindingFlags.Public | BindingFlags.Instance);
        priorityProperty?.SetValue(notification, priority);

        var color = _notificationService.GetPriorityColor(notification);

        Assert.Equal(expectedColor, color);
    }

    [Theory]
    [InlineData(NotificationType.WaterReminder, "💧")]
    [InlineData(NotificationType.MealReminder, "🍽️")]
    [InlineData(NotificationType.CalorieLimitWarning, "⚠️")]
    [InlineData(NotificationType.GoalAchievement, "🎉")]

    [InlineData(NotificationType.WeeklyProgress, "📊")]
    [InlineData(NotificationType.DailySummary, "📋")]
    public void GetTypeIcon_ReturnsCorrectIcon(NotificationType type, string expectedIcon)
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", type);

        var icon = _notificationService.GetTypeIcon(notification);

        Assert.Equal(expectedIcon, icon);
    }

    [Fact]
    public void GetTypeIcon_WithUnknownType_ReturnsDefaultIcon()
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);
        var typeProperty = typeof(Notification).GetProperty("Type", BindingFlags.Public | BindingFlags.Instance);
        typeProperty?.SetValue(notification, (NotificationType)999);

        var icon = _notificationService.GetTypeIcon(notification);

        Assert.Equal("📢", icon);
    }

    [Fact]
    public void GetPriorityColor_WithUnknownPriority_ReturnsGray()
    {
        var notification = new Notification(Guid.NewGuid(), "Test", "Message", NotificationType.WaterReminder);
        var priorityProperty = typeof(Notification).GetProperty("Priority", BindingFlags.Public | BindingFlags.Instance);
        priorityProperty?.SetValue(notification, (NotificationPriority)999);

        var color = _notificationService.GetPriorityColor(notification);

        Assert.Equal("gray", color);
    }

    [Fact]
    public void Constructor_WithContextData_CreatesNotificationWithContextData()
    {
        var userId = Guid.NewGuid();
        var title = "Test Notification";
        var message = "This is a test notification";
        var type = NotificationType.WaterReminder;
        var contextData = "{\"page\":\"water\",\"action\":\"remind\"}";

        var notification = new Notification(userId, title, message, type, NotificationPriority.Medium, contextData);

        Assert.Equal(contextData, notification.ContextData);
    }

    [Fact]
    public void Constructor_WithPriority_CreatesNotificationWithPriority()
    {
        var userId = Guid.NewGuid();
        var title = "Test Notification";
        var message = "This is a test notification";
        var type = NotificationType.WaterReminder;
        var priority = NotificationPriority.High;

        var notification = new Notification(userId, title, message, type, priority);

        Assert.Equal(priority, notification.Priority);
    }

    [Fact]
    public void Constructor_WithImageUrl_CreatesNotificationWithImageUrl()
    {
        var userId = Guid.NewGuid();
        var title = "Test Notification";
        var message = "This is a test notification";
        var type = NotificationType.WaterReminder;
        var imageUrl = "https://example.com/image.jpg";

        var notification = new Notification(userId, title, message, type, NotificationPriority.Medium, null, imageUrl);

        Assert.Equal(imageUrl, notification.ImageUrl);
    }
} 