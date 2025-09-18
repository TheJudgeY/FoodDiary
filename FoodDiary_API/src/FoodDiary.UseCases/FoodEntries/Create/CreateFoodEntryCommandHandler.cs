using Ardalis.Result;
using MediatR;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.ProductAggregate;
using AutoMapper;
using FoodDiary.UseCases.FoodEntries;

namespace FoodDiary.UseCases.FoodEntries.Create;

public class CreateFoodEntryCommandHandler : IRequestHandler<CreateFoodEntryCommand, Result<CreateFoodEntryResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly IFoodEntryService _foodEntryService;

    public CreateFoodEntryCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository,
        FoodDiary.Core.Interfaces.IRepository<Product> productRepository,
        IMapper mapper,
        IFoodEntryService foodEntryService)
    {
        _foodEntryRepository = foodEntryRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _foodEntryService = foodEntryService;
    }

    public async Task<Result<CreateFoodEntryResponse>> Handle(CreateFoodEntryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var productResult = await _productRepository.GetByIdAsync(request.ProductId);
            if (!productResult.IsSuccess)
            {
                return Result<CreateFoodEntryResponse>.NotFound($"Product with ID {request.ProductId} not found");
            }

            var product = productResult.Value;

            var foodEntry = new FoodEntry(
                request.UserId,
                request.ProductId,
                request.WeightGrams,
                request.MealType,
                request.ConsumedAt,
                request.Notes);

            var productProperty = typeof(FoodEntry).GetProperty("Product");
            productProperty?.SetValue(foodEntry, product);

            var addResult = await _foodEntryRepository.AddAsync(foodEntry);
            if (!addResult.IsSuccess)
            {
                return Result<CreateFoodEntryResponse>.Error("Failed to create food entry");
            }

            var response = new CreateFoodEntryResponse
            {
                Id = foodEntry.Id,
                UserId = foodEntry.UserId,
                ProductId = foodEntry.ProductId,
                ProductName = product.Name,
                WeightGrams = foodEntry.WeightGrams,
                MealType = foodEntry.MealType,
                MealTypeDisplayName = _foodEntryService.GetMealTypeDisplayName(foodEntry),
                ConsumedAt = foodEntry.ConsumedAt,
                Notes = foodEntry.Notes,
                Calories = _foodEntryService.CalculateCalories(foodEntry),
                Protein = _foodEntryService.CalculateProtein(foodEntry),
                Fat = _foodEntryService.CalculateFat(foodEntry),
                Carbohydrates = _foodEntryService.CalculateCarbohydrates(foodEntry),
                Message = "Food entry created successfully"
            };

            return Result<CreateFoodEntryResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateFoodEntryResponse>.Error($"Error creating food entry: {ex.Message}");
        }
    }
} 