using FluentValidation;
using FoodDiary.UseCases.Recipes;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Validation;

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeCommandValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty()
            .WithMessage("Recipe ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Recipe name cannot be empty")
            .MinimumLength(2)
            .WithMessage("Recipe name must be at least 2 characters long")
            .MaximumLength(200)
            .WithMessage("Recipe name cannot exceed 200 characters")
            .Matches(@"^[a-zA-Z0-9\s\-'\.\(\)]+$")
            .WithMessage("Recipe name contains invalid characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .Matches(@"^[a-zA-Z0-9\s\-'\.\(\)\,\!\?]+$")
            .WithMessage("Description contains invalid characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid recipe category")
            .When(x => x.Category.HasValue);

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("Servings cannot exceed 50")
            .When(x => x.Servings.HasValue);

        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Preparation time must be greater than 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Preparation time cannot exceed 480 minutes")
            .When(x => x.PreparationTimeMinutes.HasValue);

        RuleFor(x => x.CookingTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Cooking time must be greater than 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Cooking time cannot exceed 480 minutes")
            .When(x => x.CookingTimeMinutes.HasValue);

        RuleFor(x => x.Instructions)
            .MinimumLength(10)
            .WithMessage("Recipe instructions must be at least 10 characters long")
            .MaximumLength(2000)
            .WithMessage("Recipe instructions cannot exceed 2000 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Instructions));



        RuleFor(x => x.Ingredients)
            .Must(x => x == null || x.Count <= 50)
            .WithMessage("Recipe cannot have more than 50 ingredients")
            .When(x => x.Ingredients != null);

        RuleForEach(x => x.Ingredients)
            .SetValidator(new UpdateRecipeIngredientRequestValidator())
            .When(x => x.Ingredients != null);
    }


}

public class UpdateRecipeIngredientRequestValidator : AbstractValidator<UpdateRecipeIngredientRequest>
{
    public UpdateRecipeIngredientRequestValidator()
    {
        RuleFor(x => x.QuantityGrams)
            .GreaterThan(0)
            .WithMessage("Ingredient quantity must be greater than 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Ingredient quantity cannot exceed 10000 grams")
            .When(x => x.QuantityGrams.HasValue);

        RuleFor(x => x.Notes)
            .MaximumLength(200)
            .WithMessage("Ingredient notes cannot exceed 200 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Notes));

        RuleFor(x => x.CustomIngredientName)
            .MaximumLength(100)
            .WithMessage("Custom ingredient name cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CustomIngredientName))
            .Matches(@"^[a-zA-Z0-9\s\-'\.\(\)]+$")
            .WithMessage("Custom ingredient name contains invalid characters")
            .When(x => !string.IsNullOrWhiteSpace(x.CustomIngredientName));

        RuleFor(x => x.CustomCaloriesPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Custom calories per 100g cannot be negative")
            .LessThanOrEqualTo(10000)
            .WithMessage("Custom calories per 100g cannot exceed 10,000")
            .When(x => x.CustomCaloriesPer100g.HasValue);

        RuleFor(x => x.CustomProteinPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Custom protein per 100g cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Custom protein per 100g cannot exceed 100g")
            .When(x => x.CustomProteinPer100g.HasValue);

        RuleFor(x => x.CustomFatPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Custom fat per 100g cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Custom fat per 100g cannot exceed 100g")
            .When(x => x.CustomFatPer100g.HasValue);

        RuleFor(x => x.CustomCarbohydratesPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Custom carbohydrates per 100g cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Custom carbohydrates per 100g cannot exceed 100g")
            .When(x => x.CustomCarbohydratesPer100g.HasValue);

        RuleFor(x => x)
            .Must(HaveValidIngredientReference)
            .WithMessage("Each ingredient must have either a product ID or custom ingredient name")
            .When(x => x.ProductId.HasValue || !string.IsNullOrWhiteSpace(x.CustomIngredientName));

        RuleFor(x => x)
            .Must(HaveValidCustomNutritionalData)
            .WithMessage("Custom ingredient must have nutritional data when no product ID is provided")
            .When(x => (x.ProductId == null || x.ProductId == Guid.Empty) && !string.IsNullOrWhiteSpace(x.CustomIngredientName));
    }

    private static bool HaveValidIngredientReference(UpdateRecipeIngredientRequest request)
    {
        return (request.ProductId.HasValue && request.ProductId.Value != Guid.Empty) ||
               !string.IsNullOrWhiteSpace(request.CustomIngredientName);
    }

    private static bool HaveValidCustomNutritionalData(UpdateRecipeIngredientRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.CustomIngredientName))
        {
            return request.CustomCaloriesPer100g.HasValue ||
                   request.CustomProteinPer100g.HasValue ||
                   request.CustomFatPer100g.HasValue ||
                   request.CustomCarbohydratesPer100g.HasValue;
        }
        return true;
    }
}
