using FoodDiary.Core.AnalyticsAggregate;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.UseCases.Analytics;

public interface ITrendsService
{
    Task<List<string>> GetTrendInsightsAsync(Guid userId, int days = 7);
    Task<Dictionary<string, object>> GetTrendMetricsAsync(Guid userId, int days = 7);
    Task<string> GetConsistencyAnalysisAsync(Guid userId, int days = 7);
    Task<string> GetGoalAdherenceTrendAsync(Guid userId, int days = 7);
}

public class TrendsService : ITrendsService
{
    private readonly INutritionalAnalysisService _nutritionalAnalysisService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IAnalyticsCalculationService _analyticsCalculationService;

    public TrendsService(
        INutritionalAnalysisService nutritionalAnalysisService, 
        IAnalyticsService analyticsService,
        IAnalyticsCalculationService analyticsCalculationService)
    {
        _nutritionalAnalysisService = nutritionalAnalysisService;
        _analyticsService = analyticsService;
        _analyticsCalculationService = analyticsCalculationService;
    }

    public async Task<List<string>> GetTrendInsightsAsync(Guid userId, int days = 7)
    {
        try
        {
            var trends = await _nutritionalAnalysisService.GenerateTrendsAsync(userId, days);
            var analyses = await _nutritionalAnalysisService.GenerateWeeklyAnalysisAsync(userId, DateTime.UtcNow.AddDays(-days));
            
            if (!analyses.Any())
                return new List<string> { "Insufficient data for trend analysis" };

            var insights = new List<string>();
            
            insights.AddRange(GetConsistencyInsights(analyses));
            insights.AddRange(GetGoalAdherenceInsights(analyses));
            insights.AddRange(GetPatternInsights(analyses, trends));

            return insights;
        }
        catch (Exception ex)
        {
            return new List<string> { $"Unable to analyze trends: {ex.Message}" };
        }
    }

    public async Task<Dictionary<string, object>> GetTrendMetricsAsync(Guid userId, int days = 7)
    {
        try
        {
            var trends = await _nutritionalAnalysisService.GenerateTrendsAsync(userId, days);
            var analyses = await _nutritionalAnalysisService.GenerateWeeklyAnalysisAsync(userId, DateTime.UtcNow.AddDays(-days));
            
            if (!analyses.Any())
                return new Dictionary<string, object>();

            var metrics = new Dictionary<string, object>
            {
                ["calorieConsistency"] = trends.CalorieConsistency,
                ["proteinConsistency"] = trends.ProteinConsistency,
                ["fatConsistency"] = trends.FatConsistency,
                ["carbohydrateConsistency"] = trends.CarbohydrateConsistency
            };

            var goalAdherence = CalculateGoalAdherence(analyses);
            metrics["goalAdherenceRate"] = goalAdherence.adherenceRate;
            metrics["daysGoalsMet"] = goalAdherence.daysMet;
            metrics["totalDays"] = goalAdherence.totalDays;

            var overLimitMetrics = CalculateOverLimitFrequency(analyses);
            metrics["overCalorieLimitDays"] = overLimitMetrics.overCalorieDays;
            metrics["overProteinLimitDays"] = overLimitMetrics.overProteinDays;
            metrics["overFatLimitDays"] = overLimitMetrics.overFatDays;
            metrics["overCarbLimitDays"] = overLimitMetrics.overCarbDays;

            return metrics;
        }
        catch (Exception)
        {
            return new Dictionary<string, object>();
        }
    }

    public async Task<string> GetConsistencyAnalysisAsync(Guid userId, int days = 7)
    {
        try
        {
            var trends = await _nutritionalAnalysisService.GenerateTrendsAsync(userId, days);
            
            var isConsistent = _analyticsCalculationService.EvaluateConsistency(trends);
            
            return isConsistent switch
            {
                true => "Excellent consistency across all macronutrients",
                false => "Moderate to low consistency, some improvement needed"
            };
        }
        catch (Exception)
        {
            return "Unable to analyze consistency";
        }
    }

