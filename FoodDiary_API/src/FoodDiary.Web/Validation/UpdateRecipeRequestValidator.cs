using FluentValidation;
using FoodDiary.Web.Recipes;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.Web.Validation;

public class UpdateRecipeRequestValidator : AbstractValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Name cannot exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.Category)
            .Must(category => string.IsNullOrEmpty(category) || 
                (int.TryParse(category, out var cat) && Enum.IsDefined(typeof(RecipeCategory), cat)) || 
                Enum.TryParse<RecipeCategory>(category, true, out _))
            .WithMessage("Category must be a valid recipe category (Breakfast, Lunch, Dinner, etc.) or its numeric value (0-24)");

        RuleFor(x => x.Servings)
            .InclusiveBetween(1, 50)
            .When(x => x.Servings.HasValue)
            .WithMessage("Servings must be between 1 and 50");

        RuleFor(x => x.PreparationTimeMinutes)
            .InclusiveBetween(0, 480)
            .When(x => x.PreparationTimeMinutes.HasValue)
            .WithMessage("Preparation time must be between 0 and 480 minutes");

        RuleFor(x => x.CookingTimeMinutes)
            .InclusiveBetween(0, 480)
            .When(x => x.CookingTimeMinutes.HasValue)
            .WithMessage("Cooking time must be between 0 and 480 minutes");

        RuleFor(x => x.Instructions)
            .MaximumLength(5000)
            .When(x => !string.IsNullOrEmpty(x.Instructions))
            .WithMessage("Instructions cannot exceed 5000 characters");

        RuleForEach(x => x.Ingredients)
            .SetValidator(new UpdateRecipeIngredientRequestDtoValidator())
            .When(x => x.Ingredients != null);
    }
}

public class UpdateRecipeIngredientRequestDtoValidator : AbstractValidator<UpdateRecipeIngredientRequestDto>
{
    public UpdateRecipeIngredientRequestDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.QuantityGrams)
            .GreaterThan(0)
            .LessThanOrEqualTo(10000)
            .WithMessage("Quantity must be between 0.1 and 10000 grams");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");

        RuleFor(x => x.CustomIngredientName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.CustomIngredientName))
            .WithMessage("Custom ingredient name cannot exceed 100 characters");

        RuleFor(x => x.CustomCaloriesPer100g)
            .InclusiveBetween(0, 900)
            .When(x => x.CustomCaloriesPer100g.HasValue)
            .WithMessage("Custom calories must be between 0 and 900 per 100g");

        RuleFor(x => x.CustomProteinPer100g)
            .InclusiveBetween(0, 100)
            .When(x => x.CustomProteinPer100g.HasValue)
            .WithMessage("Custom protein must be between 0 and 100 per 100g");

        RuleFor(x => x.CustomFatPer100g)
            .InclusiveBetween(0, 100)
            .When(x => x.CustomFatPer100g.HasValue)
            .WithMessage("Custom fat must be between 0 and 100 per 100g");

        RuleFor(x => x.CustomCarbohydratesPer100g)
            .InclusiveBetween(0, 100)
            .When(x => x.CustomCarbohydratesPer100g.HasValue)
            .WithMessage("Custom carbohydrates must be between 0 and 100 per 100g");
    }
}
