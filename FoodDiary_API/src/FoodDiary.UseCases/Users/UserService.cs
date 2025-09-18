using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Validation;
using FluentValidation;

namespace FoodDiary.UseCases.Users;

public interface IUserService
{
    double? CalculateBMI(User user);
    string? GetBMICategory(User user);
    
    double? CalculateBMR(User user);
    double? CalculateTDEE(User user);
    double? CalculateRecommendedCalories(User user);
    
    DateTime GetLocalTime(User user, DateTime utcTime);
    DateTime GetUtcTime(User user, DateTime localTime);
    
    bool HasCompleteProfile(User user);
    void ValidateUser(User user);
    void ValidateBodyMetrics(double height, double weight, int age);
    void ValidateMacronutrientGoals(double? protein, double? fat, double? carbs);
    
    Task<double?> CalculateBMIAsync(User user);
    Task<string?> GetBMICategoryAsync(User user);
    Task<double?> CalculateBMRAsync(User user);
    Task<double?> CalculateTDEEAsync(User user);
    Task<double?> CalculateRecommendedCaloriesAsync(User user);
    Task<DateTime> GetLocalTimeAsync(User user, DateTime utcTime);
    Task<DateTime> GetUtcTimeAsync(User user, DateTime localTime);
}

public class UserService : IUserService
{
    private const double WeightLossCalorieMultiplier = 0.85;
    private const double WeightGainCalorieMultiplier = 1.15;
    private const double WeightMaintenanceCalorieMultiplier = 1.0;
    
    private const double UnderweightThreshold = 18.5;
    private const double NormalWeightThreshold = 25.0;
    private const double OverweightThreshold = 30.0;

    private const double BmrWeightMultiplier = 10.0;
    private const double BmrHeightMultiplier = 6.25;
    private const double BmrAgeMultiplier = 5.0;
    private const double BmrMaleConstant = 5.0;
    private const double BmrFemaleConstant = -161.0;

    private static readonly Dictionary<ActivityLevel, double> ActivityMultipliers = new()
    {
        { ActivityLevel.Sedentary, 1.2 },
        { ActivityLevel.LightlyActive, 1.375 },
        { ActivityLevel.ModeratelyActive, 1.55 },
        { ActivityLevel.VeryActive, 1.725 },
        { ActivityLevel.ExtremelyActive, 1.9 }
    };

    public double? CalculateBMI(User user)
    {
        if (!HasValidBMIData(user))
            return null;

        var heightM = user.HeightCm!.Value / 100;
        return user.WeightKg!.Value / (heightM * heightM);
    }

    public async Task<double?> CalculateBMIAsync(User user) =>
        await Task.Run(() => CalculateBMI(user));

    public string? GetBMICategory(User user)
    {
        var bmi = CalculateBMI(user);
        if (bmi == null) return null;

        return bmi.Value switch
        {
            < UnderweightThreshold => "Underweight",
            < NormalWeightThreshold => "Normal weight",
            < OverweightThreshold => "Overweight",
            _ => "Obese"
        };
    }

    public async Task<string?> GetBMICategoryAsync(User user) =>
        await Task.Run(() => GetBMICategory(user));

    public double? CalculateBMR(User user)
    {
        if (!HasValidBMRData(user))
            return null;

        var baseBMR = CalculateBaseBMR(user);
        return Math.Round(baseBMR, 0);
    }

    public async Task<double?> CalculateBMRAsync(User user) =>
        await Task.Run(() => CalculateBMR(user));

    public double? CalculateTDEE(User user)
    {
        var bmr = CalculateBMR(user);
        if (bmr == null || user.ActivityLevel == null)
            return null;

        var activityMultiplier = ActivityMultipliers.GetValueOrDefault(user.ActivityLevel.Value, 1.2);
        var tdee = bmr.Value * activityMultiplier;
        
        return Math.Round(tdee, 0);
    }

    public async Task<double?> CalculateTDEEAsync(User user) =>
        await Task.Run(() => CalculateTDEE(user));

    public double? CalculateRecommendedCalories(User user)
    {
        var tdee = CalculateTDEE(user);
        if (tdee == null || user.FitnessGoal == null)
            return null;

        var calorieMultiplier = GetCalorieMultiplierForGoal(user.FitnessGoal.Value);
        var recommendedCalories = tdee.Value * calorieMultiplier;
        
        return Math.Round(recommendedCalories, 0);
    }

