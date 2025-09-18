using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.FoodEntries.List;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.Web.FoodEntries;

[Authorize]
public class ListFoodEntriesEndpoint : EndpointWithoutRequest<ListFoodEntriesResponse>
{
    private readonly IMediator _mediator;

    public ListFoodEntriesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/food-entries");
        Summary(s =>
        {
            s.Summary = "List food entries";
            s.Description = "Retrieves a paginated list of food entries with optional filtering by date and meal type, including daily nutrition summary.";
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

        var page = Query<int?>("page", isRequired: false);
        var pageSize = Query<int?>("pageSize", isRequired: false);
        var dateStr = Query<string>("date", isRequired: false);
        var mealTypeStr = Query<string>("mealType", isRequired: false);

        var command = ListFoodEntriesCommand.FromQueryParameters(userId, dateStr, mealTypeStr, page, pageSize);

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

 