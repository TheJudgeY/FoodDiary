using Xunit;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UnitTests.Core.RecipeAggregate;

public class RecipeCategoryTests
{
    [Theory]
    [InlineData(RecipeCategory.Breakfast, "Breakfast")]
    [InlineData(RecipeCategory.Lunch, "Lunch")]
    [InlineData(RecipeCategory.Dinner, "Dinner")]
    [InlineData(RecipeCategory.Snack, "Snack")]
    [InlineData(RecipeCategory.Dessert, "Dessert")]
    [InlineData(RecipeCategory.Appetizer, "Appetizer")]
    [InlineData(RecipeCategory.Soup, "Soup")]
    [InlineData(RecipeCategory.Salad, "Salad")]
    [InlineData(RecipeCategory.MainCourse, "Main Course")]
    [InlineData(RecipeCategory.SideDish, "Side Dish")]
    [InlineData(RecipeCategory.Beverage, "Beverage")]
    [InlineData(RecipeCategory.Smoothie, "Smoothie")]
    [InlineData(RecipeCategory.Bread, "Bread")]
    [InlineData(RecipeCategory.Pasta, "Pasta")]
    [InlineData(RecipeCategory.Rice, "Rice")]
    [InlineData(RecipeCategory.Meat, "Meat")]
    [InlineData(RecipeCategory.Fish, "Fish")]
    [InlineData(RecipeCategory.Vegetarian, "Vegetarian")]
    [InlineData(RecipeCategory.Vegan, "Vegan")]
    [InlineData(RecipeCategory.GlutenFree, "Gluten Free")]
    [InlineData(RecipeCategory.LowCarb, "Low Carb")]
    [InlineData(RecipeCategory.HighProtein, "High Protein")]
    [InlineData(RecipeCategory.QuickMeal, "Quick Meal")]
    [InlineData(RecipeCategory.SlowCooker, "Slow Cooker")]
    [InlineData(RecipeCategory.Other, "Other")]
    public void GetDisplayName_ReturnsCorrectDisplayName(RecipeCategory category, string expected)
    {
        var result = category.GetDisplayName();

        Assert.Equal(expected, result);
    }





    [Fact]
    public void RecipeCategory_AllValues_HaveValidDisplayNames()
    {
        var categories = Enum.GetValues<RecipeCategory>();

        foreach (var category in categories)
        {
            var displayName = category.GetDisplayName();
            Assert.NotNull(displayName);
            Assert.NotEmpty(displayName);
            Assert.True(displayName.Length > 0);
        }
    }



    [Fact]
    public void RecipeCategory_EnumValues_AreSequential()
    {
        var categories = Enum.GetValues<RecipeCategory>();

        for (int i = 0; i < categories.Length; i++)
        {
            Assert.Equal(i, (int)categories[i]);
        }
    }

    [Fact]
    public void RecipeCategory_DefaultValue_IsBreakfast()
    {
        var defaultCategory = default(RecipeCategory);

        Assert.Equal(RecipeCategory.Breakfast, defaultCategory);
    }
} 