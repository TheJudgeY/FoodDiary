using System;
using System.Reflection;
using Xunit;
using FoodDiary.Core.NotificationAggregate;

namespace FoodDiary.UnitTests.Core.NotificationAggregate;

public class NotificationPreferencesTests
{
    private void SetProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, value);
    }

    [Fact]
    public void Constructor_WithValidData_CreatesPreferencesWithDefaultValues()
    {
        var userId = Guid.NewGuid();
        var preferences = new NotificationPreferences(userId);

        Assert.NotEqual(Guid.Empty, preferences.Id);
        Assert.Equal(userId, preferences.UserId);
        Assert.True(preferences.WaterRemindersEnabled);
        Assert.True(preferences.MealRemindersEnabled);
        Assert.True(preferences.CalorieLimitWarningsEnabled);
        Assert.True(preferences.GoalAchievementsEnabled);

        Assert.True(preferences.WeeklyProgressEnabled);
        Assert.True(preferences.DailySummaryEnabled);
        Assert.Equal(new TimeSpan(9, 0, 0), preferences.WaterReminderTime);
        Assert.Equal(new TimeSpan(8, 0, 0), preferences.BreakfastReminderTime);
        Assert.Equal(new TimeSpan(12, 0, 0), preferences.LunchReminderTime);
        Assert.Equal(new TimeSpan(18, 0, 0), preferences.DinnerReminderTime);
        Assert.Equal(2, preferences.WaterReminderFrequencyHours);
        Assert.True(preferences.SendNotificationsOnWeekends);
        Assert.NotEqual(default(DateTime), preferences.CreatedAt);
        Assert.NotEqual(default(DateTime), preferences.UpdatedAt);
    }

    [Theory]
    [InlineData(NotificationType.WaterReminder, true)]
    [InlineData(NotificationType.MealReminder, true)]
    [InlineData(NotificationType.CalorieLimitWarning, true)]
    [InlineData(NotificationType.GoalAchievement, true)]

    [InlineData(NotificationType.WeeklyProgress, true)]
    [InlineData(NotificationType.DailySummary, true)]
    public void IsNotificationTypeEnabled_WithDefaultPreferences_ReturnsTrue(NotificationType type, bool expected)
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());

        var result = preferences.IsNotificationTypeEnabled(type);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(NotificationType.WaterReminder)]
    [InlineData(NotificationType.MealReminder)]
    [InlineData(NotificationType.CalorieLimitWarning)]
    [InlineData(NotificationType.GoalAchievement)]

    [InlineData(NotificationType.WeeklyProgress)]
    [InlineData(NotificationType.DailySummary)]
    public void IsNotificationTypeEnabled_WithDisabledType_ReturnsFalse(NotificationType type)
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());
        
        switch (type)
        {
            case NotificationType.WaterReminder:
                SetProperty(preferences, "WaterRemindersEnabled", false);
                break;
            case NotificationType.MealReminder:
                SetProperty(preferences, "MealRemindersEnabled", false);
                break;
            case NotificationType.CalorieLimitWarning:
                SetProperty(preferences, "CalorieLimitWarningsEnabled", false);
                break;
            case NotificationType.GoalAchievement:
                SetProperty(preferences, "GoalAchievementsEnabled", false);
                break;

            case NotificationType.WeeklyProgress:
                SetProperty(preferences, "WeeklyProgressEnabled", false);
                break;
            case NotificationType.DailySummary:
                SetProperty(preferences, "DailySummaryEnabled", false);
                break;
        }

        var result = preferences.IsNotificationTypeEnabled(type);

        Assert.False(result);
    }

    [Fact]
    public void IsNotificationTypeEnabled_WithUnknownType_ReturnsTrue()
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());

        var result = preferences.IsNotificationTypeEnabled((NotificationType)999);

        Assert.True(result);
    }

    [Fact]
    public void UpdatePreferences_WithValidParameters_UpdatesOnlySpecifiedValues()
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());
        var originalUpdatedAt = preferences.UpdatedAt;

        preferences.UpdatePreferences(new NotificationPreferencesUpdate
        {
            WaterReminders = false,
            MealReminders = null,
            CalorieWarnings = false,
            GoalAchievements = null,

            WeeklyProgress = null,
            DailySummary = null,
            WaterTime = new TimeSpan(10, 0, 0),
            BreakfastTime = null,
            LunchTime = null,
            DinnerTime = null,
            WaterFrequency = 3,
            WeekendNotifications = false
        });

        Assert.False(preferences.WaterRemindersEnabled);
        Assert.True(preferences.MealRemindersEnabled);
        Assert.False(preferences.CalorieLimitWarningsEnabled);
        Assert.True(preferences.GoalAchievementsEnabled);

        Assert.True(preferences.WeeklyProgressEnabled);
        Assert.True(preferences.DailySummaryEnabled);
        Assert.Equal(new TimeSpan(10, 0, 0), preferences.WaterReminderTime);
        Assert.Equal(new TimeSpan(8, 0, 0), preferences.BreakfastReminderTime);
        Assert.Equal(new TimeSpan(12, 0, 0), preferences.LunchReminderTime);
        Assert.Equal(new TimeSpan(18, 0, 0), preferences.DinnerReminderTime);
        Assert.Equal(3, preferences.WaterReminderFrequencyHours);
        Assert.False(preferences.SendNotificationsOnWeekends);
        Assert.True(preferences.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void UpdatePreferences_WithNullParameters_DoesNotUpdateValues()
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());
        var originalWaterRemindersEnabled = preferences.WaterRemindersEnabled;
        var originalWaterReminderTime = preferences.WaterReminderTime;
        var originalUpdatedAt = preferences.UpdatedAt;

        preferences.UpdatePreferences(new NotificationPreferencesUpdate
        {
            WaterReminders = null,
            MealReminders = null,
            CalorieWarnings = null,
            GoalAchievements = null,

            WeeklyProgress = null,
            DailySummary = null,
            WaterTime = null,
            BreakfastTime = null,
            LunchTime = null,
            DinnerTime = null,
            WaterFrequency = null,
            WeekendNotifications = null
        });

        Assert.Equal(originalWaterRemindersEnabled, preferences.WaterRemindersEnabled);
        Assert.Equal(originalWaterReminderTime, preferences.WaterReminderTime);
        Assert.NotEqual(default(DateTime), preferences.UpdatedAt);
    }

    [Fact]
    public void ShouldSendNotificationToday_OnWeekday_ReturnsTrue()
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());

        var result = preferences.ShouldSendNotificationToday();

        Assert.True(result);
    }

    [Fact]
    public void ShouldSendNotificationToday_OnWeekendWithWeekendNotificationsEnabled_ReturnsTrue()
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());

        var result = preferences.ShouldSendNotificationToday();

        Assert.True(result);
    }

    [Fact]
    public void ShouldSendNotificationToday_OnWeekendWithWeekendNotificationsDisabled_ReturnsFalse()
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());
        preferences.UpdatePreferences(new NotificationPreferencesUpdate
        {
            WeekendNotifications = false
        });

        var result = preferences.ShouldSendNotificationToday();

        var today = DateTime.UtcNow.DayOfWeek;
        var isWeekend = today == DayOfWeek.Saturday || today == DayOfWeek.Sunday;
        var expected = !isWeekend;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void UpdatePreferences_UpdatesUpdatedAtTimestamp()
    {
        var preferences = new NotificationPreferences(Guid.NewGuid());
        var originalUpdatedAt = preferences.UpdatedAt;

        preferences.UpdatePreferences(new NotificationPreferencesUpdate
        {
            WaterReminders = false,
            MealReminders = null,
            CalorieWarnings = null,
            GoalAchievements = null,

            WeeklyProgress = null,
            DailySummary = null,
            WaterTime = null,
            BreakfastTime = null,
            LunchTime = null,
            DinnerTime = null,
            WaterFrequency = null,
            WeekendNotifications = null
        });

        Assert.True(preferences.UpdatedAt > originalUpdatedAt);
    }
} 