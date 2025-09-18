using MediatR;
using Ardalis.Result;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Recipes;

public record ListRecipesCommand : IRequest<Result<ListRecipesResponse>>
{
    public Guid UserId { get; init; }
    public bool IncludePublic { get; init; } = true;
    public bool IncludeFavorites { get; init; } = false;
    public RecipeCategory? Category { get; init; }
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; } = "Name";
    public bool SortDescending { get; init; } = false;
}

public record ListRecipesResponse
{
    public List<ListRecipeItemResponse> Recipes { get; init; } = new List<ListRecipeItemResponse>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

public record ListRecipeItemResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public RecipeCategory Category { get; init; }
    public string CategoryDisplayName { get; init; } = string.Empty;
    public int Servings { get; init; }
    public int PreparationTimeMinutes { get; init; }
    public int CookingTimeMinutes { get; init; }

    public string? ImageFileName { get; init; }
    public string? ImageContentType { get; init; }
    public bool HasImage { get; init; }

    public bool IsPublic { get; init; }
    public bool IsFavorite { get; init; }
    public bool IsCreator { get; init; }
    public bool IsContributor { get; init; }
    public double TotalCalories { get; init; }
    public double TotalProtein { get; init; }
    public double TotalFat { get; init; }
    public double TotalCarbohydrates { get; init; }
    
    public double CaloriesPerServing { get; init; }
    public double ProteinPerServing { get; init; }
    public double FatPerServing { get; init; }
    public double CarbohydratesPerServing { get; init; }
    
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int IngredientCount { get; init; }
}
