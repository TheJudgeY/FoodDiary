using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Recipes;

public interface IRecipeIngredientService
{
    void UpdateQuantity(RecipeIngredient ingredient, double quantityGrams);
    void UpdateNotes(RecipeIngredient ingredient, string? notes);
    void SetCustomIngredient(RecipeIngredient ingredient, string name, double? caloriesPer100g = null, 
        double? proteinPer100g = null, double? fatPer100g = null, double? carbohydratesPer100g = null);
    void SetProduct(RecipeIngredient ingredient, Guid productId);
    double GetCalories(RecipeIngredient ingredient);
    double GetProtein(RecipeIngredient ingredient);
    double GetFat(RecipeIngredient ingredient);
    double GetCarbohydrates(RecipeIngredient ingredient);
    string GetIngredientName(RecipeIngredient ingredient);
    string GetQuantityDisplay(RecipeIngredient ingredient);
    bool IsCustomIngredient(RecipeIngredient ingredient);
    bool HasProduct(RecipeIngredient ingredient);
    bool HasNutritionalData(RecipeIngredient ingredient);
}

public class RecipeIngredientService : IRecipeIngredientService
{
    private const double GRAMS_TO_PERCENTAGE_RATIO = 100.0;
    private const double GRAMS_PER_KILOGRAM = 1000.0;
    private const double MINIMUM_GRAMS_FOR_INTEGER_DISPLAY = 1.0;

    #region Ingredient Updates

    public void UpdateQuantity(RecipeIngredient ingredient, double quantityGrams)
    {
        ingredient.UpdateQuantity(quantityGrams);
    }

    public void UpdateNotes(RecipeIngredient ingredient, string? notes)
    {
        ingredient.UpdateNotes(notes?.Trim());
    }

    public void SetCustomIngredient(RecipeIngredient ingredient, string name, double? caloriesPer100g = null, 
        double? proteinPer100g = null, double? fatPer100g = null, double? carbohydratesPer100g = null)
    {
        ingredient.SetCustomIngredient(name ?? string.Empty, caloriesPer100g, proteinPer100g, fatPer100g, carbohydratesPer100g);
    }

    public void SetProduct(RecipeIngredient ingredient, Guid productId)
    {
        ingredient.SetProduct(productId);
    }

    #endregion

    #region Nutritional Calculations

    public double GetCalories(RecipeIngredient ingredient)
    {
        return Math.Round(CalculateNutritionalValue(ingredient, 
            product => product.CaloriesPer100g, 
            ingredient => ingredient.CustomCaloriesPer100g), 1);
    }

    public double GetProtein(RecipeIngredient ingredient)
    {
        return Math.Round(CalculateNutritionalValue(ingredient, 
            product => product.ProteinsPer100g, 
            ingredient => ingredient.CustomProteinPer100g), 1);
    }

    public double GetFat(RecipeIngredient ingredient)
    {
        return Math.Round(CalculateNutritionalValue(ingredient, 
            product => product.FatsPer100g, 
            ingredient => ingredient.CustomFatPer100g), 1);
    }

    public double GetCarbohydrates(RecipeIngredient ingredient)
    {
        return Math.Round(CalculateNutritionalValue(ingredient, 
            product => product.CarbohydratesPer100g, 
            ingredient => ingredient.CustomCarbohydratesPer100g), 1);
    }

    private static double CalculateNutritionalValue(RecipeIngredient ingredient, 
        Func<Product, double> getProductValue, 
        Func<RecipeIngredient, double?> getCustomValue)
    {
        if (ingredient.Product != null)
        {
            return (getProductValue(ingredient.Product) * ingredient.QuantityGrams) / GRAMS_TO_PERCENTAGE_RATIO;
        }
        
        var customValue = getCustomValue(ingredient);
        if (customValue.HasValue)
        {
            return (customValue.Value * ingredient.QuantityGrams) / GRAMS_TO_PERCENTAGE_RATIO;
        }
        
        return 0;
    }

    #endregion

    #region Ingredient Information

    public string GetIngredientName(RecipeIngredient ingredient)
    {
        if (ingredient.Product != null)
        {
            return ingredient.Product.Name;
        }
        
        return ingredient.CustomIngredientName ?? "Unknown Ingredient";
    }

    public string GetQuantityDisplay(RecipeIngredient ingredient)
    {
        var quantity = ingredient.QuantityGrams;
        
        if (quantity >= GRAMS_PER_KILOGRAM)
        {
            var kg = quantity / GRAMS_PER_KILOGRAM;
            return $"{kg:F1} kg";
        }
        
        if (quantity >= MINIMUM_GRAMS_FOR_INTEGER_DISPLAY)
        {
            return $"{quantity:F0} g";
        }
        
        return $"{quantity:F1} g";
    }

    #endregion

    #region Ingredient Validation

    public bool IsCustomIngredient(RecipeIngredient ingredient)
    {
        return !string.IsNullOrWhiteSpace(ingredient.CustomIngredientName);
    }

    public bool HasProduct(RecipeIngredient ingredient)
    {
        return ingredient.Product != null || ingredient.ProductId != Guid.Empty;
    }

    public bool HasNutritionalData(RecipeIngredient ingredient)
    {
        if (ingredient.Product != null)
        {
            return HasProductNutritionalData(ingredient.Product);
        }
        
        return HasCustomNutritionalData(ingredient);
    }

    private static bool HasProductNutritionalData(Product product)
    {
        return product.CaloriesPer100g > 0 || product.ProteinsPer100g > 0 || 
               product.FatsPer100g > 0 || product.CarbohydratesPer100g > 0;
    }

    private static bool HasCustomNutritionalData(RecipeIngredient ingredient)
    {
        return ingredient.CustomCaloriesPer100g.HasValue || ingredient.CustomProteinPer100g.HasValue || 
               ingredient.CustomFatPer100g.HasValue || ingredient.CustomCarbohydratesPer100g.HasValue;
    }

    #endregion
} 