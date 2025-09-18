using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class CreateNotificationPreferencesEndpoint : EndpointWithoutRequest<CreateNotificationPreferencesResponse>
{
    private readonly AppDbContext _context;
    private readonly ILogger<CreateNotificationPreferencesEndpoint> _logger;

    public CreateNotificationPreferencesEndpoint(
        AppDbContext context,
        ILogger<CreateNotificationPreferencesEndpoint> logger)
    {
        _context = context;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/notifications/preferences");
        Summary(s =>
        {
            s.Summary = "Create default notification preferences";
            s.Description = "Creates default notification preferences for the current user";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        try
        {
            var existingPreferences = await _context.NotificationPreferences
                .FirstOrDefaultAsync(np => np.UserId == userId, ct);

            if (existingPreferences != null)
            {
                await SendAsync(new CreateNotificationPreferencesResponse
                {
                    Success = false,
                    Message = "Notification preferences already exist for this user"
                }, 400, ct);
                return;
            }

            var preferences = new NotificationPreferences(userId);

            _context.NotificationPreferences.Add(preferences);
            await _context.SaveChangesAsync(ct);

            _logger.LogInformation("Created default notification preferences for user {UserId}", userId);

            await SendAsync(new CreateNotificationPreferencesResponse
            {
                Success = true,
                Message = "Default notification preferences created successfully"
            }, 201, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating notification preferences for user {UserId}", userId);
            
            await SendAsync(new CreateNotificationPreferencesResponse
            {
                Success = false,
                Message = $"Error creating notification preferences: {ex.Message}"
            }, 500, ct);
        }
    }
}

public class CreateNotificationPreferencesResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
} 