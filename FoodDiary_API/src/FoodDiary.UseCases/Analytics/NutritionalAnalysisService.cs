using FoodDiary.Core.AnalyticsAggregate;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UseCases.Analytics;

public class NutritionalAnalysisService : INutritionalAnalysisService
{
    private readonly IAnalyticsService _analyticsService;
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly IAnalyticsCalculationService _analyticsCalculationService;
    private readonly INutritionalDataService _nutritionalDataService;

    private const int DAYS_IN_WEEK = 7;
    private const int DAYS_IN_MONTH = 30;
    private const int TREND_ANALYSIS_DAYS = 7;
    private const double TREND_STABILITY_THRESHOLD = 5.0;
    private const double MINIMUM_TREND_DATA_POINTS = 2;

    public NutritionalAnalysisService(
        IAnalyticsService analyticsService,
        FoodDiary.Core.Interfaces.IRepository<User> userRepository,
        FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository,
        IAnalyticsCalculationService analyticsCalculationService,
        INutritionalDataService nutritionalDataService)
    {
        _analyticsService = analyticsService;
        _userRepository = userRepository;
        _foodEntryRepository = foodEntryRepository;
        _analyticsCalculationService = analyticsCalculationService;
        _nutritionalDataService = nutritionalDataService;
    }

    #region Daily Analysis

    public async Task<DailyNutritionalAnalysis> GenerateDailyAnalysisAsync(Guid userId, DateTime date)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (!user.IsSuccess)
            throw new InvalidOperationException($"User {userId} not found");

        var dailyEntries = await _nutritionalDataService.GetFoodEntriesForDateAsync(userId, date);
        
        var (totalCalories, totalProtein, totalFat, totalCarbohydrates) = CalculateNutritionalTotals(dailyEntries);

        var analysis = new DailyNutritionalAnalysis(
            userId, 
            date, 
            totalCalories, 
            totalProtein, 
            totalFat, 
            totalCarbohydrates, 
            dailyEntries.Count
        );

        analysis.SetGoals(
            user.Value.DailyCalorieGoal,
            user.Value.DailyProteinGoal,
            user.Value.DailyFatGoal,
            user.Value.DailyCarbohydrateGoal
        );

        var calorieProgress = user.Value.DailyCalorieGoal.HasValue ? Math.Round((totalCalories / user.Value.DailyCalorieGoal.Value) * 100, 1) : 0;
        var proteinProgress = user.Value.DailyProteinGoal.HasValue ? Math.Round((totalProtein / user.Value.DailyProteinGoal.Value) * 100, 1) : 0;
        var fatProgress = user.Value.DailyFatGoal.HasValue ? Math.Round((totalFat / user.Value.DailyFatGoal.Value) * 100, 1) : 0;
        var carbProgress = user.Value.DailyCarbohydrateGoal.HasValue ? Math.Round((totalCarbohydrates / user.Value.DailyCarbohydrateGoal.Value) * 100, 1) : 0;

        analysis.SetProgressPercentages(calorieProgress, proteinProgress, fatProgress, carbProgress);

        _analyticsCalculationService.CalculateOverLimitFlags(analysis);

