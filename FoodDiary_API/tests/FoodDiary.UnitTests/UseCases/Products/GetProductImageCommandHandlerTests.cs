using Xunit;
using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.Products;
using NSubstitute;
using System.Text;

namespace FoodDiary.UnitTests.UseCases.Products;

public class GetProductImageCommandHandlerTests
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IRepository<Product> _productRepository;
    private readonly IProductService _productService;
    private readonly GetProductImageCommandHandler _handler;

    public GetProductImageCommandHandlerTests()
    {
        _imageStorageService = Substitute.For<IImageStorageService>();
        _productRepository = Substitute.For<IRepository<Product>>();
        _productService = Substitute.For<IProductService>();
        _handler = new GetProductImageCommandHandler(_imageStorageService, _productRepository, _productService);
    }

    [Fact]
    public async Task Handle_ProductWithImage_ReturnsSuccess()
    {
        var productId = Guid.NewGuid();
        var imageData = Encoding.UTF8.GetBytes("fake image data");
        var contentType = "image/png";
        var fileName = "test.png";

        var product = new Product("Test Product", 100, 10, 5, 20);
        product.UpdateImage(imageData, contentType, fileName);

        var command = new GetProductImageCommand
        {
            ProductId = productId
        };

        var expectedDataUrl = "data:image/png;base64,ZmFrZSBpbWFnZSBkYXRh";

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _productService.HasImage(product).Returns(true);
        _imageStorageService.GetImageDataUrlAsync(imageData, contentType).Returns(expectedDataUrl);
        _productService.GetImageSizeInBytes(product).Returns(imageData.Length);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(product.Id, result.Value.ProductId);
        Assert.Equal(expectedDataUrl, result.Value.ImageDataUrl);
        Assert.Equal(contentType, result.Value.ImageContentType);
        Assert.Equal(fileName, result.Value.ImageFileName);
        Assert.Equal(imageData.Length, result.Value.ImageSizeInBytes);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsNotFound()
    {
        var productId = Guid.NewGuid();
        var command = new GetProductImageCommand
        {
            ProductId = productId
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.NotFound());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Contains($"Product with ID {productId} not found", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ProductWithoutImage_ReturnsNotFound()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 20);

        var command = new GetProductImageCommand
        {
            ProductId = productId
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _productService.HasImage(product).Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Contains($"Product with ID {productId} has no image", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ProductWithEmptyImageData_ReturnsNotFound()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 20);
        product.UpdateImage(new byte[0], "image/png", "test.png");

        var command = new GetProductImageCommand
        {
            ProductId = productId
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _productService.HasImage(product).Returns(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Contains($"Product with ID {productId} has no image", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ValidImage_GeneratesCorrectDataUrl()
    {
        var productId = Guid.NewGuid();
        var imageData = Encoding.UTF8.GetBytes("test image content");
        var contentType = "image/jpeg";
        var fileName = "test.jpg";

        var product = new Product("Test Product", 100, 10, 5, 20);
        product.UpdateImage(imageData, contentType, fileName);

        var command = new GetProductImageCommand
        {
            ProductId = productId
        };

        var expectedDataUrl = "data:image/jpeg;base64,dGVzdCBpbWFnZSBjb250ZW50";

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _productService.HasImage(product).Returns(true);
        _imageStorageService.GetImageDataUrlAsync(imageData, contentType).Returns(expectedDataUrl);
        _productService.GetImageSizeInBytes(product).Returns(imageData.Length);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedDataUrl, result.Value.ImageDataUrl);
        Assert.StartsWith("data:image/jpeg;base64,", result.Value.ImageDataUrl);
    }

    [Fact]
    public async Task Handle_DifferentImageFormats_HandlesCorrectly()
    {
        var productId = Guid.NewGuid();
        var imageData = Encoding.UTF8.GetBytes("webp image data");
        var contentType = "image/webp";
        var fileName = "test.webp";

        var product = new Product("Test Product", 100, 10, 5, 20);
        product.UpdateImage(imageData, contentType, fileName);

        var command = new GetProductImageCommand
        {
            ProductId = productId
        };

        var expectedDataUrl = "data:image/webp;base64,d2VicCBpbWFnZSBkYXRh";

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _productService.HasImage(product).Returns(true);
        _imageStorageService.GetImageDataUrlAsync(imageData, contentType).Returns(expectedDataUrl);
        _productService.GetImageSizeInBytes(product).Returns(imageData.Length);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedDataUrl, result.Value.ImageDataUrl);
        Assert.StartsWith("data:image/webp;base64,", result.Value.ImageDataUrl);
        Assert.Equal("image/webp", result.Value.ImageContentType);
        Assert.Equal("test.webp", result.Value.ImageFileName);
    }
} 