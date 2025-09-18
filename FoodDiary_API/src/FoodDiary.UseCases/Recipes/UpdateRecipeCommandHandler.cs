using MediatR;
using Ardalis.Result;
using AutoMapper;

using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Recipes;

public class UpdateRecipeCommandHandler : IRequestHandler<UpdateRecipeCommand, Result<UpdateRecipeResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> _ingredientRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;
    private readonly IMapper _mapper;
    private readonly IRecipeService _recipeService;

    public UpdateRecipeCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> ingredientRepository,
        FoodDiary.Core.Interfaces.IRepository<Product> productRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> recipeFavoriteRepository,
        IMapper mapper,
        IRecipeService recipeService)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _productRepository = productRepository;
        _recipeFavoriteRepository = recipeFavoriteRepository;
        _mapper = mapper;
        _recipeService = recipeService;
    }

    public async Task<Result<UpdateRecipeResponse>> Handle(UpdateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipeResult = await _recipeRepository.GetByIdAsync(request.RecipeId, "Ingredients", "Ingredients.Product");
            if (!recipeResult.IsSuccess)
            {
                return Result<UpdateRecipeResponse>.NotFound($"Recipe with ID {request.RecipeId} not found");
            }

            var recipe = recipeResult.Value;

            var userRelationshipsResult = await _recipeFavoriteRepository.ListAsync();
            if (!userRelationshipsResult.IsSuccess)
            {
                return Result<UpdateRecipeResponse>.Error($"Failed to check user relationships: {userRelationshipsResult.Errors.FirstOrDefault()}");
            }

            var userRelationships = userRelationshipsResult.Value
                .Where(f => f.UserId == request.UserId && f.RecipeId == request.RecipeId)
                .ToList();

            var isCreator = userRelationships.Any(r => r.RelationshipType == RecipeUserRelationshipType.Creator);
            var isContributor = userRelationships.Any(r => r.RelationshipType == RecipeUserRelationshipType.Contributor);
            var isPublic = recipe.IsPublic;

            if (!isCreator && !isContributor && !isPublic)
            {
                return Result<UpdateRecipeResponse>.Forbidden("Access denied. You can only update recipes you created, contributed to, or public recipes");
            }

            if (!isCreator && !isContributor && isPublic)
            {
                var contributorRelationship = new RecipeFavorite(request.UserId, request.RecipeId, RecipeUserRelationshipType.Contributor);
                var contributorResult = await _recipeFavoriteRepository.AddAsync(contributorRelationship);
                if (!contributorResult.IsSuccess)
                {
                    return Result<UpdateRecipeResponse>.Error($"Failed to create contributor relationship: {contributorResult.Errors.FirstOrDefault()}");
                }
            }

            var updatedName = request.Name?.Trim() ?? recipe.Name;
            var updatedDescription = request.Description?.Trim() ?? recipe.Description;
            var updatedCategory = request.Category ?? recipe.Category;
            var updatedServings = request.Servings ?? recipe.Servings;
            var updatedPreparationTime = request.PreparationTimeMinutes ?? recipe.PreparationTimeMinutes;
            var updatedCookingTime = request.CookingTimeMinutes ?? recipe.CookingTimeMinutes;
            var updatedInstructions = request.Instructions?.Trim() ?? recipe.Instructions;

            var updatedIsPublic = request.IsPublic ?? recipe.IsPublic;

            recipe.UpdateDetails(
                updatedName,
                updatedDescription,
                updatedCategory,
                updatedServings,
                updatedPreparationTime,
                updatedCookingTime,
                updatedInstructions
            );

            
            if (updatedIsPublic != recipe.IsPublic)
            {
                recipe.SetPublic(updatedIsPublic);
            }


            if (request.Ingredients != null)
            {
                foreach (var ingredientRequest in request.Ingredients)
                {
                    if (ingredientRequest.QuantityGrams.HasValue && 
                        (ingredientRequest.QuantityGrams.Value <= 0 || ingredientRequest.QuantityGrams.Value > 10000))
                    {
                        return Result<UpdateRecipeResponse>.Error("Ingredient quantity must be between 0.1 and 10000 grams");
                    }

                    if (ingredientRequest.ProductId.HasValue && ingredientRequest.ProductId.Value != Guid.Empty)
                    {
                        var productResult = await _productRepository.GetByIdAsync(ingredientRequest.ProductId.Value);
                        if (!productResult.IsSuccess)
                        {
                            return Result<UpdateRecipeResponse>.Error($"Product with ID {ingredientRequest.ProductId.Value} not found");
                        }
                    }
                }

                var currentIngredientsResult = await _ingredientRepository.ListAsync();
                if (!currentIngredientsResult.IsSuccess)
                {
                    return Result<UpdateRecipeResponse>.Error($"Failed to retrieve current ingredients: {currentIngredientsResult.Errors.FirstOrDefault()}");
                }

                var currentIngredients = currentIngredientsResult.Value.Where(i => i.RecipeId == recipe.Id).ToList();
                var updatedIngredients = new List<RecipeIngredient>();

                foreach (var ingredientRequest in request.Ingredients)
                {
                    if (ingredientRequest.IngredientId.HasValue)
                    {
                        var existingIngredient = currentIngredients.FirstOrDefault(i => i.Id == ingredientRequest.IngredientId.Value);
                        if (existingIngredient == null)
                        {
                            return Result<UpdateRecipeResponse>.Error($"Ingredient with ID {ingredientRequest.IngredientId.Value} not found");
                        }

                        if (ingredientRequest.QuantityGrams.HasValue)
                            existingIngredient.UpdateQuantity(ingredientRequest.QuantityGrams.Value);
                        
                        if (ingredientRequest.Notes != null)
                            existingIngredient.UpdateNotes(ingredientRequest.Notes.Trim());
                        
                        if (ingredientRequest.ProductId.HasValue)
                            existingIngredient.SetProduct(ingredientRequest.ProductId.Value);
                        
                        if (!string.IsNullOrWhiteSpace(ingredientRequest.CustomIngredientName))
                        {
                            existingIngredient.SetCustomIngredient(
                                ingredientRequest.CustomIngredientName,
                                ingredientRequest.CustomCaloriesPer100g,
                                ingredientRequest.CustomProteinPer100g,
                                ingredientRequest.CustomFatPer100g,
                                ingredientRequest.CustomCarbohydratesPer100g
                            );
                        }

                        var updateResult = await _ingredientRepository.UpdateAsync(existingIngredient);
                        if (!updateResult.IsSuccess)
                        {
                            return Result<UpdateRecipeResponse>.Error($"Failed to update ingredient: {updateResult.Errors.FirstOrDefault()}");
                        }

                        updatedIngredients.Add(existingIngredient);
                    }
                    else
                    {
                        var newIngredient = new RecipeIngredient(
                            recipe.Id,
                            ingredientRequest.ProductId ?? Guid.Empty,
                            ingredientRequest.QuantityGrams ?? 0,
                            ingredientRequest.Notes?.Trim()
                        );

                        if (ingredientRequest.ProductId.HasValue && ingredientRequest.ProductId.Value != Guid.Empty)
                        {
                            newIngredient.SetProduct(ingredientRequest.ProductId.Value);
                        }
                        else if (!string.IsNullOrWhiteSpace(ingredientRequest.CustomIngredientName))
                        {
                            newIngredient.SetCustomIngredient(
                                ingredientRequest.CustomIngredientName,
                                ingredientRequest.CustomCaloriesPer100g,
                                ingredientRequest.CustomProteinPer100g,
                                ingredientRequest.CustomFatPer100g,
                                ingredientRequest.CustomCarbohydratesPer100g
                            );
                        }
                        else
                        {
                            return Result<UpdateRecipeResponse>.Error("Each ingredient must have either a product ID or custom ingredient name");
                        }

                        var addResult = await _ingredientRepository.AddAsync(newIngredient);
                        if (!addResult.IsSuccess)
                        {
                            return Result<UpdateRecipeResponse>.Error($"Failed to create ingredient: {addResult.Errors.FirstOrDefault()}");
                        }

                        updatedIngredients.Add(newIngredient);
                    }
                }

                var requestedIngredientIds = request.Ingredients
                    .Where(i => i.IngredientId.HasValue)
                    .Select(i => i.IngredientId!.Value)
                    .ToHashSet();

                var ingredientsToRemove = currentIngredients.Where(i => !requestedIngredientIds.Contains(i.Id)).ToList();
                foreach (var ingredientToRemove in ingredientsToRemove)
                {
                    var deleteResult = await _ingredientRepository.DeleteAsync(ingredientToRemove.Id);
                    if (!deleteResult.IsSuccess)
                    {
                        return Result<UpdateRecipeResponse>.Error($"Failed to delete ingredient: {deleteResult.Errors.FirstOrDefault()}");
                    }
                }

                var recipeWithIngredientsResult = await _recipeRepository.GetByIdAsync(recipe.Id, "Ingredients", "Ingredients.Product");
                if (recipeWithIngredientsResult.IsSuccess)
                {
                    recipe.SetIngredients(recipeWithIngredientsResult.Value.Ingredients);
                }
            }

            _recipeService.CalculateNutritionalValues(recipe);

            var recipeUpdateResult = await _recipeRepository.UpdateAsync(recipe);
            if (!recipeUpdateResult.IsSuccess)
            {
                return Result<UpdateRecipeResponse>.Error($"Failed to update recipe: {recipeUpdateResult.Errors.FirstOrDefault()}");
            }

            var finalIngredientsResult = await _ingredientRepository.ListAsync();
            var finalIngredients = finalIngredientsResult.IsSuccess 
                ? finalIngredientsResult.Value.Where(i => i.RecipeId == recipe.Id).ToList()
                : new List<RecipeIngredient>();

            var userFavoritesResult = await _recipeFavoriteRepository.ListAsync();
            var isFavorited = userFavoritesResult.IsSuccess && userFavoritesResult.Value
                .Any(f => f.UserId == request.UserId && f.RecipeId == recipe.Id);

            var response = new UpdateRecipeResponse
            {
                RecipeId = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                Category = recipe.Category,
                Servings = recipe.Servings,
                PreparationTimeMinutes = recipe.PreparationTimeMinutes,
                CookingTimeMinutes = recipe.CookingTimeMinutes,
                Instructions = recipe.Instructions,

                IsPublic = recipe.IsPublic,
                IsFavorite = isFavorited,
                TotalCalories = recipe.TotalCalories,
                TotalProtein = recipe.TotalProtein,
                TotalFat = recipe.TotalFat,
                TotalCarbohydrates = recipe.TotalCarbohydrates,
                UpdatedAt = recipe.UpdatedAt,
                Ingredients = finalIngredients.Select(i => new UpdateRecipeIngredientResponse
                {
                    IngredientId = i.Id,
                    ProductId = i.ProductId != Guid.Empty ? i.ProductId : null,
                    QuantityGrams = i.QuantityGrams,
                    Notes = i.Notes,
                    CustomIngredientName = i.CustomIngredientName
                }).ToList()
            };

            return Result<UpdateRecipeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateRecipeResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
}
