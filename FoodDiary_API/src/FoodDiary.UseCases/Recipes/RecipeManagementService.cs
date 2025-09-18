using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes;

public interface IRecipeManagementService
{
    void UpdateDetails(Recipe recipe, string name, string? description, RecipeCategory category, int servings, int preparationTimeMinutes, int cookingTimeMinutes, string instructions);
    void UpdateImage(Recipe recipe, byte[]? imageData, string? imageContentType, string? imageFileName);
    void RemoveImage(Recipe recipe);
    void UpdateNutritionalInfo(Recipe recipe, double totalCalories, double totalProtein, double totalFat, double totalCarbohydrates);
    void SetPublic(Recipe recipe, bool isPublic);
    void SetIngredients(Recipe recipe, List<RecipeIngredient> ingredients);
}

public class RecipeManagementService : IRecipeManagementService
{
    public void UpdateDetails(Recipe recipe, string name, string? description, RecipeCategory category, int servings, int preparationTimeMinutes, int cookingTimeMinutes, string instructions)
    {
        recipe.UpdateDetails(name, description, category, servings, preparationTimeMinutes, cookingTimeMinutes, instructions);
        SetUpdatedTimestamp(recipe);
    }

    public void UpdateImage(Recipe recipe, byte[]? imageData, string? imageContentType, string? imageFileName)
    {
        recipe.UpdateImage(imageData, imageContentType, imageFileName);
        SetUpdatedTimestamp(recipe);
    }

    public void RemoveImage(Recipe recipe)
    {
        recipe.RemoveImage();
        SetUpdatedTimestamp(recipe);
    }

    public void UpdateNutritionalInfo(Recipe recipe, double totalCalories, double totalProtein, double totalFat, double totalCarbohydrates)
    {
        recipe.UpdateNutritionalInfo(totalCalories, totalProtein, totalFat, totalCarbohydrates);
        SetUpdatedTimestamp(recipe);
    }

    public void SetPublic(Recipe recipe, bool isPublic)
    {
        recipe.SetPublic(isPublic);
        SetUpdatedTimestamp(recipe);
    }

    public void SetIngredients(Recipe recipe, List<RecipeIngredient> ingredients)
    {
        recipe.SetIngredients(ingredients);
        SetUpdatedTimestamp(recipe);
    }

    private static void SetUpdatedTimestamp(Recipe recipe)
    {
        var updatedAtProperty = typeof(Recipe).GetProperty("UpdatedAt");
        updatedAtProperty?.SetValue(recipe, DateTime.UtcNow);
    }
}
