using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.UseCases.FoodEntries;

namespace FoodDiary.UseCases.FoodEntries.List;

public class ListFoodEntriesCommandHandler : IRequestHandler<ListFoodEntriesCommand, Result<ListFoodEntriesResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly AutoMapper.IMapper _mapper;
    private readonly IFoodEntryService _foodEntryService;

    public ListFoodEntriesCommandHandler(FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository, AutoMapper.IMapper mapper, IFoodEntryService foodEntryService)
    {
        _foodEntryRepository = foodEntryRepository;
        _mapper = mapper;
        _foodEntryService = foodEntryService;
    }

    public async Task<Result<ListFoodEntriesResponse>> Handle(ListFoodEntriesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var allFoodEntriesResult = await _foodEntryRepository.ListAsync("Product");
            if (!allFoodEntriesResult.IsSuccess)
            {
                return Result<ListFoodEntriesResponse>.Error("Failed to retrieve food entries");
            }

            var allFoodEntries = allFoodEntriesResult.Value;

            var userFoodEntries = allFoodEntries.Where(fe => fe.UserId == request.UserId).ToList();

            if (request.Date.HasValue)
            {
                var targetDate = request.Date.Value.Date;
                userFoodEntries = userFoodEntries.Where(fe => fe.ConsumedAt.Date == targetDate).ToList();
            }

            if (request.MealType.HasValue)
            {
                userFoodEntries = userFoodEntries.Where(fe => fe.MealType == request.MealType.Value).ToList();
            }

            userFoodEntries = userFoodEntries.OrderByDescending(fe => fe.ConsumedAt).ToList();

            var dailySummary = CalculateDailyNutritionSummary(userFoodEntries);

            var totalCount = userFoodEntries.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var pagedFoodEntries = userFoodEntries.Skip(skip).Take(request.PageSize).ToList();

            var foodEntryDtos = _mapper.Map<List<FoodEntryDTO>>(pagedFoodEntries);

            var response = new ListFoodEntriesResponse
            {
                FoodEntries = foodEntryDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                DailySummary = dailySummary
            };

            return Result<ListFoodEntriesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ListFoodEntriesResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }

    private DailyNutritionSummary CalculateDailyNutritionSummary(List<FoodEntry> foodEntries)
    {
        var totalCalories = foodEntries.Sum(fe => _foodEntryService.CalculateCalories(fe));
        var totalProteins = foodEntries.Sum(fe => _foodEntryService.CalculateProtein(fe));
        var totalFats = foodEntries.Sum(fe => _foodEntryService.CalculateFat(fe));
        var totalCarbohydrates = foodEntries.Sum(fe => _foodEntryService.CalculateCarbohydrates(fe));

        return new DailyNutritionSummary
        {
            TotalCalories = Math.Round(totalCalories, 2),
            TotalProteins = Math.Round(totalProteins, 2),
            TotalFats = Math.Round(totalFats, 2),
            TotalCarbohydrates = Math.Round(totalCarbohydrates, 2),
            TotalEntries = foodEntries.Count
        };
    }
} 