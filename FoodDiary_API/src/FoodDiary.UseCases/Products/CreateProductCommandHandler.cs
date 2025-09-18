using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.Products;

namespace FoodDiary.UseCases.Products;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly IImageStorageService _imageStorageService;

    public CreateProductCommandHandler(
        FoodDiary.Core.Interfaces.IRepository<Product> productRepository, 
        IMapper mapper,
        IImageStorageService imageStorageService)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var duplicateCheckResult = await CheckForDuplicateProduct(request.Name);
            if (!duplicateCheckResult.IsSuccess)
                return duplicateCheckResult;

            var product = CreateProductFromRequest(request);

            var imageResult = await HandleProductImage(request, product);
            if (!imageResult.IsSuccess)
                return imageResult;

            var saveResult = await SaveProduct(product);
            if (!saveResult.IsSuccess)
                return saveResult;

            var response = _mapper.Map<CreateProductResponse>(product);
            return Result<CreateProductResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<CreateProductResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }

    private async Task<Result> CheckForDuplicateProduct(string productName)
    {
        var allProductsResult = await _productRepository.ListAsync();
        if (!allProductsResult.IsSuccess)
            return Result.Error("Failed to check for existing products");

        var existingProduct = allProductsResult.Value
            .FirstOrDefault(p => p.Name.Equals(productName, StringComparison.OrdinalIgnoreCase));
        
        if (existingProduct != null)
            return Result.Error($"Product with name '{productName}' already exists");

        return Result.Success();
    }

    private static Product CreateProductFromRequest(CreateProductCommand request) =>
        new Product(
            request.Name.Trim(),
            request.CaloriesPer100g,
            request.ProteinsPer100g,
            request.FatsPer100g,
            request.CarbohydratesPer100g,
            request.Description?.Trim(),
            request.Category
        );

    private async Task<Result> HandleProductImage(CreateProductCommand request, Product product)
    {
        if (request.ImageData == null || request.ImageData.Length == 0)
            return Result.Success();

        var validationResult = await _imageStorageService.ValidateImageAsync(
            request.ImageData, 
            request.ImageContentType ?? "image/jpeg", 
            request.ImageFileName ?? "image.jpg");

        if (!validationResult.IsValid)
            return Result.Error(validationResult.ErrorMessage ?? "Invalid image");

        product.UpdateImage(
            validationResult.ProcessedImageData,
            validationResult.ProcessedContentType,
            request.ImageFileName ?? "image.jpg");

        return Result.Success();
    }

    private async Task<Result> SaveProduct(Product product)
    {
        var addResult = await _productRepository.AddAsync(product);
        if (!addResult.IsSuccess)
            return Result.Error($"Failed to create product: {addResult.Errors.FirstOrDefault()}");

        return Result.Success();
    }
} 