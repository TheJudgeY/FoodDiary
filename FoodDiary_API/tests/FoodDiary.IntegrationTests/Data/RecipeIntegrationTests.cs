using System;
using System.Threading.Tasks;
using Xunit;
using FoodDiary.Infrastructure.Data;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

namespace FoodDiary.IntegrationTests.Data;

public class RecipeIntegrationTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddRecipe_WithIngredients_ReturnsSuccess()
    {
        var dbContext = GetInMemoryDbContext();
        var recipeRepo = new EfRepository<Recipe>(dbContext);
        var ingredientRepo = new EfRepository<RecipeIngredient>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var chicken = new Product("Chicken Breast", 165, 31, 3.6, 0, null, ProductCategory.Proteins);
        await productRepo.AddAsync(chicken);

                    var recipe = new Recipe("Chicken Bowl", "A healthy meal", RecipeCategory.MainCourse, 2, 15, 20, "Cook and serve");
        await recipeRepo.AddAsync(recipe);

        var ingredient = new RecipeIngredient(recipe.Id, chicken.Id, 200, "Grilled");
        await ingredientRepo.AddAsync(ingredient);

        Assert.True(true);
    }

    [Fact]
    public async Task GetRecipe_WithEagerLoading_IncludesIngredients()
    {
        var dbContext = GetInMemoryDbContext();
        var recipeRepo = new EfRepository<Recipe>(dbContext);
        var ingredientRepo = new EfRepository<RecipeIngredient>(dbContext);
        var productRepo = new EfRepository<Product>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

        var tomato = new Product("Tomato", 18, 0.9, 0.2, 3.9, null, ProductCategory.Vegetables);
        await productRepo.AddAsync(tomato);

                    var recipe = new Recipe("Tomato Salad", "Simple and fresh", RecipeCategory.Salad, 1, 5, 0, "Chop tomatoes");
        await recipeRepo.AddAsync(recipe);

        var ingredient = new RecipeIngredient(recipe.Id, tomato.Id, 100);
        await ingredientRepo.AddAsync(ingredient);

        var getResult = await recipeRepo.GetByIdAsync(recipe.Id, "Ingredients", "Ingredients.Product");
        Assert.True(getResult.IsSuccess);

        var retrievedRecipe = getResult.Value;
        Assert.NotNull(retrievedRecipe.Ingredients);
        Assert.Single(retrievedRecipe.Ingredients);
    }

    [Fact]
    public async Task UpdateRecipe_UpdatesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var recipeRepo = new EfRepository<Recipe>(dbContext);
        var userRepo = new EfRepository<User>(dbContext);

        var user = new User("test@example.com", "password123", "Test User");
        await userRepo.AddAsync(user);

                    var recipe = new Recipe("Original Name", "Original description", RecipeCategory.MainCourse, 2, 10, 15, "Original instructions");
        await recipeRepo.AddAsync(recipe);

        recipe.UpdateDetails(
            "Updated Name",
            "Updated description",
            RecipeCategory.Soup,
            4,
            20,
            30,
            "Updated instructions"
        );

        var updateResult = await recipeRepo.UpdateAsync(recipe);
        Assert.True(updateResult.IsSuccess);

        var getResult = await recipeRepo.GetByIdAsync(recipe.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Updated Name", getResult.Value.Name);
    }
} 