using System;
using System.Threading.Tasks;
using Xunit;
using FoodDiary.Infrastructure.Data;
using FoodDiary.Core.ProductAggregate;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;
using System.Linq;

namespace FoodDiary.IntegrationTests.Data;

public class ProductIntegrationTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddProduct_WithCategory_ReturnsSuccess()
    {
        var dbContext = GetInMemoryDbContext();
        var productRepo = new EfRepository<Product>(dbContext);

        var product = new Product("Organic Apple", 52, 0.3, 0.2, 14, "Fresh organic apple", ProductCategory.Fruits);

        var addResult = await productRepo.AddAsync(product);
        Assert.True(addResult.IsSuccess);
        Assert.Equal(ProductCategory.Fruits, product.Category);
    }

    [Fact]
    public async Task GetProduct_ReturnsSuccess()
    {
        var dbContext = GetInMemoryDbContext();
        var productRepo = new EfRepository<Product>(dbContext);

        var product = new Product("Chicken Breast", 165, 31, 3.6, 0, null, ProductCategory.Proteins);
        await productRepo.AddAsync(product);

        var getResult = await productRepo.GetByIdAsync(product.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Chicken Breast", getResult.Value.Name);
        Assert.Equal(ProductCategory.Proteins, getResult.Value.Category);
        Assert.Equal(31, getResult.Value.ProteinsPer100g);
    }

    [Fact]
    public async Task UpdateProduct_UpdatesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var productRepo = new EfRepository<Product>(dbContext);

        var product = new Product("Original Name", 100, 10, 5, 15, null, ProductCategory.Other);
        await productRepo.AddAsync(product);

        product.UpdateDetails("Updated Name", 150, 15, 7, 20, "Updated description", ProductCategory.Vegetables);

        var updateResult = await productRepo.UpdateAsync(product);
        Assert.True(updateResult.IsSuccess);

        var getResult = await productRepo.GetByIdAsync(product.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Updated Name", getResult.Value.Name);
        Assert.Equal(150, getResult.Value.CaloriesPer100g);
        Assert.Equal(ProductCategory.Vegetables, getResult.Value.Category);
        Assert.Equal("Updated description", getResult.Value.Description);
    }

    [Fact]
    public async Task DeleteProduct_RemovesSuccessfully()
    {
        var dbContext = GetInMemoryDbContext();
        var productRepo = new EfRepository<Product>(dbContext);

        var product = new Product("Delete Test", 200, 20, 10, 30, null, ProductCategory.Snacks);
        await productRepo.AddAsync(product);

        var deleteResult = await productRepo.DeleteAsync(product.Id);
        Assert.True(deleteResult.IsSuccess);

        var getResult = await productRepo.GetByIdAsync(product.Id);
        Assert.Equal(ResultStatus.NotFound, getResult.Status);
    }

    [Fact]
    public async Task ListProducts_ReturnsAllProducts()
    {
        var dbContext = GetInMemoryDbContext();
        var productRepo = new EfRepository<Product>(dbContext);

        var product1 = new Product("Apple", 52, 0.3, 0.2, 14, null, ProductCategory.Fruits);
        var product2 = new Product("Chicken", 165, 31, 3.6, 0, null, ProductCategory.Proteins);
        var product3 = new Product("Rice", 130, 2.7, 0.3, 28, null, ProductCategory.Grains);

        await productRepo.AddAsync(product1);
        await productRepo.AddAsync(product2);
        await productRepo.AddAsync(product3);

        var listResult = await productRepo.ListAsync();
        Assert.True(listResult.IsSuccess);
        Assert.Equal(3, listResult.Value.Count);
    }

    [Fact]
    public async Task ListProducts_ByCategory_ReturnsFilteredProducts()
    {
        var dbContext = GetInMemoryDbContext();
        var productRepo = new EfRepository<Product>(dbContext);

        var apple = new Product("Apple", 52, 0.3, 0.2, 14, null, ProductCategory.Fruits);
        var banana = new Product("Banana", 89, 1.1, 0.3, 23, null, ProductCategory.Fruits);
        var chicken = new Product("Chicken", 165, 31, 3.6, 0, null, ProductCategory.Proteins);

        await productRepo.AddAsync(apple);
        await productRepo.AddAsync(banana);
        await productRepo.AddAsync(chicken);

        var listResult = await productRepo.ListAsync();
        Assert.True(listResult.IsSuccess);
        
        var allProducts = listResult.Value;
        var fruits = allProducts.Where(p => p.Category == ProductCategory.Fruits).ToList();
        var proteins = allProducts.Where(p => p.Category == ProductCategory.Proteins).ToList();

        Assert.Equal(2, fruits.Count);
        Assert.Single(proteins);
        Assert.Contains(fruits, p => p.Name == "Apple");
        Assert.Contains(fruits, p => p.Name == "Banana");
        Assert.Contains(proteins, p => p.Name == "Chicken");
    }

    [Fact]
    public async Task ProductNutritionalValues_AreCorrect()
    {
        var dbContext = GetInMemoryDbContext();
        var productRepo = new EfRepository<Product>(dbContext);

        var product = new Product("Test Product", 250, 20, 10, 15, null, ProductCategory.Other);
        await productRepo.AddAsync(product);

        var getResult = await productRepo.GetByIdAsync(product.Id);
        Assert.True(getResult.IsSuccess);
        
        var retrievedProduct = getResult.Value;
        Assert.Equal(250, retrievedProduct.CaloriesPer100g);
        Assert.Equal(20, retrievedProduct.ProteinsPer100g);
        Assert.Equal(10, retrievedProduct.FatsPer100g);
        Assert.Equal(15, retrievedProduct.CarbohydratesPer100g);
    }
} 