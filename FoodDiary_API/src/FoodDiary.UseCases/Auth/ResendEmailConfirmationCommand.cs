using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Auth;

public record ResendEmailConfirmationCommand : IRequest<Result<ResendEmailConfirmationResponse>>
{
    public string Email { get; init; } = string.Empty;
}

public record ResendEmailConfirmationResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
} 