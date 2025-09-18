using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries.Get;

public class GetFoodEntryCommandHandler : IRequestHandler<GetFoodEntryCommand, Result<GetFoodEntryResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly AutoMapper.IMapper _mapper;

    public GetFoodEntryCommandHandler(FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository, AutoMapper.IMapper mapper)
    {
        _foodEntryRepository = foodEntryRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetFoodEntryResponse>> Handle(GetFoodEntryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var foodEntryResult = await _foodEntryRepository.GetByIdAsync(request.Id, "Product");
            if (!foodEntryResult.IsSuccess)
            {
                return Result<GetFoodEntryResponse>.Error("Food entry not found");
            }

            var foodEntry = foodEntryResult.Value;

            if (foodEntry.UserId != request.UserId)
            {
                return Result<GetFoodEntryResponse>.Error("Access denied");
            }

            var foodEntryDto = _mapper.Map<FoodEntryDTO>(foodEntry);

            var response = new GetFoodEntryResponse
            {
                FoodEntry = foodEntryDto
            };

            return Result<GetFoodEntryResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetFoodEntryResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
} 