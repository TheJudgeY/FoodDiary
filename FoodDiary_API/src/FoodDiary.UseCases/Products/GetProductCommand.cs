using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Products;

public record GetProductCommand : IRequest<Result<GetProductResponse>>
{
    public Guid ProductId { get; init; }
}

public record GetProductResponse
{
    public Guid ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public double CaloriesPer100g { get; init; }
    public string? Description { get; init; }
} 