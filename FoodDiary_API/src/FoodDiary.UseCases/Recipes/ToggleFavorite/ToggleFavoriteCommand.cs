using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Recipes.ToggleFavorite;

public record ToggleFavoriteCommand : IRequest<Result<ToggleFavoriteResponse>>
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }
}

public record ToggleFavoriteResponse
{
    public Guid RecipeId { get; init; }
    public bool IsFavorite { get; init; }
    public string Message { get; init; } = string.Empty;
}
