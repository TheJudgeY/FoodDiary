using MediatR;
using Ardalis.Result;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes;

public record UpdateRecipeCommand : IRequest<Result<UpdateRecipeResponse>>
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public RecipeCategory? Category { get; init; }
    public int? Servings { get; init; }
    public int? PreparationTimeMinutes { get; init; }
    public int? CookingTimeMinutes { get; init; }
    public string? Instructions { get; init; }
    public bool? IsPublic { get; init; }
    public bool? IsFavorite { get; init; }
    public List<UpdateRecipeIngredientRequest>? Ingredients { get; init; }
}

public record UpdateRecipeIngredientRequest
{
    public Guid? IngredientId { get; init; }
    public Guid? ProductId { get; init; }
    public double? QuantityGrams { get; init; }
    public string? Notes { get; init; }
    public string? CustomIngredientName { get; init; }
    public double? CustomCaloriesPer100g { get; init; }
    public double? CustomProteinPer100g { get; init; }
    public double? CustomFatPer100g { get; init; }
    public double? CustomCarbohydratesPer100g { get; init; }
}

public record UpdateRecipeResponse
{
    public Guid RecipeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public RecipeCategory Category { get; init; }
    public int Servings { get; init; }
    public int PreparationTimeMinutes { get; init; }
    public int CookingTimeMinutes { get; init; }
    public string Instructions { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public bool IsFavorite { get; init; }
    public double TotalCalories { get; init; }
    public double TotalProtein { get; init; }
    public double TotalFat { get; init; }
    public double TotalCarbohydrates { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<UpdateRecipeIngredientResponse> Ingredients { get; init; } = new List<UpdateRecipeIngredientResponse>();
}

public record UpdateRecipeIngredientResponse
{
    public Guid IngredientId { get; init; }
    public Guid? ProductId { get; init; }
    public double QuantityGrams { get; init; }
    public string? Notes { get; init; }
    public string? CustomIngredientName { get; init; }
}
