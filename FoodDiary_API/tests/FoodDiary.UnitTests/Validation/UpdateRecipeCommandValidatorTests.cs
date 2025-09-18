using Xunit;
using FluentValidation;
using FoodDiary.UseCases.Recipes;
using FoodDiary.UseCases.Validation;
using FoodDiary.Core.RecipeAggregate;

namespace FoodDiary.UnitTests.Validation;

public class UpdateRecipeCommandValidatorTests
{
    private readonly UpdateRecipeCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Valid_Command()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "Updated Recipe",
            Description = "Updated description",
            Category = RecipeCategory.Breakfast,
            Servings = 4,
            PreparationTimeMinutes = 15,
            CookingTimeMinutes = 30,
            Instructions = "Updated cooking instructions that are long enough to meet the minimum requirement."
        };

        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_Name_Is_Empty(string name)
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = name
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.Name));
    }

    [Fact]
    public void Should_Fail_When_Name_Too_Short()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = "A"
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.Name));
    }

    [Fact]
    public void Should_Fail_When_Name_Too_Long()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Name = new string('a', 201)
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.Name));
    }

    [Fact]
    public void Should_Fail_When_Description_Too_Long()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Description = new string('a', 1001)
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.Description));
    }

    [Fact]
    public void Should_Fail_When_Instructions_Too_Short()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Instructions = "Short"
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.Instructions));
    }

    [Fact]
    public void Should_Fail_When_Instructions_Too_Long()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Instructions = new string('a', 2001)
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.Instructions));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(51)]
    public void Should_Fail_When_Servings_Invalid(int servings)
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Servings = servings
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.Servings));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(481)]
    public void Should_Fail_When_PreparationTime_Invalid(int time)
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PreparationTimeMinutes = time
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.PreparationTimeMinutes));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(481)]
    public void Should_Fail_When_CookingTime_Invalid(int time)
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            CookingTimeMinutes = time
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.CookingTimeMinutes));
    }



    [Fact]
    public void Should_Fail_When_RecipeId_Empty()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.Empty,
            UserId = Guid.NewGuid()
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.RecipeId));
    }

    [Fact]
    public void Should_Fail_When_UserId_Empty()
    {
        var command = new UpdateRecipeCommand
        {
            RecipeId = Guid.NewGuid(),
            UserId = Guid.Empty
        };

        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateRecipeCommand.UserId));
    }
}
