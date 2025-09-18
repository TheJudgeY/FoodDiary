using System.Reflection;
using Xunit;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.FoodEntries;

namespace FoodDiary.UnitTests.Core.FoodEntryAggregate;

public class FoodEntryTests
{
    private readonly IFoodEntryService _foodEntryService;

    public FoodEntryTests()
    {
        _foodEntryService = new FoodEntryService();
    }

    private void SetNavigationProperty<T>(T entity, string propertyName, object value) where T : class
    {
        var property = typeof(T).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
        property?.SetValue(entity, value);
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesFoodEntry()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var weightGrams = 150.0;
        var mealType = MealType.Lunch;
        var consumedAt = DateTime.UtcNow;

        var foodEntry = new FoodEntry(userId, productId, weightGrams, mealType, consumedAt);

        Assert.NotEqual(Guid.Empty, foodEntry.Id);
        Assert.Equal(userId, foodEntry.UserId);
        Assert.Equal(productId, foodEntry.ProductId);
        Assert.Equal(weightGrams, foodEntry.WeightGrams);
        Assert.Equal(mealType, foodEntry.MealType);
        Assert.Equal(consumedAt, foodEntry.ConsumedAt);
    }

    [Fact]
    public void CalculateCalories_WithProduct_ReturnsCorrectCalories()
    {
        var product = new Product("Test Product", 250.0, 20.0, 15.0, 30.0);

        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 200.0, MealType.Lunch, DateTime.UtcNow);
        
        SetNavigationProperty(foodEntry, "Product", product);

        var calories = _foodEntryService.CalculateCalories(foodEntry);

