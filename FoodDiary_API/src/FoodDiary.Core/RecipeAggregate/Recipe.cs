using FoodDiary.Core.UserAggregate;

namespace FoodDiary.Core.RecipeAggregate;

public static class RecipeDefaults
{
    public static readonly int DefaultServings = 1;
    public static readonly int DefaultPreparationTimeMinutes = 30;
    public static readonly int DefaultCookingTimeMinutes = 30;
    public static readonly double DefaultNutritionalValue = 0.0;
}

public class Recipe
{
    public Guid Id { get; private set; }
    
    public string Name { get; private set; } = string.Empty;
    
    public string? Description { get; private set; }
    
    public RecipeCategory Category { get; private set; } = RecipeCategory.Other;
    
    public int Servings { get; private set; } = RecipeDefaults.DefaultServings;
    
    public int PreparationTimeMinutes { get; private set; } = RecipeDefaults.DefaultPreparationTimeMinutes;
    
    public int CookingTimeMinutes { get; private set; } = RecipeDefaults.DefaultCookingTimeMinutes;
    
    public string Instructions { get; private set; } = string.Empty;
    
    public byte[]? ImageData { get; private set; }
    public string? ImageContentType { get; private set; }
    public string? ImageFileName { get; private set; }
    
    public bool IsPublic { get; private set; }
    
    public List<RecipeIngredient> Ingredients { get; private set; } = new List<RecipeIngredient>();
    public List<RecipeFavorite> Favorites { get; private set; } = new List<RecipeFavorite>();
    
    public double TotalCalories { get; private set; } = RecipeDefaults.DefaultNutritionalValue;
    public double TotalProtein { get; private set; } = RecipeDefaults.DefaultNutritionalValue;
    public double TotalFat { get; private set; } = RecipeDefaults.DefaultNutritionalValue;
    public double TotalCarbohydrates { get; private set; } = RecipeDefaults.DefaultNutritionalValue;
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

    public Recipe(string name, string? description, RecipeCategory category, int servings, int preparationTimeMinutes, int cookingTimeMinutes, string instructions)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Category = category;
        Servings = servings;
        PreparationTimeMinutes = preparationTimeMinutes;
        CookingTimeMinutes = cookingTimeMinutes;
        Instructions = instructions;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Recipe() { }
    
    public void UpdateDetails(string name, string? description, RecipeCategory category, int servings, 
        int preparationTimeMinutes, int cookingTimeMinutes, string instructions)
    {
        Name = name;
        Description = description;
        Category = category;
        Servings = servings;
        PreparationTimeMinutes = preparationTimeMinutes;
        CookingTimeMinutes = cookingTimeMinutes;
        Instructions = instructions;
    }
    
    public void UpdateImage(byte[]? imageData, string? imageContentType, string? imageFileName)
    {
        ImageData = imageData;
        ImageContentType = imageContentType;
        ImageFileName = imageFileName;
    }
    
    public void RemoveImage()
    {
        ImageData = null;
        ImageContentType = null;
        ImageFileName = null;
    }
    
    public void UpdateNutritionalInfo(double totalCalories, double totalProtein, double totalFat, double totalCarbohydrates)
    {
        TotalCalories = totalCalories;
        TotalProtein = totalProtein;
        TotalFat = totalFat;
        TotalCarbohydrates = totalCarbohydrates;
    }

    public void SetPublic(bool isPublic)
    {
        IsPublic = isPublic;
    }

    public void SetIngredients(List<RecipeIngredient> ingredients)
    {
        Ingredients = ingredients;
    }
} 