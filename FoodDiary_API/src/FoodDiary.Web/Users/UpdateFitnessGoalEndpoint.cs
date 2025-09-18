using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Users.UpdateFitnessGoal;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Web.Users;

[Authorize]
public class UpdateFitnessGoalEndpoint : Endpoint<UpdateFitnessGoalRequest, UpdateFitnessGoalResponse>
{
    private readonly IMediator _mediator;

    public UpdateFitnessGoalEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/users/fitness-goal");
        Summary(s =>
        {
            s.Summary = "Update user fitness goal";
            s.Description = "Updates the user's fitness goal and target weight. Calculates recommended daily calories based on the goal.";
        });
    }

    public override async Task HandleAsync(UpdateFitnessGoalRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new UpdateFitnessGoalCommand
        {
            UserId = userId,
            FitnessGoal = req.FitnessGoal,
            TargetWeightKg = req.TargetWeightKg
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

public class UpdateFitnessGoalRequest
{
    public FitnessGoal FitnessGoal { get; set; }
    public double? TargetWeightKg { get; set; }
} 