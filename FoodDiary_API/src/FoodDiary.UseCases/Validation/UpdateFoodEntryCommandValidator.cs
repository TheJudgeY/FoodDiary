using FluentValidation;
using FoodDiary.UseCases.FoodEntries.Update;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.Validation;

public class UpdateFoodEntryCommandValidator : AbstractValidator<UpdateFoodEntryCommand>
{
    public UpdateFoodEntryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Food entry ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.WeightGrams)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0");

        RuleFor(x => x.MealType)
            .IsInEnum()
            .WithMessage("Invalid meal type");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");

        RuleFor(x => x.ConsumedAt)
            .Must(consumedAt => !consumedAt.HasValue || consumedAt.Value > DateTime.MinValue)
            .When(x => x.ConsumedAt.HasValue)
            .WithMessage("ConsumedAt must be a valid date");
    }
}
