using MediatR;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Auth;

public record RegisterUserCommand : IRequest<Result<RegisterUserResponse>>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
}

public record RegisterUserResponse
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public bool EmailConfirmationRequired { get; init; } = true;
    public string Message { get; init; } = string.Empty;

} 