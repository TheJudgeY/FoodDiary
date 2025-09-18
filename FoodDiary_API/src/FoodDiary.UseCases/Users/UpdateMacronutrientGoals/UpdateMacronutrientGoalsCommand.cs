using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Users.UpdateMacronutrientGoals;

public record UpdateMacronutrientGoalsCommand : IRequest<Result<UpdateMacronutrientGoalsResponse>>
{
    public Guid UserId { get; init; }
    public double? DailyCalorieGoal { get; init; }
    public double? DailyProteinGoal { get; init; }
    public double? DailyFatGoal { get; init; }
    public double? DailyCarbohydrateGoal { get; init; }
}

public record UpdateMacronutrientGoalsResponse
{
    public Guid UserId { get; init; }
    public double? DailyCalorieGoal { get; init; }
    public double? DailyProteinGoal { get; init; }
    public double? DailyFatGoal { get; init; }
    public double? DailyCarbohydrateGoal { get; init; }
    public string Message { get; init; } = string.Empty;
} 