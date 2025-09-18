using FoodDiary.Core.AnalyticsAggregate;

namespace FoodDiary.UseCases.Analytics;

public interface IAnalyticsService
{
    bool IsCalorieGoalMet(DailyNutritionalAnalysis analysis);
    bool IsProteinGoalMet(DailyNutritionalAnalysis analysis);
    bool IsFatGoalMet(DailyNutritionalAnalysis analysis);
    bool IsCarbohydrateGoalMet(DailyNutritionalAnalysis analysis);
    
    bool IsOverCalorieLimit(DailyNutritionalAnalysis analysis);
    bool IsOverProteinLimit(DailyNutritionalAnalysis analysis);
    bool IsOverFatLimit(DailyNutritionalAnalysis analysis);
    bool IsOverCarbohydrateLimit(DailyNutritionalAnalysis analysis);
    
    string GetOverallStatus(DailyNutritionalAnalysis analysis);
    Task<string> GetOverallStatusAsync(DailyNutritionalAnalysis analysis);
}

public class AnalyticsService : IAnalyticsService
{
    private const double GoalAchievementThreshold = 0.8;
    private const double ExcellentThreshold = 90.0;
    private const double GoodThreshold = 75.0;
    private const double FairThreshold = 60.0;
    private const double NeedsImprovementThreshold = 40.0;

    public bool IsCalorieGoalMet(DailyNutritionalAnalysis analysis) =>
        IsGoalMet(analysis.DailyCalorieGoal, analysis.TotalCalories);

    public bool IsProteinGoalMet(DailyNutritionalAnalysis analysis) =>
        IsGoalMet(analysis.DailyProteinGoal, analysis.TotalProtein);

    public bool IsFatGoalMet(DailyNutritionalAnalysis analysis) =>
        IsGoalMet(analysis.DailyFatGoal, analysis.TotalFat);

    public bool IsCarbohydrateGoalMet(DailyNutritionalAnalysis analysis) =>
        IsGoalMet(analysis.DailyCarbohydrateGoal, analysis.TotalCarbohydrates);

    public bool IsOverCalorieLimit(DailyNutritionalAnalysis analysis) =>
        IsOverLimit(analysis.DailyCalorieGoal, analysis.TotalCalories);

    public bool IsOverProteinLimit(DailyNutritionalAnalysis analysis) =>
        IsOverLimit(analysis.DailyProteinGoal, analysis.TotalProtein);

    public bool IsOverFatLimit(DailyNutritionalAnalysis analysis) =>
        IsOverLimit(analysis.DailyFatGoal, analysis.TotalFat);

    public bool IsOverCarbohydrateLimit(DailyNutritionalAnalysis analysis) =>
        IsOverLimit(analysis.DailyCarbohydrateGoal, analysis.TotalCarbohydrates);

    public string GetOverallStatus(DailyNutritionalAnalysis analysis)
    {
        var (metGoals, totalGoals) = CalculateGoalAchievement(analysis);
        
        if (totalGoals == 0) return "No goals set";
        
        var percentage = CalculateAchievementPercentage(metGoals, totalGoals);
        return DetermineStatusFromPercentage(percentage);
    }

    public async Task<string> GetOverallStatusAsync(DailyNutritionalAnalysis analysis) =>
        await Task.Run(() => GetOverallStatus(analysis));

    private static bool IsGoalMet(double? goal, double actual) =>
        goal.HasValue && actual >= goal.Value * GoalAchievementThreshold;

    private static bool IsOverLimit(double? goal, double actual) =>
        goal.HasValue && actual > goal.Value * 1.1;

    private static (int metGoals, int totalGoals) CalculateGoalAchievement(DailyNutritionalAnalysis analysis)
    {
        var metGoals = 0;
        var totalGoals = 0;
        
        if (analysis.DailyCalorieGoal.HasValue)
        {
            totalGoals++;
            if (IsGoalMet(analysis.DailyCalorieGoal, analysis.TotalCalories) && 
                !IsOverLimit(analysis.DailyCalorieGoal, analysis.TotalCalories)) 
                metGoals++;
        }
        
        if (analysis.DailyProteinGoal.HasValue)
        {
            totalGoals++;
            if (IsGoalMet(analysis.DailyProteinGoal, analysis.TotalProtein)) 
                metGoals++;
        }
        
        if (analysis.DailyFatGoal.HasValue)
        {
            totalGoals++;
            if (IsGoalMet(analysis.DailyFatGoal, analysis.TotalFat)) 
                metGoals++;
        }
        
        if (analysis.DailyCarbohydrateGoal.HasValue)
        {
            totalGoals++;
            if (IsGoalMet(analysis.DailyCarbohydrateGoal, analysis.TotalCarbohydrates)) 
                metGoals++;
        }
        
        return (metGoals, totalGoals);
    }

    private static double CalculateAchievementPercentage(int metGoals, int totalGoals) =>
        (double)metGoals / totalGoals * 100;

    private static string DetermineStatusFromPercentage(double percentage) =>
        percentage switch
        {
            >= ExcellentThreshold => "Excellent",
            >= GoodThreshold => "Good",
            >= FairThreshold => "Fair",
            >= NeedsImprovementThreshold => "Needs Improvement",
            _ => "Poor"
        };
} 