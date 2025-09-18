using MediatR;
using Ardalis.Result;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries.Update;

public record UpdateFoodEntryCommand : IRequest<Result<UpdateFoodEntryResponse>>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public double WeightGrams { get; init; }
    public MealType MealType { get; init; }
    public DateTime? ConsumedAt { get; init; }
    public string? Notes { get; init; }
}

public record UpdateFoodEntryResponse
{
    public FoodEntryDTO FoodEntry { get; init; } = new();
    public string Message { get; init; } = "Food entry updated successfully";
} 