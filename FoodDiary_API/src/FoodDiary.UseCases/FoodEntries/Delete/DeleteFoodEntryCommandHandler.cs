using MediatR;
using Ardalis.Result;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries.Delete;

public class DeleteFoodEntryCommandHandler : IRequestHandler<DeleteFoodEntryCommand, Result<DeleteFoodEntryResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;

    public DeleteFoodEntryCommandHandler(FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository)
    {
        _foodEntryRepository = foodEntryRepository;
    }

    public async Task<Result<DeleteFoodEntryResponse>> Handle(DeleteFoodEntryCommand request, CancellationToken cancellationToken)
    {
        var foodEntryResult = await _foodEntryRepository.GetByIdAsync(request.Id, "Product");
        if (!foodEntryResult.IsSuccess)
        {
            return Result<DeleteFoodEntryResponse>.Error("Food entry not found");
        }

        var foodEntry = foodEntryResult.Value;
        if (foodEntry.UserId != request.UserId)
        {
            return Result<DeleteFoodEntryResponse>.Error("Access denied");
        }

        var deleteResult = await _foodEntryRepository.DeleteAsync(foodEntry.Id);
        if (!deleteResult.IsSuccess)
        {
            return Result<DeleteFoodEntryResponse>.Error("Failed to delete food entry");
        }

        var response = new DeleteFoodEntryResponse { Message = "Food entry deleted successfully" };
        return Result<DeleteFoodEntryResponse>.Success(response);
    }
} 