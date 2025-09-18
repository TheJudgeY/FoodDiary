using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public record ListProductsCommand : IRequest<Result<ListProductsResponse>>
{
    public string? SearchTerm { get; init; }
    public ProductCategory? Category { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record ListProductsResponse
{
    public List<ProductSummaryDto> Products { get; init; } = new();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

public record ProductSummaryDto
{
    public Guid Id { get; init; }
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