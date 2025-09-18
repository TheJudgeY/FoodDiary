using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Web.Users;

[Authorize]
public class UpdateTimeZoneEndpoint : Endpoint<UpdateTimeZoneRequest, UpdateTimeZoneResponse>
{
    private readonly IRepository<User> _userRepository;

    public UpdateTimeZoneEndpoint(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public override void Configure()
    {
        Put("/api/users/timezone");
        Summary(s =>
        {
            s.Summary = "Update user timezone";
            s.Description = "Updates the user's timezone preference for notification scheduling";
        });
    }

    public override async Task HandleAsync(UpdateTimeZoneRequest request, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var userResult = await _userRepository.GetByIdAsync(userId);
        if (!userResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var user = userResult.Value;

        try
        {
            user.UpdateTimeZone(request.TimeZoneId);
            await _userRepository.UpdateAsync(user);

            var response = new UpdateTimeZoneResponse
            {
                Success = true,
                TimeZoneId = user.TimeZoneId,
                Message = "Timezone updated successfully"
            };

            await SendAsync(response, 200, ct);
        }
        catch (ArgumentException ex)
        {
            var response = new UpdateTimeZoneResponse
            {
                Success = false,
                TimeZoneId = user.TimeZoneId,
                Message = ex.Message
            };

            await SendAsync(response, 400, ct);
        }
        catch (Exception)
        {
            var response = new UpdateTimeZoneResponse
            {
                Success = false,
                TimeZoneId = user.TimeZoneId,
                Message = "Error updating timezone"
            };

            await SendAsync(response, 500, ct);
        }
    }
}

public class UpdateTimeZoneRequest
{
    public string TimeZoneId { get; set; } = string.Empty;
}

public class UpdateTimeZoneResponse
{
    public bool Success { get; set; }
    public string TimeZoneId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
} 