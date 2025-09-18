using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Recipes;

public record UploadRecipeImageCommand : IRequest<Result<UploadRecipeImageResponse>>
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }
    public byte[] ImageData { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
}

public record UploadRecipeImageResponse
{
    public Guid RecipeId { get; init; }
    public string? ImageFileName { get; init; }
    public string? ImageContentType { get; init; }
    public long ImageSizeInBytes { get; init; }
} 