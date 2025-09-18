using FoodDiary.Core.AnalyticsAggregate;

namespace FoodDiary.UseCases.Analytics;

public interface IAnalyticsCalculationService
{
    string CalculateOverallTrend(NutritionalTrends trends);
    List<string> GenerateTrendInsights(NutritionalTrends trends);
    bool EvaluateConsistency(NutritionalTrends trends);
    bool EvaluateImprovement(NutritionalTrends trends);
    void CalculateOverLimitFlags(DailyNutritionalAnalysis analysis);
}

public class AnalyticsCalculationService : IAnalyticsCalculationService
{
    public string CalculateOverallTrend(NutritionalTrends trends)
    {
        var positiveTrends = 0;
        var totalTrends = 0;
        
        if (!string.IsNullOrEmpty(trends.CalorieTrend))
        {
            totalTrends++;
            if (trends.CalorieTrend == "Improving") positiveTrends++;
        }
        
        if (!string.IsNullOrEmpty(trends.ProteinTrend))
        {
            totalTrends++;
            if (trends.ProteinTrend == "Improving") positiveTrends++;
        }
        
        if (!string.IsNullOrEmpty(trends.FatTrend))
        {
            totalTrends++;
            if (trends.FatTrend == "Improving") positiveTrends++;
        }
        
        if (!string.IsNullOrEmpty(trends.CarbohydrateTrend))
        {
            totalTrends++;
            if (trends.CarbohydrateTrend == "Improving") positiveTrends++;
        }
        
        if (totalTrends == 0) return "No data";
        
        var percentage = (double)positiveTrends / totalTrends * 100;
        
        return percentage switch
        {
            >= 75 => "Strongly Improving",
            >= 50 => "Improving",
            >= 25 => "Slightly Improving",
            >= 0 => "Declining",
            _ => "No clear trend"
        };
    }
    
    public List<string> GenerateTrendInsights(NutritionalTrends trends)
    {
        var insights = new List<string>();
        
        if (trends.GoalAdherenceRate >= 80)
        {
            insights.Add($"Excellent goal adherence: {trends.GoalAdherenceRate:F1}% of individual goals met across all days");
        }
        else if (trends.GoalAdherenceRate >= 60)
        {
            insights.Add($"Good goal adherence: {trends.GoalAdherenceRate:F1}% of individual goals met across all days");
        }
        else if (trends.GoalAdherenceRate >= 40)
        {
            insights.Add($"Moderate goal adherence: {trends.GoalAdherenceRate:F1}% of individual goals met across all days");
        }
        else
        {
            insights.Add($"Low goal adherence: {trends.GoalAdherenceRate:F1}% of individual goals met across all days");
        }
        
        if (trends.DaysGoalsMet > 0)
        {
            insights.Add($"Days with goals met: {trends.DaysGoalsMet} out of {trends.TotalDaysAnalyzed} days met at least one goal");
        }
        else
        {
            insights.Add($"No days with goals met: {trends.DaysGoalsMet} out of {trends.TotalDaysAnalyzed} days met any goals");
        }
        
        if (trends.CalorieConsistency >= 80)
        {
            insights.Add($"High calorie consistency: {trends.CalorieConsistency:F1}%");
        }
        else if (trends.CalorieConsistency >= 60)
        {
            insights.Add($"Moderate calorie consistency: {trends.CalorieConsistency:F1}%");
        }
        else
        {
            insights.Add($"Low calorie consistency: {trends.CalorieConsistency:F1}%");
        }
        
        insights.Add($"Average meals per day: {trends.AverageMealsPerDay:F1}");
        
        if (trends.AverageMealsPerDay < 3)
        {
            insights.Add("Below recommended minimum of 3 meals per day");
        }
        
        return insights;
    }
    
    public bool EvaluateConsistency(NutritionalTrends trends)
    {
        return trends.CalorieConsistency >= 70 && 
               trends.ProteinConsistency >= 70 && 
               trends.FatConsistency >= 70 && 
               trends.CarbohydrateConsistency >= 70;
    }
    
    public bool EvaluateImprovement(NutritionalTrends trends)
    {
        var overallTrend = CalculateOverallTrend(trends);
        return overallTrend.Contains("Improving");
    }
    
    public void CalculateOverLimitFlags(DailyNutritionalAnalysis analysis)
    {
        const double OVER_LIMIT_TOLERANCE = 1.1;
        
        var isOverCalorieLimit = analysis.DailyCalorieGoal.HasValue && 
            analysis.TotalCalories > analysis.DailyCalorieGoal.Value * OVER_LIMIT_TOLERANCE;
        var isOverProteinLimit = analysis.DailyProteinGoal.HasValue && 
            analysis.TotalProtein > analysis.DailyProteinGoal.Value * OVER_LIMIT_TOLERANCE;
        var isOverFatLimit = analysis.DailyFatGoal.HasValue && 
            analysis.TotalFat > analysis.DailyFatGoal.Value * OVER_LIMIT_TOLERANCE;
        var isOverCarbohydrateLimit = analysis.DailyCarbohydrateGoal.HasValue && 
            analysis.TotalCarbohydrates > analysis.DailyCarbohydrateGoal.Value * OVER_LIMIT_TOLERANCE;
        
        analysis.SetOverLimitFlags(isOverCalorieLimit, isOverProteinLimit, isOverFatLimit, isOverCarbohydrateLimit);
    }
}
