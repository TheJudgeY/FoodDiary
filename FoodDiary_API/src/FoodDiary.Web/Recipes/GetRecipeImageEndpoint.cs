using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.Web.Recipes;

[Authorize]
public class GetRecipeImageEndpoint : Endpoint<GetRecipeImageRequest, GetRecipeImageResponse>
{
    private readonly IMediator _mediator;

    public GetRecipeImageEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/recipes/{recipeId}/image");
        Summary(s =>
        {
            s.Summary = "Get image for a recipe";
            s.Description = "Retrieves the image for a specific recipe.";
        });
    }

    public override async Task HandleAsync(GetRecipeImageRequest request, CancellationToken cancellationToken)
    {
        var command = new GetRecipeImageCommand
        {
            RecipeId = request.RecipeId
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            var response = new GetRecipeImageResponse
            {
                RecipeId = result.Value.RecipeId,
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

public record GetRecipeImageRequest
{
    public Guid RecipeId { get; init; }
}

public record GetRecipeImageResponse
{
    public Guid RecipeId { get; init; }
    public string ImageDataUrl { get; init; } = string.Empty;
    public string? ImageContentType { get; init; }
    public string? ImageFileName { get; init; }
    public long ImageSizeInBytes { get; init; }
} 