    public async Task<string> GetGoalAdherenceTrendAsync(Guid userId, int days = 7)
    {
        try
        {
            var analyses = await _nutritionalAnalysisService.GenerateWeeklyAnalysisAsync(userId, DateTime.UtcNow.AddDays(-days));
            
            if (!analyses.Any())
                return "Insufficient data for goal adherence analysis";

            var goalAdherence = CalculateGoalAdherence(analyses);
            var adherenceRate = goalAdherence.adherenceRate;
            
            return adherenceRate switch
            {
                >= 80 => $"Excellent goal adherence: {adherenceRate:F1}% of days met all goals",
                >= 60 => $"Good goal adherence: {adherenceRate:F1}% of days met all goals",
                >= 40 => $"Moderate goal adherence: {adherenceRate:F1}% of days met all goals",
                _ => $"Low goal adherence: {adherenceRate:F1}% of days met all goals"
            };
        }
        catch (Exception)
        {
            return "Unable to analyze goal adherence trends";
        }
    }

    private static List<string> GetConsistencyInsights(List<DailyNutritionalAnalysis> analyses)
    {
        if (analyses.Count < 3) return new List<string>();

        var insights = new List<string>();
        
        var calorieValues = analyses.Select(a => a.TotalCalories).ToList();
        var calorieConsistency = CalculateConsistency(calorieValues);
        
        if (calorieConsistency < 50)
            insights.Add($"Your calorie intake is highly inconsistent (consistency: {calorieConsistency:F1}%)");
        else if (calorieConsistency < 70)
            insights.Add($"Your calorie intake shows moderate inconsistency (consistency: {calorieConsistency:F1}%)");

        var proteinValues = analyses.Select(a => a.TotalProtein).ToList();
        var proteinConsistency = CalculateConsistency(proteinValues);
        
        if (proteinConsistency < 50)
            insights.Add($"Your protein intake is highly inconsistent (consistency: {proteinConsistency:F1}%)");
        else if (proteinConsistency < 70)
            insights.Add($"Your protein intake shows moderate inconsistency (consistency: {proteinConsistency:F1}%)");

        return insights;
    }

    private static List<string> GetGoalAdherenceInsights(List<DailyNutritionalAnalysis> analyses)
    {
        var insights = new List<string>();
        var totalDays = analyses.Count;
        
        var calorieGoalDays = analyses.Count(a => IsCalorieGoalMet(a));
        var proteinGoalDays = analyses.Count(a => IsProteinGoalMet(a));
        var fatGoalDays = analyses.Count(a => IsFatGoalMet(a));
        var carbGoalDays = analyses.Count(a => IsCarbohydrateGoalMet(a));

        var calorieAdherence = Math.Round((double)calorieGoalDays / totalDays * 100, 1);
        var proteinAdherence = Math.Round((double)proteinGoalDays / totalDays * 100, 1);
        var fatAdherence = Math.Round((double)fatGoalDays / totalDays * 100, 1);
        var carbAdherence = Math.Round((double)carbGoalDays / totalDays * 100, 1);

        AddAdherenceInsight(insights, "calorie", calorieAdherence);
        AddAdherenceInsight(insights, "protein", proteinAdherence);
        AddAdherenceInsight(insights, "fat", fatAdherence);
        AddAdherenceInsight(insights, "carbohydrate", carbAdherence);

        return insights;
    }

    private static void AddAdherenceInsight(List<string> insights, string nutrient, double adherence)
    {
        if (adherence < 50)
            insights.Add($"You rarely meet your {nutrient} goals ({adherence:F1}% of days)");
        else if (adherence < 70)
            insights.Add($"You sometimes meet your {nutrient} goals ({adherence:F1}% of days)");
    }

