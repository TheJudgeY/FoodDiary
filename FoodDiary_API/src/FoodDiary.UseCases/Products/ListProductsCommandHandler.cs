using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public class ListProductsCommandHandler : IRequestHandler<ListProductsCommand, Result<ListProductsResponse>>
{
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly AutoMapper.IMapper _mapper;

    public ListProductsCommandHandler(FoodDiary.Core.Interfaces.IRepository<Product> productRepository, AutoMapper.IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<Result<ListProductsResponse>> Handle(ListProductsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var allProductsResult = await _productRepository.ListAsync();
            if (!allProductsResult.IsSuccess)
            {
                return Result<ListProductsResponse>.Error("Failed to retrieve products");
            }

            var allProducts = allProductsResult.Value;

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.Trim().ToLowerInvariant();
                allProducts = allProducts.Where(p => 
                    p.Name.ToLowerInvariant().Contains(searchTerm) ||
                    (p.Description != null && p.Description.ToLowerInvariant().Contains(searchTerm))
                ).ToList();
            }

            if (request.Category.HasValue)
            {
                allProducts = allProducts.Where(p => p.Category == request.Category.Value).ToList();
            }

            allProducts = allProducts.OrderBy(p => p.Name).ToList();

            var totalCount = allProducts.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var skip = (request.Page - 1) * request.PageSize;
            var pagedProducts = allProducts.Skip(skip).Take(request.PageSize).ToList();

            var productSummaries = _mapper.Map<List<ProductSummaryDto>>(pagedProducts);

            var response = new ListProductsResponse
            {
                Products = productSummaries,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };

            return Result<ListProductsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<ListProductsResponse>.Error($"Unexpected error: {ex.Message}");
        }
    }
} 