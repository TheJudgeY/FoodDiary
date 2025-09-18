using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Products;

public record DeleteProductCommand : IRequest<Result<DeleteProductResponse>>
{
    public Guid ProductId { get; init; }
}

public record DeleteProductResponse
{
    public bool CanDelete { get; init; }
    public string Message { get; init; } = string.Empty;
    public int FoodEntryCount { get; init; }
    public int RecipeIngredientCount { get; init; }
    public List<string> References { get; init; } = new();
} 