        return analysis;
    }

    private static (double calories, double protein, double fat, double carbs) CalculateNutritionalTotals(List<FoodEntry> entries)
    {
        var totalCalories = 0.0;
        var totalProtein = 0.0;
        var totalFat = 0.0;
        var totalCarbohydrates = 0.0;

        foreach (var entry in entries)
        {
            if (entry.Product != null)
            {
                var weightRatio = entry.WeightGrams / 100.0;
                totalCalories += entry.Product.CaloriesPer100g * weightRatio;
                totalProtein += entry.Product.ProteinsPer100g * weightRatio;
                totalFat += entry.Product.FatsPer100g * weightRatio;
                totalCarbohydrates += entry.Product.CarbohydratesPer100g * weightRatio;
            }
        }

        return (
            Math.Round(totalCalories, 1),
            Math.Round(totalProtein, 1),
            Math.Round(totalFat, 1),
            Math.Round(totalCarbohydrates, 1)
        );
    }

    #endregion

    #region Period Analysis

    public async Task<List<DailyNutritionalAnalysis>> GenerateWeeklyAnalysisAsync(Guid userId, DateTime startDate)
    {
        return await GeneratePeriodAnalysisAsync(userId, startDate, DAYS_IN_WEEK);
    }

    public async Task<List<DailyNutritionalAnalysis>> GenerateMonthlyAnalysisAsync(Guid userId, DateTime startDate)
    {
        return await GeneratePeriodAnalysisAsync(userId, startDate, DAYS_IN_MONTH);
    }

    private async Task<List<DailyNutritionalAnalysis>> GeneratePeriodAnalysisAsync(Guid userId, DateTime startDate, int days)
    {
        var analyses = new List<DailyNutritionalAnalysis>();
        var periodStart = DateTime.SpecifyKind(startDate.Date, DateTimeKind.Utc);
        var user = await _userRepository.GetByIdAsync(userId);
        if (!user.IsSuccess)
            throw new InvalidOperationException($"User {userId} not found");
            
        var userCreatedDate = DateTime.SpecifyKind(user.Value.CreatedAt.Date, DateTimeKind.Utc);

        for (int i = 0; i < days; i++)
        {
            var date = periodStart.AddDays(i);
            
            if (date < userCreatedDate)
                continue;
                
            var analysis = await GenerateDailyAnalysisAsync(userId, date);
            analyses.Add(analysis);
        }

        return analyses;
    }

    #endregion

    #region Trends Analysis

    public async Task<NutritionalTrends> GenerateTrendsAsync(Guid userId, int days)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (!user.IsSuccess)
            throw new InvalidOperationException($"User {userId} not found");
            
        var endDate = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var requestedStartDate = endDate.AddDays(-days + 1);
        
        var userCreatedDate = DateTime.SpecifyKind(user.Value.CreatedAt.Date, DateTimeKind.Utc);
        var startDate = requestedStartDate < userCreatedDate ? userCreatedDate : requestedStartDate;
        
        var actualDaysToAnalyze = (int)(endDate - startDate).TotalDays + 1;
        
        var analyses = await GeneratePeriodAnalysisAsync(userId, startDate, actualDaysToAnalyze);
        
        var trends = CreateTrendsAnalysis(userId, actualDaysToAnalyze);
        if (!analyses.Any()) return trends;

        CalculateTrendAverages(trends, analyses);
        
        if (actualDaysToAnalyze > 30)
        {
            await CalculateTrendPatternsAsync(trends, analyses);
        }
        else
        {
            CalculateTrendPatterns(trends, analyses);
        }
        
        CalculateGoalAdherence(trends, analyses, actualDaysToAnalyze);
        await CalculateMealPatterns(trends, userId, startDate, endDate);

        return trends;
    }

    private NutritionalTrends CreateTrendsAnalysis(Guid userId, int days)
    {
        return new NutritionalTrends(userId, DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc), days);
    }

    private void CalculateTrendAverages(NutritionalTrends trends, List<DailyNutritionalAnalysis> analyses)
    {
        if (!analyses.Any())
        {
            trends.SetAverageIntake(0, 0, 0, 0);
            return;
        }
        
        var averageCalories = Math.Round(analyses.Average(a => a.TotalCalories), 1);
        var averageProtein = Math.Round(analyses.Average(a => a.TotalProtein), 1);
        var averageFat = Math.Round(analyses.Average(a => a.TotalFat), 1);
        var averageCarbohydrates = Math.Round(analyses.Average(a => a.TotalCarbohydrates), 1);
        
        trends.SetAverageIntake(averageCalories, averageProtein, averageFat, averageCarbohydrates);
    }

    private async Task CalculateTrendPatternsAsync(NutritionalTrends trends, List<DailyNutritionalAnalysis> analyses)
    {
        var calorieValues = analyses.Select(a => a.TotalCalories).ToList();
        var proteinValues = analyses.Select(a => a.TotalProtein).ToList();
        var fatValues = analyses.Select(a => a.TotalFat).ToList();
        var carbValues = analyses.Select(a => a.TotalCarbohydrates).ToList();

        var trendTasks = new[]
        {
            CalculateTrendAsync(calorieValues),
            CalculateTrendAsync(proteinValues),
            CalculateTrendAsync(fatValues),
            CalculateTrendAsync(carbValues)
        };

        var consistencyTasks = new[]
        {
            CalculateConsistencyAsync(calorieValues),
            CalculateConsistencyAsync(proteinValues),
            CalculateConsistencyAsync(fatValues),
            CalculateConsistencyAsync(carbValues)
        };

        var trendResults = await Task.WhenAll(trendTasks);
        var consistencyResults = await Task.WhenAll(consistencyTasks);

        trends.SetTrends(trendResults[0], trendResults[1], trendResults[2], trendResults[3]);
        trends.SetConsistencyMetrics(consistencyResults[0], consistencyResults[1], consistencyResults[2], consistencyResults[3]);
    }

    private void CalculateTrendPatterns(NutritionalTrends trends, List<DailyNutritionalAnalysis> analyses)
    {
        var calorieTrend = CalculateTrend(analyses.Select(a => a.TotalCalories).ToList());
        var proteinTrend = CalculateTrend(analyses.Select(a => a.TotalProtein).ToList());
        var fatTrend = CalculateTrend(analyses.Select(a => a.TotalFat).ToList());
        var carbTrend = CalculateTrend(analyses.Select(a => a.TotalCarbohydrates).ToList());

        var calorieConsistency = CalculateConsistency(analyses.Select(a => a.TotalCalories).ToList());
        var proteinConsistency = CalculateConsistency(analyses.Select(a => a.TotalProtein).ToList());
        var fatConsistency = CalculateConsistency(analyses.Select(a => a.TotalFat).ToList());
        var carbConsistency = CalculateConsistency(analyses.Select(a => a.TotalCarbohydrates).ToList());
        
        trends.SetTrends(calorieTrend, proteinTrend, fatTrend, carbTrend);
        trends.SetConsistencyMetrics(calorieConsistency, proteinConsistency, fatConsistency, carbConsistency);
    }

    private void CalculateGoalAdherence(NutritionalTrends trends, List<DailyNutritionalAnalysis> analyses, int days)
    {
        if (!analyses.Any())
        {
            trends.SetGoalAdherence(0, 0, 0);
            return;
        }
        
        var calorieGoalDays = analyses.Count(a => _analyticsService.IsCalorieGoalMet(a));
        var proteinGoalDays = analyses.Count(a => _analyticsService.IsProteinGoalMet(a));
        var fatGoalDays = analyses.Count(a => _analyticsService.IsFatGoalMet(a));
        var carbGoalDays = analyses.Count(a => _analyticsService.IsCarbohydrateGoalMet(a));
        
        var daysWithAnyGoalMet = analyses.Count(a => 
            _analyticsService.IsCalorieGoalMet(a) || 
            _analyticsService.IsProteinGoalMet(a) || 
            _analyticsService.IsFatGoalMet(a) || 
            _analyticsService.IsCarbohydrateGoalMet(a));
        
        var totalPossibleGoals = days * 4;
        var totalGoalsMet = calorieGoalDays + proteinGoalDays + fatGoalDays + carbGoalDays;
        var flexibleAdherenceRate = Math.Round((double)totalGoalsMet / totalPossibleGoals * 100, 1);
        
        trends.SetGoalAdherence(flexibleAdherenceRate, daysWithAnyGoalMet, days);
    }

    private async Task CalculateMealPatterns(NutritionalTrends trends, Guid userId, DateTime startDate, DateTime endDate)
    {
        var foodEntries = await _nutritionalDataService.GetFoodEntriesForPeriodAsync(userId, startDate, endDate);
        
        if (!foodEntries.Any())
        {
            trends.SetMealPatterns(0, "", "");
            return;
        }
        
        var dailyMealCounts = foodEntries
            .GroupBy(fe => fe.ConsumedAt.Date)
            .Select(g => g.Count())
            .ToList();

        var averageMealsPerDay = dailyMealCounts.Any() ? Math.Round(dailyMealCounts.Average(), 1) : 0;

        var mealTimeGroups = foodEntries
            .GroupBy(fe => fe.MealType)
            .OrderByDescending(g => g.Count())
            .ToList();

        var mostCommonMealTime = mealTimeGroups.FirstOrDefault()?.Key.ToString() ?? "";
        var leastCommonMealTime = mealTimeGroups.LastOrDefault()?.Key.ToString() ?? "";
        
        trends.SetMealPatterns(averageMealsPerDay, mostCommonMealTime, leastCommonMealTime);
    }

    #endregion

    #region Statistical Calculations

    private async Task<string> CalculateTrendAsync(List<double> values)
    {
        return await Task.Run(() => CalculateTrend(values));
    }

    private string CalculateTrend(List<double> values)
    {
        if (values.Count < MINIMUM_TREND_DATA_POINTS) return "Insufficient data";

        var firstHalf = values.Take(values.Count / 2).Average();
        var secondHalf = values.Skip(values.Count / 2).Average();

        var difference = secondHalf - firstHalf;
        var percentageChange = Math.Abs(difference) / firstHalf * 100;

        if (percentageChange < TREND_STABILITY_THRESHOLD) return "Stable";
        return difference > 0 ? "Improving" : "Declining";
    }

    private async Task<double> CalculateConsistencyAsync(List<double> values)
    {
        return await Task.Run(() => CalculateConsistency(values));
    }

    private double CalculateConsistency(List<double> values)
    {
        if (values.Count < MINIMUM_TREND_DATA_POINTS) return 100;

        var mean = values.Average();
        if (mean == 0) return 100;

        var variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
        var standardDeviation = Math.Sqrt(variance);
        var coefficientOfVariation = standardDeviation / mean;

        var rawConsistency = Math.Max(0, 100 - (coefficientOfVariation * 100));
        var adjustedConsistency = Math.Max(10, rawConsistency);
        
        return Math.Round(adjustedConsistency, 1);
    }

    #endregion
}
