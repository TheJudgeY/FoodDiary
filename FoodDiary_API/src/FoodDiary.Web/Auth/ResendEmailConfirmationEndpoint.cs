using FastEndpoints;
using MediatR;
using FoodDiary.UseCases.Auth;

namespace FoodDiary.Web.Auth;

public class ResendEmailConfirmationEndpoint : Endpoint<ResendEmailConfirmationRequest, FoodDiary.UseCases.Auth.ResendEmailConfirmationResponse>
{
    private readonly IMediator _mediator;

    public ResendEmailConfirmationEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/resend-email-confirmation");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Resend email confirmation";
            s.Description = "Resends an email confirmation link to the specified email address";
            s.ExampleRequest = new ResendEmailConfirmationRequest
            {
                Email = "user@example.com"
            };
        });
    }

    public override async Task HandleAsync(ResendEmailConfirmationRequest req, CancellationToken ct)
    {
        var command = new ResendEmailConfirmationCommand
        {
            Email = req.Email
        };

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Errors.FirstOrDefault() ?? "Unknown error occurred";
            await SendAsync(new FoodDiary.UseCases.Auth.ResendEmailConfirmationResponse
            {
                Success = false,
                Message = errorMessage
            }, 400, ct);
            return;
        }

        await SendAsync(result.Value, 200, ct);
    }
}

public class ResendEmailConfirmationRequest
{
    public string Email { get; set; } = string.Empty;
} 