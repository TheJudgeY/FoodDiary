namespace FoodDiary.Core.Interfaces;

public interface INotificationSchedulerService
{
    Task ProcessDailyNotificationsAsync();
    Task ProcessMealRemindersAsync();
    Task ProcessCalorieLimitWarningsAsync();
    Task ProcessWeeklyProgressNotificationsAsync();
    Task ProcessGoalAchievementNotificationsAsync();
} 