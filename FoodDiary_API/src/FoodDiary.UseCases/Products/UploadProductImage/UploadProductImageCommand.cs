using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Products;

public record UploadProductImageCommand : IRequest<Result<UploadProductImageResponse>>
{
    public Guid ProductId { get; init; }
    public byte[] ImageData { get; init; } = Array.Empty<byte>();
    public string ContentType { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
}

public record UploadProductImageResponse
{
    public Guid ProductId { get; init; }
    public string? ImageFileName { get; init; }
    public string? ImageContentType { get; init; }
    public long ImageSizeInBytes { get; init; }
} 