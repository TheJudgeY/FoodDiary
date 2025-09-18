using Xunit;
using FluentValidation.TestHelper;
using FoodDiary.UseCases.FoodEntries.Create;
using FoodDiary.UseCases.Validation;
using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UnitTests.Validation;

public class CreateFoodEntryCommandValidatorTests
{
    private readonly CreateFoodEntryCommandValidator _validator;

    public CreateFoodEntryCommandValidatorTests()
    {
        _validator = new CreateFoodEntryCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Valid_Command()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow,
            Notes = "Delicious breakfast"
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_UserId_Is_Empty()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.Empty,
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Fail_When_ProductId_Is_Empty()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.Empty,
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-100.0)]
    public void Should_Fail_When_WeightGrams_Is_Not_Positive(double weightGrams)
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = weightGrams,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.WeightGrams);
    }

    [Fact]
    public void Should_Fail_When_WeightGrams_Exceeds_Maximum()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 10001.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.WeightGrams);
    }

    [Fact]
    public void Should_Pass_When_WeightGrams_Is_At_Maximum()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 10000.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.WeightGrams);
    }

    [Fact]
    public void Should_Fail_When_MealType_Is_Invalid()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = (MealType)999,
            ConsumedAt = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.MealType);
    }

    [Theory]
    [InlineData(MealType.Breakfast)]
    [InlineData(MealType.Lunch)]
    [InlineData(MealType.Dinner)]
    [InlineData(MealType.Snack)]
    public void Should_Pass_When_MealType_Is_Valid(MealType mealType)
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = mealType,
            ConsumedAt = DateTime.UtcNow
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.MealType);
    }

    [Fact]
    public void Should_Fail_When_ConsumedAt_Is_In_Future()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow.AddDays(2)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConsumedAt);
    }

    [Fact]
    public void Should_Pass_When_ConsumedAt_Is_Today()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.Today
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ConsumedAt);
    }

    [Fact]
    public void Should_Fail_When_ConsumedAt_Is_Too_Far_In_Past()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow.AddYears(-2)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ConsumedAt);
    }

    [Fact]
    public void Should_Pass_When_Notes_Is_Within_Limit()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow,
            Notes = new string('a', 500)
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Should_Fail_When_Notes_Exceeds_Limit()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow,
            Notes = new string('a', 501)
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Should_Pass_When_Notes_Is_Null()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow,
            Notes = null
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Should_Pass_When_Notes_Is_Empty()
    {
        var command = new CreateFoodEntryCommand
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            WeightGrams = 150.0,
            MealType = MealType.Breakfast,
            ConsumedAt = DateTime.UtcNow,
            Notes = ""
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }
} 