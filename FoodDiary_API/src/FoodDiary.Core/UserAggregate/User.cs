using System;
using System.Collections.Generic;
using FoodDiary.Core.NotificationAggregate;

namespace FoodDiary.Core.UserAggregate;

public enum Gender
{
    Male,
    Female,
    Other
}

public enum ActivityLevel
{
    Sedentary,
    LightlyActive,
    ModeratelyActive,
    VeryActive,
    ExtremelyActive
}

public enum FitnessGoal
{
    LoseWeight,
    MaintainWeight,
    GainWeight
}

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool EmailConfirmed { get; private set; }
    public string? EmailConfirmationToken { get; private set; }
    public DateTime? EmailConfirmationTokenExpiresAt { get; private set; }

    public double? HeightCm { get; private set; }
    public double? WeightKg { get; private set; }
    public int? Age { get; private set; }
    public Gender? Gender { get; private set; }
    public ActivityLevel? ActivityLevel { get; private set; }
    
    public FitnessGoal? FitnessGoal { get; private set; }
    public double? TargetWeightKg { get; private set; }
    public double? DailyCalorieGoal { get; private set; }
    public double? DailyProteinGoal { get; private set; }
    public double? DailyFatGoal { get; private set; }
    public double? DailyCarbohydrateGoal { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    
    public string TimeZoneId { get; private set; } = NotificationDefaults.DefaultTimeZoneId;

    public ICollection<FoodEntryAggregate.FoodEntry> FoodEntries { get; private set; } = new List<FoodEntryAggregate.FoodEntry>();
    public ICollection<NotificationAggregate.Notification> Notifications { get; private set; } = new List<NotificationAggregate.Notification>();
    public NotificationAggregate.NotificationPreferences? NotificationPreferences { get; private set; }

    public User(string email, string passwordHash, string name)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        Name = name;
        CreatedAt = DateTime.UtcNow;
    }

    private User() { }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void UpdateName(string name)
    {
        Name = name;
    }

    public void SetEmailConfirmationToken(string token, DateTime expiresAt)
    {
        EmailConfirmationToken = token;
        EmailConfirmationTokenExpiresAt = expiresAt;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
        EmailConfirmationToken = null;
        EmailConfirmationTokenExpiresAt = null;
    }

    public void UpdateBodyMetrics(double heightCm, double weightKg, int age, Gender gender, ActivityLevel activityLevel)
    {
        HeightCm = heightCm;
        WeightKg = weightKg;
        Age = age;
        Gender = gender;
        ActivityLevel = activityLevel;
    }

    public void UpdateFitnessGoal(FitnessGoal goal, double? targetWeight = null)
    {
        FitnessGoal = goal;
        TargetWeightKg = targetWeight;
    }

    public void UpdateMacronutrientGoals(double? proteinGoal, double? fatGoal, double? carbGoal, double? calorieGoal = null)
    {
        DailyProteinGoal = proteinGoal;
        DailyFatGoal = fatGoal;
        DailyCarbohydrateGoal = carbGoal;
        DailyCalorieGoal = calorieGoal;
    }

    public void UpdateTimeZone(string timeZoneId)
    {
        TimeZoneId = timeZoneId;
    }
}
