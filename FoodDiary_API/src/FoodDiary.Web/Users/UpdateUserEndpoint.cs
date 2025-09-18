using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Users.UpdateUser;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Web.Users;

[Authorize]
public class UpdateUserEndpoint : Endpoint<UpdateUserRequest, UpdateUserResponse>
{
    private readonly IMediator _mediator;

    public UpdateUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/users/me");
        Summary(s =>
        {
            s.Summary = "Update user profile";
            s.Description = "Updates the current user's profile information including body metrics, fitness goals, macronutrient goals, and timezone. All fields are optional - only provided fields will be updated.";
        });
    }

    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new UpdateUserCommand
        {
            UserId = userId,
            
            Name = req.Name?.Trim(),
            
            HeightCm = req.HeightCm,
            WeightKg = req.WeightKg,
            Age = req.Age,
            Gender = req.Gender,
            ActivityLevel = req.ActivityLevel,
            
            FitnessGoal = req.FitnessGoal,
            TargetWeightKg = req.TargetWeightKg,
            
            DailyCalorieGoal = req.DailyCalorieGoal,
            DailyProteinGoal = req.DailyProteinGoal,
            DailyFatGoal = req.DailyFatGoal,
            DailyCarbohydrateGoal = req.DailyCarbohydrateGoal,
            
            TimeZoneId = req.TimeZoneId?.Trim()
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

public class UpdateUserRequest
{
    public string? Name { get; set; }
    
    public double? HeightCm { get; set; }
    public double? WeightKg { get; set; }
    public int? Age { get; set; }
    public Gender? Gender { get; set; }
    public ActivityLevel? ActivityLevel { get; set; }
    
    public FitnessGoal? FitnessGoal { get; set; }
    public double? TargetWeightKg { get; set; }
    
    public double? DailyCalorieGoal { get; set; }
    public double? DailyProteinGoal { get; set; }
    public double? DailyFatGoal { get; set; }
    public double? DailyCarbohydrateGoal { get; set; }
    
    public string? TimeZoneId { get; set; }
}
