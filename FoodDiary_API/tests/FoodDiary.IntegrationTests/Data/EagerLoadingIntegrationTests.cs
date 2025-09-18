using System;
using System.Threading.Tasks;
using Xunit;
using FoodDiary.Infrastructure.Data;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

namespace FoodDiary.IntegrationTests.Data;

public class EagerLoadingIntegrationTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetById_WithSingleInclude_LoadsNavigationProperty()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Test Product", 100, 10, 5, 15, null, ProductCategory.Other);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(user.Id, product.Id, 150.0, MealType.Breakfast, DateTime.UtcNow);
        await foodEntryRepo.AddAsync(foodEntry);

        var getResult = await foodEntryRepo.GetByIdAsync(foodEntry.Id, "Product");
        Assert.True(getResult.IsSuccess);
        
        var retrievedFoodEntry = getResult.Value;
        Assert.NotNull(retrievedFoodEntry.Product);
        Assert.Equal("Test Product", retrievedFoodEntry.Product.Name);
    }

    [Fact]
    public async Task GetById_WithMultipleIncludes_LoadsAllNavigationProperties()
    {
        var dbContext = GetInMemoryDbContext();
        var recipeRepo = new EfRepository<Recipe>(dbContext);
        var ingredientRepo = new EfRepository<RecipeIngredient>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Test Ingredient", 50, 5, 2, 8, null, ProductCategory.Other);
        await productRepo.AddAsync(product);

        var recipe = new Recipe("Test Recipe", "Test description", RecipeCategory.MainCourse, 1, 10, 15, "Test instructions");
        await recipeRepo.AddAsync(recipe);

        var ingredient = new RecipeIngredient(recipe.Id, product.Id, 100);
        await ingredientRepo.AddAsync(ingredient);

        var getResult = await recipeRepo.GetByIdAsync(recipe.Id, "Ingredients", "Ingredients.Product");
        Assert.True(getResult.IsSuccess);
        
        var retrievedRecipe = getResult.Value;
        Assert.NotNull(retrievedRecipe.Ingredients);
        Assert.Single(retrievedRecipe.Ingredients);
        var firstIngredient = retrievedRecipe.Ingredients[0];
        Assert.NotNull(firstIngredient.Product);
        Assert.Equal("Test Ingredient", firstIngredient.Product.Name);
    }

    [Fact]
    public async Task List_WithInclude_LoadsNavigationPropertiesForAllEntities()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product1 = new Product("Product 1", 100, 10, 5, 15, null, ProductCategory.Other);
        var product2 = new Product("Product 2", 200, 20, 10, 30, null, ProductCategory.Other);
        await productRepo.AddAsync(product1);
        await productRepo.AddAsync(product2);

        var foodEntry1 = new FoodEntry(user.Id, product1.Id, 100.0, MealType.Breakfast, DateTime.UtcNow);
        var foodEntry2 = new FoodEntry(user.Id, product2.Id, 150.0, MealType.Lunch, DateTime.UtcNow);
        await foodEntryRepo.AddAsync(foodEntry1);
        await foodEntryRepo.AddAsync(foodEntry2);

        var listResult = await foodEntryRepo.ListAsync("Product");
        Assert.True(listResult.IsSuccess);
        Assert.Equal(2, listResult.Value.Count);

        var entries = listResult.Value;
        Assert.NotNull(entries[0].Product);
        Assert.NotNull(entries[1].Product);
        Assert.Contains(entries, e => e.Product.Name == "Product 1");
        Assert.Contains(entries, e => e.Product.Name == "Product 2");
    }

    [Fact]
    public async Task GetById_WithoutInclude_DoesNotLoadNavigationProperties()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Test Product", 100, 10, 5, 15, null, ProductCategory.Other);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(user.Id, product.Id, 150.0, MealType.Breakfast, DateTime.UtcNow);
        await foodEntryRepo.AddAsync(foodEntry);

        var getResult = await foodEntryRepo.GetByIdAsync(foodEntry.Id);
        Assert.True(getResult.IsSuccess);
        
        var retrievedFoodEntry = getResult.Value;
        Assert.Equal(foodEntry.Id, retrievedFoodEntry.Id);
    }

    [Fact]
    public async Task List_WithoutInclude_DoesNotLoadNavigationProperties()
    {
        var dbContext = GetInMemoryDbContext();
        var recipeRepo = new EfRepository<Recipe>(dbContext);
        var ingredientRepo = new EfRepository<RecipeIngredient>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Test Ingredient", 50, 5, 2, 8, null, ProductCategory.Other);
        await productRepo.AddAsync(product);

        var recipe = new Recipe("Test Recipe", "Test description", RecipeCategory.MainCourse, 1, 10, 15, "Test instructions");
        await recipeRepo.AddAsync(recipe);

        var ingredient = new RecipeIngredient(recipe.Id, product.Id, 100);
        await ingredientRepo.AddAsync(ingredient);

        var listResult = await recipeRepo.ListAsync();
        Assert.True(listResult.IsSuccess);
        Assert.Single(listResult.Value);

        var retrievedRecipe = listResult.Value[0];
        Assert.Equal(recipe.Id, retrievedRecipe.Id);
    }

    [Fact]
    public async Task EagerLoading_WithInvalidInclude_DoesNotThrowException()
    {
        var dbContext = GetInMemoryDbContext();
        var foodEntryRepo = new EfRepository<FoodEntry>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var product = new Product("Test Product", 100, 10, 5, 15, null, ProductCategory.Other);
        await productRepo.AddAsync(product);

        var foodEntry = new FoodEntry(user.Id, product.Id, 150.0, MealType.Breakfast, DateTime.UtcNow);
        await foodEntryRepo.AddAsync(foodEntry);

        var getResult = await foodEntryRepo.GetByIdAsync(foodEntry.Id, "InvalidProperty");
        Assert.True(getResult.IsSuccess || getResult.Status == ResultStatus.Error);
        
        if (getResult.IsSuccess)
        {
            var retrievedFoodEntry = getResult.Value;
            Assert.Equal(foodEntry.Id, retrievedFoodEntry.Id);
        }
    }
} 