using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Analytics.GetDailyAnalysis;

namespace FoodDiary.Web.Analytics;

[Authorize]
public class GetDailyAnalysisEndpoint : Endpoint<GetDailyAnalysisRequest, GetDailyAnalysisResponse>
{
    private readonly IMediator _mediator;

    public GetDailyAnalysisEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/analytics/daily");
        Summary(s =>
        {
            s.Summary = "Get daily nutritional analysis";
            s.Description = "Generates a comprehensive analysis of the user's nutritional intake for a specific date, including progress towards goals and recommendations.";
        });
    }

    public override async Task HandleAsync(GetDailyAnalysisRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new GetDailyAnalysisCommand
        {
            UserId = userId,
            Date = req.Date ?? DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc)
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

public class GetDailyAnalysisRequest
{
    public DateTime? Date { get; set; }
} 