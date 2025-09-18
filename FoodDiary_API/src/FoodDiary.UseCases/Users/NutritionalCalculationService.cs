using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Users;

public interface INutritionalCalculationService
{
    double? CalculateBMI(User user);
    string? GetBMICategory(User user);
    double? CalculateBMR(User user);
    double? CalculateTDEE(User user);
    double? CalculateRecommendedCalories(User user);
}

public class NutritionalCalculationService : INutritionalCalculationService
{
    private const double WEIGHT_LOSS_CALORIE_MULTIPLIER = 0.85;
    private const double WEIGHT_GAIN_CALORIE_MULTIPLIER = 1.15;
    private const double WEIGHT_MAINTENANCE_CALORIE_MULTIPLIER = 1.0;
    
    private static readonly Dictionary<ActivityLevel, double> ActivityMultipliers = new()
    {
        { ActivityLevel.Sedentary, 1.2 },
        { ActivityLevel.LightlyActive, 1.375 },
        { ActivityLevel.ModeratelyActive, 1.55 },
        { ActivityLevel.VeryActive, 1.725 },
        { ActivityLevel.ExtremelyActive, 1.9 }
    };

    private const double UNDERWEIGHT_THRESHOLD = 18.5;
    private const double NORMAL_WEIGHT_THRESHOLD = 25.0;
    private const double OVERWEIGHT_THRESHOLD = 30.0;

    private const double BMR_WEIGHT_MULTIPLIER = 10.0;
    private const double BMR_HEIGHT_MULTIPLIER = 6.25;
    private const double BMR_AGE_MULTIPLIER = 5.0;
    private const double BMR_MALE_CONSTANT = 5.0;
    private const double BMR_FEMALE_CONSTANT = -161.0;

    public double? CalculateBMI(User user)
    {
        if (!HasValidBMIData(user))
            return null;

        var heightM = user.HeightCm!.Value / 100;
        return user.WeightKg!.Value / (heightM * heightM);
    }

    public string? GetBMICategory(User user)
    {
        var bmi = CalculateBMI(user);
        if (bmi == null) return null;

        return bmi.Value switch
        {
            < UNDERWEIGHT_THRESHOLD => "Underweight",
            < NORMAL_WEIGHT_THRESHOLD => "Normal weight",
            < OVERWEIGHT_THRESHOLD => "Overweight",
            _ => "Obese"
        };
    }

    public double? CalculateBMR(User user)
    {
        if (!HasValidBMRData(user))
            return null;

        var baseBMR = CalculateBaseBMR(user);
        return user.Gender!.Value switch
        {
            Gender.Male => baseBMR + BMR_MALE_CONSTANT,
            Gender.Female => baseBMR + BMR_FEMALE_CONSTANT,
            _ => null
        };
    }

    public double? CalculateTDEE(User user)
    {
        var bmr = CalculateBMR(user);
        if (bmr == null || user.ActivityLevel == null)
            return null;

        var multiplier = ActivityMultipliers.GetValueOrDefault(user.ActivityLevel.Value, ActivityMultipliers[ActivityLevel.Sedentary]);
        return bmr.Value * multiplier;
    }

    public double? CalculateRecommendedCalories(User user)
    {
        var tdee = CalculateTDEE(user);
        if (tdee == null || user.FitnessGoal == null)
            return null;

        var multiplier = GetCalorieMultiplierForGoal(user.FitnessGoal.Value);
        return tdee.Value * multiplier;
    }

    private bool HasValidBMIData(User user)
    {
        return user.HeightCm.HasValue && user.WeightKg.HasValue && 
               user.HeightCm.Value > 0 && user.WeightKg.Value > 0;
    }

    private bool HasValidBMRData(User user)
    {
        return user.WeightKg.HasValue && user.HeightCm.HasValue && 
               user.Age.HasValue && user.Gender.HasValue;
    }

    private double CalculateBaseBMR(User user)
    {
        return (BMR_WEIGHT_MULTIPLIER * user.WeightKg!.Value) + 
               (BMR_HEIGHT_MULTIPLIER * user.HeightCm!.Value) - 
               (BMR_AGE_MULTIPLIER * user.Age!.Value);
    }

    private double GetCalorieMultiplierForGoal(FitnessGoal goal)
    {
        return goal switch
        {
            FitnessGoal.LoseWeight => WEIGHT_LOSS_CALORIE_MULTIPLIER,
            FitnessGoal.MaintainWeight => WEIGHT_MAINTENANCE_CALORIE_MULTIPLIER,
            FitnessGoal.GainWeight => WEIGHT_GAIN_CALORIE_MULTIPLIER,
            _ => WEIGHT_MAINTENANCE_CALORIE_MULTIPLIER
        };
    }
} 