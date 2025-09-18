using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.FoodEntries.Update;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.Web.FoodEntries;

[Authorize]
public class UpdateFoodEntryEndpoint : Endpoint<UpdateFoodEntryCommand, UpdateFoodEntryResponse>
{
    private readonly IMediator _mediator;

    public UpdateFoodEntryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/food-entries/{id}");
        Summary(s =>
        {
            s.Summary = "Update a food entry";
            s.Description = "Updates a food entry's details. User can only update their own food entries.";
        });
    }

    public override async Task HandleAsync(UpdateFoodEntryCommand req, CancellationToken ct)
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
            await SendErrorsAsync(400, ct);
        }
    }
}

 