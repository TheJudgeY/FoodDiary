using FastEndpoints;
using MediatR;
using FoodDiary.UseCases.Auth;
using FoodDiary.Web.Validation;

namespace FoodDiary.Web.Auth;

public class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IMediator _mediator;

    public LoginEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Login user";
            s.Description = "Authenticates user credentials and returns a JWT token";
            s.ExampleRequest = new LoginRequest
            {
                Email = "user@example.com",
                Password = "password123"
            };
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        if (!await FastEndpointsValidationService.ValidateRequestAsync(req, HttpContext))
        {
            return;
        }

        var command = new LoginUserCommand
        {
            Email = req.Email.Trim(),
            Password = req.Password
        };

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        var response = new LoginResponse
        {
            UserId = result.Value.UserId,
            Email = result.Value.Email,
            Name = result.Value.Name,
            Token = result.Value.Token
        };

        await SendAsync(response, 200, ct);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
} 