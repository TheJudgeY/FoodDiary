using FluentValidation;
using FoodDiary.UseCases.Users.UpdateFitnessGoal;

namespace FoodDiary.UseCases.Validation;

public class UpdateFitnessGoalCommandValidator : AbstractValidator<UpdateFitnessGoalCommand>
{
    public UpdateFitnessGoalCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.FitnessGoal)
            .IsInEnum()
            .WithMessage("Invalid fitness goal value");

        RuleFor(x => x.TargetWeightKg)
            .GreaterThan(0)
            .WithMessage("Target weight must be positive")
            .LessThanOrEqualTo(500)
            .WithMessage("Target weight cannot exceed 500 kg")
            .When(x => x.TargetWeightKg.HasValue);
    }
} 