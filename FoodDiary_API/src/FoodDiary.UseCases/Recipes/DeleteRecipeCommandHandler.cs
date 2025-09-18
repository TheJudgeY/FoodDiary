using MediatR;
using Ardalis.Result;

using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes;

public class DeleteRecipeCommandHandler : IRequestHandler<DeleteRecipeCommand, Result>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Recipe> _recipeRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> _ingredientRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> _recipeFavoriteRepository;

    public DeleteRecipeCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Recipe> recipeRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeIngredient> ingredientRepository,
        FoodDiary.Core.Interfaces.IRepository<RecipeFavorite> recipeFavoriteRepository)
    {
        _recipeRepository = recipeRepository;
        _ingredientRepository = ingredientRepository;
        _recipeFavoriteRepository = recipeFavoriteRepository;
    }

    public async Task<Result> Handle(DeleteRecipeCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var recipeResult = await _recipeRepository.GetByIdAsync(request.RecipeId);
            if (!recipeResult.IsSuccess)
            {
                return Result.NotFound($"Recipe with ID {request.RecipeId} not found");
            }

            var recipe = recipeResult.Value;

            var userRelationshipsResult = await _recipeFavoriteRepository.ListAsync();
            if (!userRelationshipsResult.IsSuccess)
            {
                return Result.Error($"Failed to check user relationships: {userRelationshipsResult.Errors.FirstOrDefault()}");
            }

            var isCreator = userRelationshipsResult.Value
                .Any(f => f.UserId == request.UserId && f.RecipeId == request.RecipeId && f.RelationshipType == RecipeUserRelationshipType.Creator);

            if (!isCreator)
            {
                return Result.Forbidden("Access denied. You can only delete recipes you created");
            }

            var ingredientsResult = await _ingredientRepository.ListAsync();
            if (!ingredientsResult.IsSuccess)
            {
                return Result.Error($"Failed to retrieve ingredients: {ingredientsResult.Errors.FirstOrDefault()}");
            }

            var ingredients = ingredientsResult.Value.Where(i => i.RecipeId == recipe.Id).ToList();

            foreach (var ingredient in ingredients)
            {
                var deleteIngredientResult = await _ingredientRepository.DeleteAsync(ingredient.Id);
                if (!deleteIngredientResult.IsSuccess)
                {
                    return Result.Error($"Failed to delete ingredient {ingredient.Id}: {deleteIngredientResult.Errors.FirstOrDefault()}");
                }
            }

            var deleteRecipeResult = await _recipeRepository.DeleteAsync(recipe.Id);
            if (!deleteRecipeResult.IsSuccess)
            {
                return Result.Error($"Failed to delete recipe: {deleteRecipeResult.Errors.FirstOrDefault()}");
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Error($"Unexpected error: {ex.Message}");
        }
    }
}
