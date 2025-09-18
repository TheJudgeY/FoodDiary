using FluentValidation;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UseCases.Validation;

public class RecipeValidator : AbstractValidator<Recipe>
{
    public RecipeValidator()
    {
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
            .WithMessage("Invalid recipe category");

        RuleFor(x => x.Servings)
            .GreaterThan(0)
            .WithMessage("Servings must be between 1 and 50")
            .LessThanOrEqualTo(50)
            .WithMessage("Servings must be between 1 and 50");

        RuleFor(x => x.PreparationTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Preparation time must be between 1 and 480 minutes")
            .LessThanOrEqualTo(480)
            .WithMessage("Preparation time must be between 1 and 480 minutes");

        RuleFor(x => x.CookingTimeMinutes)
            .GreaterThan(0)
            .WithMessage("Cooking time must be between 1 and 480 minutes")
            .LessThanOrEqualTo(480)
            .WithMessage("Cooking time must be between 1 and 480 minutes");

        RuleFor(x => x.Instructions)
            .NotEmpty()
            .WithMessage("Instructions cannot be empty")
            .MinimumLength(10)
            .WithMessage("Recipe instructions must be at least 10 characters long")
            .MaximumLength(2000)
            .WithMessage("Recipe instructions cannot exceed 2000 characters");


    }


} 