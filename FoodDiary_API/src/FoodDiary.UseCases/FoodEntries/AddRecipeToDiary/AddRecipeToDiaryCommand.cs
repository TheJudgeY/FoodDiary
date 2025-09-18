using Ardalis.Result;
using FoodDiary.Core.FoodEntryAggregate;
using MediatR;

namespace FoodDiary.UseCases.FoodEntries.AddRecipeToDiary;

public record AddRecipeToDiaryCommand : IRequest<Result<AddRecipeToDiaryResponse>>
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }
    public MealType MealType { get; init; }
    public DateTime ConsumedAt { get; init; }
    public string? Notes { get; init; }
    public int ServingsConsumed { get; init; } = 1;
} 