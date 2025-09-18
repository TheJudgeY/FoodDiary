using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.Web.Recipes;

[Authorize]
public class GetRecipeEndpoint : EndpointWithoutRequest<GetRecipeResponse>
{
    private readonly IMediator _mediator;

    public GetRecipeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/recipes/{id}");
        Summary(s =>
        {
            s.Summary = "Get a recipe by ID";
            s.Description = "Retrieves a recipe with all its ingredients. User can only access their own recipes or public recipes.";
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

        var command = new GetRecipeCommand
        {
            RecipeId = recipeId,
            UserId = userId
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
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