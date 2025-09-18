using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Recipes;

public record GetRecipeImageCommand : IRequest<Result<GetRecipeImageResponse>>
{
    public Guid RecipeId { get; init; }
}

public record GetRecipeImageResponse
{
    public Guid RecipeId { get; init; }
    public string ImageDataUrl { get; init; } = string.Empty;
    public string? ImageContentType { get; init; }
    public string? ImageFileName { get; init; }
    public long ImageSizeInBytes { get; init; }
} 