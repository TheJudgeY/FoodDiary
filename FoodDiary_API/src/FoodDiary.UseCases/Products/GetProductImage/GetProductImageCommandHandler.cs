using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.Products;

namespace FoodDiary.UseCases.Products;

public class GetProductImageCommandHandler : IRequestHandler<GetProductImageCommand, Result<GetProductImageResponse>>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly IProductService _productService;

    public GetProductImageCommandHandler(
        IImageStorageService imageStorageService,
        FoodDiary.Core.Interfaces.IRepository<Product> productRepository,
        IProductService productService)
    {
        _imageStorageService = imageStorageService;
        _productRepository = productRepository;
        _productService = productService;
    }

    public async Task<Result<GetProductImageResponse>> Handle(GetProductImageCommand request, CancellationToken cancellationToken)
    {
        var productResult = await _productRepository.GetByIdAsync(request.ProductId);
        if (!productResult.IsSuccess || productResult.Value == null)
        {
            return Result<GetProductImageResponse>.NotFound($"Product with ID {request.ProductId} not found");
        }

        var product = productResult.Value;

        if (!_productService.HasImage(product))
        {
            return Result<GetProductImageResponse>.NotFound($"Product with ID {request.ProductId} has no image");
        }

        var imageDataUrl = await _imageStorageService.GetImageDataUrlAsync(
            product.ImageData!, 
            product.ImageContentType!);

        var response = new GetProductImageResponse
        {
            ProductId = product.Id,
            ImageDataUrl = imageDataUrl,
            ImageContentType = product.ImageContentType,
            ImageFileName = product.ImageFileName,
            ImageSizeInBytes = _productService.GetImageSizeInBytes(product)
        };

        return Result<GetProductImageResponse>.Success(response);
    }
} 