using System;
using System.Reflection;
using Xunit;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.UnitTests.Core.RecipeAggregate;

public class RecipeIngredientTests
{
    private readonly IRecipeIngredientService _recipeIngredientService;

    public RecipeIngredientTests()
    {
        _recipeIngredientService = new RecipeIngredientService();
    }

    private void SetNavigationProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, value);
    }

    [Fact]
    public void Constructor_WithValidData_CreatesRecipeIngredientWithDefaultValues()
    {
        var recipeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantityGrams = 150.0;
        var notes = "Test notes";

        var ingredient = new RecipeIngredient(recipeId, productId, quantityGrams, notes);

        Assert.NotEqual(Guid.Empty, ingredient.Id);
        Assert.Equal(recipeId, ingredient.RecipeId);
        Assert.Equal(productId, ingredient.ProductId);
        Assert.Equal(quantityGrams, ingredient.QuantityGrams);
        Assert.Equal(notes, ingredient.Notes);
        Assert.Null(ingredient.CustomIngredientName);
        Assert.Null(ingredient.CustomCaloriesPer100g);
        Assert.Null(ingredient.CustomProteinPer100g);
        Assert.Null(ingredient.CustomFatPer100g);
        Assert.Null(ingredient.CustomCarbohydratesPer100g);
    }

    [Fact]
    public void Constructor_WithMinimalData_CreatesRecipeIngredientWithDefaults()
    {
        var recipeId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var quantityGrams = 100.0;

        var ingredient = new RecipeIngredient(recipeId, productId, quantityGrams);

        Assert.NotEqual(Guid.Empty, ingredient.Id);
        Assert.Equal(recipeId, ingredient.RecipeId);
        Assert.Equal(productId, ingredient.ProductId);
        Assert.Equal(quantityGrams, ingredient.QuantityGrams);
        Assert.Null(ingredient.Notes);
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_UpdatesQuantity()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        var newQuantity = 200.0;

        ingredient.UpdateQuantity(newQuantity);

        Assert.Equal(newQuantity, ingredient.QuantityGrams);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.05)]
    [InlineData(10001.0)]
    [InlineData(15000.0)]
    public void UpdateQuantity_WithInvalidQuantity_UpdatesQuantity(double quantity)
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);

        ingredient.UpdateQuantity(quantity);

        Assert.Equal(quantity, ingredient.QuantityGrams);
    }

    [Fact]
    public void UpdateNotes_WithValidNotes_UpdatesNotes()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        var newNotes = "Updated notes";

        ingredient.UpdateNotes(newNotes);

        Assert.Equal(newNotes, ingredient.Notes);
    }

    [Fact]
    public void UpdateNotes_WithNotesTooLong_UpdatesNotes()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        var longNotes = new string('a', 1001);

        ingredient.UpdateNotes(longNotes);

        Assert.Equal(longNotes, ingredient.Notes);
    }

    [Fact]
    public void UpdateNotes_WithNullNotes_UpdatesToNull()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0, "Original notes");

        ingredient.UpdateNotes(null);

        Assert.Null(ingredient.Notes);
    }

    [Fact]
    public void SetCustomIngredient_WithValidData_SetsCustomIngredient()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        var name = "Custom Ingredient";
        var calories = 150.0;
        var protein = 20.0;
        var fat = 5.0;
        var carbs = 10.0;

        ingredient.SetCustomIngredient(name, calories, protein, fat, carbs);

        Assert.Equal(name, ingredient.CustomIngredientName);
        Assert.Equal(calories, ingredient.CustomCaloriesPer100g);
        Assert.Equal(protein, ingredient.CustomProteinPer100g);
        Assert.Equal(fat, ingredient.CustomFatPer100g);
        Assert.Equal(carbs, ingredient.CustomCarbohydratesPer100g);
        Assert.Equal(Guid.Empty, ingredient.ProductId);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void SetCustomIngredient_WithEmptyName_SetsCustomIngredient(string name)
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);

        ingredient.SetCustomIngredient(name, 100.0);

        Assert.Equal(name, ingredient.CustomIngredientName);
        Assert.Equal(100.0, ingredient.CustomCaloriesPer100g);
    }

    [Fact]
    public void SetCustomIngredient_WithNameTooLong_SetsCustomIngredient()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        var longName = new string('a', 101);

        ingredient.SetCustomIngredient(longName, 100.0);

        Assert.Equal(longName, ingredient.CustomIngredientName);
        Assert.Equal(100.0, ingredient.CustomCaloriesPer100g);
    }

    [Fact]
    public void SetProduct_WithValidProductId_SetsProduct()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        ingredient.SetCustomIngredient("Custom", 100.0);
        var productId = Guid.NewGuid();

        ingredient.SetProduct(productId);

        Assert.Equal(productId, ingredient.ProductId);
        Assert.Null(ingredient.CustomIngredientName);
        Assert.Null(ingredient.CustomCaloriesPer100g);
        Assert.Null(ingredient.CustomProteinPer100g);
        Assert.Null(ingredient.CustomFatPer100g);
        Assert.Null(ingredient.CustomCarbohydratesPer100g);
    }

    [Fact]
    public void GetCalories_WithProduct_ReturnsCorrectCalories()
    {
        var product = new Product("Test Product", 200, 15, 8, 25);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 150.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.GetCalories(ingredient);

        Assert.Equal(300.0, result);
    }

    [Fact]
    public void GetCalories_WithCustomIngredient_ReturnsCorrectCalories()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 200.0);
        ingredient.SetCustomIngredient("Test Ingredient", 180.0);

        var result = _recipeIngredientService.GetCalories(ingredient);

        Assert.Equal(360.0, result);
    }

    [Fact]
    public void GetCalories_WithNoNutritionalData_ReturnsZero()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);

        var result = _recipeIngredientService.GetCalories(ingredient);

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void GetProtein_WithProduct_ReturnsCorrectProtein()
    {
        var product = new Product("Test Product", 100, 25, 5, 10);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 120.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.GetProtein(ingredient);

        Assert.Equal(30.0, result);
    }

    [Fact]
    public void GetProtein_WithCustomIngredient_ReturnsCorrectProtein()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 300.0);
        ingredient.SetCustomIngredient("Test Ingredient", null, 15.0);

        var result = _recipeIngredientService.GetProtein(ingredient);

        Assert.Equal(45.0, result);
    }

    [Fact]
    public void GetFat_WithProduct_ReturnsCorrectFat()
    {
        var product = new Product("Test Product", 100, 10, 5.0, 20);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 100.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.GetFat(ingredient);

        Assert.Equal(5.0, result);
    }

    [Fact]
    public void GetFat_WithCustomIngredient_ReturnsCorrectFat()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 75.0);
        ingredient.SetCustomIngredient("Test Ingredient", null, null, 8.0);

        var result = _recipeIngredientService.GetFat(ingredient);

        Assert.Equal(6.0, result);
    }

    [Fact]
    public void GetCarbohydrates_WithProduct_ReturnsCorrectCarbohydrates()
    {
        var product = new Product("Test Product", 100, 10, 5, 30.0);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 200.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.GetCarbohydrates(ingredient);

        Assert.Equal(60.0, result);
    }

    [Fact]
    public void GetCarbohydrates_WithCustomIngredient_ReturnsCorrectCarbohydrates()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 120.0);
        ingredient.SetCustomIngredient("Test Ingredient", null, null, null, 25.0);

        var result = _recipeIngredientService.GetCarbohydrates(ingredient);

        Assert.Equal(30.0, result);
    }

    [Fact]
    public void GetIngredientName_WithProduct_ReturnsProductName()
    {
        var product = new Product("Chicken Breast", 100, 10, 5, 10);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 100.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.GetIngredientName(ingredient);

        Assert.Equal("Chicken Breast", result);
    }

    [Fact]
    public void GetIngredientName_WithCustomIngredient_ReturnsCustomName()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        ingredient.SetCustomIngredient("Custom Spice Mix");

        var result = _recipeIngredientService.GetIngredientName(ingredient);

        Assert.Equal("Custom Spice Mix", result);
    }

    [Fact]
    public void GetIngredientName_WithNoName_ReturnsDefaultName()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);

        var result = _recipeIngredientService.GetIngredientName(ingredient);

        Assert.Equal("Unknown Ingredient", result);
    }

    [Theory]
    [InlineData(0.5, "0.5 g")]
    [InlineData(1.0, "1 g")]
    [InlineData(100.0, "100 g")]
    [InlineData(999.0, "999 g")]
    [InlineData(1000.0, "1.0 kg")]
    [InlineData(1500.0, "1.5 kg")]
    [InlineData(2500.0, "2.5 kg")]
    public void GetQuantityDisplay_ReturnsCorrectFormat(double quantity, string expected)
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, quantity);

        var result = _recipeIngredientService.GetQuantityDisplay(ingredient);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsCustomIngredient_WithCustomIngredient_ReturnsTrue()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        ingredient.SetCustomIngredient("Custom Ingredient");

        var result = _recipeIngredientService.IsCustomIngredient(ingredient);

        Assert.True(result);
    }

    [Fact]
    public void IsCustomIngredient_WithProduct_ReturnsFalse()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.NewGuid(), 100.0);

        var result = _recipeIngredientService.IsCustomIngredient(ingredient);

        Assert.False(result);
    }

    [Fact]
    public void HasProduct_WithProduct_ReturnsTrue()
    {
        var product = new Product("Test Product", 100, 10, 5, 10);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 100.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.HasProduct(ingredient);

        Assert.True(result);
    }

    [Fact]
    public void HasProduct_WithProductId_ReturnsTrue()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.NewGuid(), 100.0);

        var result = _recipeIngredientService.HasProduct(ingredient);

        Assert.True(result);
    }

    [Fact]
    public void HasProduct_WithNoProduct_ReturnsFalse()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);

        var result = _recipeIngredientService.HasProduct(ingredient);

        Assert.False(result);
    }

    [Fact]
    public void HasNutritionalData_WithProductWithData_ReturnsTrue()
    {
        var product = new Product("Test Product", 100, 10, 5, 10);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 100.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.HasNutritionalData(ingredient);

        Assert.True(result);
    }

    [Fact]
    public void HasNutritionalData_WithProductWithoutData_ReturnsFalse()
    {
        var product = new Product("Test Product", 0, 0, 0, 0);
        var ingredient = new RecipeIngredient(Guid.Empty, product.Id, 100.0);
        SetNavigationProperty(ingredient, "Product", product);

        var result = _recipeIngredientService.HasNutritionalData(ingredient);

        Assert.False(result);
    }

    [Fact]
    public void HasNutritionalData_WithCustomIngredientWithData_ReturnsTrue()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        ingredient.SetCustomIngredient("Test Ingredient", 100.0, 10.0);

        var result = _recipeIngredientService.HasNutritionalData(ingredient);

        Assert.True(result);
    }

    [Fact]
    public void HasNutritionalData_WithCustomIngredientWithoutData_ReturnsFalse()
    {
        var ingredient = new RecipeIngredient(Guid.Empty, Guid.Empty, 100.0);
        ingredient.SetCustomIngredient("Test Ingredient");

        var result = _recipeIngredientService.HasNutritionalData(ingredient);

        Assert.False(result);
    }
} 