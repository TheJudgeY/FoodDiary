using Xunit;
using FluentValidation;
using FoodDiary.UseCases.Products;
using FoodDiary.UseCases.Validation;

namespace FoodDiary.UnitTests.Validation;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Valid_Command()
    {
        var command = new CreateProductCommand
        {
            Name = "Apple",
            CaloriesPer100g = 52,
            Description = "Fresh apple"
        };
        var result = _validator.Validate(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Fail_When_Name_Is_Empty(string name)
    {
        var command = new CreateProductCommand
        {
            Name = name,
            CaloriesPer100g = 52
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Should_Fail_When_Name_Is_Null()
    {
        var command = new CreateProductCommand
        {
            Name = null!,
            CaloriesPer100g = 52
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.Name));
    }

    [Fact]
    public void Should_Fail_When_Calories_Negative()
    {
        var command = new CreateProductCommand
        {
            Name = "Apple",
            CaloriesPer100g = -10
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.CaloriesPer100g));
    }

    [Fact]
    public void Should_Fail_When_Description_Too_Long()
    {
        var command = new CreateProductCommand
        {
            Name = "Apple",
            CaloriesPer100g = 52,
            Description = new string('a', 501)
        };
        var result = _validator.Validate(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateProductCommand.Description));
    }
} 