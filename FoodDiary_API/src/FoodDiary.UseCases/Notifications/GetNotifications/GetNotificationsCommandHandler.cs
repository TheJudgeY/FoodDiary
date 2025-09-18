using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.Notifications.GetNotifications;
using FoodDiary.UseCases.Notifications;
using System.Collections.Generic;
using System.Linq;

namespace FoodDiary.UseCases.Notifications.GetNotifications;

public class GetNotificationsCommandHandler : IRequestHandler<GetNotificationsCommand, Result<GetNotificationsResponse>>
{
    private readonly FoodDiary.Core.Interfaces.INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly FoodDiary.UseCases.Notifications.INotificationService _notificationBusinessService;

    public GetNotificationsCommandHandler(FoodDiary.Core.Interfaces.INotificationService notificationService, IMapper mapper, FoodDiary.UseCases.Notifications.INotificationService notificationBusinessService)
    {
        _notificationService = notificationService;
        _mapper = mapper;
        _notificationBusinessService = notificationBusinessService;
    }

    public async Task<Result<GetNotificationsResponse>> Handle(GetNotificationsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(
                request.UserId, 
                request.Page, 
                request.PageSize, 
                request.IncludeRead);

            var notificationDtos = _mapper.Map<List<FoodDiary.UseCases.Notifications.NotificationDTO>>(notifications);
            
            foreach (var dto in notificationDtos)
            {
                var notification = notifications.FirstOrDefault(n => n.Id == dto.Id);
                if (notification != null)
                {
                    dto.IsUnread = _notificationBusinessService.IsUnread(notification);
                    dto.IsRead = _notificationBusinessService.IsRead(notification);
                    dto.Age = _notificationBusinessService.GetAge(notification);
                    dto.IsRecent = _notificationBusinessService.IsRecent(notification);
                    dto.PriorityColor = _notificationBusinessService.GetPriorityColor(notification);
                    dto.TypeIcon = _notificationBusinessService.GetTypeIcon(notification);
                }
            }

            var totalCount = await _notificationService.GetUnreadNotificationCountAsync(request.UserId);
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

            var response = new GetNotificationsResponse
            {
                Notifications = notificationDtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                Message = $"Retrieved {notificationDtos.Count} notifications"
            };

            return Result.Success(response);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error retrieving notifications: {ex.Message}");
        }
    }
} 