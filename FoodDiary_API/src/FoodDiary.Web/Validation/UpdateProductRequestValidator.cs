using FluentValidation;
using FoodDiary.Web.Products;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.Web.Validation;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
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

        RuleFor(x => x.ProteinsPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Proteins per 100g cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Proteins per 100g cannot exceed 100g");

        RuleFor(x => x.FatsPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Fats per 100g cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Fats per 100g cannot exceed 100g");

        RuleFor(x => x.CarbohydratesPer100g)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Carbohydrates per 100g cannot be negative")
            .LessThanOrEqualTo(100)
            .WithMessage("Carbohydrates per 100g cannot exceed 100g");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .Matches(@"^[a-zA-Z0-9\s\-'\.\(\)\,\!\?]+$")
            .WithMessage("Description can only contain letters, numbers, spaces, hyphens, apostrophes, periods, parentheses, commas, exclamation marks, and question marks")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Category)
            .Must(category => string.IsNullOrEmpty(category) || 
                (int.TryParse(category, out var cat) && Enum.IsDefined(typeof(ProductCategory), cat)) || 
                Enum.TryParse<ProductCategory>(category, true, out _))
            .WithMessage("Category must be a valid product category (Fruits, Vegetables, Grains, etc.) or its numeric value (0-10)");

        RuleFor(x => x)
            .Must(x => x.ProteinsPer100g + x.FatsPer100g + x.CarbohydratesPer100g <= 100)
            .WithMessage("Total macronutrients cannot exceed 100g per 100g of product");
    }
}
