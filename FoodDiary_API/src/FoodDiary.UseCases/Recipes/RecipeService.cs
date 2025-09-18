using FoodDiary.Core.RecipeAggregate;
using FoodDiary.UseCases.Validation;
using FluentValidation;

namespace FoodDiary.UseCases.Recipes;

public interface IRecipeService
{
    bool HasImage(Recipe recipe);
    long GetImageSizeInBytes(Recipe recipe);
    
    bool HasIngredients(Recipe recipe);
    int GetIngredientCount(Recipe recipe);
    void TogglePublic(Recipe recipe);
    void SetPublic(Recipe recipe, bool isPublic);
    
    double GetCaloriesPerServing(Recipe recipe);
    double GetProteinPerServing(Recipe recipe);
    double GetFatPerServing(Recipe recipe);
    double GetCarbohydratesPerServing(Recipe recipe);
    
    int GetTotalTimeMinutes(Recipe recipe);
    string GetTotalTimeDisplay(Recipe recipe);
    
    bool IsComplete(Recipe recipe);
    void CalculateNutritionalValues(Recipe recipe);
    void UpdateDetails(Recipe recipe, string name, string description, RecipeCategory category, 
        int servings, int preparationTimeMinutes, int cookingTimeMinutes, string instructions);
    
    void ValidateRecipe(Recipe recipe);
    void ValidateUpdateDetails(Recipe recipe, string name, string description, RecipeCategory category, 
        int servings, int preparationTimeMinutes, int cookingTimeMinutes, string instructions);
}

public class RecipeService : IRecipeService
{
    private const double GramsToPercentageRatio = 100.0;
    private const int MinutesPerHour = 60;

    public bool HasImage(Recipe recipe) =>
        recipe.ImageData != null && recipe.ImageData.Length > 0;

    public long GetImageSizeInBytes(Recipe recipe) =>
        recipe.ImageData?.Length ?? 0;

    public bool HasIngredients(Recipe recipe) =>
        recipe.Ingredients.Any();

    public int GetIngredientCount(Recipe recipe) =>
        recipe.Ingredients.Count;

    public void TogglePublic(Recipe recipe) =>
        recipe.SetPublic(!recipe.IsPublic);

    public void SetPublic(Recipe recipe, bool isPublic) =>
        recipe.SetPublic(isPublic);

    public double GetCaloriesPerServing(Recipe recipe) =>
        Math.Round(CalculatePerServingValue(recipe.TotalCalories, recipe.Servings), 1);

    public double GetProteinPerServing(Recipe recipe) =>
        Math.Round(CalculatePerServingValue(recipe.TotalProtein, recipe.Servings), 1);

    public double GetFatPerServing(Recipe recipe) =>
        Math.Round(CalculatePerServingValue(recipe.TotalFat, recipe.Servings), 1);

    public double GetCarbohydratesPerServing(Recipe recipe) =>
        Math.Round(CalculatePerServingValue(recipe.TotalCarbohydrates, recipe.Servings), 1);

    private static double CalculatePerServingValue(double totalValue, int servings) =>
        servings > 0 ? totalValue / servings : 0;

    public void CalculateNutritionalValues(Recipe recipe)
    {
        if (!recipe.Ingredients.Any())
        {
            ResetNutritionalValues(recipe);
            return;
        }

        var totalCalories = 0.0;
        var totalProtein = 0.0;
        var totalFat = 0.0;
        var totalCarbohydrates = 0.0;

        foreach (var ingredient in recipe.Ingredients)
        {
            if (ingredient.Product == null) continue;

            var servingRatio = ingredient.QuantityGrams / GramsToPercentageRatio;
            
            totalCalories += ingredient.Product.CaloriesPer100g * servingRatio;
            totalProtein += ingredient.Product.ProteinsPer100g * servingRatio;
            totalFat += ingredient.Product.FatsPer100g * servingRatio;
            totalCarbohydrates += ingredient.Product.CarbohydratesPer100g * servingRatio;
        }

        recipe.UpdateNutritionalInfo(
            Math.Round(totalCalories, 1),
            Math.Round(totalProtein, 1),
            Math.Round(totalFat, 1),
            Math.Round(totalCarbohydrates, 1)
        );
    }

    private static void ResetNutritionalValues(Recipe recipe)
    {
        recipe.UpdateNutritionalInfo(0, 0, 0, 0);
    }

    public int GetTotalTimeMinutes(Recipe recipe) =>
        recipe.PreparationTimeMinutes + recipe.CookingTimeMinutes;

    public string GetTotalTimeDisplay(Recipe recipe) =>
        FormatTimeDisplay(GetTotalTimeMinutes(recipe));

    private static string FormatTimeDisplay(int totalMinutes)
    {
        if (totalMinutes < MinutesPerHour)
            return $"{totalMinutes} min";

        var hours = totalMinutes / MinutesPerHour;
        var minutes = totalMinutes % MinutesPerHour;

        return minutes > 0 ? $"{hours}h {minutes}m" : $"{hours}h";
    }

    public bool IsComplete(Recipe recipe) =>
        HasRequiredBasicInfo(recipe) &&
        HasRequiredIngredients(recipe) &&
        HasRequiredServingInfo(recipe) &&
        HasRequiredTimeInfo(recipe);

    private static bool HasRequiredBasicInfo(Recipe recipe) =>
        !string.IsNullOrWhiteSpace(recipe.Name) &&
        !string.IsNullOrWhiteSpace(recipe.Description) &&
        !string.IsNullOrWhiteSpace(recipe.Instructions);

    private static bool HasRequiredIngredients(Recipe recipe) =>
        recipe.Ingredients.Any() && recipe.Ingredients.All(i => i.Product != null && i.QuantityGrams > 0);

    private static bool HasRequiredServingInfo(Recipe recipe) =>
        recipe.Servings > 0;

    private static bool HasRequiredTimeInfo(Recipe recipe) =>
        recipe.PreparationTimeMinutes >= 0 && recipe.CookingTimeMinutes >= 0;

    public void UpdateDetails(Recipe recipe, string name, string description, RecipeCategory category, 
        int servings, int preparationTimeMinutes, int cookingTimeMinutes, string instructions)
    {
        ValidateUpdateDetails(recipe, name, description, category, servings, preparationTimeMinutes, cookingTimeMinutes, instructions);
        
        recipe.UpdateDetails(name, description, category, servings, preparationTimeMinutes, cookingTimeMinutes, instructions);
        CalculateNutritionalValues(recipe);
    }

    public void ValidateRecipe(Recipe recipe)
    {
        var validator = new RecipeValidator();
        var validationResult = validator.Validate(recipe);
        
        if (!validationResult.IsValid)
        {
            var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException($"Recipe validation failed: {errors}");
        }
    }

    public void ValidateUpdateDetails(Recipe recipe, string name, string description, RecipeCategory category, 
        int servings, int preparationTimeMinutes, int cookingTimeMinutes, string instructions)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Recipe name cannot be empty", nameof(name));
        
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Recipe description cannot be empty", nameof(description));
        
        if (string.IsNullOrWhiteSpace(instructions))
            throw new ArgumentException("Recipe instructions cannot be empty", nameof(instructions));
        
        if (servings <= 0)
            throw new ArgumentException("Servings must be greater than 0", nameof(servings));
        
        if (preparationTimeMinutes < 0)
            throw new ArgumentException("Preparation time cannot be negative", nameof(preparationTimeMinutes));
        
        if (cookingTimeMinutes < 0)
            throw new ArgumentException("Cooking time cannot be negative", nameof(cookingTimeMinutes));
    }
} 