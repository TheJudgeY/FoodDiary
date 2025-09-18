using Xunit;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.Products;

namespace FoodDiary.UnitTests.Core.ProductAggregate;

public class ProductCategoryTests
{
    private readonly ProductDisplayService _displayService;

    public ProductCategoryTests()
    {
        _displayService = new ProductDisplayService();
    }

    [Theory]
    [InlineData(ProductCategory.Fruits, "Fruits")]
    [InlineData(ProductCategory.Vegetables, "Vegetables")]
    [InlineData(ProductCategory.Grains, "Grains")]
    [InlineData(ProductCategory.Proteins, "Proteins")]
    [InlineData(ProductCategory.Dairy, "Dairy")]
    [InlineData(ProductCategory.NutsAndSeeds, "Nuts & Seeds")]
    [InlineData(ProductCategory.Beverages, "Beverages")]
    [InlineData(ProductCategory.Snacks, "Snacks")]
    [InlineData(ProductCategory.Condiments, "Condiments")]
    [InlineData(ProductCategory.Supplements, "Supplements")]
    [InlineData(ProductCategory.Other, "Other")]
    public void GetDisplayName_ReturnsCorrectDisplayName(ProductCategory category, string expected)
    {
        var result = _displayService.GetCategoryDisplayName(category);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetDisplayName_AllCategoriesHaveDisplayNames()
    {
        var categories = Enum.GetValues<ProductCategory>();

        foreach (var category in categories)
        {
            var displayName = _displayService.GetCategoryDisplayName(category);
            Assert.False(string.IsNullOrWhiteSpace(displayName));
            
            if (category == ProductCategory.NutsAndSeeds)
            {
                Assert.NotEqual(category.ToString(), displayName);
            }
        }
    }
}
