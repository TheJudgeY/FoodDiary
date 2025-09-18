using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.Interfaces;

namespace FoodDiary.UseCases.Recipes;

public class ListRecipesCommandHandler : IRequestHandler<ListRecipesCommand, Result<ListRecipesResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> _ingredientRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;
    private readonly IMapper _mapper;

    public ListRecipesCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> ingredientRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> recipeFavoriteRepository,
        IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _recipeFavoriteRepository = recipeFavoriteRepository;
        _mapper = mapper;
    }

    public async Task<Result<ListRecipesResponse>> Handle(ListRecipesCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var recipesResult = await _recipeRepository.ListAsync("Ingredients");
            if (!recipesResult.IsSuccess)
            {
                return Result<ListRecipesResponse>.Error($"Failed to retrieve recipes: {recipesResult.Errors.FirstOrDefault()}");
            }

            var allRecipes = recipesResult.Value;

            var userFavoritesResult = await _recipeFavoriteRepository.ListAsync();
            if (!userFavoritesResult.IsSuccess)
            {
                return Result<ListRecipesResponse>.Error($"Failed to retrieve user favorites: {userFavoritesResult.Errors.FirstOrDefault()}");
            }
            
            var userFavoriteRecipeIds = userFavoritesResult.Value
                .Where(f => f.UserId == request.UserId)
                .Select(f => f.RecipeId)
                .ToHashSet();

            var userCreatorRecipeIds = userFavoritesResult.Value
                .Where(f => f.UserId == request.UserId && f.RelationshipType == RecipeUserRelationshipType.Creator)
                .Select(f => f.RecipeId)
                .ToHashSet();

            var userContributorRecipeIds = userFavoritesResult.Value
                .Where(f => f.UserId == request.UserId && f.RelationshipType == RecipeUserRelationshipType.Contributor)
                .Select(f => f.RecipeId)
                .ToHashSet();

            var filteredRecipes = allRecipes.Where(r => 
                userCreatorRecipeIds.Contains(r.Id) || 
                userContributorRecipeIds.Contains(r.Id) ||
                (request.IncludePublic && r.IsPublic)
            ).ToList();

            if (request.Category.HasValue)
            {
                filteredRecipes = filteredRecipes.Where(r => r.Category == request.Category.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLowerInvariant();
                filteredRecipes = filteredRecipes.Where(r => 
                    r.Name.ToLowerInvariant().Contains(searchTerm) ||
                    (r.Description != null && r.Description.ToLowerInvariant().Contains(searchTerm))
                ).ToList();
            }

            if (request.IncludeFavorites)
            {
                filteredRecipes = filteredRecipes.Where(r => userFavoriteRecipeIds.Contains(r.Id)).ToList();
            }

            var ingredientCounts = allRecipes
                .ToDictionary(r => r.Id, r => r.Ingredients?.Count ?? 0);

            var sortedRecipes = request.SortBy?.ToLowerInvariant() switch
            {
                "name" => request.SortDescending 
                    ? filteredRecipes.OrderByDescending(r => r.Name).ToList()
                    : filteredRecipes.OrderBy(r => r.Name).ToList(),
                "createdat" => request.SortDescending 
                    ? filteredRecipes.OrderByDescending(r => r.CreatedAt).ToList()
                    : filteredRecipes.OrderBy(r => r.CreatedAt).ToList(),
                "updatedat" => request.SortDescending 
                    ? filteredRecipes.OrderByDescending(r => r.UpdatedAt).ToList()
                    : filteredRecipes.OrderBy(r => r.UpdatedAt).ToList(),
                "totalcalories" => request.SortDescending 
                    ? filteredRecipes.OrderByDescending(r => r.TotalCalories).ToList()
                    : filteredRecipes.OrderBy(r => r.TotalCalories).ToList(),
                _ => request.SortDescending 
                    ? filteredRecipes.OrderByDescending(r => r.Name).ToList()
                    : filteredRecipes.OrderBy(r => r.Name).ToList()
            };

            var totalCount = sortedRecipes.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var pagedRecipes = sortedRecipes.Skip(skip).Take(request.PageSize).ToList();

            var recipeResponses = pagedRecipes.Select(r => new ListRecipeItemResponse
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Category = r.Category,
                CategoryDisplayName = r.Category.GetDisplayName(),
                Servings = r.Servings,
                PreparationTimeMinutes = r.PreparationTimeMinutes,
                CookingTimeMinutes = r.CookingTimeMinutes,

                HasImage = r.ImageData != null && r.ImageData.Length > 0,
                ImageFileName = r.ImageFileName,
                ImageContentType = r.ImageContentType,

                IsPublic = r.IsPublic,
                IsFavorite = userFavoriteRecipeIds.Contains(r.Id),
                IsCreator = userCreatorRecipeIds.Contains(r.Id),
                IsContributor = userContributorRecipeIds.Contains(r.Id),
                TotalCalories = r.TotalCalories,
                TotalProtein = r.TotalProtein,
                TotalFat = r.TotalFat,
                TotalCarbohydrates = r.TotalCarbohydrates,
                
                CaloriesPerServing = r.Servings > 0 ? Math.Round(r.TotalCalories / r.Servings, 1) : 0,
                ProteinPerServing = r.Servings > 0 ? Math.Round(r.TotalProtein / r.Servings, 1) : 0,
                FatPerServing = r.Servings > 0 ? Math.Round(r.TotalFat / r.Servings, 1) : 0,
                CarbohydratesPerServing = r.Servings > 0 ? Math.Round(r.TotalCarbohydrates / r.Servings, 1) : 0,
                
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                IngredientCount = ingredientCounts.GetValueOrDefault(r.Id, 0)
            }).ToList();

            var response = new ListRecipesResponse
            {
                Recipes = recipeResponses,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return Result<ListRecipesResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ListRecipesResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
}
