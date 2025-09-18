using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Recipes;

public class CreateRecipeCommandHandler : IRequestHandler<CreateRecipeCommand, Result<CreateRecipeResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> _ingredientRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;
    private readonly IMapper _mapper;
    private readonly IRecipeService _recipeService;

    public CreateRecipeCommandHandler(
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

    public async Task<Result<CreateRecipeResponse>> Handle(CreateRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var ingredient in request.Ingredients)
            {
                if (ingredient.ProductId.HasValue && ingredient.ProductId != Guid.Empty)
                {
                    var productResult = await _productRepository.GetByIdAsync(ingredient.ProductId.Value);
                    if (!productResult.IsSuccess)
                    {
                        return Result<CreateRecipeResponse>.Error($"Product with ID {ingredient.ProductId.Value} not found");
                    }
                }
            }

            var recipe = new Recipe(
                request.Name.Trim(),
                request.Description?.Trim(),
                request.Category,
                request.Servings,
                request.PreparationTimeMinutes,
                request.CookingTimeMinutes,
                request.Instructions.Trim()
            );

            
            if (request.IsPublic)
            {
                recipe.SetPublic(true);
            }


            if (request.ImageData != null && request.ImageData.Length > 0)
            {
                recipe.UpdateImage(request.ImageData, request.ImageContentType, request.ImageFileName);
            }

            var recipeResult = await _recipeRepository.AddAsync(recipe);
            if (!recipeResult.IsSuccess)
            {
                return Result<CreateRecipeResponse>.Error($"Failed to create recipe: {recipeResult.Errors.FirstOrDefault()}");
            }

            var creatorRelationship = new RecipeFavorite(request.UserId, recipe.Id, RecipeUserRelationshipType.Creator);
            var creatorResult = await _recipeFavoriteRepository.AddAsync(creatorRelationship);
            if (!creatorResult.IsSuccess)
            {
                return Result<CreateRecipeResponse>.Error($"Failed to create creator relationship: {creatorResult.Errors.FirstOrDefault()}");
            }

            var ingredients = new List<RecipeIngredient>();
            foreach (var ingredientRequest in request.Ingredients)
            {
                var ingredient = new RecipeIngredient(
                    recipe.Id,
                    ingredientRequest.ProductId ?? Guid.Empty,
                    ingredientRequest.QuantityGrams,
                    ingredientRequest.Notes?.Trim()
                );

                if (ingredientRequest.ProductId.HasValue && ingredientRequest.ProductId != Guid.Empty)
                {
                    ingredient.SetProduct(ingredientRequest.ProductId.Value);
                }
                else if (!string.IsNullOrWhiteSpace(ingredientRequest.CustomIngredientName))
                {
                    ingredient.SetCustomIngredient(
                        ingredientRequest.CustomIngredientName,
                        ingredientRequest.CustomCaloriesPer100g,
                        ingredientRequest.CustomProteinPer100g,
                        ingredientRequest.CustomFatPer100g,
                        ingredientRequest.CustomCarbohydratesPer100g
                    );
                }
                else
                {
                    return Result<CreateRecipeResponse>.Error("Each ingredient must have either a product ID or custom ingredient name");
                }

                var ingredientResult = await _ingredientRepository.AddAsync(ingredient);
                if (!ingredientResult.IsSuccess)
                {
                    return Result<CreateRecipeResponse>.Error($"Failed to create ingredient: {ingredientResult.Errors.FirstOrDefault()}");
                }

                ingredients.Add(ingredient);
            }

            recipe.SetIngredients(ingredients);
            _recipeService.CalculateNutritionalValues(recipe);

            var updateResult = await _recipeRepository.UpdateAsync(recipe);
            if (!updateResult.IsSuccess)
            {
                return Result<CreateRecipeResponse>.Error($"Failed to update recipe with nutritional values: {updateResult.Errors.FirstOrDefault()}");
            }

            var response = new CreateRecipeResponse
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
                IsFavorite = false,
                TotalCalories = recipe.TotalCalories,
                TotalProtein = recipe.TotalProtein,
                TotalFat = recipe.TotalFat,
                TotalCarbohydrates = recipe.TotalCarbohydrates,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt,
                Ingredients = ingredients.Select(i => new CreateRecipeIngredientResponse
                {
                    IngredientId = i.Id,
                    ProductId = i.ProductId != Guid.Empty ? i.ProductId : null,
                    QuantityGrams = i.QuantityGrams,
                    Notes = i.Notes,
                    CustomIngredientName = i.CustomIngredientName
                }).ToList()
            };

            return Result<CreateRecipeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateRecipeResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
}
