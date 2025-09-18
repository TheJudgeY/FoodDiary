using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.FoodEntries.Get;

public record GetFoodEntryCommand : IRequest<Result<GetFoodEntryResponse>>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
}

public record GetFoodEntryResponse
{
    public FoodEntryDTO FoodEntry { get; init; } = new();
} 