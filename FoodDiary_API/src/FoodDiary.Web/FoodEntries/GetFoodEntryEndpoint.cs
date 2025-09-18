using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.FoodEntries.Get;

namespace FoodDiary.Web.FoodEntries;

[Authorize]
public class GetFoodEntryEndpoint : Endpoint<GetFoodEntryCommand, GetFoodEntryResponse>
{
    private readonly IMediator _mediator;

    public GetFoodEntryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/food-entries/{id}");
        Summary(s =>
        {
            s.Summary = "Get a food entry by ID";
            s.Description = "Retrieves a specific food entry. User can only access their own food entries.";
        });
    }

    public override async Task HandleAsync(GetFoodEntryCommand req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var command = req with { UserId = userId };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
        }
        else
        {
            await SendErrorsAsync(404, ct);
        }
    }
} 