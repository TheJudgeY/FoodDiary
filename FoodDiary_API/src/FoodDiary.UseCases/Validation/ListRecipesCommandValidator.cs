using FluentValidation;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.UseCases.Validation;

public class ListRecipesCommandValidator : AbstractValidator<ListRecipesCommand>
{
    public ListRecipesCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Page must be greater than 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size cannot exceed 100");

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .WithMessage("Search term cannot exceed 100 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));

        RuleFor(x => x.SortBy)
            .Must(BeValidSortBy)
            .WithMessage("Invalid sort by option. Valid options are: Name, CreatedAt, UpdatedAt, TotalCalories")
            .When(x => !string.IsNullOrWhiteSpace(x.SortBy));
    }

    private static bool BeValidSortBy(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return true;
        
        var validSortOptions = new[] { "Name", "CreatedAt", "UpdatedAt", "TotalCalories" };
        return validSortOptions.Contains(sortBy, StringComparer.OrdinalIgnoreCase);
    }
}
