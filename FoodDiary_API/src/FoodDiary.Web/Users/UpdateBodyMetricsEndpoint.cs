using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Users.UpdateBodyMetrics;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Web.Users;

[Authorize]
public class UpdateBodyMetricsEndpoint : Endpoint<UpdateBodyMetricsRequest, UpdateBodyMetricsResponse>
{
    private readonly IMediator _mediator;

    public UpdateBodyMetricsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/users/body-metrics");
        Summary(s =>
        {
            s.Summary = "Update user body metrics";
            s.Description = "Updates the user's body metrics including height, weight, age, gender, and activity level. Calculates BMI, BMR, and TDEE.";
        });
    }

    public override async Task HandleAsync(UpdateBodyMetricsRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new UpdateBodyMetricsCommand
        {
            UserId = userId,
            HeightCm = req.HeightCm,
            WeightKg = req.WeightKg,
            Age = req.Age,
            Gender = req.Gender,
            ActivityLevel = req.ActivityLevel
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

public class UpdateBodyMetricsRequest
{
    public double HeightCm { get; set; }
    public double WeightKg { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public ActivityLevel ActivityLevel { get; set; }
} 