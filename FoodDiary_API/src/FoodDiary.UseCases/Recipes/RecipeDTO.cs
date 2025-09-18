using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes;

public class RecipeDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public RecipeCategory Category { get; set; }
    public string CategoryDisplayName { get; set; } = string.Empty;
    public int Servings { get; set; }
    public int PreparationTimeMinutes { get; set; }
    public int CookingTimeMinutes { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public string? ImageFileName { get; set; }
    public string? ImageContentType { get; set; }
    public bool HasImage { get; set; }
    public bool IsPublic { get; set; }
    public bool IsFavorite { get; set; }
    public double TotalCalories { get; set; }
    public double TotalProtein { get; set; }
    public double TotalFat { get; set; }
    public double TotalCarbohydrates { get; set; }
    
    public double CaloriesPerServing { get; set; }
    public double ProteinPerServing { get; set; }
    public double FatPerServing { get; set; }
    public double CarbohydratesPerServing { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<RecipeIngredientDTO> Ingredients { get; set; } = new List<RecipeIngredientDTO>();
}

public class RecipeIngredientDTO
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public Guid? ProductId { get; set; }
    public double QuantityGrams { get; set; }
    public string? Notes { get; set; }
    public string? CustomIngredientName { get; set; }
    public double? CustomCaloriesPer100g { get; set; }
    public double? CustomProteinPer100g { get; set; }
    public double? CustomFatPer100g { get; set; }
    public double? CustomCarbohydratesPer100g { get; set; }
    public ProductDTO? Product { get; set; }
}

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double CaloriesPer100g { get; set; }
    public double ProteinsPer100g { get; set; }
    public double FatsPer100g { get; set; }
    public double CarbohydratesPer100g { get; set; }
    public string? Description { get; set; }
} 