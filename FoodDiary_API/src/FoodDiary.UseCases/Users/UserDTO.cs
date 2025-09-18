using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Users;

public class UserDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
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
    
    public double? BMI { get; set; }
    public string? BMICategory { get; set; }
    public double? BMR { get; set; }
    public double? TDEE { get; set; }
    public double? RecommendedCalories { get; set; }
    public bool HasCompleteProfile { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string TimeZoneId { get; set; } = "UTC";
}
