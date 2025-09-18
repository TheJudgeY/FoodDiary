using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.UseCases.Recipes;

public class GetRecipeCommandHandler : IRequestHandler<GetRecipeCommand, Result<GetRecipeResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> _ingredientRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;
    private readonly IMapper _mapper;

    public GetRecipeCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> ingredientRepository,
        FoodDiary.Core.Interfaces.IRepository<Product> productRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> recipeFavoriteRepository,
        IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _productRepository = productRepository;
        _recipeFavoriteRepository = recipeFavoriteRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetRecipeResponse>> Handle(GetRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var recipeResult = await _recipeRepository.GetByIdAsync(request.RecipeId, "Ingredients", "Ingredients.Product");
            if (!recipeResult.IsSuccess)
            {
                return Result<GetRecipeResponse>.NotFound($"Recipe with ID {request.RecipeId} not found");
            }

            var recipe = recipeResult.Value;

            var userRelationshipsResult = await _recipeFavoriteRepository.ListAsync();
            if (!userRelationshipsResult.IsSuccess)
            {
                return Result<GetRecipeResponse>.Error($"Failed to check user relationships: {userRelationshipsResult.Errors.FirstOrDefault()}");
            }

            var userRelationships = userRelationshipsResult.Value
                .Where(f => f.UserId == request.UserId && f.RecipeId == request.RecipeId)
                .ToList();

            var isCreator = userRelationships.Any(r => r.RelationshipType == RecipeUserRelationshipType.Creator);
            var isContributor = userRelationships.Any(r => r.RelationshipType == RecipeUserRelationshipType.Contributor);
            var isFavorited = userRelationships.Any(r => r.RelationshipType == RecipeUserRelationshipType.Favorite);

            if (!isCreator && !isContributor && !recipe.IsPublic)
            {
                return Result<GetRecipeResponse>.Forbidden("Access denied. Recipe is private and you did not create or contribute to it");
            }

            var ingredients = recipe.Ingredients?.ToList() ?? new List<RecipeIngredient>();

            var ingredientResponses = new List<GetRecipeIngredientResponse>();
            foreach (var ingredient in ingredients)
            {
                var product = ingredient.Product;
                
                var ingredientResponse = new GetRecipeIngredientResponse
                {
                    Id = ingredient.Id,
                    ProductId = ingredient.ProductId != Guid.Empty ? ingredient.ProductId : null,
                    QuantityGrams = ingredient.QuantityGrams,
                    Notes = ingredient.Notes,
                    CustomIngredientName = ingredient.CustomIngredientName,
                    CustomCaloriesPer100g = ingredient.CustomCaloriesPer100g,
                    CustomProteinPer100g = ingredient.CustomProteinPer100g,
                    CustomFatPer100g = ingredient.CustomFatPer100g,
                    CustomCarbohydratesPer100g = ingredient.CustomCarbohydratesPer100g,
                    Product = product != null ? new GetRecipeProductResponse
                    {
                        Id = product.Id,
                        Name = product.Name,
                        CaloriesPer100g = product.CaloriesPer100g,
                        ProteinsPer100g = product.ProteinsPer100g,
                        FatsPer100g = product.FatsPer100g,
                        CarbohydratesPer100g = product.CarbohydratesPer100g,
                        Description = product.Description
                    } : null
                };

                ingredientResponses.Add(ingredientResponse);
            }

            var response = new GetRecipeResponse
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Description = recipe.Description,
                Category = recipe.Category,
                Servings = recipe.Servings,
                PreparationTimeMinutes = recipe.PreparationTimeMinutes,
                CookingTimeMinutes = recipe.CookingTimeMinutes,
                Instructions = recipe.Instructions,

                IsPublic = recipe.IsPublic,
                IsFavorite = isFavorited,
                IsCreator = isCreator,
                IsContributor = isContributor,
                TotalCalories = recipe.TotalCalories,
                TotalProtein = recipe.TotalProtein,
                TotalFat = recipe.TotalFat,
                TotalCarbohydrates = recipe.TotalCarbohydrates,
                CreatedAt = recipe.CreatedAt,
                UpdatedAt = recipe.UpdatedAt,
                Ingredients = ingredientResponses
            };

            return Result<GetRecipeResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetRecipeResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
}
