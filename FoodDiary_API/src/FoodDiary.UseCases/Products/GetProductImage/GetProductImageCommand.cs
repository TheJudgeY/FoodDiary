using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Products;

public record GetProductImageCommand : IRequest<Result<GetProductImageResponse>>
{
    public Guid ProductId { get; init; }
}

public record GetProductImageResponse
{
    public Guid ProductId { get; init; }
    public string ImageDataUrl { get; init; } = string.Empty;
    public string? ImageContentType { get; init; }
    public string? ImageFileName { get; init; }
    public long ImageSizeInBytes { get; init; }
} 