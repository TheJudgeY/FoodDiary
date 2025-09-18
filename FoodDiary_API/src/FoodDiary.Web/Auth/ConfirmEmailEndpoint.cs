using FastEndpoints;
using MediatR;
using FoodDiary.UseCases.Auth;
using Microsoft.AspNetCore.Authorization;

namespace FoodDiary.Web.Auth;

[AllowAnonymous]
public class ConfirmEmailEndpoint : EndpointWithoutRequest<FoodDiary.UseCases.Auth.ConfirmEmailResponse>
{
    private readonly IMediator _mediator;

    public ConfirmEmailEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/auth/confirm-email");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Confirm email address";
            s.Description = "Confirms a user's email address using the provided token";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var email = Query<string>("email", isRequired: true) ?? string.Empty;
        var token = Query<string>("token", isRequired: true) ?? string.Empty;

        var command = new ConfirmEmailCommand
        {
            Email = email,
            Token = token
        };

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Errors.FirstOrDefault() ?? "Unknown error occurred";
            await SendAsync(new FoodDiary.UseCases.Auth.ConfirmEmailResponse
            {
                Success = false,
                Message = errorMessage
            }, 400, ct);
            return;
        }

        await SendAsync(result.Value, 200, ct);
    }
} 