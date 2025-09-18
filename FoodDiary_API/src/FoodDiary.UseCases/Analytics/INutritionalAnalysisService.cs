using FoodDiary.Core.AnalyticsAggregate;

namespace FoodDiary.UseCases.Analytics;

public interface INutritionalAnalysisService
{
    Task<DailyNutritionalAnalysis> GenerateDailyAnalysisAsync(Guid userId, DateTime date);
    Task<List<DailyNutritionalAnalysis>> GenerateWeeklyAnalysisAsync(Guid userId, DateTime startDate);
    Task<List<DailyNutritionalAnalysis>> GenerateMonthlyAnalysisAsync(Guid userId, DateTime startDate);
    Task<NutritionalTrends> GenerateTrendsAsync(Guid userId, int days);
}
