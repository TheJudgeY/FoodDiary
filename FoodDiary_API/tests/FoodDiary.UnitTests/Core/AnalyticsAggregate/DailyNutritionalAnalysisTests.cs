using Xunit;
using FoodDiary.Core.AnalyticsAggregate;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UnitTests.Core.AnalyticsAggregate;

public class DailyNutritionalAnalysisTests
{
    private readonly IAnalyticsService _analyticsService;

    public DailyNutritionalAnalysisTests()
    {
        _analyticsService = new AnalyticsService();
    }

    [Fact]
    public void IsCalorieGoalMet_WithMetGoal_ReturnsTrue()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 1800, 90, 45, 180, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.IsCalorieGoalMet(analysis);

        Assert.True(result);
    }

    [Fact]
    public void IsCalorieGoalMet_WithUnmetGoal_ReturnsFalse()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 1500, 90, 45, 180, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.IsCalorieGoalMet(analysis);

        Assert.False(result);
    }

    [Fact]
    public void IsOverCalorieLimit_WithOverLimit_ReturnsTrue()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 2300, 90, 45, 180, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.IsOverCalorieLimit(analysis);

        Assert.True(result);
    }

    [Fact]
    public void IsOverCalorieLimit_WithUnderLimit_ReturnsFalse()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 1800, 90, 45, 180, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.IsOverCalorieLimit(analysis);

        Assert.False(result);
    }

    [Fact]
    public void GetOverallStatus_WithAllGoalsMet_ReturnsExcellent()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 2000, 100, 50, 200, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.GetOverallStatus(analysis);

        Assert.Equal("Excellent", result);
    }

    [Fact]
    public void GetOverallStatus_WithNoGoalsSet_ReturnsNoGoalsSet()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 1800, 90, 45, 180, 3);

        var result = _analyticsService.GetOverallStatus(analysis);

        Assert.Equal("No goals set", result);
    }



    [Fact]
    public void IsProteinGoalMet_WithMetGoal_ReturnsTrue()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 1800, 90, 45, 180, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.IsProteinGoalMet(analysis);

        Assert.True(result);
    }

    [Fact]
    public void IsFatGoalMet_WithMetGoal_ReturnsTrue()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 1800, 90, 45, 180, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.IsFatGoalMet(analysis);

        Assert.True(result);
    }

    [Fact]
    public void IsCarbohydrateGoalMet_WithMetGoal_ReturnsTrue()
    {
        var analysis = new DailyNutritionalAnalysis(Guid.NewGuid(), DateTime.Today, 1800, 90, 45, 180, 3);
        analysis.SetGoals(2000, 100, 50, 200);

        var result = _analyticsService.IsCarbohydrateGoalMet(analysis);

        Assert.True(result);
    }
} 