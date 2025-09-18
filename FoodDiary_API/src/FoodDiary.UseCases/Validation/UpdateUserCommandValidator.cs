using FluentValidation;
using FoodDiary.UseCases.Users.UpdateUser;

namespace FoodDiary.UseCases.Validation;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        When(x => !string.IsNullOrWhiteSpace(x.Name), () =>
        {
            RuleFor(x => x.Name)
                .MaximumLength(100)
                .WithMessage("Name cannot exceed 100 characters");
        });

        When(x => x.HeightCm.HasValue || x.WeightKg.HasValue || x.Age.HasValue || 
                  x.Gender.HasValue || x.ActivityLevel.HasValue, () =>
        {
            RuleFor(x => x.HeightCm)
                .GreaterThan(0)
                .WithMessage("Height must be positive")
                .LessThanOrEqualTo(300)
                .WithMessage("Height cannot exceed 300 cm")
                .When(x => x.HeightCm.HasValue);

            RuleFor(x => x.WeightKg)
                .GreaterThan(0)
                .WithMessage("Weight must be positive")
                .LessThanOrEqualTo(500)
                .WithMessage("Weight cannot exceed 500 kg")
                .When(x => x.WeightKg.HasValue);

            RuleFor(x => x.Age)
                .GreaterThan(0)
                .WithMessage("Age must be positive")
                .LessThanOrEqualTo(150)
                .WithMessage("Age cannot exceed 150 years")
                .When(x => x.Age.HasValue);

            RuleFor(x => x.Gender)
                .IsInEnum()
                .WithMessage("Invalid gender value")
                .When(x => x.Gender.HasValue);

            RuleFor(x => x.ActivityLevel)
                .IsInEnum()
                .WithMessage("Invalid activity level value")
                .When(x => x.ActivityLevel.HasValue);
        });

        When(x => x.FitnessGoal.HasValue, () =>
        {
            RuleFor(x => x.FitnessGoal)
                .IsInEnum()
                .WithMessage("Invalid fitness goal value");

            RuleFor(x => x.TargetWeightKg)
                .GreaterThan(0)
                .WithMessage("Target weight must be positive")
                .LessThanOrEqualTo(500)
                .WithMessage("Target weight cannot exceed 500 kg")
                .When(x => x.TargetWeightKg.HasValue);
        });

        When(x => x.DailyProteinGoal.HasValue || x.DailyFatGoal.HasValue || 
                  x.DailyCarbohydrateGoal.HasValue || x.DailyCalorieGoal.HasValue, () =>
        {
            RuleFor(x => x.DailyCalorieGoal)
                .GreaterThan(0)
                .WithMessage("Daily calorie goal must be positive")
                .LessThanOrEqualTo(10000)
                .WithMessage("Daily calorie goal cannot exceed 10,000")
                .When(x => x.DailyCalorieGoal.HasValue);

            RuleFor(x => x.DailyProteinGoal)
                .GreaterThan(0)
                .WithMessage("Daily protein goal must be positive")
                .LessThanOrEqualTo(1000)
                .WithMessage("Daily protein goal cannot exceed 1,000g")
                .When(x => x.DailyProteinGoal.HasValue);

            RuleFor(x => x.DailyFatGoal)
                .GreaterThan(0)
                .WithMessage("Daily fat goal must be positive")
                .LessThanOrEqualTo(500)
                .WithMessage("Daily fat goal cannot exceed 500g")
                .When(x => x.DailyFatGoal.HasValue);

            RuleFor(x => x.DailyCarbohydrateGoal)
                .GreaterThan(0)
                .WithMessage("Daily carbohydrate goal must be positive")
                .LessThanOrEqualTo(2000)
                .WithMessage("Daily carbohydrate goal cannot exceed 2,000g")
                .When(x => x.DailyCarbohydrateGoal.HasValue);
        });

        When(x => !string.IsNullOrWhiteSpace(x.TimeZoneId), () =>
        {
            RuleFor(x => x.TimeZoneId)
                .Must(BeValidTimeZone)
                .WithMessage("Invalid timezone ID");
        });
    }

    private bool BeValidTimeZone(string? timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
            return false;
            
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
