using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using FoodDiary.Infrastructure.Data;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

using FoodDiary.UseCases.FoodEntries;
using System.Linq;

namespace FoodDiary.IntegrationTests.Data;

public class FoodEntryIntegrationTests
{
    private readonly IFoodEntryService _foodEntryService;

    public FoodEntryIntegrationTests()
    {
        _foodEntryService = new FoodEntryService();
    }

    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddFoodEntry_WithProduct_ReturnsSuccess()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Chicken Breast", 165, 31, 3.6, 0, null, ProductCategory.Proteins);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(
            user.Id,
            product.Id,
            200.0,
            MealType.Lunch,
            DateTime.UtcNow,
            "Grilled chicken"
        );

        var result = await foodEntryRepo.AddAsync(foodEntry);
        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal(product.Id, result.Value.ProductId);
        Assert.Equal(200.0, result.Value.WeightGrams);
        Assert.Equal(MealType.Lunch, result.Value.MealType);
        Assert.Equal("Grilled chicken", result.Value.Notes);
    }

    [Fact]
    public async Task GetFoodEntry_WithEagerLoading_IncludesProduct()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Salmon", 208, 25, 12, 0, null, ProductCategory.Proteins);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(
            user.Id,
            product.Id,
            150.0,
            MealType.Dinner,
            DateTime.UtcNow
        );
        await foodEntryRepo.AddAsync(foodEntry);

        var result = await foodEntryRepo.GetByIdAsync(foodEntry.Id, "Product");
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.Product);
        Assert.Equal(product.Name, result.Value.Product.Name);
        Assert.Equal(product.CaloriesPer100g, result.Value.Product.CaloriesPer100g);
    }

    [Fact]
    public async Task ListFoodEntries_WithEagerLoading_IncludesProducts()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product1 = new Product("Oatmeal", 389, 13, 7, 66, null, ProductCategory.Grains);
        var product2 = new Product("Banana", 89, 1.1, 0.3, 23, null, ProductCategory.Fruits);
        await productRepo.AddAsync(product1);
        await productRepo.AddAsync(product2);

        var foodEntry1 = new FoodEntry(
            user.Id,
            product1.Id,
            100.0,
            MealType.Breakfast,
            DateTime.UtcNow
        );
        var foodEntry2 = new FoodEntry(
            user.Id,
            product2.Id,
            120.0,
            MealType.Snack,
            DateTime.UtcNow
        );
        await foodEntryRepo.AddAsync(foodEntry1);
        await foodEntryRepo.AddAsync(foodEntry2);

        var result = await foodEntryRepo.ListAsync("Product");
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);

        var entries = result.Value.ToList();
        Assert.NotNull(entries[0].Product);
        Assert.NotNull(entries[1].Product);
        Assert.Equal("Oatmeal", entries[0].Product.Name);
        Assert.Equal("Banana", entries[1].Product.Name);
    }

    [Fact]
    public async Task UpdateFoodEntry_UpdatesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Rice", 130, 2.7, 0.3, 28, null, ProductCategory.Grains);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(
            user.Id,
            product.Id,
            100.0,
            MealType.Lunch,
            DateTime.UtcNow,
            "Original notes"
        );
        await foodEntryRepo.AddAsync(foodEntry);

        foodEntry.UpdateDetails(150.0, MealType.Dinner, "Updated notes");
        var updateResult = await foodEntryRepo.UpdateAsync(foodEntry);
        Assert.True(updateResult.IsSuccess);

        var getResult = await foodEntryRepo.GetByIdAsync(foodEntry.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal(150.0, getResult.Value.WeightGrams);
        Assert.Equal(MealType.Dinner, getResult.Value.MealType);
        Assert.Equal("Updated notes", getResult.Value.Notes);
    }

    [Fact]
    public async Task DeleteFoodEntry_RemovesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Salmon", 208, 25, 12, 0, null, ProductCategory.Proteins);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(
            user.Id,
            product.Id,
            120.0,
            MealType.Dinner,
            DateTime.UtcNow
        );
        await foodEntryRepo.AddAsync(foodEntry);

        var deleteResult = await foodEntryRepo.DeleteAsync(foodEntry.Id);
        Assert.True(deleteResult.IsSuccess);

        var getResult = await foodEntryRepo.GetByIdAsync(foodEntry.Id);
        Assert.Equal(ResultStatus.NotFound, getResult.Status);
    }

    [Fact]
    public async Task CalculateNutritionalValues_WithProduct_ReturnsCorrectValues()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Oatmeal", 389, 16.9, 6.9, 66.3, null, ProductCategory.Grains);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(
            user.Id,
            product.Id,
            200.0,
            MealType.Breakfast,
            DateTime.UtcNow
        );
        await foodEntryRepo.AddAsync(foodEntry);

        var getResult = await foodEntryRepo.GetByIdAsync(foodEntry.Id, "Product");
        Assert.True(getResult.IsSuccess);

        var retrievedFoodEntry = getResult.Value;
        
        var expectedCalories = 389 * 2.0;
        var expectedProtein = 16.9 * 2.0;
        var expectedFat = 6.9 * 2.0;
        var expectedCarbs = 66.3 * 2.0;

        Assert.Equal(expectedCalories, _foodEntryService.CalculateCalories(retrievedFoodEntry), 1);
        Assert.Equal(expectedProtein, _foodEntryService.CalculateProtein(retrievedFoodEntry), 1);
        Assert.Equal(expectedFat, _foodEntryService.CalculateFat(retrievedFoodEntry), 1);
        Assert.Equal(expectedCarbs, _foodEntryService.CalculateCarbohydrates(retrievedFoodEntry), 1);
    }
} 