using FluentValidation;
using FoodDiary.UseCases.FoodEntries.Create;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.Validation;

public class CreateFoodEntryCommandValidator : AbstractValidator<CreateFoodEntryCommand>
{
    public CreateFoodEntryCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required");

        RuleFor(x => x.WeightGrams)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(10000)
            .WithMessage("Weight cannot exceed 10,000 grams");

        RuleFor(x => x.MealType)
            .IsInEnum()
            .WithMessage("Invalid meal type");

        RuleFor(x => x.ConsumedAt)
            .NotEmpty()
            .WithMessage("Consumption time is required")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .WithMessage("Consumption time cannot be in the future")
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddYears(-1))
            .WithMessage("Consumption time cannot be more than 1 year ago");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));
    }
} 