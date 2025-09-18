using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries.Update;

public class UpdateFoodEntryCommandHandler : IRequestHandler<UpdateFoodEntryCommand, Result<UpdateFoodEntryResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly AutoMapper.IMapper _mapper;

    public UpdateFoodEntryCommandHandler(FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository, AutoMapper.IMapper mapper)
    {
        _foodEntryRepository = foodEntryRepository;
        _mapper = mapper;
    }

    public async Task<Result<UpdateFoodEntryResponse>> Handle(UpdateFoodEntryCommand request, CancellationToken cancellationToken)
    {
        var foodEntryResult = await _foodEntryRepository.GetByIdAsync(request.Id, "Product");
        if (!foodEntryResult.IsSuccess)
        {
            return Result<UpdateFoodEntryResponse>.Error("Food entry not found");
        }

        var foodEntry = foodEntryResult.Value;
        if (foodEntry.UserId != request.UserId)
        {
            return Result<UpdateFoodEntryResponse>.Error("Access denied");
        }

        foodEntry.UpdateDetails(request.WeightGrams, request.MealType, request.Notes);
        
        if (request.ConsumedAt.HasValue)
        {
            foodEntry.UpdateConsumptionTime(request.ConsumedAt.Value);
        }

        var updateResult = await _foodEntryRepository.UpdateAsync(foodEntry);
        if (!updateResult.IsSuccess)
        {
            return Result<UpdateFoodEntryResponse>.Error("Failed to update food entry");
        }

        var foodEntryDto = _mapper.Map<FoodEntryDTO>(foodEntry);
        var response = new UpdateFoodEntryResponse { FoodEntry = foodEntryDto };
        return Result<UpdateFoodEntryResponse>.Success(response);
    }
} 