        Assert.Equal(500.0, calories);
    }

    [Fact]
    public void CalculateProtein_WithProduct_ReturnsCorrectProtein()
    {
        var product = new Product("Test Product", 250.0, 20.0, 15.0, 30.0);

        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 150.0, MealType.Dinner, DateTime.UtcNow);
        
        SetNavigationProperty(foodEntry, "Product", product);

        var protein = _foodEntryService.CalculateProtein(foodEntry);

        Assert.Equal(30.0, protein);
    }

    [Fact]
    public void CalculateFat_WithProduct_ReturnsCorrectFat()
    {
        var product = new Product("Test Product", 250.0, 20.0, 15.0, 30.0);

        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Snack, DateTime.UtcNow);
        
        SetNavigationProperty(foodEntry, "Product", product);

        var fat = _foodEntryService.CalculateFat(foodEntry);

        Assert.Equal(15.0, fat);
    }

    [Fact]
    public void CalculateCarbohydrates_WithProduct_ReturnsCorrectCarbohydrates()
    {
        var product = new Product("Test Product", 250.0, 20.0, 15.0, 30.0);

        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 120.0, MealType.Breakfast, DateTime.UtcNow);
        
        SetNavigationProperty(foodEntry, "Product", product);

        var carbohydrates = _foodEntryService.CalculateCarbohydrates(foodEntry);

        Assert.Equal(36.0, carbohydrates);
    }

    [Fact]
    public void CalculateNutritionalValues_WithNullProduct_ReturnsZero()
    {
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Lunch, DateTime.UtcNow);

        var calories = _foodEntryService.CalculateCalories(foodEntry);
        var protein = _foodEntryService.CalculateProtein(foodEntry);
        var fat = _foodEntryService.CalculateFat(foodEntry);
        var carbohydrates = _foodEntryService.CalculateCarbohydrates(foodEntry);

        Assert.Equal(0.0, calories);
        Assert.Equal(0.0, protein);
        Assert.Equal(0.0, fat);
        Assert.Equal(0.0, carbohydrates);
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_UpdatesProperties()
    {
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Lunch, DateTime.UtcNow);

        foodEntry.UpdateDetails(200.0, MealType.Dinner, "Updated notes");

        Assert.Equal(200.0, foodEntry.WeightGrams);
        Assert.Equal(MealType.Dinner, foodEntry.MealType);
        Assert.Equal("Updated notes", foodEntry.Notes);
    }

    [Fact]
    public void UpdateConsumptionTime_WithValidDateTime_UpdatesConsumedAt()
    {
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Lunch, DateTime.UtcNow);
        var newConsumedAt = DateTime.UtcNow.AddHours(2);

        foodEntry.UpdateConsumptionTime(newConsumedAt);

        Assert.Equal(newConsumedAt, foodEntry.ConsumedAt);
    }

    [Fact]
    public void IsConsumedToday_WithTodayDate_ReturnsTrue()
    {
        var today = DateTime.Today.AddHours(12);
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Lunch, today);

        var result = _foodEntryService.IsConsumedToday(foodEntry);

        Assert.True(result);
    }

    [Fact]
    public void IsConsumedToday_WithYesterdayDate_ReturnsFalse()
    {
        var yesterday = DateTime.Today.AddDays(-1).AddHours(12);
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Lunch, yesterday);

        var result = _foodEntryService.IsConsumedToday(foodEntry);

        Assert.False(result);
    }

    [Fact]
    public void IsConsumedOnDate_WithMatchingDate_ReturnsTrue()
    {
        var targetDate = DateTime.Today.AddDays(-2);
        var consumedAt = targetDate.AddHours(12);
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Lunch, consumedAt);

        var result = _foodEntryService.IsConsumedOnDate(foodEntry, targetDate);

        Assert.True(result);
    }

    [Fact]
    public void IsConsumedOnDate_WithDifferentDate_ReturnsFalse()
    {
        var targetDate = DateTime.Today.AddDays(-2);
        var consumedAt = DateTime.Today.AddDays(-1).AddHours(12);
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Lunch, consumedAt);

        var result = _foodEntryService.IsConsumedOnDate(foodEntry, targetDate);

        Assert.False(result);
    }

    [Theory]
    [InlineData(MealType.Breakfast, "Breakfast")]
    [InlineData(MealType.Lunch, "Lunch")]
    [InlineData(MealType.Dinner, "Dinner")]
    [InlineData(MealType.Snack, "Snack")]
    public void GetMealTypeDisplayName_ReturnsCorrectDisplayName(MealType mealType, string expectedDisplayName)
    {
        var foodEntry = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, mealType, DateTime.UtcNow);

        var displayName = _foodEntryService.GetMealTypeDisplayName(foodEntry);

        Assert.Equal(expectedDisplayName, displayName);
    }

    [Fact]
    public void CalculateNutritionalValues_WithMultipleProducts_ReturnsCorrectTotals()
    {
        var product1 = new Product("Product 1", 200.0, 15.0, 10.0, 25.0);
        var product2 = new Product("Product 2", 300.0, 25.0, 20.0, 35.0);

        var foodEntry1 = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 100.0, MealType.Breakfast, DateTime.UtcNow);
        var foodEntry2 = new FoodEntry(Guid.NewGuid(), Guid.NewGuid(), 150.0, MealType.Lunch, DateTime.UtcNow);

        SetNavigationProperty(foodEntry1, "Product", product1);
        SetNavigationProperty(foodEntry2, "Product", product2);

        var foodEntries = new List<FoodEntry> { foodEntry1, foodEntry2 };

        var totalCalories = foodEntries.Sum(e => _foodEntryService.CalculateCalories(e));
        var totalProtein = foodEntries.Sum(e => _foodEntryService.CalculateProtein(e));
        var totalFat = foodEntries.Sum(e => _foodEntryService.CalculateFat(e));
        var totalCarbohydrates = foodEntries.Sum(e => _foodEntryService.CalculateCarbohydrates(e));

        Assert.Equal(650.0, totalCalories);
        Assert.Equal(52.5, totalProtein);
        Assert.Equal(40.0, totalFat);
        Assert.Equal(77.5, totalCarbohydrates);
    }
} 