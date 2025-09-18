using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users;
using FoodDiary.Core.NotificationAggregate;

namespace FoodDiary.Infrastructure.Services;

public class NotificationSchedulerService : INotificationSchedulerService
{
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationSchedulerService> _logger;
    private readonly IUserService _userService;

    private const int DAILY_NOTIFICATION_START_HOUR = 7;
    private const int DAILY_NOTIFICATION_END_HOUR = 9;
    private const int BREAKFAST_HOUR = 8;
    private const int LUNCH_HOUR = 12;
    private const int DINNER_HOUR = 18;
    private const int SNACK_HOUR = 15;

    public NotificationSchedulerService(
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        INotificationService notificationService,
        ILogger<NotificationSchedulerService> logger,
        IUserService userService)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
        _userService = userService;
    }

    #region Daily Notifications

    public async Task ProcessDailyNotificationsAsync()
    {
        _logger.LogInformation("Processing daily notifications for all users");
        
        var users = await GetUsersAsync();
        if (users == null) return;

        foreach (var user in users)
        {
            await ProcessDailyNotificationForUserAsync(user);
        }
    }

    private async Task ProcessDailyNotificationForUserAsync(User user)
    {
        try
        {
            var userLocalTime = await _userService.GetLocalTimeAsync(user, DateTime.UtcNow);
            
            if (IsWithinDailyNotificationWindow(userLocalTime))
            {
                await _notificationService.CreateWaterReminderAsync(user.Id);
                _logger.LogInformation("Sent daily water reminder to user {UserId}", user.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing daily notification for user {UserId}", user.Id);
        }
    }

    private bool IsWithinDailyNotificationWindow(DateTime localTime)
    {
        return localTime.Hour >= DAILY_NOTIFICATION_START_HOUR && localTime.Hour <= DAILY_NOTIFICATION_END_HOUR;
    }

    #endregion

    #region Meal Reminders

    public async Task ProcessMealRemindersAsync()
    {
        _logger.LogInformation("Processing meal reminder notifications for all users");
        
        var users = await GetUsersAsync();
        if (users == null) return;

        foreach (var user in users)
        {
            await ProcessMealReminderForUserAsync(user);
        }
    }

    private async Task ProcessMealReminderForUserAsync(User user)
    {
        try
        {
            var userLocalTime = await _userService.GetLocalTimeAsync(user, DateTime.UtcNow);
            
            if (IsMealTime(userLocalTime))
            {
                await _notificationService.CreateMealReminderAsync(user.Id, userLocalTime);
                _logger.LogInformation("Sent meal reminder to user {UserId} at {LocalTime}", user.Id, userLocalTime);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing meal reminder for user {UserId}", user.Id);
        }
    }

    private static bool IsMealTime(DateTime localTime)
    {
        return localTime.Hour == BREAKFAST_HOUR || 
               localTime.Hour == LUNCH_HOUR || 
               localTime.Hour == DINNER_HOUR || 
               localTime.Hour == SNACK_HOUR;
    }

    #endregion

    #region Calorie Limit Warnings

    public async Task ProcessCalorieLimitWarningsAsync()
    {
        _logger.LogInformation("Processing calorie limit warnings for all users");
        
        var users = await GetUsersAsync();
        if (users == null) return;

        foreach (var user in users)
        {
            await ProcessCalorieLimitWarningForUserAsync(user);
        }
    }

    private async Task ProcessCalorieLimitWarningForUserAsync(User user)
    {
        try
        {
            var userLocalTime = await _userService.GetLocalTimeAsync(user, DateTime.UtcNow);
            
            if (IsEveningTime(userLocalTime))
            {
                await _notificationService.CreateCalorieLimitWarningAsync(user.Id);
                _logger.LogInformation("Sent calorie limit warning to user {UserId}", user.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing calorie limit warning for user {UserId}", user.Id);
        }
    }

    private static bool IsEveningTime(DateTime localTime)
    {
        return localTime.Hour >= 19 && localTime.Hour <= 21;
    }

    #endregion

    #region Weekly Progress

    public async Task ProcessWeeklyProgressNotificationsAsync()
    {
        _logger.LogInformation("Processing weekly progress notifications for all users");
        
        var users = await GetUsersAsync();
        if (users == null) return;

        foreach (var user in users)
        {
            await ProcessWeeklyProgressForUserAsync(user);
        }
    }

    private async Task ProcessWeeklyProgressForUserAsync(User user)
    {
        try
        {
            var userLocalTime = await _userService.GetLocalTimeAsync(user, DateTime.UtcNow);
            
            if (IsWeeklyProgressTime(userLocalTime))
            {
                await _notificationService.CreateWeeklyProgressNotificationAsync(user.Id);
                _logger.LogInformation("Sent weekly progress notification to user {UserId}", user.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing weekly progress for user {UserId}", user.Id);
        }
    }

    private static bool IsWeeklyProgressTime(DateTime localTime)
    {
        return localTime.DayOfWeek == DayOfWeek.Sunday && localTime.Hour >= 18 && localTime.Hour <= 20;
    }

    #endregion

    #region Goal Achievement

    public async Task ProcessGoalAchievementNotificationsAsync()
    {
        _logger.LogInformation("Processing goal achievement notifications for all users");
        
        var users = await GetUsersAsync();
        if (users == null) return;

        foreach (var user in users)
        {
            await ProcessGoalAchievementForUserAsync(user);
        }
    }

    private async Task ProcessGoalAchievementForUserAsync(User user)
    {
        try
        {
            var userLocalTime = await _userService.GetLocalTimeAsync(user, DateTime.UtcNow);
            
            if (IsGoalAchievementTime(userLocalTime))
            {
                await _notificationService.CreateGoalAchievementNotificationAsync(user.Id);
                _logger.LogInformation("Sent goal achievement notification to user {UserId}", user.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing goal achievement for user {UserId}", user.Id);
        }
    }

    private static bool IsGoalAchievementTime(DateTime localTime)
    {
        return localTime.Hour >= 8 && localTime.Hour <= 10;
    }

    #endregion

    #region Helper Methods

    private async Task<List<User>?> GetUsersAsync()
    {
        var usersResult = await _userRepository.ListAsync();
        if (!usersResult.IsSuccess)
        {
            _logger.LogError("Failed to retrieve users for notifications");
            return null;
        }
        return usersResult.Value;
    }

    #endregion
} 