using Xunit;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.Products;
using NSubstitute;

namespace FoodDiary.UnitTests.Core.ProductAggregate;

public class ProductTests
{
    private readonly IProductService _productService;

    public ProductTests()
    {
        _productService = new ProductService();
    }

    [Fact]
    public void IsValidNutritionalData_WithValidData_ReturnsTrue()
    {
        var product = new Product("Test Product", 100, 10, 5, 15);

        var result = _productService.IsValidNutritionalData(product);

        Assert.True(result);
    }

    [Fact]
    public void IsValidNutritionalData_WithNegativeCalories_ReturnsFalse()
    {
        var product = new Product("Test Product", -10, 10, 5, 15);

        var result = _productService.IsValidNutritionalData(product);

        Assert.False(result);
    }

    [Fact]
    public void IsValidNutritionalData_WithNegativeProteins_ReturnsFalse()
    {
        var product = new Product("Test Product", 100, -5, 5, 15);

        var result = _productService.IsValidNutritionalData(product);

        Assert.False(result);
    }

    [Fact]
    public void GetTotalMacronutrientsPer100g_ReturnsCorrectSum()
    {
        var product = new Product("Test Product", 100, 10, 5, 15);

        var result = _productService.GetTotalMacronutrientsPer100g(product);

        Assert.Equal(30, result);
    }

    [Fact]
    public void IsNutritionalDataConsistent_WithConsistentData_ReturnsTrue()
    {
        var product = new Product("Test Product", 100, 10, 5, 4);

        var result = _productService.IsNutritionalDataConsistent(product);

        Assert.True(result);
    }

    [Fact]
    public void IsNutritionalDataConsistent_WithInconsistentData_ReturnsFalse()
    {
        var product = new Product("Test Product", 50, 10, 5, 4);

        var result = _productService.IsNutritionalDataConsistent(product);

        Assert.False(result);
    }

    [Fact]
    public void IsNutritionalDataConsistent_WithZeroMacros_ReturnsTrue()
    {
        var product = new Product("Test Product", 0, 0, 0, 0);

        var result = _productService.IsNutritionalDataConsistent(product);

        Assert.True(result);
    }

    [Fact]
    public void IsNutritionalDataConsistent_WithHighFiberFood_ReturnsTrue()
    {
        var product = new Product("High Fiber Food", 80, 5, 2, 8);

        var result = _productService.IsNutritionalDataConsistent(product);

        Assert.True(result);
    }

    [Fact]
    public void HasImage_WithImageData_ReturnsTrue()
    {
        var product = new Product("Test Product", 100, 10, 5, 15);
        product.UpdateImage(new byte[] { 1, 2, 3, 4 }, "image/jpeg", "test.jpg");

        var result = _productService.HasImage(product);

        Assert.True(result);
    }

    [Fact]
    public void HasImage_WithoutImageData_ReturnsFalse()
    {
        var product = new Product("Test Product", 100, 10, 5, 15);

        var result = _productService.HasImage(product);

        Assert.False(result);
    }

    [Fact]
    public void GetImageSizeInBytes_WithImageData_ReturnsCorrectSize()
    {
        var imageData = new byte[] { 1, 2, 3, 4, 5 };
        var product = new Product("Test Product", 100, 10, 5, 15);
        product.UpdateImage(imageData, "image/jpeg", "test.jpg");

        var result = _productService.GetImageSizeInBytes(product);

        Assert.Equal(5, result);
    }

    [Fact]
    public void GetImageSizeInBytes_WithoutImageData_ReturnsZero()
    {
        var product = new Product("Test Product", 100, 10, 5, 15);

        var result = _productService.GetImageSizeInBytes(product);

        Assert.Equal(0, result);
    }
} 