    private static List<string> GetPatternInsights(List<DailyNutritionalAnalysis> analyses, NutritionalTrends trends)
    {
        var insights = new List<string>();
        
        AddTrendInsight(insights, "calorie", trends.CalorieTrend);
        AddTrendInsight(insights, "protein", trends.ProteinTrend);

        if (trends.AverageMealsPerDay < 3)
            insights.Add($"You average only {trends.AverageMealsPerDay:F1} meals per day, which is below the recommended minimum");

        return insights;
    }

    private static void AddTrendInsight(List<string> insights, string nutrient, string trend)
    {
        var insight = trend switch
        {
            "Declining" => $"Your {nutrient} intake has been declining over the past week",
            "Improving" => $"Your {nutrient} intake has been improving over the past week",
            "Stable" => $"Your {nutrient} intake has been stable over the past week",
            _ => string.Empty
        };

        if (!string.IsNullOrEmpty(insight))
            insights.Add(insight);
    }

    private static (double adherenceRate, int daysMet, int totalDays) CalculateGoalAdherence(List<DailyNutritionalAnalysis> analyses)
    {
        var totalDays = analyses.Count;
        var daysMet = analyses.Count(a => 
            IsCalorieGoalMet(a) && 
            IsProteinGoalMet(a) && 
            IsFatGoalMet(a) && 
            IsCarbohydrateGoalMet(a));

        var adherenceRate = totalDays > 0 ? Math.Round((double)daysMet / totalDays * 100, 1) : 0;
        return (adherenceRate, daysMet, totalDays);
    }

    private static (int overCalorieDays, int overProteinDays, int overFatDays, int overCarbDays) CalculateOverLimitFrequency(List<DailyNutritionalAnalysis> analyses)
    {
        var overCalorieDays = analyses.Count(a => IsOverCalorieLimit(a));
        var overProteinDays = analyses.Count(a => IsOverProteinLimit(a));
        var overFatDays = analyses.Count(a => IsOverFatLimit(a));
        var overCarbDays = analyses.Count(a => IsOverCarbohydrateLimit(a));

        return (overCalorieDays, overProteinDays, overFatDays, overCarbDays);
    }

    private static double CalculateConsistency(List<double> values)
    {
        if (values.Count < 2) return 100;

        var mean = values.Average();
        if (mean == 0) return 100;

        var variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
        var standardDeviation = Math.Sqrt(variance);
        var coefficientOfVariation = standardDeviation / mean;

        return Math.Round(Math.Max(0, 100 - (coefficientOfVariation * 100)), 1);
    }

    private static bool IsCalorieGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyCalorieGoal.HasValue && analysis.TotalCalories >= analysis.DailyCalorieGoal.Value * 0.8;

    private static bool IsProteinGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyProteinGoal.HasValue && analysis.TotalProtein >= analysis.DailyProteinGoal.Value * 0.8;

    private static bool IsFatGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyFatGoal.HasValue && analysis.TotalFat >= analysis.DailyFatGoal.Value * 0.8;

    private static bool IsCarbohydrateGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyCarbohydrateGoal.HasValue && analysis.TotalCarbohydrates >= analysis.DailyCarbohydrateGoal.Value * 0.8;

    private static bool IsOverCalorieLimit(DailyNutritionalAnalysis analysis) =>
        analysis.DailyCalorieGoal.HasValue && analysis.TotalCalories > analysis.DailyCalorieGoal.Value * 1.1;

    private static bool IsOverProteinLimit(DailyNutritionalAnalysis analysis) =>
        analysis.DailyProteinGoal.HasValue && analysis.TotalProtein > analysis.DailyProteinGoal.Value * 1.1;

    private static bool IsOverFatLimit(DailyNutritionalAnalysis analysis) =>
        analysis.DailyFatGoal.HasValue && analysis.TotalFat > analysis.DailyFatGoal.Value * 1.1;

    private static bool IsOverCarbohydrateLimit(DailyNutritionalAnalysis analysis) =>
        analysis.DailyCarbohydrateGoal.HasValue && analysis.TotalCarbohydrates > analysis.DailyCarbohydrateGoal.Value * 1.1;
}
