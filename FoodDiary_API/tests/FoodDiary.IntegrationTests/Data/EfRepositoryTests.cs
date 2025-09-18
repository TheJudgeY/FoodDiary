using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using FoodDiary.Infrastructure.Data;
using FoodDiary.Core.ProductAggregate;
using Microsoft.EntityFrameworkCore;
using Ardalis.Result;

namespace FoodDiary.IntegrationTests.Data;

public class EfRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAndGetById_ReturnsSuccess()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var product = new Product("Test", 100, 10, 5, 15);
        var addResult = await repo.AddAsync(product);

        Assert.True(addResult.IsSuccess);

        var getResult = await repo.GetByIdAsync(product.Id);
        Assert.True(getResult.IsSuccess);
        Assert.Equal("Test", getResult.Value.Name);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var result = await repo.GetByIdAsync(Guid.NewGuid());
        Assert.Equal(ResultStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task ListAsync_ReturnsAllEntities()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var product1 = new Product("A", 50, 5, 2, 8);
        var product2 = new Product("B", 150, 15, 7, 20);
        await repo.AddAsync(product1);
        await repo.AddAsync(product2);

        var listResult = await repo.ListAsync();
        Assert.True(listResult.IsSuccess);
        Assert.Equal(2, listResult.Value.Count);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var product = new Product("Old", 100, 10, 5, 15);
        await repo.AddAsync(product);

        product.UpdateDetails("New", 100, 10, 5, 15);
        var updateResult = await repo.UpdateAsync(product);
        Assert.True(updateResult.IsSuccess);

        var getResult = await repo.GetByIdAsync(product.Id);
        Assert.Equal("New", getResult.Value.Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntity()
    {
        var dbContext = GetInMemoryDbContext();
        var repo = new EfRepository<Product>(dbContext);

        var product = new Product("ToDelete", 100, 10, 5, 15);
        await repo.AddAsync(product);

        var deleteResult = await repo.DeleteAsync(product.Id);
        Assert.True(deleteResult.IsSuccess);

        var getResult = await repo.GetByIdAsync(product.Id);
        Assert.Equal(ResultStatus.NotFound, getResult.Status);
    }
} 