using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Analytics.GetTrends;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.Web.Analytics;

[Authorize]
public class GetTrendsEndpoint : Endpoint<GetTrendsRequest, GetTrendsResponse>
{
    private readonly IMediator _mediator;
    private readonly ITrendsService _trendsService;
    private readonly IRecommendationsService _recommendationsService;

    public GetTrendsEndpoint(IMediator mediator, ITrendsService trendsService, IRecommendationsService recommendationsService)
    {
        _mediator = mediator;
        _trendsService = trendsService;
        _recommendationsService = recommendationsService;
    }

    public override void Configure()
    {
        Post("/api/analytics/trends");
        Summary(s =>
        {
            s.Summary = "Get nutritional trends and recommendations";
            s.Description = "Generates nutritional trends, patterns, and personalized recommendations over a specified number of days.";
        });
    }

    public override async Task HandleAsync(GetTrendsRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new GetTrendsCommand
        {
            UserId = userId,
            Days = req.Days ?? 7
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            var insights = await _trendsService.GetTrendInsightsAsync(userId, req.Days ?? 7);
            var recommendations = await _recommendationsService.GetPersonalizedRecommendationsAsync(userId, req.Days ?? 7);
            
            var updatedResponse = new GetTrendsResponse
            {
                Trends = result.Value.Trends,
                Recommendations = recommendations,
                Message = result.Value.Message
            };
            
            await SendAsync(updatedResponse, 200, ct);
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

public class GetTrendsRequest
{
    public int? Days { get; set; }
} 