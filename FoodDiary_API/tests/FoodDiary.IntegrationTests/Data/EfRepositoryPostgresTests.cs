using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using FoodDiary.Infrastructure.Data;
using FoodDiary.Core.ProductAggregate;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

namespace FoodDiary.IntegrationTests.Data;

public class EfRepositoryPostgresTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task Add_Product_InMemoryDb()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var product = new Product("Test_InMemory", 123, 12, 6, 18);
        var addResult = await repo.AddAsync(product);
        Assert.True(addResult.IsSuccess);

        var getResult = await repo.GetByIdAsync(product.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Test_InMemory", getResult.Value.Name);
        Assert.Equal(123, getResult.Value.CaloriesPer100g);
    }

    [Fact]
    public async Task GetById_ProductNotFound_ReturnsNotFound()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var nonExistentId = Guid.NewGuid();
        var getResult = await repo.GetByIdAsync(nonExistentId);
        
        Assert.False(getResult.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, getResult.Status);
    }

    [Fact]
    public async Task Add_And_Delete_Product_InMemoryDb()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var product = new Product("Test_Delete", 456, 45, 22, 68);
        
        var addResult = await repo.AddAsync(product);
        Assert.True(addResult.IsSuccess);

        var getResult = await repo.GetByIdAsync(product.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Test_Delete", getResult.Value.Name);

        var deleteResult = await repo.DeleteAsync(product.Id);
        Assert.True(deleteResult.IsSuccess);

        var getAfterDeleteResult = await repo.GetByIdAsync(product.Id);
        Assert.False(getAfterDeleteResult.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, getAfterDeleteResult.Status);
    }
} 