using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.Infrastructure.Services;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(15);

    public NotificationBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<NotificationBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogDebug("Running notification scheduler checks");

                using var scope = _serviceScopeFactory.CreateScope();
                var notificationScheduler = scope.ServiceProvider.GetRequiredService<INotificationSchedulerService>();

                await notificationScheduler.ProcessDailyNotificationsAsync();
                await notificationScheduler.ProcessMealRemindersAsync();
                await notificationScheduler.ProcessCalorieLimitWarningsAsync();
                await notificationScheduler.ProcessWeeklyProgressNotificationsAsync();
                await notificationScheduler.ProcessGoalAchievementNotificationsAsync();

                _logger.LogDebug("Completed notification scheduler checks");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing notifications");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Notification Background Service stopped");
    }
} 