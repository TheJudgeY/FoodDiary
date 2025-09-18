using FluentValidation;
using FoodDiary.UseCases.Users.UpdateBodyMetrics;

namespace FoodDiary.UseCases.Validation;

public class UpdateBodyMetricsCommandValidator : AbstractValidator<UpdateBodyMetricsCommand>
{
    public UpdateBodyMetricsCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.HeightCm)
            .GreaterThan(0)
            .WithMessage("Height must be positive")
            .LessThanOrEqualTo(300)
            .WithMessage("Height cannot exceed 300 cm");

        RuleFor(x => x.WeightKg)
            .GreaterThan(0)
            .WithMessage("Weight must be positive")
            .LessThanOrEqualTo(500)
            .WithMessage("Weight cannot exceed 500 kg");

        RuleFor(x => x.Age)
            .GreaterThan(0)
            .WithMessage("Age must be positive")
            .LessThanOrEqualTo(150)
            .WithMessage("Age cannot exceed 150 years");

        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage("Invalid gender value");

        RuleFor(x => x.ActivityLevel)
            .IsInEnum()
            .WithMessage("Invalid activity level value");
    }
} 