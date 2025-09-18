using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Recipes;
using FoodDiary.Web.Validation;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Web.Recipes;

[Authorize]
public class CreateRecipeEndpoint : Endpoint<EmptyRequest, CreateRecipeResponse>
{
    private readonly IMediator _mediator;

    public CreateRecipeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/recipes");
        AllowFileUploads();
        Summary(s =>
        {
            s.Summary = "Create a new recipe";
            s.Description = "Creates a new recipe with ingredients for the authenticated user. Always use multipart/form-data for consistency with products. Supports optional image upload.";
        });
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        await HandleMultipartRequestAsync(userId, ct);
    }

    private async Task HandleMultipartRequestAsync(Guid userId, CancellationToken ct)
    {
        var name = Form["name"].FirstOrDefault() ?? "";
        var description = Form["description"].FirstOrDefault();
        var category = Form["category"].FirstOrDefault() ?? "";
        var servingsStr = Form["servings"].FirstOrDefault() ?? "0";
        var prepTimeStr = Form["preparationTimeMinutes"].FirstOrDefault() ?? "0";
        var cookTimeStr = Form["cookingTimeMinutes"].FirstOrDefault() ?? "0";
        var instructions = Form["instructions"].FirstOrDefault() ?? "";
        var isPublicStr = Form["isPublic"].FirstOrDefault() ?? "false";

        if (!int.TryParse(servingsStr, out var servings) || servings <= 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        if (!int.TryParse(prepTimeStr, out var prepTime) || prepTime <= 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        if (!int.TryParse(cookTimeStr, out var cookTime) || cookTime <= 0)
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var isPublic = bool.TryParse(isPublicStr, out var isPublicValue) && isPublicValue;

        var ingredients = new List<CreateRecipeIngredientRequest>();
        var ingredientIndex = 0;
        
        while (true)
        {
            var productIdStr = Form[$"ingredients[{ingredientIndex}][productId]"].FirstOrDefault();
            var quantityStr = Form[$"ingredients[{ingredientIndex}][quantityGrams]"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(productIdStr) || string.IsNullOrEmpty(quantityStr))
                break;

            if (Guid.TryParse(productIdStr, out var productId) && 
                double.TryParse(quantityStr, out var quantity) && quantity > 0)
            {
                var notes = Form[$"ingredients[{ingredientIndex}][notes]"].FirstOrDefault();
                
                ingredients.Add(new CreateRecipeIngredientRequest
                {
                    ProductId = productId,
                    QuantityGrams = quantity,
                    Notes = notes?.Trim()
                });
            }
            
            ingredientIndex++;
        }

        if (ingredients.Count == 0)
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

        var command = new CreateRecipeCommand
        {
            UserId = userId,
            Name = name.Trim(),
            Description = description?.Trim(),
            Category = !string.IsNullOrEmpty(category) ? 
                (int.TryParse(category, out var cat) ? (RecipeCategory)cat : Enum.Parse<RecipeCategory>(category)) : 
                RecipeCategory.Other,
            Servings = servings,
            PreparationTimeMinutes = prepTime,
            CookingTimeMinutes = cookTime,
            Instructions = instructions.Trim(),
            IsPublic = isPublic,
            ImageData = imageData,
            ImageContentType = imageContentType,
            ImageFileName = imageFileName,
            Ingredients = ingredients
        };

        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
        {
            var errorMessage = result.Errors.FirstOrDefault() ?? "Unknown error occurred";
            await SendErrorsAsync(400, ct);
            return;
        }

        await SendAsync(result.Value, 201, ct);
    }


}

 