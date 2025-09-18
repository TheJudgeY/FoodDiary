using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Recipes;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Web.Recipes;

[Authorize]
public class ListRecipesEndpoint : EndpointWithoutRequest<ListRecipesResponse>
{
    private readonly IMediator _mediator;

    public ListRecipesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/recipes");
        Summary(s =>
        {
            s.Summary = "List recipes";
            s.Description = "Retrieves a paginated list of recipes for the authenticated user, with optional filtering and sorting.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var page = Query<int?>("page", isRequired: false) ?? 1;
        var pageSize = Query<int?>("pageSize", isRequired: false) ?? 20;
        var searchTerm = Query<string>("searchTerm", isRequired: false);
        var category = Query<string>("category", isRequired: false);
        var sortBy = Query<string>("sortBy", isRequired: false) ?? "Name";
        var includePublic = Query<bool?>("includePublic", isRequired: false) ?? true;
        var includeFavorites = Query<bool?>("includeFavorites", isRequired: false) ?? false;

        var command = new ListRecipesCommand
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm?.Trim(),
            Category = !string.IsNullOrEmpty(category) ? 
                (int.TryParse(category, out var cat) ? (RecipeCategory)cat : Enum.Parse<RecipeCategory>(category)) : 
                null,
            SortBy = sortBy.Trim(),
            IncludePublic = includePublic,
            IncludeFavorites = includeFavorites
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }
} 