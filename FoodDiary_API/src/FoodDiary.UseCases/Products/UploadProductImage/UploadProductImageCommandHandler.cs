using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.Products;

namespace FoodDiary.UseCases.Products;

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, Result<UploadProductImageResponse>>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly IProductService _productService;

    public UploadProductImageCommandHandler(
        IImageStorageService imageStorageService,
        FoodDiary.Core.Interfaces.IRepository<Product> productRepository,
        IProductService productService)
    {
        _imageStorageService = imageStorageService;
        _productRepository = productRepository;
        _productService = productService;
    }

    public async Task<Result<UploadProductImageResponse>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var productResult = await _productRepository.GetByIdAsync(request.ProductId);
        if (!productResult.IsSuccess || productResult.Value == null)
        {
            return Result<UploadProductImageResponse>.NotFound($"Product with ID {request.ProductId} not found");
        }

        var product = productResult.Value;

        var validationResult = await _imageStorageService.ValidateImageAsync(
            request.ImageData, 
            request.ContentType, 
            request.FileName);

        if (!validationResult.IsValid)
        {
            return Result<UploadProductImageResponse>.Error(validationResult.ErrorMessage ?? "Invalid image");
        }

        product.UpdateImage(
            validationResult.ProcessedImageData,
            validationResult.ProcessedContentType,
            request.FileName);

        var updateResult = await _productRepository.UpdateAsync(product);
        if (!updateResult.IsSuccess)
        {
            return Result<UploadProductImageResponse>.Error("Failed to update product with image");
        }

        var response = new UploadProductImageResponse
        {
            ProductId = product.Id,
            ImageFileName = product.ImageFileName,
            ImageContentType = product.ImageContentType,
            ImageSizeInBytes = _productService.GetImageSizeInBytes(product)
        };

        return Result<UploadProductImageResponse>.Success(response);
    }
} 