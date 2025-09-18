using FastEndpoints;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.UseCases.FoodEntries.AddRecipeToDiary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FoodDiary.Web.FoodEntries;

[Authorize]
public class AddRecipeToDiaryEndpoint : Endpoint<AddRecipeToDiaryRequest, AddRecipeToDiaryResponse>
{
    private readonly IMediator _mediator;

    public AddRecipeToDiaryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/food-entries/add-recipe");
        Summary(s =>
        {
            s.Summary = "Add a recipe to the food diary";
            s.Description = "Adds a recipe to the food diary by creating individual food entries for each ingredient.";
        });
    }

    public override async Task HandleAsync(AddRecipeToDiaryRequest request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, cancellationToken);
            return;
        }

        var command = new AddRecipeToDiaryCommand
        {
            RecipeId = request.RecipeId,
            UserId = userId,
            MealType = request.MealType,
            ConsumedAt = request.ConsumedAt,
            Notes = request.Notes,
            ServingsConsumed = request.ServingsConsumed
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 201, cancellationToken);
        }
        else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
        {
            await SendNotFoundAsync(cancellationToken);
        }
        else if (result.Status == Ardalis.Result.ResultStatus.Forbidden)
        {
            await SendErrorsAsync(403, cancellationToken);
        }
        else
        {
            await SendErrorsAsync(500, cancellationToken);
        }
    }
}

public record AddRecipeToDiaryRequest
{
    public Guid RecipeId { get; init; }
    public MealType MealType { get; init; }
    public DateTime ConsumedAt { get; init; }
    public string? Notes { get; init; }
    public int ServingsConsumed { get; init; } = 1;
} 