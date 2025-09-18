using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Notifications.GetNotifications;

namespace FoodDiary.Web.Notifications;

[Authorize]
public class GetNotificationsEndpoint : Endpoint<GetNotificationsRequest, GetNotificationsResponse>
{
    private readonly IMediator _mediator;

    public GetNotificationsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/notifications");
        Summary(s =>
        {
            s.Summary = "Get user notifications";
            s.Description = "Retrieves notifications for the authenticated user with pagination support.";
        });
    }

    public override async Task HandleAsync(GetNotificationsRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new GetNotificationsCommand
        {
            UserId = userId,
            Page = req.Page ?? 1,
            PageSize = req.PageSize ?? 20,
            IncludeRead = req.IncludeRead ?? false
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }
}

public class GetNotificationsRequest
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public bool? IncludeRead { get; set; }
} 