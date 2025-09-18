using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Products;

namespace FoodDiary.Web.Products;

[Authorize]
public class GetProductImageEndpoint : Endpoint<GetProductImageRequest, GetProductImageResponse>
{
    private readonly IMediator _mediator;

    public GetProductImageEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/products/{productId}/image");
        Summary(s =>
        {
            s.Summary = "Get image for a product";
            s.Description = "Retrieves the image for a specific product.";
        });
    }

    public override async Task HandleAsync(GetProductImageRequest request, CancellationToken cancellationToken)
    {
        var command = new GetProductImageCommand
        {
            ProductId = request.ProductId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            var response = new GetProductImageResponse
            {
                ProductId = result.Value.ProductId,
                ImageDataUrl = result.Value.ImageDataUrl,
                ImageContentType = result.Value.ImageContentType,
                ImageFileName = result.Value.ImageFileName,
                ImageSizeInBytes = result.Value.ImageSizeInBytes
            };

            await SendAsync(response, 200, cancellationToken);
        }
        else
        {
            await SendNotFoundAsync(cancellationToken);
        }
    }
}

public record GetProductImageRequest
{
    public Guid ProductId { get; init; }
}

public record GetProductImageResponse
{
    public Guid ProductId { get; init; }
    public string ImageDataUrl { get; init; } = string.Empty;
    public string? ImageContentType { get; init; }
    public string? ImageFileName { get; init; }
    public long ImageSizeInBytes { get; init; }
} 