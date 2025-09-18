using System;
using System.Collections.Generic;

namespace FoodDiary.UseCases.Analytics;

public class DailyNutritionalAnalysisDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime AnalysisDate { get; set; }
    
    public double TotalCalories { get; set; }
    public double TotalProtein { get; set; }
    public double TotalFat { get; set; }
    public double TotalCarbohydrates { get; set; }
    
    public double? DailyCalorieGoal { get; set; }
    public double? DailyProteinGoal { get; set; }
    public double? DailyFatGoal { get; set; }
    public double? DailyCarbohydrateGoal { get; set; }
    
    public double CalorieProgressPercentage { get; set; }
    public double ProteinProgressPercentage { get; set; }
    public double FatProgressPercentage { get; set; }
    public double CarbohydrateProgressPercentage { get; set; }
    
    public int TotalFoodEntries { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public bool IsCalorieGoalMet { get; set; }
    public bool IsProteinGoalMet { get; set; }
    public bool IsFatGoalMet { get; set; }
    public bool IsCarbohydrateGoalMet { get; set; }
    public bool IsOverCalorieLimit { get; set; }
    public bool IsOverProteinLimit { get; set; }
    public bool IsOverFatLimit { get; set; }
    public bool IsOverCarbohydrateLimit { get; set; }
    public string OverallStatus { get; set; } = string.Empty;
    public List<string> KeyInsights { get; set; } = new List<string>();
    public List<string> Recommendations { get; set; } = new List<string>();
}

public class NutritionalTrendsDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime AnalysisDate { get; set; }
    public int DaysAnalyzed { get; set; }
    
    public double AverageCalories { get; set; }
    public double AverageProtein { get; set; }
    public double AverageFat { get; set; }
    public double AverageCarbohydrates { get; set; }
    
    public string CalorieTrend { get; set; } = string.Empty;
    public string ProteinTrend { get; set; } = string.Empty;
    public string FatTrend { get; set; } = string.Empty;
    public string CarbohydrateTrend { get; set; } = string.Empty;
    
    public double CalorieConsistency { get; set; }
    public double ProteinConsistency { get; set; }
    public double FatConsistency { get; set; }
    public double CarbohydrateConsistency { get; set; }
    
    public double GoalAdherenceRate { get; set; }
    public int DaysGoalsMet { get; set; }
    public int TotalDaysAnalyzed { get; set; }
    
    public double AverageMealsPerDay { get; set; }
    public string MostCommonMealTime { get; set; } = string.Empty;
    public string LeastCommonMealTime { get; set; } = string.Empty;
    
    public string OverallTrend { get; set; } = string.Empty;
    public List<string> TrendInsights { get; set; } = new List<string>();
    public bool IsConsistent { get; set; }
    public bool IsImproving { get; set; }
}

public class AnalyticsSummaryDTO
{
    public DailyNutritionalAnalysisDTO TodayAnalysis { get; set; } = null!;
    public NutritionalTrendsDTO WeeklyTrends { get; set; } = null!;
    public List<string> PersonalizedRecommendations { get; set; } = new List<string>();
} 