    public async Task<double?> CalculateRecommendedCaloriesAsync(User user) =>
        await Task.Run(() => CalculateRecommendedCalories(user));

    public DateTime GetLocalTime(User user, DateTime utcTime)
    {
        if (string.IsNullOrEmpty(user.TimeZoneId))
            return utcTime;

        try
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            return utcTime;
        }
    }

    public async Task<DateTime> GetLocalTimeAsync(User user, DateTime utcTime) =>
        await Task.Run(() => GetLocalTime(user, utcTime));

    public DateTime GetUtcTime(User user, DateTime localTime)
    {
        if (string.IsNullOrEmpty(user.TimeZoneId))
            return localTime;

        try
        {
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZoneId);
            return TimeZoneInfo.ConvertTimeToUtc(localTime, timezone);
        }
        catch (TimeZoneNotFoundException)
        {
            return localTime;
        }
    }

    public async Task<DateTime> GetUtcTimeAsync(User user, DateTime localTime) =>
        await Task.Run(() => GetUtcTime(user, localTime));

    public bool HasCompleteProfile(User user) =>
        !string.IsNullOrWhiteSpace(user.Email) &&
        user.HeightCm.HasValue &&
        user.WeightKg.HasValue &&
        user.Age.HasValue &&
        user.Gender.HasValue &&
        user.ActivityLevel.HasValue &&
        user.FitnessGoal.HasValue;

    public void ValidateUser(User user)
    {
        var validator = new UserValidator();
        var validationResult = validator.Validate(user);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"User validation failed: {errors}");
        }
    }

    public void ValidateBodyMetrics(double height, double weight, int age)
    {
        if (height <= 0)
            throw new ArgumentException("Height must be greater than 0", nameof(height));
        
        if (weight <= 0)
            throw new ArgumentException("Weight must be greater than 0", nameof(weight));
        
        if (age <= 0)
            throw new ArgumentException("Age must be greater than 0", nameof(age));
        
        if (height > 300)
            throw new ArgumentException("Height cannot exceed 300 cm", nameof(height));
        
        if (weight > 1000)
            throw new ArgumentException("Weight cannot exceed 1000 kg", nameof(weight));
        
        if (age > 150)
            throw new ArgumentException("Age cannot exceed 150 years", nameof(age));
    }

    public void ValidateMacronutrientGoals(double? protein, double? fat, double? carbs)
    {
        if (protein.HasValue && (protein.Value < 0 || protein.Value > 1000))
            throw new ArgumentException("Protein goal must be between 0 and 1000 grams", nameof(protein));
        
        if (fat.HasValue && (fat.Value < 0 || fat.Value > 1000))
            throw new ArgumentException("Fat goal must be between 0 and 1000 grams", nameof(fat));
        
        if (carbs.HasValue && (carbs.Value < 0 || carbs.Value > 1000))
            throw new ArgumentException("Carbohydrate goal must be between 0 and 1000 grams", nameof(carbs));
    }

    private static bool HasValidBMIData(User user) =>
        user.HeightCm.HasValue && user.WeightKg.HasValue &&
        user.HeightCm.Value > 0 && user.WeightKg.Value > 0;

    private static bool HasValidBMRData(User user) =>
        user.HeightCm.HasValue && user.WeightKg.HasValue && user.Age.HasValue &&
        user.HeightCm.Value > 0 && user.WeightKg.Value > 0 && user.Age.Value > 0;

    private static double CalculateBaseBMR(User user)
    {
        var weightComponent = BmrWeightMultiplier * user.WeightKg!.Value;
        var heightComponent = BmrHeightMultiplier * user.HeightCm!.Value;
        var ageComponent = BmrAgeMultiplier * user.Age!.Value;
        var genderConstant = user.Gender == Gender.Male ? BmrMaleConstant : BmrFemaleConstant;
        
        return weightComponent + heightComponent - ageComponent + genderConstant;
    }

    private static double GetCalorieMultiplierForGoal(FitnessGoal goal) =>
        goal switch
        {
            FitnessGoal.LoseWeight => WeightLossCalorieMultiplier,
            FitnessGoal.GainWeight => WeightGainCalorieMultiplier,
            FitnessGoal.MaintainWeight => WeightMaintenanceCalorieMultiplier,
            _ => WeightMaintenanceCalorieMultiplier
        };
} 