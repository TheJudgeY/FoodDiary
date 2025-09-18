using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Analytics.GetRecommendations;

namespace FoodDiary.Web.Analytics;

[Authorize]
public class GetRecommendationsEndpoint : Endpoint<GetRecommendationsRequest, GetRecommendationsResponse>
{
    private readonly IMediator _mediator;

    public GetRecommendationsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/analytics/recommendations");
        Summary(s =>
        {
            s.Summary = "Get personalized recommendations";
            s.Description = "Generates personalized nutritional recommendations based on the user's current intake, goals, and eating patterns.";
        });
    }

    public override async Task HandleAsync(GetRecommendationsRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new GetRecommendationsCommand
        {
            UserId = userId,
            Date = req.Date ?? DateTime.UtcNow.Date
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
        }
        else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }
}

public class GetRecommendationsRequest
{
    public DateTime? Date { get; set; }
} 