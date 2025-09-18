using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.Analytics;

public interface INutritionalDataService
{
    Task<List<FoodEntry>> GetFoodEntriesForDateAsync(Guid userId, DateTime date);
    Task<List<FoodEntry>> GetFoodEntriesForPeriodAsync(Guid userId, DateTime startDate, DateTime endDate);
}
