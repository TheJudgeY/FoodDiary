using MediatR;
using Ardalis.Result;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Users.UpdateFitnessGoal;

public record UpdateFitnessGoalCommand : IRequest<Result<UpdateFitnessGoalResponse>>
{
    public Guid UserId { get; init; }
    public FitnessGoal FitnessGoal { get; init; }
    public double? TargetWeightKg { get; init; }
}

public record UpdateFitnessGoalResponse
{
    public Guid UserId { get; init; }
    public FitnessGoal FitnessGoal { get; init; }
    public double? TargetWeightKg { get; init; }
    public double? RecommendedCalories { get; init; }
    public string Message { get; init; } = string.Empty;
} 