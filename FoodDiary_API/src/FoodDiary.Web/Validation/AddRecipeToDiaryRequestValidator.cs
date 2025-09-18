using FluentValidation;
using FoodDiary.Web.FoodEntries;

namespace FoodDiary.Web.Validation;

public class AddRecipeToDiaryRequestValidator : AbstractValidator<AddRecipeToDiaryRequest>
{
    public AddRecipeToDiaryRequestValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty()
            .WithMessage("Recipe ID is required.");

        RuleFor(x => x.MealType)
            .IsInEnum()
            .WithMessage("Invalid meal type.");

        RuleFor(x => x.ConsumedAt)
            .NotEmpty()
            .WithMessage("Consumption time is required.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Consumption time cannot be in the future.");

        RuleFor(x => x.ServingsConsumed)
            .GreaterThan(0)
            .WithMessage("Servings consumed must be greater than 0.")
            .LessThanOrEqualTo(20)
            .WithMessage("Servings consumed cannot exceed 20.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters.");
    }
} 