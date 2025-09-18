using FluentValidation;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.UseCases.Validation;

public class GetRecipeCommandValidator : AbstractValidator<GetRecipeCommand>
{
    public GetRecipeCommandValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty()
            .WithMessage("Recipe ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
