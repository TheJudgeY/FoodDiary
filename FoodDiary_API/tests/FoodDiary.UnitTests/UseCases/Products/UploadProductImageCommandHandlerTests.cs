using Xunit;
using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.Products;
using NSubstitute;
using System.Text;

namespace FoodDiary.UnitTests.UseCases.Products;

public class UploadProductImageCommandHandlerTests
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IRepository<Product> _productRepository;
    private readonly IProductService _productService;
    private readonly UploadProductImageCommandHandler _handler;

    public UploadProductImageCommandHandlerTests()
    {
        _imageStorageService = Substitute.For<IImageStorageService>();
        _productRepository = Substitute.For<IRepository<Product>>();
        _productService = Substitute.For<IProductService>();
        _handler = new UploadProductImageCommandHandler(_imageStorageService, _productRepository, _productService);
    }

    [Fact]
    public async Task Handle_ValidImage_ReturnsSuccess()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 20);

        var imageData = Encoding.UTF8.GetBytes("fake image data");
        var contentType = "image/png";
        var fileName = "test.png";

        var command = new UploadProductImageCommand
        {
            ProductId = productId,
            ImageData = imageData,
            ContentType = contentType,
            FileName = fileName
        };

        var validationResult = new ImageValidationResult
        {
            IsValid = true,
            ProcessedImageData = imageData,
            ProcessedContentType = contentType,
            OriginalSizeInBytes = imageData.Length,
            ProcessedSizeInBytes = imageData.Length
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _imageStorageService.ValidateImageAsync(imageData, contentType, fileName).Returns(validationResult);
        _productRepository.UpdateAsync(product).Returns(Result.Success());
        _productService.GetImageSizeInBytes(product).Returns(imageData.Length);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(product.Id, result.Value.ProductId);
        Assert.Equal(fileName, result.Value.ImageFileName);
        Assert.Equal(contentType, result.Value.ImageContentType);
        Assert.Equal(imageData.Length, result.Value.ImageSizeInBytes);

        Assert.Equal(imageData, product.ImageData);
        Assert.Equal(contentType, product.ImageContentType);
        Assert.Equal(fileName, product.ImageFileName);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsNotFound()
    {
        var productId = Guid.NewGuid();
        var imageData = Encoding.UTF8.GetBytes("fake image data");

        var command = new UploadProductImageCommand
        {
            ProductId = productId,
            ImageData = imageData,
            ContentType = "image/png",
            FileName = "test.png"
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.NotFound());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.NotFound, result.Status);
        Assert.Contains($"Product with ID {productId} not found", result.Errors.First());
    }

    [Fact]
    public async Task Handle_InvalidImage_ReturnsError()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 20);

        var imageData = Encoding.UTF8.GetBytes("invalid image data");
        var contentType = "text/plain";
        var fileName = "test.txt";

        var command = new UploadProductImageCommand
        {
            ProductId = productId,
            ImageData = imageData,
            ContentType = contentType,
            FileName = fileName
        };

        var validationResult = new ImageValidationResult
        {
            IsValid = false,
            ErrorMessage = "Invalid image type",
            OriginalSizeInBytes = imageData.Length
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _imageStorageService.ValidateImageAsync(imageData, contentType, fileName).Returns(validationResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Contains("Invalid image", result.Errors.First());
    }

    [Fact]
    public async Task Handle_UpdateFails_ReturnsError()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 20);

        var imageData = Encoding.UTF8.GetBytes("fake image data");
        var contentType = "image/png";
        var fileName = "test.png";

        var command = new UploadProductImageCommand
        {
            ProductId = productId,
            ImageData = imageData,
            ContentType = contentType,
            FileName = fileName
        };

        var validationResult = new ImageValidationResult
        {
            IsValid = true,
            ProcessedImageData = imageData,
            ProcessedContentType = contentType,
            OriginalSizeInBytes = imageData.Length,
            ProcessedSizeInBytes = imageData.Length
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _imageStorageService.ValidateImageAsync(imageData, contentType, fileName).Returns(validationResult);
        _productRepository.UpdateAsync(product).Returns(Result.Error("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Contains("Failed to update product with image", result.Errors.First());
    }

    [Fact]
    public async Task Handle_LargeImage_ValidatesSize()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 20);

        var largeImageData = new byte[11 * 1024 * 1024];
        var contentType = "image/png";
        var fileName = "large.png";

        var command = new UploadProductImageCommand
        {
            ProductId = productId,
            ImageData = largeImageData,
            ContentType = contentType,
            FileName = fileName
        };

        var validationResult = new ImageValidationResult
        {
            IsValid = false,
            ErrorMessage = "Image size (11MB) exceeds maximum allowed size (10MB)",
            OriginalSizeInBytes = largeImageData.Length
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _imageStorageService.ValidateImageAsync(largeImageData, contentType, fileName).Returns(validationResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Contains("exceeds maximum allowed size", result.Errors.First());
    }

    [Fact]
    public async Task Handle_EmptyImageData_ValidatesProperly()
    {
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 20);

        var emptyImageData = new byte[0];
        var contentType = "image/png";
        var fileName = "empty.png";

        var command = new UploadProductImageCommand
        {
            ProductId = productId,
            ImageData = emptyImageData,
            ContentType = contentType,
            FileName = fileName
        };

        var validationResult = new ImageValidationResult
        {
            IsValid = false,
            ErrorMessage = "Image data is empty",
            OriginalSizeInBytes = 0
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _imageStorageService.ValidateImageAsync(emptyImageData, contentType, fileName).Returns(validationResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ResultStatus.Error, result.Status);
        Assert.Contains("Image data is empty", result.Errors.First());
    }
} 