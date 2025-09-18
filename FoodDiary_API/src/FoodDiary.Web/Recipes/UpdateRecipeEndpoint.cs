using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FoodDiary.UseCases.Recipes;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Web.Recipes;

[Authorize]
public class UpdateRecipeEndpoint : Endpoint<UpdateRecipeRequest, UpdateRecipeResponse>
{
    private readonly IMediator _mediator;

    public UpdateRecipeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/recipes/{id}");
        Summary(s =>
        {
            s.Summary = "Update a recipe";
            s.Description = "Updates a recipe's details and ingredients. Supports both JSON and multipart/form-data. User can only update their own recipes.";
        });
    }

    public override async Task HandleAsync(UpdateRecipeRequest req, CancellationToken ct)
    {
        var idParam = Route<string>("id");
        if (string.IsNullOrEmpty(idParam) || !Guid.TryParse(idParam, out var recipeId))
        {
            await SendErrorsAsync(400, ct);
            return;
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            await SendErrorsAsync(401, ct);
            return;
        }

        if (HttpContext.Request.ContentType?.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase) == true)
        {
            await SendErrorsAsync(415, ct);
            return;
        }

        var command = new UpdateRecipeCommand
        {
            RecipeId = recipeId,
            UserId = userId,
            Name = req.Name?.Trim(),
            Description = req.Description?.Trim(),
            Category = !string.IsNullOrEmpty(req.Category) ? 
                (int.TryParse(req.Category, out var cat) ? (RecipeCategory)cat : Enum.Parse<RecipeCategory>(req.Category)) : 
                null,
            Servings = req.Servings,
            PreparationTimeMinutes = req.PreparationTimeMinutes,
            CookingTimeMinutes = req.CookingTimeMinutes,
            Instructions = req.Instructions?.Trim(),

            IsPublic = req.IsPublic,
            Ingredients = req.Ingredients?.Select(i => new UpdateRecipeIngredientRequest
            {
                IngredientId = i.IngredientId,
                ProductId = i.ProductId,
                QuantityGrams = i.QuantityGrams,
                Notes = i.Notes?.Trim(),
                CustomIngredientName = i.CustomIngredientName?.Trim(),
                CustomCaloriesPer100g = i.CustomCaloriesPer100g,
                CustomProteinPer100g = i.CustomProteinPer100g,
                CustomFatPer100g = i.CustomFatPer100g,
                CustomCarbohydratesPer100g = i.CustomCarbohydratesPer100g
            }).ToList()
        };

        var result = await _mediator.Send(command, ct);

        if (result.IsSuccess)
        {
            await SendAsync(result.Value, 200, ct);
        }
        else if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
        }
        else if (result.Status == ResultStatus.Forbidden)
        {
            await SendForbiddenAsync(ct);
        }
        else
        {
            var errorMessage = result.Errors.FirstOrDefault() ?? "Unknown error occurred";
            await SendErrorsAsync(400, ct);
        }
    }
}

public class UpdateRecipeRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int? Servings { get; set; }
    public int? PreparationTimeMinutes { get; set; }
    public int? CookingTimeMinutes { get; set; }
    public string? Instructions { get; set; }

    public bool? IsPublic { get; set; }
    public List<UpdateRecipeIngredientRequestDto>? Ingredients { get; set; }
}

public class UpdateRecipeIngredientRequestDto
{
    public Guid? IngredientId { get; set; }
    public Guid ProductId { get; set; }
    public double QuantityGrams { get; set; }
    public string? Notes { get; set; }
    public string? CustomIngredientName { get; set; }
    public double? CustomCaloriesPer100g { get; set; }
    public double? CustomProteinPer100g { get; set; }
    public double? CustomFatPer100g { get; set; }
    public double? CustomCarbohydratesPer100g { get; set; }
} 