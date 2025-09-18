using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Recipes;

public record DeleteRecipeCommand : IRequest<Result>
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }
}
