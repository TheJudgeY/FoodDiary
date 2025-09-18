using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public record UpdateProductCommand : IRequest<Result<UpdateProductResponse>>
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public double CaloriesPer100g { get; init; }
    public double ProteinsPer100g { get; init; }
    public double FatsPer100g { get; init; }
    public double CarbohydratesPer100g { get; init; }
    public string? Description { get; init; }
    public ProductCategory Category { get; init; }
    public byte[]? ImageData { get; init; }
    public string? ImageContentType { get; init; }
    public string? ImageFileName { get; init; }
}

public record UpdateProductResponse
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public double CaloriesPer100g { get; init; }
    public double ProteinsPer100g { get; init; }
    public double FatsPer100g { get; init; }
    public double CarbohydratesPer100g { get; init; }
    public string? Description { get; init; }
    public ProductCategory Category { get; init; }
    public string CategoryDisplayName { get; init; } = string.Empty;
    public string? ImageFileName { get; init; }
    public string? ImageContentType { get; init; }
    public bool HasImage { get; init; }
} 