using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Users.UpdateMacronutrientGoals;

namespace FoodDiary.Web.Users;

[Authorize]
public class UpdateMacronutrientGoalsEndpoint : Endpoint<UpdateMacronutrientGoalsRequest, UpdateMacronutrientGoalsResponse>
{
    private readonly IMediator _mediator;

    public UpdateMacronutrientGoalsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/users/macronutrient-goals");
        Summary(s =>
        {
            s.Summary = "Update user macronutrient goals";
            s.Description = "Updates the user's daily macronutrient goals for protein, fat, and carbohydrates.";
        });
    }

    public override async Task HandleAsync(UpdateMacronutrientGoalsRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new UpdateMacronutrientGoalsCommand
        {
            UserId = userId,
            DailyCalorieGoal = req.DailyCalorieGoal,
            DailyProteinGoal = req.DailyProteinGoal,
            DailyFatGoal = req.DailyFatGoal,
            DailyCarbohydrateGoal = req.DailyCarbohydrateGoal
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

public class UpdateMacronutrientGoalsRequest
{
    public double? DailyCalorieGoal { get; set; }
    public double? DailyProteinGoal { get; set; }
    public double? DailyFatGoal { get; set; }
    public double? DailyCarbohydrateGoal { get; set; }
} 