using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.UseCases.Notifications;
using FoodDiary.UseCases.Notifications.GetNotifications;
using NSubstitute;
using Xunit;
using Ardalis.Result;

namespace FoodDiary.UnitTests.UseCases.Notifications;

public class GetNotificationsCommandHandlerTests
{
    private readonly FoodDiary.Core.Interfaces.INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly FoodDiary.UseCases.Notifications.INotificationService _notificationBusinessService;
    private readonly GetNotificationsCommandHandler _handler;

    public GetNotificationsCommandHandlerTests()
    {
        _notificationService = Substitute.For<FoodDiary.Core.Interfaces.INotificationService>();
        _mapper = Substitute.For<IMapper>();
        _notificationBusinessService = Substitute.For<FoodDiary.UseCases.Notifications.INotificationService>();
        _handler = new GetNotificationsCommandHandler(_notificationService, _mapper, _notificationBusinessService);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsNotifications()
    {
        var userId = Guid.NewGuid();
        var command = new GetNotificationsCommand
        {
            UserId = userId,
            Page = 1,
            PageSize = 10,
            IncludeRead = true
        };

        var notifications = new List<Notification>
        {
            new Notification(userId, "Test 1", "Test message 1", NotificationType.WaterReminder),
            new Notification(userId, "Test 2", "Test message 2", NotificationType.MealReminder)
        };

        var expectedDtos = new List<NotificationDTO>
        {
            new() { Id = notifications[0].Id, UserId = userId, Title = "Test 1", Type = "WaterReminder" },
            new() { Id = notifications[1].Id, UserId = userId, Title = "Test 2", Type = "MealReminder" }
        };

        _notificationService.GetUserNotificationsAsync(userId, 1, 10, true)
            .Returns(notifications);

        _notificationService.GetUnreadNotificationCountAsync(userId)
            .Returns(2);

        _mapper.Map<List<NotificationDTO>>(notifications)
            .Returns(expectedDtos);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Notifications.Count);
        Assert.Equal(expectedDtos[0].Id, result.Value.Notifications[0].Id);
        Assert.Equal(expectedDtos[1].Id, result.Value.Notifications[1].Id);
        
        await _notificationService.Received(1).GetUserNotificationsAsync(userId, 1, 10, true);
        _mapper.Received(1).Map<List<NotificationDTO>>(notifications);
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        var userId = Guid.NewGuid();
        var command = new GetNotificationsCommand
        {
            UserId = userId,
            Page = 2,
            PageSize = 5,
            IncludeRead = false
        };

        var notifications = new List<Notification>
        {
            new Notification(userId, "Test 1", "Test message 1", NotificationType.WaterReminder),
            new Notification(userId, "Test 2", "Test message 2", NotificationType.MealReminder)
        };

        var expectedDtos = new List<NotificationDTO>
        {
            new() { Id = notifications[0].Id, UserId = userId, Title = "Test 1", Type = "WaterReminder" },
            new() { Id = notifications[1].Id, UserId = userId, Title = "Test 2", Type = "MealReminder" }
        };

        _notificationService.GetUserNotificationsAsync(userId, 2, 5, false)
            .Returns(notifications);

        _notificationService.GetUnreadNotificationCountAsync(userId)
            .Returns(2);

        _mapper.Map<List<NotificationDTO>>(notifications)
            .Returns(expectedDtos);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Notifications.Count);
        
        await _notificationService.Received(1).GetUserNotificationsAsync(userId, 2, 5, false);
    }

    [Fact]
    public async Task Handle_WithNoNotifications_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var command = new GetNotificationsCommand
        {
            UserId = userId,
            Page = 1,
            PageSize = 10,
            IncludeRead = true
        };

        var emptyNotifications = new List<Notification>();
        var emptyDtos = new List<NotificationDTO>();

        _notificationService.GetUserNotificationsAsync(userId, 1, 10, true)
            .Returns(emptyNotifications);

        _notificationService.GetUnreadNotificationCountAsync(userId)
            .Returns(0);

        _mapper.Map<List<NotificationDTO>>(emptyNotifications)
            .Returns(emptyDtos);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Notifications);
        
        await _notificationService.Received(1).GetUserNotificationsAsync(userId, 1, 10, true);
        _mapper.Received(1).Map<List<NotificationDTO>>(emptyNotifications);
    }

    [Fact]
    public async Task Handle_WithDefaultValues_UsesDefaultPagination()
    {
        var userId = Guid.NewGuid();
        var command = new GetNotificationsCommand
        {
            UserId = userId
        };

        var notifications = new List<Notification>
        {
            new Notification(userId, "Test", "Test message", NotificationType.WaterReminder)
        };

        var expectedDtos = new List<NotificationDTO>
        {
            new() { Id = notifications[0].Id, UserId = userId, Title = "Test", Type = "WaterReminder" }
        };

        _notificationService.GetUserNotificationsAsync(userId, 1, 20, false)
            .Returns(notifications);

        _notificationService.GetUnreadNotificationCountAsync(userId)
            .Returns(1);

        _mapper.Map<List<NotificationDTO>>(notifications)
            .Returns(expectedDtos);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Single(result.Value.Notifications);
        
        await _notificationService.Received(1).GetUserNotificationsAsync(userId, 1, 20, false);
    }

    [Fact]
    public async Task Handle_WithCancellationRequested_ReturnsCancelledResult()
    {
        var userId = Guid.NewGuid();
        var command = new GetNotificationsCommand
        {
            UserId = userId,
            Page = 1,
            PageSize = 10,
            IncludeRead = true
        };

        var cancellationToken = new CancellationToken(true);

        var result = await _handler.Handle(command, cancellationToken);

        Assert.False(result.IsSuccess);
        Assert.Contains("error", result.Errors.First().ToLower());
    }

    [Fact]
    public async Task Handle_WithServiceException_PropagatesException()
    {
        var userId = Guid.NewGuid();
        var command = new GetNotificationsCommand
        {
            UserId = userId,
            Page = 1,
            PageSize = 10,
            IncludeRead = true
        };

        var expectedException = new InvalidOperationException("Service error");
        _notificationService.GetUserNotificationsAsync(userId, 1, 10, true)
            .Returns(Task.FromException<List<Notification>>(expectedException));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Error retrieving notifications", result.Errors.First());
    }

    [Fact]
    public async Task Handle_WithMapperException_PropagatesException()
    {
        var userId = Guid.NewGuid();
        var command = new GetNotificationsCommand
        {
            UserId = userId,
            Page = 1,
            PageSize = 10,
            IncludeRead = true
        };

        var notifications = new List<Notification>
        {
            new Notification(userId, "Test", "Test message", NotificationType.WaterReminder)
        };

        var expectedException = new AutoMapperMappingException("Mapping error");
        
        _notificationService.GetUserNotificationsAsync(userId, 1, 10, true)
            .Returns(notifications);

        _notificationService.GetUnreadNotificationCountAsync(userId)
            .Returns(1);

        _mapper.Map<List<NotificationDTO>>(notifications)
            .Returns(x => throw expectedException);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Error retrieving notifications", result.Errors.First());
    }
} 