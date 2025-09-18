using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes.ToggleFavorite;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, Result<ToggleFavoriteResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;

    public ToggleFavoriteCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> recipeFavoriteRepository)
    {
        _recipeRepository = recipeRepository;
        _recipeFavoriteRepository = recipeFavoriteRepository;
    }

    public async Task<Result<ToggleFavoriteResponse>> Handle(ToggleFavoriteCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recipeResult = await _recipeRepository.GetByIdAsync(request.RecipeId);
            if (!recipeResult.IsSuccess)
            {
                return Result<ToggleFavoriteResponse>.NotFound($"Recipe with ID {request.RecipeId} not found");
            }

            var recipe = recipeResult.Value;

            var userRelationshipsResult = await _recipeFavoriteRepository.ListAsync();
            if (!userRelationshipsResult.IsSuccess)
            {
                return Result<ToggleFavoriteResponse>.Error($"Failed to check user relationships: {userRelationshipsResult.Errors.FirstOrDefault()}");
            }

            var userRelationships = userRelationshipsResult.Value
                .Where(f => f.UserId == request.UserId && f.RecipeId == request.RecipeId)
                .ToList();

            var isCreator = userRelationships.Any(r => r.RelationshipType == RecipeUserRelationshipType.Creator);
            var isPublic = recipe.IsPublic;

            if (!isCreator && !isPublic)
            {
                return Result<ToggleFavoriteResponse>.Forbidden("You can only favorite recipes you created or public recipes");
            }

            var userFavoritesResult = await _recipeFavoriteRepository.ListAsync();
            if (!userFavoritesResult.IsSuccess)
            {
                return Result<ToggleFavoriteResponse>.Error($"Failed to check favorite status: {userFavoritesResult.Errors.FirstOrDefault()}");
            }

            var existingFavorite = userFavoritesResult.Value
                .FirstOrDefault(f => f.UserId == request.UserId && f.RecipeId == request.RecipeId && f.RelationshipType == RecipeUserRelationshipType.Favorite);

            bool isCurrentlyFavorited = existingFavorite != null;
            string message;

            if (isCurrentlyFavorited && existingFavorite != null)
            {
                if (isCreator)
                {
                    return Result<ToggleFavoriteResponse>.Forbidden("Recipe creators cannot unfavorite their own recipes");
                }

                var removeResult = await _recipeFavoriteRepository.DeleteAsync(existingFavorite.Id);
                if (!removeResult.IsSuccess)
                {
                    return Result<ToggleFavoriteResponse>.Error($"Failed to remove from favorites: {removeResult.Errors.FirstOrDefault()}");
                }
                message = "Recipe removed from favorites";
            }
            else
            {
                var recipeFavorite = new RecipeFavorite(request.UserId, request.RecipeId, RecipeUserRelationshipType.Favorite);
                var addResult = await _recipeFavoriteRepository.AddAsync(recipeFavorite);
                if (!addResult.IsSuccess)
                {
                    return Result<ToggleFavoriteResponse>.Error($"Failed to add to favorites: {addResult.Errors.FirstOrDefault()}");
                }
                message = "Recipe added to favorites";
            }

            return Result<ToggleFavoriteResponse>.Success(new ToggleFavoriteResponse
            {
                RecipeId = recipe.Id,
                IsFavorite = !isCurrentlyFavorited,
                Message = message
            });
        }
        catch (Exception ex)
        {
            return Result<ToggleFavoriteResponse>.Error($"An error occurred while toggling favorite status: {ex.Message}");
        }
    }
}
