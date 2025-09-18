using FastEndpoints;
using MediatR;
using FoodDiary.UseCases.Products;
using FoodDiary.Core.ProductAggregate;
using Microsoft.AspNetCore.Authorization;
using FoodDiary.Web.Validation;

namespace FoodDiary.Web.Products;

[Authorize]
public class UpdateProductEndpoint : Endpoint<UpdateProductRequest, UpdateProductResponse>
{
    private readonly IMediator _mediator;

    public UpdateProductEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/products/{id}");
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Update a product";
            s.Description = "Updates a product's details. Can be called with JSON body (no image) or multipart/form-data (with image). Always use multipart/form-data for consistency with product creation.";
        });
    }

    public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
    {
        var idParam = Route<string>("id");
        if (string.IsNullOrEmpty(idParam) || !Guid.TryParse(idParam, out var productId))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        await HandleMultipartRequestAsync(productId, ct);
    }

    private async Task HandleMultipartRequestAsync(Guid productId, CancellationToken ct)
    {
        var name = Form["name"].FirstOrDefault() ?? "";
        var caloriesStr = Form["caloriesPer100g"].FirstOrDefault() ?? "0";
        var proteinsStr = Form["proteinsPer100g"].FirstOrDefault() ?? "0";
        var fatsStr = Form["fatsPer100g"].FirstOrDefault() ?? "0";
        var carbsStr = Form["carbohydratesPer100g"].FirstOrDefault() ?? "0";
        var description = Form["description"].FirstOrDefault();
        var category = Form["category"].FirstOrDefault() ?? "";

        if (!double.TryParse(caloriesStr, out var calories) || calories < 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        if (!double.TryParse(proteinsStr, out var proteins) || proteins < 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        if (!double.TryParse(fatsStr, out var fats) || fats < 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        if (!double.TryParse(carbsStr, out var carbs) || carbs < 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        byte[]? imageData = null;
        string? imageContentType = null;
        string? imageFileName = null;

        var imageFile = Form.Files.FirstOrDefault(f => f.Name == "image");
        if (imageFile != null && imageFile.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream, ct);
            imageData = memoryStream.ToArray();
            imageContentType = imageFile.ContentType;
            imageFileName = imageFile.FileName;
        }

        var command = new UpdateProductCommand
        {
            ProductId = productId,
            Name = name.Trim(),
            CaloriesPer100g = calories,
            ProteinsPer100g = proteins,
            FatsPer100g = fats,
            CarbohydratesPer100g = carbs,
            Description = description?.Trim(),
            Category = !string.IsNullOrEmpty(category) ? 
                (int.TryParse(category, out var cat) ? (ProductCategory)cat : Enum.Parse<ProductCategory>(category)) : 
                ProductCategory.Other,
            ImageData = imageData,
            ImageContentType = imageContentType,
            ImageFileName = imageFileName
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, cancellation: ct);
        }
        else if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
        }
        else
        {
            await SendAsync(result.Value, 400, cancellation: ct);
        }
    }
}

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public double CaloriesPer100g { get; set; }
    public double ProteinsPer100g { get; set; }
    public double FatsPer100g { get; set; }
    public double CarbohydratesPer100g { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
}
