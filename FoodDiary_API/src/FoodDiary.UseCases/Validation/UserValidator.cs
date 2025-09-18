using FluentValidation;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("User name cannot be empty")
            .MaximumLength(100)
            .WithMessage("User name cannot exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email cannot be empty")
            .EmailAddress()
            .WithMessage("Email must be a valid email address")
            .MaximumLength(255)
            .WithMessage("Email cannot exceed 255 characters");

        RuleFor(x => x.PasswordHash)
            .NotEmpty()
            .WithMessage("Password hash cannot be empty");

        RuleFor(x => x.HeightCm)
            .GreaterThan(0)
            .WithMessage("Height must be greater than 0")
            .LessThanOrEqualTo(300)
            .WithMessage("Height cannot exceed 300 cm")
            .When(x => x.HeightCm.HasValue);

        RuleFor(x => x.WeightKg)
            .GreaterThan(0)
            .WithMessage("Weight must be greater than 0")
            .LessThanOrEqualTo(500)
            .WithMessage("Weight cannot exceed 500 kg")
            .When(x => x.WeightKg.HasValue);

        RuleFor(x => x.Age)
            .GreaterThan(0)
            .WithMessage("Age must be greater than 0")
            .LessThanOrEqualTo(150)
            .WithMessage("Age cannot exceed 150")
            .When(x => x.Age.HasValue);

        RuleFor(x => x.DailyProteinGoal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Daily protein goal cannot be negative")
            .When(x => x.DailyProteinGoal.HasValue);

        RuleFor(x => x.DailyFatGoal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Daily fat goal cannot be negative")
            .When(x => x.DailyFatGoal.HasValue);

        RuleFor(x => x.DailyCarbohydrateGoal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Daily carbohydrate goal cannot be negative")
            .When(x => x.DailyCarbohydrateGoal.HasValue);
    }
} 