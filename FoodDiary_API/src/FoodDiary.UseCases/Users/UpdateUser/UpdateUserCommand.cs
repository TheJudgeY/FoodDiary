using MediatR;
using Ardalis.Result;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Users.UpdateUser;

public record UpdateUserCommand : IRequest<Result<UpdateUserResponse>>
{
    public Guid UserId { get; init; }
    
    public string? Name { get; init; }
    
    public double? HeightCm { get; init; }
    public double? WeightKg { get; init; }
    public int? Age { get; init; }
    public Gender? Gender { get; init; }
    public ActivityLevel? ActivityLevel { get; init; }
    
    public FitnessGoal? FitnessGoal { get; init; }
    public double? TargetWeightKg { get; init; }
    
    public double? DailyCalorieGoal { get; init; }
    public double? DailyProteinGoal { get; init; }
    public double? DailyFatGoal { get; init; }
    public double? DailyCarbohydrateGoal { get; init; }
    
    public string? TimeZoneId { get; init; }
}

public record UpdateUserResponse
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    
    public double? HeightCm { get; init; }
    public double? WeightKg { get; init; }
    public int? Age { get; init; }
    public Gender? Gender { get; init; }
    public ActivityLevel? ActivityLevel { get; init; }
    
    public FitnessGoal? FitnessGoal { get; init; }
    public double? TargetWeightKg { get; init; }
    
    public double? DailyCalorieGoal { get; init; }
    public double? DailyProteinGoal { get; init; }
    public double? DailyFatGoal { get; init; }
    public double? DailyCarbohydrateGoal { get; init; }
    
    public double? BMI { get; init; }
    public string? BMICategory { get; init; }
    public double? BMR { get; init; }
    public double? TDEE { get; init; }
    public double? RecommendedCalories { get; init; }
    
    public string TimeZoneId { get; init; } = string.Empty;
    
    public string Message { get; init; } = string.Empty;
}
