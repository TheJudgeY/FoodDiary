

namespace FoodDiary.Core.RecipeAggregate;

public class RecipeIngredient
{
    public Guid Id { get; private set; }
    public Guid RecipeId { get; private set; }
    public Guid ProductId { get; private set; }
    
    public double QuantityGrams { get; private set; }
    
    public string? Notes { get; private set; }
    
    public string? CustomIngredientName { get; private set; }
    
    public double? CustomCaloriesPer100g { get; private set; }
    public double? CustomProteinPer100g { get; private set; }
    public double? CustomFatPer100g { get; private set; }
    public double? CustomCarbohydratesPer100g { get; private set; }
    
    public Recipe Recipe { get; private set; } = null!;
    public ProductAggregate.Product? Product { get; private set; }

    public RecipeIngredient(Guid recipeId, Guid productId, double quantityGrams, string? notes = null)
    {
        Id = Guid.NewGuid();
        RecipeId = recipeId;
        ProductId = productId;
        QuantityGrams = quantityGrams;
        Notes = notes;
    }

    private RecipeIngredient() { }
    
    public void UpdateQuantity(double quantityGrams)
    {
        QuantityGrams = quantityGrams;
    }
    
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
    
    public void SetCustomIngredient(string name, double? caloriesPer100g = null, double? proteinPer100g = null, 
        double? fatPer100g = null, double? carbohydratesPer100g = null)
    {
        CustomIngredientName = name;
        CustomCaloriesPer100g = caloriesPer100g;
        CustomProteinPer100g = proteinPer100g;
        CustomFatPer100g = fatPer100g;
        CustomCarbohydratesPer100g = carbohydratesPer100g;
        
        ProductId = Guid.Empty;
        Product = null;
    }
    
    public void SetProduct(Guid productId)
    {
        ProductId = productId;
        
        CustomIngredientName = null;
        CustomCaloriesPer100g = null;
        CustomProteinPer100g = null;
        CustomFatPer100g = null;
        CustomCarbohydratesPer100g = null;
    }
} 