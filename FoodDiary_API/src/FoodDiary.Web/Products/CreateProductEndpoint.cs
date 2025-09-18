using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using FoodDiary.UseCases.Products;
using FoodDiary.Web.Validation;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.Web.Products;

[Authorize]
public class CreateProductEndpoint : Endpoint<CreateProductRequest, CreateProductResponse>
{
    private readonly IMediator _mediator;
    private readonly AutoMapper.IMapper _mapper;

    public CreateProductEndpoint(IMediator mediator, AutoMapper.IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public override void Configure()
    {
        Post("/api/products");
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Create a new product";
            s.Description = "Creates a new product with optional image upload. Can be called with JSON body (no image) or multipart/form-data (with image)";
            s.ExampleRequest = new CreateProductRequest
            {
                Name = "Apple",
                CaloriesPer100g = 52,
                Description = "Fresh red apple"
            };
        });
    }

    public override async Task HandleAsync(CreateProductRequest req, CancellationToken ct)
    {
        if (!await FastEndpointsValidationService.ValidateRequestAsync(req, HttpContext))
        {
            return;
        }

        byte[]? imageData = null;
        string? imageContentType = null;
        string? imageFileName = null;

        if (HttpContext.Request.ContentType?.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) == true)
        {
            var imageFile = Form.Files.FirstOrDefault(f => f.Name == "image");
            if (imageFile != null && imageFile.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await imageFile.CopyToAsync(memoryStream, ct);
                imageData = memoryStream.ToArray();
                imageContentType = imageFile.ContentType;
                imageFileName = imageFile.FileName;
            }
        }

        var command = new CreateProductCommand
        {
            Name = req.Name.Trim(),
            CaloriesPer100g = req.CaloriesPer100g,
            ProteinsPer100g = req.ProteinsPer100g,
            FatsPer100g = req.FatsPer100g,
            CarbohydratesPer100g = req.CarbohydratesPer100g,
            Description = req.Description?.Trim(),
            Category = !string.IsNullOrEmpty(req.Category) ? Enum.Parse<ProductCategory>(req.Category) : ProductCategory.Other,
            ImageData = imageData,
            ImageContentType = imageContentType,
            ImageFileName = imageFileName
        };

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Errors.FirstOrDefault() ?? "Unknown error occurred";
            await SendErrorsAsync(400, ct);
            return;
        }

        var response = _mapper.Map<CreateProductResponse>(result.Value);

        await SendAsync(response, 201, ct);
    }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public double CaloriesPer100g { get; set; }
    public double ProteinsPer100g { get; set; }
    public double FatsPer100g { get; set; }
    public double CarbohydratesPer100g { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
}

public class CreateProductResponse
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double CaloriesPer100g { get; set; }
    public double ProteinsPer100g { get; set; }
    public double FatsPer100g { get; set; }
    public double CarbohydratesPer100g { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
} 