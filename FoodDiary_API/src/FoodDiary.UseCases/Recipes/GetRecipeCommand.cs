using MediatR;
using Ardalis.Result;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes;

public record GetRecipeCommand : IRequest<Result<GetRecipeResponse>>
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }
}

public record GetRecipeResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public RecipeCategory Category { get; init; }
    public int Servings { get; init; }
    public int PreparationTimeMinutes { get; init; }
    public int CookingTimeMinutes { get; init; }
    public string Instructions { get; init; } = string.Empty;

    public bool IsPublic { get; init; }
    public bool IsFavorite { get; init; }
    public bool IsCreator { get; init; }
    public bool IsContributor { get; init; }
    public double TotalCalories { get; init; }
    public double TotalProtein { get; init; }
    public double TotalFat { get; init; }
    public double TotalCarbohydrates { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<GetRecipeIngredientResponse> Ingredients { get; init; } = new List<GetRecipeIngredientResponse>();
}

public record GetRecipeIngredientResponse
{
    public Guid Id { get; init; }
    public Guid? ProductId { get; init; }
    public double QuantityGrams { get; init; }
    public string? Notes { get; init; }
    public string? CustomIngredientName { get; init; }
    public double? CustomCaloriesPer100g { get; init; }
    public double? CustomProteinPer100g { get; init; }
    public double? CustomFatPer100g { get; init; }
    public double? CustomCarbohydratesPer100g { get; init; }
    public GetRecipeProductResponse? Product { get; init; }
}

public record GetRecipeProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public double CaloriesPer100g { get; init; }
    public double ProteinsPer100g { get; init; }
    public double FatsPer100g { get; init; }
    public double CarbohydratesPer100g { get; init; }
    public string? Description { get; init; }
}
