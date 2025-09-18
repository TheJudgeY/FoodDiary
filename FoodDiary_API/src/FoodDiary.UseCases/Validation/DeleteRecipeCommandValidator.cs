using FluentValidation;
using FoodDiary.UseCases.Recipes;

namespace FoodDiary.UseCases.Validation;

public class DeleteRecipeCommandValidator : AbstractValidator<DeleteRecipeCommand>
{
    public DeleteRecipeCommandValidator()
    {
        RuleFor(x => x.RecipeId)
            .NotEmpty()
            .WithMessage("Recipe ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");
    }
}
