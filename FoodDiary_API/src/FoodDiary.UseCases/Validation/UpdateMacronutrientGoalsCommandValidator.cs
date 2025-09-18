using FluentValidation;
using FoodDiary.UseCases.Users.UpdateMacronutrientGoals;

namespace FoodDiary.UseCases.Validation;

public class UpdateMacronutrientGoalsCommandValidator : AbstractValidator<UpdateMacronutrientGoalsCommand>
{
    public UpdateMacronutrientGoalsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.DailyProteinGoal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Daily protein goal cannot be negative")
            .LessThanOrEqualTo(1000)
            .WithMessage("Daily protein goal cannot exceed 1000g")
            .When(x => x.DailyProteinGoal.HasValue);

        RuleFor(x => x.DailyFatGoal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Daily fat goal cannot be negative")
            .LessThanOrEqualTo(500)
            .WithMessage("Daily fat goal cannot exceed 500g")
            .When(x => x.DailyFatGoal.HasValue);

        RuleFor(x => x.DailyCarbohydrateGoal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Daily carbohydrate goal cannot be negative")
            .LessThanOrEqualTo(1000)
            .WithMessage("Daily carbohydrate goal cannot exceed 1000g")
            .When(x => x.DailyCarbohydrateGoal.HasValue);
    }
} 