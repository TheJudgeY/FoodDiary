using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.Web.Recipes;

[Authorize]
public class DeleteRecipeEndpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public DeleteRecipeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/recipes/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a recipe";
            s.Description = "Deletes a recipe and all its ingredients. User can only delete their own recipes.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
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

        var command = new DeleteRecipeCommand
        {
            RecipeId = recipeId,
            UserId = userId
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendOkAsync(ct);
        }
        else if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
        }
        else if (result.Status == ResultStatus.Forbidden)
        {
            await SendForbiddenAsync(ct);
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }
} 