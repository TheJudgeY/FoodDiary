using MediatR;
using Ardalis.Result;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries.List;

public record ListFoodEntriesCommand : IRequest<Result<ListFoodEntriesResponse>>
{
    public Guid UserId { get; init; }
    public DateTime? Date { get; init; }
    public MealType? MealType { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    
    public static ListFoodEntriesCommand FromQueryParameters(Guid userId, string? dateStr, string? mealTypeStr, int? page, int? pageSize)
    {
        DateTime? date = null;
        if (!string.IsNullOrEmpty(dateStr) && DateTime.TryParse(dateStr, out var parsedDate))
        {
            date = parsedDate;
        }

        MealType? mealType = null;
        if (!string.IsNullOrEmpty(mealTypeStr) && Enum.TryParse<MealType>(mealTypeStr, out var parsedMealType))
        {
            mealType = parsedMealType;
        }

        return new ListFoodEntriesCommand
        {
            UserId = userId,
            Date = date,
            MealType = mealType,
            Page = page ?? 1,
            PageSize = pageSize ?? 20
        };
    }
}

public record ListFoodEntriesResponse
{
    public List<FoodEntryDTO> FoodEntries { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public DailyNutritionSummary DailySummary { get; init; } = new();
}

public record DailyNutritionSummary
{
    public double TotalCalories { get; init; }
    public double TotalProteins { get; init; }
    public double TotalFats { get; init; }
    public double TotalCarbohydrates { get; init; }
    public int TotalEntries { get; init; }
} 