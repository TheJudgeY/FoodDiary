using Xunit;
using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.UseCases.Products;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.ProductAggregate;
using NSubstitute;

namespace FoodDiary.UnitTests.UseCases.Products;

public class ListProductsCommandHandlerTests
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _mockProductRepository;
    private readonly AutoMapper.IMapper _mockMapper;
    private readonly ListProductsCommandHandler _handler;

    public ListProductsCommandHandlerTests()
    {
        _mockProductRepository = Substitute.For<FoodDiary.Core.Interfaces.IRepository<Product>>();
        _mockMapper = Substitute.For<AutoMapper.IMapper>();
        _handler = new ListProductsCommandHandler(_mockProductRepository, _mockMapper);
    }

    [Fact]
    public async Task Handle_NoSearchTerm_ReturnsAllProducts()
    {
        var command = new ListProductsCommand
        {
            Page = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Apple", 52, 0.3, 0.2, 14),
            new Product("Banana", 89, 1.1, 0.3, 23),
            new Product("Orange", 47, 0.9, 0.1, 12)
        };

        var productSummaries = products.Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            CaloriesPer100g = p.CaloriesPer100g,
            ProteinsPer100g = p.ProteinsPer100g,
            FatsPer100g = p.FatsPer100g,
            CarbohydratesPer100g = p.CarbohydratesPer100g,
            Description = p.Description,
            Category = p.Category
        }).ToList();

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Success(products));
        _mockMapper.Map<List<ProductSummaryDto>>(Arg.Any<List<Product>>())
            .Returns(productSummaries);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value.Products.Count);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(1, result.Value.Page);
        Assert.Equal(10, result.Value.PageSize);
        Assert.Equal(1, result.Value.TotalPages);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ReturnsFilteredProducts()
    {
        var command = new ListProductsCommand
        {
            SearchTerm = "apple",
            Page = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Apple", 52, 0.3, 0.2, 14),
            new Product("Banana", 89, 1.1, 0.3, 23),
            new Product("Pineapple", 50, 0.5, 0.1, 13)
        };

        var filteredProducts = new List<Product>
        {
            new Product("Apple", 52, 0.3, 0.2, 14),
            new Product("Pineapple", 50, 0.5, 0.1, 13)
        };

        var productSummaries = filteredProducts.Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            CaloriesPer100g = p.CaloriesPer100g,
            ProteinsPer100g = p.ProteinsPer100g,
            FatsPer100g = p.FatsPer100g,
            CarbohydratesPer100g = p.CarbohydratesPer100g,
            Description = p.Description,
            Category = p.Category
        }).ToList();

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Success(products));
        _mockMapper.Map<List<ProductSummaryDto>>(Arg.Any<List<Product>>())
            .Returns(productSummaries);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Products.Count);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Contains(result.Value.Products, p => p.Name == "Apple");
        Assert.Contains(result.Value.Products, p => p.Name == "Pineapple");
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        var command = new ListProductsCommand
        {
            Page = 2,
            PageSize = 2
        };

        var products = new List<Product>
        {
            new Product("Apple", 52, 0.3, 0.2, 14),
            new Product("Banana", 89, 1.1, 0.3, 23),
            new Product("Orange", 47, 0.9, 0.1, 12),
            new Product("Grape", 62, 0.6, 0.2, 16)
        };

        var pagedProducts = new List<Product>
        {
            new Product("Orange", 47, 0.9, 0.1, 12),
            new Product("Grape", 62, 0.6, 0.2, 16)
        };

        var productSummaries = pagedProducts.Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            CaloriesPer100g = p.CaloriesPer100g,
            ProteinsPer100g = p.ProteinsPer100g,
            FatsPer100g = p.FatsPer100g,
            CarbohydratesPer100g = p.CarbohydratesPer100g,
            Description = p.Description,
            Category = p.Category
        }).ToList();

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Success(products));
        _mockMapper.Map<List<ProductSummaryDto>>(Arg.Any<List<Product>>())
            .Returns(productSummaries);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Products.Count);
        Assert.Equal(4, result.Value.TotalCount);
        Assert.Equal(2, result.Value.Page);
        Assert.Equal(2, result.Value.PageSize);
        Assert.Equal(2, result.Value.TotalPages);
    }

    [Fact]
    public async Task Handle_RepositoryError_ReturnsError()
    {
        var command = new ListProductsCommand
        {
            Page = 1,
            PageSize = 10
        };

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Error("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Failed to retrieve products", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ProductsSortedByName()
    {
        var command = new ListProductsCommand
        {
            Page = 1,
            PageSize = 10
        };

        var products = new List<Product>
        {
            new Product("Zucchini", 17, 1.2, 0.3, 3.1),
            new Product("Apple", 52, 0.3, 0.2, 14),
            new Product("Banana", 89, 1.1, 0.3, 23)
        };

        var sortedProducts = new List<Product>
        {
            new Product("Apple", 52, 0.3, 0.2, 14),
            new Product("Banana", 89, 1.1, 0.3, 23),
            new Product("Zucchini", 17, 1.2, 0.3, 3.1)
        };

        var productSummaries = sortedProducts.Select(p => new ProductSummaryDto
        {
            Id = p.Id,
            Name = p.Name,
            CaloriesPer100g = p.CaloriesPer100g,
            ProteinsPer100g = p.ProteinsPer100g,
            FatsPer100g = p.FatsPer100g,
            CarbohydratesPer100g = p.CarbohydratesPer100g,
            Description = p.Description,
            Category = p.Category
        }).ToList();

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Success(products));
        _mockMapper.Map<List<ProductSummaryDto>>(Arg.Any<List<Product>>())
            .Returns(productSummaries);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Apple", result.Value.Products[0].Name);
        Assert.Equal("Banana", result.Value.Products[1].Name);
        Assert.Equal("Zucchini", result.Value.Products[2].Name);
    }
} 