using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public class GetProductCommandHandler : IRequestHandler<GetProductCommand, Result<GetProductResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly AutoMapper.IMapper _mapper;

    public GetProductCommandHandler(FoodDiary.Core.Interfaces.IRepository<Product> productRepository, AutoMapper.IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<GetProductResponse>> Handle(GetProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var productResult = await _productRepository.GetByIdAsync(request.ProductId);
            if (!productResult.IsSuccess)
            {
                return Result<GetProductResponse>.NotFound();
            }

            var response = _mapper.Map<GetProductResponse>(productResult.Value);

            return Result<GetProductResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<GetProductResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
} 