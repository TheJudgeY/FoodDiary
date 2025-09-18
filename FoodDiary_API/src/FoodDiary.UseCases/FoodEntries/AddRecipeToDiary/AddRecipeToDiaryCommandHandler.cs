using Ardalis.Result;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.RecipeAggregate;
using MediatR;
using FoodDiary.UseCases.Recipes;
using FoodDiary.UseCases.FoodEntries;

namespace FoodDiary.UseCases.FoodEntries.AddRecipeToDiary;

public class AddRecipeToDiaryCommandHandler : IRequestHandler<AddRecipeToDiaryCommand, Result<AddRecipeToDiaryResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;
    private readonly IRecipeService _recipeService;
    private readonly IFoodEntryService _foodEntryService;

    public AddRecipeToDiaryCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        FoodDiary.Core.Interfaces.IRepository<FoodEntry> foodEntryRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> recipeFavoriteRepository,
        IRecipeService recipeService,
        IFoodEntryService foodEntryService)
    {
        _recipeRepository = recipeRepository;
        _foodEntryRepository = foodEntryRepository;
        _recipeFavoriteRepository = recipeFavoriteRepository;
        _recipeService = recipeService;
        _foodEntryService = foodEntryService;
    }

    public async Task<Result<AddRecipeToDiaryResponse>> Handle(AddRecipeToDiaryCommand request, CancellationToken cancellationToken)
    {
        var recipeResult = await _recipeRepository.GetByIdAsync(request.RecipeId, "Ingredients", "Ingredients.Product");
        if (!recipeResult.IsSuccess || recipeResult.Value == null)
        {
            return Result<AddRecipeToDiaryResponse>.NotFound($"Recipe with ID {request.RecipeId} not found.");
        }

        var recipe = recipeResult.Value;

        var userRelationshipsResult = await _recipeFavoriteRepository.ListAsync();
        if (!userRelationshipsResult.IsSuccess)
        {
            return Result<AddRecipeToDiaryResponse>.Error($"Failed to check user relationships: {userRelationshipsResult.Errors.FirstOrDefault()}");
        }

        var isCreator = userRelationshipsResult.Value
            .Any(f => f.UserId == request.UserId && f.RecipeId == request.RecipeId && f.RelationshipType == RecipeUserRelationshipType.Creator);

        if (!isCreator && !recipe.IsPublic)
        {
            return Result<AddRecipeToDiaryResponse>.Forbidden("You can only add recipes you created or public recipes to the diary.");
        }

        if (!_recipeService.HasIngredients(recipe))
        {
            return Result<AddRecipeToDiaryResponse>.Error("Recipe has no ingredients to add to diary.");
        }

        var servingRatio = (double)request.ServingsConsumed / recipe.Servings;

        var createdFoodEntries = new List<FoodEntrySummary>();
        var totalCalories = 0.0;
        var totalProtein = 0.0;
        var totalFat = 0.0;
        var totalCarbohydrates = 0.0;

        foreach (var ingredient in recipe.Ingredients)
        {
            if (ingredient.Product == null)
            {
                continue;
            }

            var consumedWeight = ingredient.QuantityGrams * servingRatio;

            var foodEntry = new FoodEntry(
                request.UserId,
                ingredient.ProductId,
                consumedWeight,
                request.MealType,
                request.ConsumedAt,
                $"{recipe.Name} - {ingredient.Notes ?? ingredient.Product.Name}"
            );

            var addResult = await _foodEntryRepository.AddAsync(foodEntry);
            if (!addResult.IsSuccess)
            {
                return Result<AddRecipeToDiaryResponse>.Error($"Failed to add food entry for ingredient {ingredient.Product.Name}");
            }

            var calories = _foodEntryService.CalculateCalories(foodEntry);
            var protein = _foodEntryService.CalculateProtein(foodEntry);
            var fat = _foodEntryService.CalculateFat(foodEntry);
            var carbohydrates = _foodEntryService.CalculateCarbohydrates(foodEntry);

            totalCalories += calories;
            totalProtein += protein;
            totalFat += fat;
            totalCarbohydrates += carbohydrates;

            createdFoodEntries.Add(new FoodEntrySummary
            {
                Id = foodEntry.Id,
                ProductId = ingredient.ProductId,
                ProductName = ingredient.Product.Name,
                WeightGrams = consumedWeight,
                Calories = calories,
                Protein = protein,
                Fat = fat,
                Carbohydrates = carbohydrates
            });
        }

        var response = new AddRecipeToDiaryResponse
        {
            RecipeId = recipe.Id,
            RecipeName = recipe.Name,
            FoodEntriesCreated = createdFoodEntries.Count,
            TotalCalories = Math.Round(totalCalories, 1),
            TotalProtein = Math.Round(totalProtein, 1),
            TotalFat = Math.Round(totalFat, 1),
            TotalCarbohydrates = Math.Round(totalCarbohydrates, 1),
            FoodEntries = createdFoodEntries,
            Message = $"Successfully added {recipe.Name} to diary with {createdFoodEntries.Count} ingredients."
        };

        return Result<AddRecipeToDiaryResponse>.Success(response);
    }
} 