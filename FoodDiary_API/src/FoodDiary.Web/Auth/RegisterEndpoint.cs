using FastEndpoints;
using MediatR;
using FoodDiary.UseCases.Auth;
using FoodDiary.Web.Validation;

namespace FoodDiary.Web.Auth;

public class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly IMediator _mediator;

    public RegisterEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/register");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register a new user";
            s.Description = "Creates a new user account and returns a JWT token";
            s.ExampleRequest = new RegisterRequest
            {
                Email = "user@example.com",
                Password = "password123",
                Name = "John Doe"
            };
        });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        if (!await FastEndpointsValidationService.ValidateRequestAsync(req, HttpContext))
        {
            return;
        }

        var command = new RegisterUserCommand
        {
            Email = req.Email.Trim(),
            Password = req.Password,
            Name = req.Name.Trim()
        };

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var response = new RegisterResponse
        {
            UserId = result.Value.UserId,
            Email = result.Value.Email,
            Name = result.Value.Name,
            Token = result.Value.Token,
            EmailConfirmationRequired = result.Value.EmailConfirmationRequired,
            Message = result.Value.Message
        };

        await SendAsync(response, 201, ct);
    }
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string EmailConfirmationToken { get; set; } = string.Empty;
    public bool EmailConfirmationRequired { get; set; } = true;
    public string Message { get; set; } = string.Empty;
} 