using Xunit;
using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.UseCases.Products;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.ProductAggregate;
using NSubstitute;

namespace FoodDiary.UnitTests.UseCases.Products;

public class CreateProductCommandHandlerTests
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _mockProductRepository;
    private readonly AutoMapper.IMapper _mockMapper;
    private readonly IImageStorageService _mockImageStorageService;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockProductRepository = Substitute.For<FoodDiary.Core.Interfaces.IRepository<Product>>();
        _mockMapper = Substitute.For<AutoMapper.IMapper>();
        _mockImageStorageService = Substitute.For<IImageStorageService>();
        _handler = new CreateProductCommandHandler(_mockProductRepository, _mockMapper, _mockImageStorageService);
    }

    [Fact]
    public async Task Handle_ValidProduct_ReturnsSuccess()
    {
        var command = new CreateProductCommand
        {
            Name = "Apple",
            CaloriesPer100g = 52,
            Description = "Fresh red apple"
        };

        var product = new Product("Apple", 52, 0.3, 0.2, 14, "Fresh red apple");

        var response = new CreateProductResponse
        {
            ProductId = product.Id,
            Name = product.Name,
            CaloriesPer100g = product.CaloriesPer100g,
            Description = product.Description
        };

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Success(new List<Product>()));

        _mockProductRepository.AddAsync(Arg.Any<Product>())
            .Returns(Result<Product>.Success(product));

        _mockMapper.Map<CreateProductResponse>(Arg.Any<Product>())
            .Returns(response);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.ProductId);
        Assert.Equal("Apple", result.Value.Name);
        Assert.Equal(52, result.Value.CaloriesPer100g);
        Assert.Equal("Fresh red apple", result.Value.Description);
    }

    [Fact]
    public async Task Handle_DuplicateName_ReturnsError()
    {
        var command = new CreateProductCommand
        {
            Name = "Apple",
            CaloriesPer100g = 52
        };

        var existingProduct = new Product("Apple", 50, 0.3, 0.2, 14);

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Success(new List<Product> { existingProduct }));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Product with name 'Apple' already exists", result.Errors.First());
    }

    [Fact]
    public async Task Handle_RepositoryError_ReturnsError()
    {
        var command = new CreateProductCommand
        {
            Name = "Apple",
            CaloriesPer100g = 52
        };

        _mockProductRepository.ListAsync()
            .Returns(Result<List<Product>>.Success(new List<Product>()));

        _mockProductRepository.AddAsync(Arg.Any<Product>())
            .Returns(Result<Product>.Error("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Database error", result.Errors.First());
    }
} 