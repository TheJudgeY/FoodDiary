using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.FoodEntries.Delete;

namespace FoodDiary.Web.FoodEntries;

[Authorize]
public class DeleteFoodEntryEndpoint : EndpointWithoutRequest<DeleteFoodEntryResponse>
{
    private readonly IMediator _mediator;

    public DeleteFoodEntryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/api/food-entries/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a food entry";
            s.Description = "Deletes a food entry. User can only delete their own food entries.";
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

        var id = Route<Guid>("id");
        
        var command = new DeleteFoodEntryCommand
        {
            Id = id,
            UserId = userId
        };

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

 