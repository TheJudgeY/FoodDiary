using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.Products;

namespace FoodDiary.Web.Products;

[Authorize]
public class GetProductEndpoint : Endpoint<GetProductRequest, GetProductResponse>
{
    private readonly IMediator _mediator;
    private readonly IRepository<Product> _productRepository;
    private readonly AutoMapper.IMapper _mapper;

    public GetProductEndpoint(IMediator mediator, IRepository<Product> productRepository, AutoMapper.IMapper mapper)
    {
        _mediator = mediator;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Get("/api/products/{id}");
        Summary(s =>
        {
            s.Summary = "Get a product by ID";
            s.Description = "Retrieves a specific product with optional image data";
        });
    }

    public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        var productResult = await _productRepository.GetByIdAsync(req.Id);
        if (!productResult.IsSuccess)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var product = productResult.Value;

        var includeImage = Query<bool>("includeImage", isRequired: false);

        if (includeImage && product.ImageData != null && product.ImageData.Length > 0)
        {
            HttpContext.Response.ContentType = product.ImageContentType ?? "image/jpeg";
            await HttpContext.Response.Body.WriteAsync(product.ImageData, ct);
            return;
        }

        var productDto = _mapper.Map<ProductDTO>(product);

        var response = new GetProductResponse
        {
            Product = productDto
        };

        await SendAsync(response, 200, ct);
    }
}

public class GetProductRequest
{
    public Guid Id { get; set; }
}

public class GetProductResponse
{
    public ProductDTO Product { get; set; } = new();
} 