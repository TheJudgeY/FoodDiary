using Xunit;
using MediatR;
using NSubstitute;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users.UpdateBodyMetrics;
using FoodDiary.UseCases.Users;
using AutoMapper;

namespace FoodDiary.UnitTests.UseCases.Users;

public class UpdateBodyMetricsCommandHandlerTests
{
    private readonly IRepository<User> _userRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly UpdateBodyMetricsCommandHandler _handler;

    public UpdateBodyMetricsCommandHandlerTests()
    {
        _userRepository = Substitute.For<IRepository<User>>();
        _mapper = Substitute.For<IMapper>();
        _userService = Substitute.For<IUserService>();
        _handler = new UpdateBodyMetricsCommandHandler(_userRepository, _mapper, _userService);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hashedPassword", "Test User");
        var command = new UpdateBodyMetricsCommand
        {
            UserId = userId,
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        _userRepository.GetByIdAsync(userId).Returns(Result<User>.Success(user));
        _userRepository.UpdateAsync(user).Returns(Result.Success());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal(175, result.Value.HeightCm);
        Assert.Equal(70, result.Value.WeightKg);
        Assert.Equal(30, result.Value.Age);
        Assert.Equal(Gender.Male, result.Value.Gender);
        Assert.Equal(ActivityLevel.ModeratelyActive, result.Value.ActivityLevel);
        Assert.Equal("Body metrics updated successfully", result.Value.Message);
    }

    [Fact]
    public async Task Handle_UserNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var command = new UpdateBodyMetricsCommand
        {
            UserId = userId,
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        _userRepository.GetByIdAsync(userId).Returns(Result<User>.NotFound());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(Ardalis.Result.ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task Handle_UpdateFails_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hashedPassword", "Test User");
        var command = new UpdateBodyMetricsCommand
        {
            UserId = userId,
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        _userRepository.GetByIdAsync(userId).Returns(Result<User>.Success(user));
        _userRepository.UpdateAsync(user).Returns(Result.Error("Update failed"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(Ardalis.Result.ResultStatus.Error, result.Status);
        Assert.Contains("Failed to update user body metrics", result.Errors.First());
    }

    [Fact]
    public async Task Handle_InvalidBodyMetrics_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "hashedPassword", "Test User");
        var command = new UpdateBodyMetricsCommand
        {
            UserId = userId,
            HeightCm = -10,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        _userRepository.GetByIdAsync(userId).Returns(Result<User>.Success(user));
        _userService.When(x => x.ValidateBodyMetrics(-10, 70, 30)).Do(x => { throw new ArgumentException("Height must be greater than 0"); });

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(Ardalis.Result.ResultStatus.Error, result.Status);
        Assert.Contains("Invalid body metrics", result.Errors.First());
        Assert.Contains("Height must be greater than 0", result.Errors.First());
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var command = new UpdateBodyMetricsCommand
        {
            UserId = userId,
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        _userRepository.GetByIdAsync(userId).Returns(Task.FromException<Result<User>>(new Exception("Database error")));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(Ardalis.Result.ResultStatus.Error, result.Status);
        Assert.Contains("Error updating body metrics", result.Errors.First());
    }
} 