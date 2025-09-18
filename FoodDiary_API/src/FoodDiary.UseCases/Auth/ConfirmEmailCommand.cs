using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Auth;

public record ConfirmEmailCommand : IRequest<Result<ConfirmEmailResponse>>
{
    public string Email { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}

public record ConfirmEmailResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
} 