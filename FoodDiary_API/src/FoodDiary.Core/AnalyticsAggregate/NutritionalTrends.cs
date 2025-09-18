using System;
using System.Collections.Generic;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Core.AnalyticsAggregate;

public class NutritionalTrends
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AnalysisDate { get; private set; }
    public int DaysAnalyzed { get; private set; }
    
    public double AverageCalories { get; private set; }
    public double AverageProtein { get; private set; }
    public double AverageFat { get; private set; }
    public double AverageCarbohydrates { get; private set; }
    
    public string CalorieTrend { get; private set; } = string.Empty;
    public string ProteinTrend { get; private set; } = string.Empty;
    public string FatTrend { get; private set; } = string.Empty;
    public string CarbohydrateTrend { get; private set; } = string.Empty;
    
    public double CalorieConsistency { get; private set; }
    public double ProteinConsistency { get; private set; }
    public double FatConsistency { get; private set; }
    public double CarbohydrateConsistency { get; private set; }
    
    public double GoalAdherenceRate { get; private set; }
    public int DaysGoalsMet { get; private set; }
    public int TotalDaysAnalyzed { get; private set; }
    
    public double AverageMealsPerDay { get; private set; }
    public string MostCommonMealTime { get; private set; } = string.Empty;
    public string LeastCommonMealTime { get; private set; } = string.Empty;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public User User { get; private set; } = null!;

    public NutritionalTrends(Guid userId, DateTime analysisDate, int daysAnalyzed)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        AnalysisDate = analysisDate;
        DaysAnalyzed = daysAnalyzed;
        CreatedAt = DateTime.UtcNow;
    }

    private NutritionalTrends() { }

    public void SetAverageIntake(double averageCalories, double averageProtein, double averageFat, double averageCarbohydrates)
    {
        AverageCalories = averageCalories;
        AverageProtein = averageProtein;
        AverageFat = averageFat;
        AverageCarbohydrates = averageCarbohydrates;
    }

    public void SetTrends(string calorieTrend, string proteinTrend, string fatTrend, string carbohydrateTrend)
    {
        CalorieTrend = calorieTrend;
        ProteinTrend = proteinTrend;
        FatTrend = fatTrend;
        CarbohydrateTrend = carbohydrateTrend;
    }

    public void SetConsistencyMetrics(double calorieConsistency, double proteinConsistency, double fatConsistency, double carbohydrateConsistency)
    {
        CalorieConsistency = calorieConsistency;
        ProteinConsistency = proteinConsistency;
        FatConsistency = fatConsistency;
        CarbohydrateConsistency = carbohydrateConsistency;
    }

    public void SetGoalAdherence(double goalAdherenceRate, int daysGoalsMet, int totalDaysAnalyzed)
    {
        GoalAdherenceRate = goalAdherenceRate;
        DaysGoalsMet = daysGoalsMet;
        TotalDaysAnalyzed = totalDaysAnalyzed;
    }

    public void SetMealPatterns(double averageMealsPerDay, string mostCommonMealTime, string leastCommonMealTime)
    {
        AverageMealsPerDay = averageMealsPerDay;
        MostCommonMealTime = mostCommonMealTime;
        LeastCommonMealTime = leastCommonMealTime;
    }
} 