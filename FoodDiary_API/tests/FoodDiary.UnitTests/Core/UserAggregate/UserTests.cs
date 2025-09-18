using System.Reflection;
using Xunit;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users;

namespace FoodDiary.UnitTests.Core.UserAggregate;

public class UserTests
{
    private readonly IUserService _userService;

    public UserTests()
    {
        _userService = new UserService();
    }

    private void SetNavigationProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, value);
    }

    [Fact]
    public void CalculateBMI_WithValidMetrics_ReturnsCorrectBMI()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var bmi = _userService.CalculateBMI(user);

        Assert.NotNull(bmi);
        Assert.Equal(22.86, bmi.Value, 2);
    }

    [Fact]
    public void CalculateBMI_WithMissingHeight_ReturnsNull()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(0, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var bmi = _userService.CalculateBMI(user);

        Assert.Null(bmi);
    }

    [Fact]
    public void CalculateBMI_WithMissingWeight_ReturnsNull()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 0, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var bmi = _userService.CalculateBMI(user);

        Assert.Null(bmi);
    }

    [Fact]
    public void GetBMICategory_WithUnderweightBMI_ReturnsUnderweight()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 50, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var category = _userService.GetBMICategory(user);

        Assert.Equal("Underweight", category);
    }

    [Fact]
    public void GetBMICategory_WithNormalBMI_ReturnsNormalWeight()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var category = _userService.GetBMICategory(user);

        Assert.Equal("Normal weight", category);
    }

    [Fact]
    public void GetBMICategory_WithOverweightBMI_ReturnsOverweight()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 85, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var category = _userService.GetBMICategory(user);

        Assert.Equal("Overweight", category);
    }

    [Fact]
    public void GetBMICategory_WithObeseBMI_ReturnsObese()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 100, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var category = _userService.GetBMICategory(user);

        Assert.Equal("Obese", category);
    }

    [Fact]
    public void CalculateBMR_WithMaleUser_ReturnsCorrectBMR()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var bmr = _userService.CalculateBMR(user);

        Assert.NotNull(bmr);
        Assert.Equal(1649, bmr.Value, 0);
    }

    [Fact]
    public void CalculateBMR_WithFemaleUser_ReturnsCorrectBMR()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(165, 60, 25, Gender.Female, ActivityLevel.ModeratelyActive);

        var bmr = _userService.CalculateBMR(user);

        Assert.NotNull(bmr);
        Assert.Equal(1345, bmr.Value, 0);
    }

    [Fact]
    public void CalculateBMR_WithMissingData_ReturnsNull()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");

        var bmr = _userService.CalculateBMR(user);

        Assert.Null(bmr);
    }

    [Fact]
    public void CalculateTDEE_WithSedentaryActivity_ReturnsCorrectTDEE()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.Sedentary);

        var tdee = _userService.CalculateTDEE(user);

        Assert.NotNull(tdee);
        Assert.Equal(1979, tdee.Value, 0);
    }

    [Fact]
    public void CalculateTDEE_WithVeryActiveActivity_ReturnsCorrectTDEE()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.VeryActive);

        var tdee = _userService.CalculateTDEE(user);

        Assert.NotNull(tdee);
        Assert.Equal(2845, tdee.Value, 0);
    }

    [Fact]
    public void CalculateRecommendedCalories_WithLoseWeightGoal_ReturnsDeficit()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);
        user.UpdateFitnessGoal(FitnessGoal.LoseWeight, 65);

        var calories = _userService.CalculateRecommendedCalories(user);

        Assert.NotNull(calories);
        Assert.Equal(2173, calories.Value, 0);
    }

    [Fact]
    public void CalculateRecommendedCalories_WithGainWeightGoal_ReturnsSurplus()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);
        user.UpdateFitnessGoal(FitnessGoal.GainWeight, 75);

        var calories = _userService.CalculateRecommendedCalories(user);

        Assert.NotNull(calories);
        Assert.Equal(2939, calories.Value, 0);
    }



    [Fact]
    public void UpdateBodyMetrics_WithValidData_UpdatesProperties()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        var height = 180.0;
        var weight = 75.0;
        var age = 35;
        var gender = Gender.Female;
        var activityLevel = ActivityLevel.VeryActive;

        user.UpdateBodyMetrics(height, weight, age, gender, activityLevel);

        Assert.Equal(height, user.HeightCm);
        Assert.Equal(weight, user.WeightKg);
        Assert.Equal(age, user.Age);
        Assert.Equal(gender, user.Gender);
        Assert.Equal(activityLevel, user.ActivityLevel);
        Assert.NotEqual(default(DateTime), user.UpdatedAt);
    }

    [Fact]
    public void UpdateFitnessGoal_WithValidData_UpdatesProperties()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        var goal = FitnessGoal.LoseWeight;
        var targetWeight = 65.0;

        user.UpdateFitnessGoal(goal, targetWeight);

        Assert.Equal(goal, user.FitnessGoal);
        Assert.Equal(targetWeight, user.TargetWeightKg);
        Assert.NotEqual(default(DateTime), user.UpdatedAt);
    }

    [Fact]
    public void UpdateMacronutrientGoals_WithValidData_UpdatesProperties()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        var proteinGoal = 150.0;
        var fatGoal = 65.0;
        var carbGoal = 200.0;

        user.UpdateMacronutrientGoals(proteinGoal, fatGoal, carbGoal);

        Assert.Equal(proteinGoal, user.DailyProteinGoal);
        Assert.Equal(fatGoal, user.DailyFatGoal);
        Assert.Equal(carbGoal, user.DailyCarbohydrateGoal);
        Assert.NotEqual(default(DateTime), user.UpdatedAt);
    }

    [Fact]
    public void HasCompleteProfile_WithCompleteData_ReturnsTrue()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);
        user.UpdateFitnessGoal(FitnessGoal.MaintainWeight, 70);
        user.UpdateMacronutrientGoals(150, 65, 200);

        var isComplete = _userService.HasCompleteProfile(user);

        Assert.True(isComplete);
    }

    [Fact]
    public void HasCompleteProfile_WithIncompleteData_ReturnsFalse()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateBodyMetrics(175, 70, 30, Gender.Male, ActivityLevel.ModeratelyActive);

        var isComplete = _userService.HasCompleteProfile(user);

        Assert.False(isComplete);
    }

    [Fact]
    public void GetLocalTime_WithValidTimezone_ReturnsCorrectTime()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateTimeZone("America/New_York");
        var utcTime = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var localTime = _userService.GetLocalTime(user, utcTime);

        Assert.Equal(7, localTime.Hour);
    }

    [Fact]
    public void GetUtcTime_WithValidTimezone_ReturnsCorrectTime()
    {
        var user = new User("test@example.com", "hashedPassword", "Test User");
        user.UpdateTimeZone("America/New_York");
        var localTime = new DateTime(2024, 1, 1, 7, 0, 0, DateTimeKind.Unspecified);

        var utcTime = _userService.GetUtcTime(user, localTime);

        Assert.Equal(12, utcTime.Hour);
    }
} 