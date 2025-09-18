using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Auth;

public record LoginUserCommand : IRequest<Result<LoginUserResponse>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public record LoginUserResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
} 