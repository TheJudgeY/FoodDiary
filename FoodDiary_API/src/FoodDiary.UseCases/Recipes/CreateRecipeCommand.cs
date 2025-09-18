using MediatR;
using Ardalis.Result;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes;

public record CreateRecipeCommand : IRequest<Result<CreateRecipeResponse>>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public RecipeCategory Category { get; init; }
    public int Servings { get; init; }
    public int PreparationTimeMinutes { get; init; }
    public int CookingTimeMinutes { get; init; }
    public string Instructions { get; init; } = string.Empty;
    public bool IsPublic { get; init; } = false;
    public byte[]? ImageData { get; init; }
    public string? ImageContentType { get; init; }
    public string? ImageFileName { get; init; }
    public List<CreateRecipeIngredientRequest> Ingredients { get; init; } = new List<CreateRecipeIngredientRequest>();
}

public record CreateRecipeIngredientRequest
{
    public Guid? ProductId { get; init; }
    public double QuantityGrams { get; init; }
    public string? Notes { get; init; }
    public string? CustomIngredientName { get; init; }
    public double? CustomCaloriesPer100g { get; init; }
    public double? CustomProteinPer100g { get; init; }
    public double? CustomFatPer100g { get; init; }
    public double? CustomCarbohydratesPer100g { get; init; }
}

public record CreateRecipeResponse
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
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<CreateRecipeIngredientResponse> Ingredients { get; init; } = new List<CreateRecipeIngredientResponse>();
}

public record CreateRecipeIngredientResponse
{
    public Guid IngredientId { get; init; }
    public Guid? ProductId { get; init; }
    public double QuantityGrams { get; init; }
    public string? Notes { get; init; }
    public string? CustomIngredientName { get; init; }
}
