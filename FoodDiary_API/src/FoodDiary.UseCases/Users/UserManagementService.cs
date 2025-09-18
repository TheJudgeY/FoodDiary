using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Users;

public interface IUserManagementService
{
    void UpdatePasswordHash(User user, string newPasswordHash);
    void UpdateName(User user, string name);
    void SetEmailConfirmationToken(User user, string token, DateTime expiresAt);
    void ConfirmEmail(User user);
    void UpdateBodyMetrics(User user, double heightCm, double weightKg, int age, Gender gender, ActivityLevel activityLevel);
    void UpdateFitnessGoal(User user, FitnessGoal goal, double? targetWeight = null);
    void UpdateMacronutrientGoals(User user, double? proteinGoal, double? fatGoal, double? carbGoal, double? calorieGoal = null);
    void UpdateTimeZone(User user, string timeZoneId);
}

public class UserManagementService : IUserManagementService
{
    public void UpdatePasswordHash(User user, string newPasswordHash)
    {
        user.UpdatePasswordHash(newPasswordHash);
        SetUpdatedTimestamp(user);
    }

    public void UpdateName(User user, string name)
    {
        user.UpdateName(name);
        SetUpdatedTimestamp(user);
    }

    public void SetEmailConfirmationToken(User user, string token, DateTime expiresAt)
    {
        user.SetEmailConfirmationToken(token, expiresAt);
        SetUpdatedTimestamp(user);
    }

    public void ConfirmEmail(User user)
    {
        user.ConfirmEmail();
        SetUpdatedTimestamp(user);
    }

    public void UpdateBodyMetrics(User user, double heightCm, double weightKg, int age, Gender gender, ActivityLevel activityLevel)
    {
        user.UpdateBodyMetrics(heightCm, weightKg, age, gender, activityLevel);
        SetUpdatedTimestamp(user);
    }

    public void UpdateFitnessGoal(User user, FitnessGoal goal, double? targetWeight = null)
    {
        user.UpdateFitnessGoal(goal, targetWeight);
        SetUpdatedTimestamp(user);
    }

    public void UpdateMacronutrientGoals(User user, double? proteinGoal, double? fatGoal, double? carbGoal, double? calorieGoal = null)
    {
        user.UpdateMacronutrientGoals(proteinGoal, fatGoal, carbGoal, calorieGoal);
        SetUpdatedTimestamp(user);
    }

    public void UpdateTimeZone(User user, string timeZoneId)
    {
        user.UpdateTimeZone(timeZoneId);
        SetUpdatedTimestamp(user);
    }

    private static void SetUpdatedTimestamp(User user)
    {
        var updatedAtProperty = typeof(User).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(user, DateTime.UtcNow);
    }
}
