using Xunit;
using NSubstitute;
using AutoMapper;
using Ardalis.Result;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.FoodEntries.Update;
using FoodDiary.UseCases.FoodEntries;

namespace FoodDiary.UnitTests.UseCases.FoodEntries;

public class UpdateFoodEntryCommandHandlerTests
{
    private readonly IRepository<FoodEntry> _mockRepository;
    private readonly IMapper _mockMapper;
    private readonly UpdateFoodEntryCommandHandler _handler;

    public UpdateFoodEntryCommandHandlerTests()
    {
        _mockRepository = Substitute.For<IRepository<FoodEntry>>();
        _mockMapper = Substitute.For<IMapper>();
        _handler = new UpdateFoodEntryCommandHandler(_mockRepository, _mockMapper);
    }

    [Fact]
    public async Task Handle_WhenFoodEntryNotFound_ReturnsError()
    {
        var command = new UpdateFoodEntryCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            Notes = "Test notes"
        };

        _mockRepository.GetByIdAsync(command.Id, "Product")
            .Returns(Result<FoodEntry>.NotFound());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Food entry not found", result.Errors.First());
    }

    [Fact]
    public async Task Handle_WhenUserNotOwner_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var foodEntry = new FoodEntry(
            userId,
            Guid.NewGuid(),
            100.0,
            MealType.Breakfast,
            DateTime.UtcNow
        );

        var command = new UpdateFoodEntryCommand
        {
            Id = foodEntry.Id,
            UserId = differentUserId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            Notes = "Test notes"
        };

        _mockRepository.GetByIdAsync(command.Id, "Product")
            .Returns(Task.FromResult(Result<FoodEntry>.Success(foodEntry)));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Access denied", result.Errors.First());
    }

    [Fact]
    public async Task Handle_WhenValidRequest_UpdatesFoodEntrySuccessfully()
    {
        var userId = Guid.NewGuid();
        var originalConsumedAt = DateTime.UtcNow.AddHours(-2);
        var foodEntry = new FoodEntry(
            userId,
            Guid.NewGuid(),
            100.0,
            MealType.Breakfast,
            originalConsumedAt,
            "Original notes"
        );

        var command = new UpdateFoodEntryCommand
        {
            Id = foodEntry.Id,
            UserId = userId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            Notes = "Updated notes"
        };

        var foodEntryDto = new FoodEntryDTO
        {
            Id = foodEntry.Id,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            Notes = "Updated notes"
        };

        _mockRepository.GetByIdAsync(command.Id, "Product")
            .Returns(Result<FoodEntry>.Success(foodEntry));
        _mockRepository.UpdateAsync(Arg.Any<FoodEntry>())
            .Returns(Result.Success());
        _mockMapper.Map<FoodEntryDTO>(Arg.Any<FoodEntry>())
            .Returns(foodEntryDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Food entry updated successfully", result.Value.Message);
        Assert.Equal(200.0, result.Value.FoodEntry.WeightGrams);
        Assert.Equal(MealType.Lunch, result.Value.FoodEntry.MealType);
        Assert.Equal("Updated notes", result.Value.FoodEntry.Notes);
    }

    [Fact]
    public async Task Handle_WhenConsumedAtProvided_UpdatesConsumptionTime()
    {
        var userId = Guid.NewGuid();
        var originalConsumedAt = DateTime.UtcNow.AddHours(-2);
        var newConsumedAt = DateTime.UtcNow.AddHours(-1);
        var foodEntry = new FoodEntry(
            userId,
            Guid.NewGuid(),
            100.0,
            MealType.Breakfast,
            originalConsumedAt
        );

        var command = new UpdateFoodEntryCommand
        {
            Id = foodEntry.Id,
            UserId = userId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            ConsumedAt = newConsumedAt
        };

        var foodEntryDto = new FoodEntryDTO
        {
            Id = foodEntry.Id,
            WeightGrams = 200.0,
            MealType = MealType.Lunch
        };

        _mockRepository.GetByIdAsync(command.Id, "Product")
            .Returns(Result<FoodEntry>.Success(foodEntry));
        _mockRepository.UpdateAsync(Arg.Any<FoodEntry>())
            .Returns(Result.Success());
        _mockMapper.Map<FoodEntryDTO>(Arg.Any<FoodEntry>())
            .Returns(foodEntryDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        await _mockRepository.Received(1).UpdateAsync(Arg.Is<FoodEntry>(fe => 
            fe.ConsumedAt == DateTime.SpecifyKind(newConsumedAt, DateTimeKind.Utc)));
    }

    [Fact]
    public async Task Handle_WhenConsumedAtNotProvided_LeavesConsumptionTimeUntouched()
    {
        var userId = Guid.NewGuid();
        var originalConsumedAt = DateTime.UtcNow.AddHours(-2);
        var foodEntry = new FoodEntry(
            userId,
            Guid.NewGuid(),
            100.0,
            MealType.Breakfast,
            originalConsumedAt
        );

        var command = new UpdateFoodEntryCommand
        {
            Id = foodEntry.Id,
            UserId = userId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            ConsumedAt = null
        };

        var foodEntryDto = new FoodEntryDTO
        {
            Id = foodEntry.Id,
            WeightGrams = 200.0,
            MealType = MealType.Lunch
        };

        _mockRepository.GetByIdAsync(command.Id, "Product")
            .Returns(Result<FoodEntry>.Success(foodEntry));
        _mockRepository.UpdateAsync(Arg.Any<FoodEntry>())
            .Returns(Result.Success());
        _mockMapper.Map<FoodEntryDTO>(Arg.Any<FoodEntry>())
            .Returns(foodEntryDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(originalConsumedAt, foodEntry.ConsumedAt);
    }

    [Fact]
    public async Task Handle_WhenUpdateFails_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var foodEntry = new FoodEntry(
            userId,
            Guid.NewGuid(),
            100.0,
            MealType.Breakfast,
            DateTime.UtcNow
        );

        var command = new UpdateFoodEntryCommand
        {
            Id = foodEntry.Id,
            UserId = userId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch
        };

        _mockRepository.GetByIdAsync(command.Id, "Product")
            .Returns(Result<FoodEntry>.Success(foodEntry));
        _mockRepository.UpdateAsync(Arg.Any<FoodEntry>())
            .Returns(Result.Error("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to update food entry", result.Errors.First());
    }
}
