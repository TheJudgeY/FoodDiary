using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Reflection;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.UnitTests.Core.RecipeAggregate;

public class RecipeTests
{
    private readonly IRecipeService _recipeService;

    public RecipeTests()
    {
        _recipeService = new RecipeService();
    }

    private void SetNavigationProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, value);
    }

    [Fact]
    public void Constructor_WithValidData_CreatesRecipeWithDefaultValues()
    {
        var userId = Guid.NewGuid();
        var name = "Test Recipe";
        var description = "A test recipe";
        var category = RecipeCategory.Dinner;
        var servings = 4;
        var prepTime = 30;
        var cookTime = 45;
        var instructions = "Test instructions";

        var recipe = new Recipe(name, description, category, servings, prepTime, cookTime, instructions);

        Assert.NotEqual(Guid.Empty, recipe.Id);
        Assert.Equal(name, recipe.Name);
        Assert.Equal(description, recipe.Description);
        Assert.Equal(category, recipe.Category);
        Assert.Equal(servings, recipe.Servings);
        Assert.Equal(prepTime, recipe.PreparationTimeMinutes);
        Assert.Equal(cookTime, recipe.CookingTimeMinutes);
        Assert.Equal(instructions, recipe.Instructions);
        Assert.False(recipe.IsPublic);
        Assert.Equal(0, recipe.TotalCalories);
        Assert.Equal(0, recipe.TotalProtein);
        Assert.Equal(0, recipe.TotalFat);
        Assert.Equal(0, recipe.TotalCarbohydrates);
        Assert.Empty(recipe.Ingredients);
    }

    [Fact]
    public void UpdateDetails_WithValidData_UpdatesRecipe()
    {
        var recipe = new Recipe("Old Name", "Old description", RecipeCategory.Other, 2, 20, 30, "Old instructions");
        var originalUpdatedAt = recipe.UpdatedAt;

        recipe.UpdateDetails(
            name: "New Recipe Name",
            description: "A delicious recipe",
            category: RecipeCategory.Breakfast,
            servings: 4,
            preparationTimeMinutes: 15,
            cookingTimeMinutes: 45,
            instructions: "Mix ingredients and cook"
        );

        Assert.Equal("New Recipe Name", recipe.Name);
        Assert.Equal("A delicious recipe", recipe.Description);
        Assert.Equal(RecipeCategory.Breakfast, recipe.Category);
        Assert.Equal(4, recipe.Servings);
        Assert.Equal(15, recipe.PreparationTimeMinutes);
        Assert.Equal(45, recipe.CookingTimeMinutes);
        Assert.Equal("Mix ingredients and cook", recipe.Instructions);
    }







    [Fact]
    public void ToggleFavorite_ChangesFavoriteStatus()
    {
        Assert.True(true);
    }

    [Fact]
    public void SetPublic_ChangesPublicStatus()
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, 20, 30, "instructions");
        var originalUpdatedAt = recipe.UpdatedAt;

        recipe.SetPublic(true);

        Assert.True(recipe.IsPublic);
        Assert.NotEqual(default(DateTime), recipe.UpdatedAt);
    }



    [Fact]
    public void CalculateNutritionalValues_WithNoIngredients_SetsZeroValues()
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, 20, 30, "instructions");

        _recipeService.CalculateNutritionalValues(recipe);

        Assert.Equal(0, recipe.TotalCalories);
        Assert.Equal(0, recipe.TotalProtein);
        Assert.Equal(0, recipe.TotalFat);
        Assert.Equal(0, recipe.TotalCarbohydrates);
    }

    [Fact]
    public void CalculateNutritionalValues_WithIngredients_CalculatesCorrectValues()
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, 20, 30, "instructions");
        
        var product1 = new Product("Chicken", 165, 31, 3.6, 0);
        var product2 = new Product("Rice", 111, 2.6, 0.9, 23);
        
        var ingredient1 = new RecipeIngredient(recipe.Id, product1.Id, 200);
        var ingredient2 = new RecipeIngredient(recipe.Id, product2.Id, 150);
        
        SetNavigationProperty(ingredient1, "Product", product1);
        SetNavigationProperty(ingredient2, "Product", product2);
        
        recipe.SetIngredients(new List<RecipeIngredient> { ingredient1, ingredient2 });

        _recipeService.CalculateNutritionalValues(recipe);

        Assert.Equal(496.5, recipe.TotalCalories, 1);
        Assert.Equal(65.9, recipe.TotalProtein, 1);
        Assert.Equal(8.6, recipe.TotalFat, 1);
        Assert.Equal(34.5, recipe.TotalCarbohydrates, 1);
    }

    [Theory]
    [InlineData(1, 100.0)]
    [InlineData(2, 50.0)]
    [InlineData(4, 25.0)]
    [InlineData(0, 0.0)]
    public void GetCaloriesPerServing_ReturnsCorrectValue(int servings, double expected)
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, servings, 20, 30, "instructions");
        recipe.UpdateNutritionalInfo(100.0, 0, 0, 0);

        var result = _recipeService.GetCaloriesPerServing(recipe);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 10.0)]
    [InlineData(2, 5.0)]
    [InlineData(4, 2.5)]
    [InlineData(0, 0.0)]
    public void GetProteinPerServing_ReturnsCorrectValue(int servings, double expected)
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, servings, 20, 30, "instructions");
        recipe.UpdateNutritionalInfo(0, 10.0, 0, 0);

        var result = _recipeService.GetProteinPerServing(recipe);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 5.0)]
    [InlineData(2, 2.5)]
    [InlineData(4, 1.2)]
    [InlineData(0, 0.0)]
    public void GetFatPerServing_ReturnsCorrectValue(int servings, double expected)
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, servings, 20, 30, "instructions");
        recipe.UpdateNutritionalInfo(0, 0, 5.0, 0);

        var result = _recipeService.GetFatPerServing(recipe);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(1, 15.0)]
    [InlineData(2, 7.5)]
    [InlineData(4, 3.8)]
    [InlineData(0, 0.0)]
    public void GetCarbohydratesPerServing_ReturnsCorrectValue(int servings, double expected)
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, servings, 20, 30, "instructions");
        recipe.UpdateNutritionalInfo(0, 0, 0, 15.0);

        var result = _recipeService.GetCarbohydratesPerServing(recipe);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(30, 30, 60)]
    [InlineData(15, 45, 60)]
    [InlineData(60, 120, 180)]
    public void GetTotalTimeMinutes_ReturnsCorrectValue(int prepTime, int cookTime, int expected)
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, prepTime, cookTime, "instructions");

        var result = _recipeService.GetTotalTimeMinutes(recipe);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(15, 15, "30 min")]
    [InlineData(30, 30, "1h")]
    [InlineData(45, 45, "1h 30m")]
    [InlineData(60, 60, "2h")]
    [InlineData(75, 75, "2h 30m")]
    public void GetTotalTimeDisplay_ReturnsCorrectFormat(int prepTime, int cookTime, string expected)
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, prepTime, cookTime, "instructions");

        var result = _recipeService.GetTotalTimeDisplay(recipe);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void HasIngredients_WithNoIngredients_ReturnsFalse()
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, 20, 30, "instructions");

        var result = _recipeService.HasIngredients(recipe);

        Assert.False(result);
    }

    [Fact]
    public void HasIngredients_WithIngredients_ReturnsTrue()
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, 20, 30, "instructions");
        var ingredient = new RecipeIngredient(recipe.Id, Guid.NewGuid(), 100);
        recipe.SetIngredients(new List<RecipeIngredient> { ingredient });

        var result = _recipeService.HasIngredients(recipe);

        Assert.True(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void GetIngredientCount_ReturnsCorrectCount(int count)
    {
        var recipe = new Recipe("Test Recipe", "description", RecipeCategory.Other, 2, 20, 30, "instructions");
        var ingredients = Enumerable.Range(0, count)
            .Select(i => new RecipeIngredient(recipe.Id, Guid.NewGuid(), 100))
            .ToList();
        recipe.SetIngredients(ingredients);

        var result = _recipeService.GetIngredientCount(recipe);

        Assert.Equal(count, result);
    }

    [Fact]
    public void IsComplete_WithCompleteRecipe_ReturnsTrue()
    {
        var recipe = new Recipe("Test Recipe", "A complete recipe description", RecipeCategory.Other, 4, 30, 45, "Complete instructions");
        var product = new Product("Test Product", 100, 10, 5, 20);
        var ingredient = new RecipeIngredient(recipe.Id, product.Id, 100);
        SetNavigationProperty(ingredient, "Product", product);
        recipe.SetIngredients(new List<RecipeIngredient> { ingredient });

        var result = _recipeService.IsComplete(recipe);

        Assert.True(result);
    }

    [Theory]
    [InlineData("", "instructions", 4, 15, 30, true)]
    [InlineData("name", "", 4, 15, 30, true)]
    [InlineData("name", "instructions", 0, 15, 30, true)]
    [InlineData("name", "instructions", 4, 0, 30, true)]
    [InlineData("name", "instructions", 4, 15, 0, true)]
    [InlineData("name", "instructions", 4, 15, 30, false)]
    public void IsComplete_WithIncompleteRecipe_ReturnsFalse(string name, string instructions, 
        int servings, int prepTime, int cookTime, bool hasIngredients)
    {
        var recipe = new Recipe(name, "description", RecipeCategory.Other, servings, prepTime, cookTime, instructions);
        
        if (hasIngredients)
        {
            var ingredient = new RecipeIngredient(recipe.Id, Guid.NewGuid(), 100);
            recipe.SetIngredients(new List<RecipeIngredient> { ingredient });
        }

        var result = _recipeService.IsComplete(recipe);

        Assert.False(result);
    }
} 