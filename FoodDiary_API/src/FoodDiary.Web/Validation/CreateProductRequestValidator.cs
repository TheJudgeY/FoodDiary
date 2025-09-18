using FluentValidation;
using FoodDiary.Web.Products;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.Web.Validation;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required")
            .MinimumLength(2)
            .WithMessage("Product name must be at least 2 characters long")
            .MaximumLength(100)
            .WithMessage("Product name cannot exceed 100 characters")
            .Matches(@"^[a-zA-Z0-9\s\-'\.\(\)]+$")
            .WithMessage("Product name can only contain letters, numbers, spaces, hyphens, apostrophes, periods, and parentheses");

        RuleFor(x => x.CaloriesPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Calories per 100g cannot be negative")
            .LessThanOrEqualTo(10000)
            .WithMessage("Calories per 100g cannot exceed 10,000");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .Matches(@"^[a-zA-Z0-9\s\-'\.\(\)\,\!\?]+$")
            .WithMessage("Description can only contain letters, numbers, spaces, hyphens, apostrophes, periods, parentheses, commas, exclamation marks, and question marks")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .Must(category => string.IsNullOrEmpty(category) || Enum.TryParse<ProductCategory>(category, true, out _))
            .WithMessage("Category must be a valid product category (Fruits, Vegetables, Grains, Proteins, Dairy, NutsAndSeeds, Beverages, Snacks, Condiments, Supplements, Other)")
            .When(x => !string.IsNullOrWhiteSpace(x.Category));
    }
} 