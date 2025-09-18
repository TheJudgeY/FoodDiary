using MediatR;
using Ardalis.Result;
using FoodDiary.Core.ProductAggregate;
using AutoMapper;

namespace FoodDiary.UseCases.Products;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly AutoMapper.IMapper _mapper;

    public UpdateProductCommandHandler(FoodDiary.Core.Interfaces.IRepository<Product> productRepository, AutoMapper.IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<UpdateProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var productResult = await _productRepository.GetByIdAsync(request.ProductId);
            if (!productResult.IsSuccess)
            {
                return Result<UpdateProductResponse>.NotFound($"Product with ID {request.ProductId} not found");
            }

            var product = productResult.Value;

            product.UpdateDetails(
                request.Name,
                request.CaloriesPer100g,
                request.ProteinsPer100g,
                request.FatsPer100g,
                request.CarbohydratesPer100g,
                request.Description,
                request.Category
            );

            if (request.ImageData != null && request.ImageData.Length > 0)
            {
                product.UpdateImage(request.ImageData, request.ImageContentType, request.ImageFileName);
            }

            var updateResult = await _productRepository.UpdateAsync(product);
            if (!updateResult.IsSuccess)
            {
                return Result<UpdateProductResponse>.Error($"Failed to update product: {updateResult.Errors.FirstOrDefault()}");
            }

            var response = _mapper.Map<UpdateProductResponse>(product);

            return Result<UpdateProductResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<UpdateProductResponse>.Error($"Failed to update product: {ex.Message}");
        }
    }
}
