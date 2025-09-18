using Xunit;
using AutoMapper;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.Products;

namespace FoodDiary.UnitTests.Core.Services;

public class AutoMapperProductProfileTests
{
    private readonly IMapper _mapper;

    public AutoMapperProductProfileTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
        });
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void ProductProfile_Configuration_ShouldBeValid()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProductProfile>();
        });

        config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Product_Maps_To_ProductDTO_WithAllProperties()
    {
        var product = new Product("Test Product", 250.5, 20.0, 10.5, 30.0, "A test product description");

        var dto = _mapper.Map<ProductDTO>(product);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.CaloriesPer100g, dto.CaloriesPer100g);
        Assert.Equal(product.ProteinsPer100g, dto.ProteinsPer100g);
        Assert.Equal(product.FatsPer100g, dto.FatsPer100g);
        Assert.Equal(product.CarbohydratesPer100g, dto.CarbohydratesPer100g);
        Assert.Equal(product.Description, dto.Description);
    }

    [Fact]
    public void Product_Maps_To_ProductDTO_WithNullDescription()
    {
        var product = new Product("Test Product", 250.5, 20.0, 10.5, 30.0);

        var dto = _mapper.Map<ProductDTO>(product);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.CaloriesPer100g, dto.CaloriesPer100g);
        Assert.Equal(product.ProteinsPer100g, dto.ProteinsPer100g);
        Assert.Equal(product.FatsPer100g, dto.FatsPer100g);
        Assert.Equal(product.CarbohydratesPer100g, dto.CarbohydratesPer100g);
        Assert.Null(dto.Description);
    }

    [Fact]
    public void Product_Maps_To_ProductDTO_WithEmptyStrings()
    {
        var product = new Product("", 0.0, 0.0, 0.0, 0.0, "");

        var dto = _mapper.Map<ProductDTO>(product);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.CaloriesPer100g, dto.CaloriesPer100g);
        Assert.Equal(product.ProteinsPer100g, dto.ProteinsPer100g);
        Assert.Equal(product.FatsPer100g, dto.FatsPer100g);
        Assert.Equal(product.CarbohydratesPer100g, dto.CarbohydratesPer100g);
        Assert.Equal(product.Description, dto.Description);
    }

    [Fact]
    public void Product_Maps_To_ProductDTO_WithSpecialCharacters()
    {
        var product = new Product("José's Special Product & More", 123.45, 15.0, 8.0, 20.0, "Product with special chars: !@#$%^&*()");

        var dto = _mapper.Map<ProductDTO>(product);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.CaloriesPer100g, dto.CaloriesPer100g);
        Assert.Equal(product.Description, dto.Description);
    }

    [Fact]
    public void Product_Maps_To_ProductDTO_WithVeryLongStrings()
    {
        var longName = new string('a', 200);
        var longDescription = new string('b', 500);
        var product = new Product(longName, 999.99, 25.0, 15.0, 40.0, longDescription);

        var dto = _mapper.Map<ProductDTO>(product);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.CaloriesPer100g, dto.CaloriesPer100g);
        Assert.Equal(product.Description, dto.Description);
    }

    [Fact]
    public void Product_Maps_To_ProductDTO_WithMinimalData()
    {
        var product = new Product("Minimal Product", 0.0, 0.0, 0.0, 0.0);

        var dto = _mapper.Map<ProductDTO>(product);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
        Assert.Equal(product.CaloriesPer100g, dto.CaloriesPer100g);
        Assert.Null(dto.Description);
    }

    [Fact]
    public void Product_Maps_To_ProductDTO_PreservesDataIntegrity()
    {
        var originalName = "Integrity Test Product";
        var originalCalories = 250.75;
        var originalDescription = "Test description for integrity";
        
        var product = new Product(originalName, originalCalories, 20.0, 10.0, 30.0, originalDescription);

        var dto = _mapper.Map<ProductDTO>(product);

        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(originalName, dto.Name);
        Assert.Equal(originalCalories, dto.CaloriesPer100g);
        Assert.Equal(originalDescription, dto.Description);
        
        Assert.Equal(originalName, product.Name);
        Assert.Equal(originalCalories, product.CaloriesPer100g);
        Assert.Equal(originalDescription, product.Description);
    }
} 