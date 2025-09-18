using System;
using System.Collections.Generic;
using MediatR;
using Ardalis.Result;
using FoodDiary.UseCases.Notifications;

namespace FoodDiary.UseCases.Notifications.GetNotifications;

public record GetNotificationsCommand : IRequest<Result<GetNotificationsResponse>>
{
    public Guid UserId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public bool IncludeRead { get; init; } = false;
}

public record GetNotificationsResponse
{
    public List<NotificationDTO> Notifications { get; init; } = new List<NotificationDTO>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public string Message { get; init; } = string.Empty;
} 