using System;
using System.Collections.Generic;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Core.AnalyticsAggregate;

public class DailyNutritionalAnalysis
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime AnalysisDate { get; private set; }
    
    public double TotalCalories { get; private set; }
    public double TotalProtein { get; private set; }
    public double TotalFat { get; private set; }
    public double TotalCarbohydrates { get; private set; }
    
    public double? DailyCalorieGoal { get; private set; }
    public double? DailyProteinGoal { get; private set; }
    public double? DailyFatGoal { get; private set; }
    public double? DailyCarbohydrateGoal { get; private set; }
    
    public double CalorieProgressPercentage { get; private set; }
    public double ProteinProgressPercentage { get; private set; }
    public double FatProgressPercentage { get; private set; }
    public double CarbohydrateProgressPercentage { get; private set; }
    
    public bool IsOverCalorieLimit { get; private set; }
    public bool IsOverProteinLimit { get; private set; }
    public bool IsOverFatLimit { get; private set; }
    public bool IsOverCarbohydrateLimit { get; private set; }
    
    public int TotalFoodEntries { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public User User { get; private set; } = null!;

    public DailyNutritionalAnalysis(Guid userId, DateTime analysisDate, double totalCalories, double totalProtein, double totalFat, double totalCarbohydrates, int totalFoodEntries)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        AnalysisDate = analysisDate;
        TotalCalories = totalCalories;
        TotalProtein = totalProtein;
        TotalFat = totalFat;
        TotalCarbohydrates = totalCarbohydrates;
        TotalFoodEntries = totalFoodEntries;
        CreatedAt = DateTime.UtcNow;
    }

    private DailyNutritionalAnalysis() { }

    public void SetGoals(double? dailyCalorieGoal, double? dailyProteinGoal, double? dailyFatGoal, double? dailyCarbohydrateGoal)
    {
        DailyCalorieGoal = dailyCalorieGoal;
        DailyProteinGoal = dailyProteinGoal;
        DailyFatGoal = dailyFatGoal;
        DailyCarbohydrateGoal = dailyCarbohydrateGoal;
    }

    public void SetProgressPercentages(double calorieProgress, double proteinProgress, double fatProgress, double carbohydrateProgress)
    {
        CalorieProgressPercentage = calorieProgress;
        ProteinProgressPercentage = proteinProgress;
        FatProgressPercentage = fatProgress;
        CarbohydrateProgressPercentage = carbohydrateProgress;
    }

    public void SetTotalValues(double totalCalories, double totalProtein, double totalFat, double totalCarbohydrates)
    {
        TotalCalories = totalCalories;
        TotalProtein = totalProtein;
        TotalFat = totalFat;
        TotalCarbohydrates = totalCarbohydrates;
    }

    public void SetOverLimitFlags(bool isOverCalorieLimit, bool isOverProteinLimit, bool isOverFatLimit, bool isOverCarbohydrateLimit)
    {
        IsOverCalorieLimit = isOverCalorieLimit;
        IsOverProteinLimit = isOverProteinLimit;
        IsOverFatLimit = isOverFatLimit;
        IsOverCarbohydrateLimit = isOverCarbohydrateLimit;
    }
} 