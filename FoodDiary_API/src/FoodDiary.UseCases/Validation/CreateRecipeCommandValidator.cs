using FluentValidation;
using FoodDiary.UseCases.Recipes;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Validation;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Recipe name is required")
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
            .WithMessage("Invalid recipe category");

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be greater than 0")
            .LessThanOrEqualTo(50)
            .WithMessage("Servings cannot exceed 50");

        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Preparation time must be greater than 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Preparation time cannot exceed 480 minutes");

        RuleFor(x => x.CookingTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Cooking time must be greater than 0")
            .LessThanOrEqualTo(480)
            .WithMessage("Cooking time cannot exceed 480 minutes");

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .WithMessage("Recipe instructions are required")
            .MinimumLength(10)
            .WithMessage("Recipe instructions must be at least 10 characters long")
            .MaximumLength(2000)
            .WithMessage("Recipe instructions cannot exceed 2000 characters");



        RuleFor(x => x.Ingredients)
            .NotEmpty()
            .WithMessage("Recipe must have at least one ingredient")
            .Must(x => x.Count <= 50)
            .WithMessage("Recipe cannot have more than 50 ingredients");

        RuleForEach(x => x.Ingredients)
            .SetValidator(new CreateRecipeIngredientRequestValidator());
    }


}

public class CreateRecipeIngredientRequestValidator : AbstractValidator<CreateRecipeIngredientRequest>
{
    public CreateRecipeIngredientRequestValidator()
    {
        RuleFor(x => x.QuantityGrams)
            .GreaterThan(0)
            .WithMessage("Ingredient quantity must be greater than 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Ingredient quantity cannot exceed 10000 grams");

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
            .WithMessage("Each ingredient must have either a product ID or custom ingredient name");

        RuleFor(x => x)
            .Must(HaveValidCustomNutritionalData)
            .WithMessage("Custom ingredient must have nutritional data when no product ID is provided")
            .When(x => x.ProductId == null || x.ProductId == Guid.Empty);
    }

    private static bool HaveValidIngredientReference(CreateRecipeIngredientRequest request)
    {
        return (request.ProductId.HasValue && request.ProductId.Value != Guid.Empty) ||
               !string.IsNullOrWhiteSpace(request.CustomIngredientName);
    }

    private static bool HaveValidCustomNutritionalData(CreateRecipeIngredientRequest request)
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
