using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.FoodEntries.Create;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.Web.FoodEntries;

[Authorize]
public class CreateFoodEntryEndpoint : Endpoint<CreateFoodEntryCommand, CreateFoodEntryResponse>
{
    private readonly IMediator _mediator;

    public CreateFoodEntryEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/food-entries");
        Summary(s =>
        {
            s.Summary = "Create a new food entry";
            s.Description = "Creates a new food entry for the authenticated user with calculated nutritional values based on product and weight.";
        });
    }

    public override async Task HandleAsync(CreateFoodEntryCommand req, CancellationToken ct)
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
            await SendAsync(result.Value, 201, ct);
        }
        else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
        }
        else
        {
            await SendErrorsAsync(500, ct);
        }
    }
} 