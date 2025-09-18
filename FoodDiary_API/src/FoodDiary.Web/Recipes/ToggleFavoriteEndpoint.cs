using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Recipes.ToggleFavorite;

namespace FoodDiary.Web.Recipes;

[Authorize]
public class ToggleFavoriteEndpoint : Endpoint<EmptyRequest, ToggleFavoriteResponse>
{
    private readonly IMediator _mediator;

    public ToggleFavoriteEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/recipes/{id}/toggle-favorite");
        Summary(s =>
        {
            s.Summary = "Toggle recipe favorite status";
            s.Description = "Adds or removes a recipe from the current user's favorites. Users can favorite their own recipes or public recipes from other users.";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var idParam = Route<string>("id");
        if (string.IsNullOrEmpty(idParam) || !Guid.TryParse(idParam, out var recipeId))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = new ToggleFavoriteCommand
        {
            RecipeId = recipeId,
            UserId = userId
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
        }
        else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
        }
        else if (result.Status == Ardalis.Result.ResultStatus.Forbidden)
        {
            await SendAsync(new ToggleFavoriteResponse
            {
                RecipeId = command.RecipeId,
                IsFavorite = true,
                Message = result.Errors.FirstOrDefault() ?? "Operation forbidden"
            }, 403, ct);
        }
        else
        {
            await SendErrorsAsync(400, ct);
        }
    }
}


