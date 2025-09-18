using System;
using System.Threading.Tasks;
using Xunit;
using FoodDiary.Infrastructure.Data;
using FoodDiary.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

using FoodDiary.UseCases.Users;

namespace FoodDiary.IntegrationTests.Data;

public class UserIntegrationTests
{
    private readonly IUserService _userService;

    public UserIntegrationTests()
    {
        _userService = new UserService();
    }

    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddUser_ReturnsSuccess()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");

        var result = await userRepo.AddAsync(user);
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
    }

    [Fact]
    public async Task GetUser_ReturnsSuccess()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var result = await userRepo.GetByIdAsync(user.Id);
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.Id);
        Assert.Equal(user.Name, result.Value.Name);
    }

    [Fact]
    public async Task UpdateUser_UpdatesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Original Name");
        await userRepo.AddAsync(user);

        user.UpdateName("Updated Name");
        var updateResult = await userRepo.UpdateAsync(user);
        Assert.True(updateResult.IsSuccess);

        var getResult = await userRepo.GetByIdAsync(user.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Updated Name", getResult.Value.Name);
    }

    [Fact]
    public async Task UpdateUserTimeZone_UpdatesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("timezone@example.com", "password123", "TimeZone Test");
        await userRepo.AddAsync(user);

        user.UpdateTimeZone("America/New_York");
        var updateResult = await userRepo.UpdateAsync(user);
        Assert.True(updateResult.IsSuccess);

        var getResult = await userRepo.GetByIdAsync(user.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("America/New_York", getResult.Value.TimeZoneId);
    }

    [Fact]
    public async Task UserTimeZoneConversion_WorksCorrectly()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("timezone@example.com", "password123", "TimeZone Test");
        user.UpdateTimeZone("Europe/London");
        await userRepo.AddAsync(user);

        var utcTime = DateTime.UtcNow;
        var localTime = _userService.GetLocalTime(user, utcTime);
        var convertedBackToUtc = _userService.GetUtcTime(user, localTime);

        Assert.Equal(utcTime.Date, convertedBackToUtc.Date);
        Assert.Equal(utcTime.Hour, convertedBackToUtc.Hour);
    }

    [Fact]
    public async Task DeleteUser_RemovesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("delete@example.com", "password123", "Delete Test");
        await userRepo.AddAsync(user);

        var deleteResult = await userRepo.DeleteAsync(user.Id);
        Assert.True(deleteResult.IsSuccess);

        var getResult = await userRepo.GetByIdAsync(user.Id);
        Assert.Equal(ResultStatus.NotFound, getResult.Status);
    }

    [Fact]
    public async Task ListUsers_ReturnsAllUsers()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user1 = new User("user1@example.com", "password123", "User 1");
        var user2 = new User("user2@example.com", "password123", "User 2");
        var user3 = new User("user3@example.com", "password123", "User 3");

        await userRepo.AddAsync(user1);
        await userRepo.AddAsync(user2);
        await userRepo.AddAsync(user3);

        var listResult = await userRepo.ListAsync();
        Assert.True(listResult.IsSuccess);
        Assert.Equal(3, listResult.Value.Count);
    }

    [Fact]
    public async Task UserEmailConfirmation_WorksCorrectly()
    {
        var dbContext = GetInMemoryDbContext();
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("confirm@example.com", "password123", "Confirm Test");
        await userRepo.AddAsync(user);

        Assert.False(user.EmailConfirmed);

        user.ConfirmEmail();
        var updateResult = await userRepo.UpdateAsync(user);
        Assert.True(updateResult.IsSuccess);

        var getResult = await userRepo.GetByIdAsync(user.Id);
        Assert.True(getResult.IsSuccess);
        Assert.True(getResult.Value.EmailConfirmed);
        Assert.Null(getResult.Value.EmailConfirmationToken);
        Assert.Null(getResult.Value.EmailConfirmationTokenExpiresAt);
    }
} 