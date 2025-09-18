using Ardalis.Result;
using MediatR;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries.Create;

public record CreateFoodEntryCommand : IRequest<Result<CreateFoodEntryResponse>>
{
    public Guid UserId { get; init; }
    public Guid ProductId { get; init; }
    public double WeightGrams { get; init; }
    public MealType MealType { get; init; }
    public DateTime ConsumedAt { get; init; }
    public string? Notes { get; init; }
}

public record CreateFoodEntryResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public double WeightGrams { get; init; }
    public MealType MealType { get; init; }
    public string MealTypeDisplayName { get; init; } = string.Empty;
    public DateTime ConsumedAt { get; init; }
    public string? Notes { get; init; }
    
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Fat { get; init; }
    public double Carbohydrates { get; init; }
    
    public string Message { get; init; } = string.Empty;
} 