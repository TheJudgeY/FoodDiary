using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.FoodEntries.Delete;

public record DeleteFoodEntryCommand : IRequest<Result<DeleteFoodEntryResponse>>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
}

public record DeleteFoodEntryResponse
{
    public string Message { get; init; } = "Food entry deleted successfully";
} 