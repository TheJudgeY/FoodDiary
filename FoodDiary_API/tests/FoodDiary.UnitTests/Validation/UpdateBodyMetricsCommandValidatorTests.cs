using Xunit;
using FluentValidation.TestHelper;
using FoodDiary.UseCases.Users.UpdateBodyMetrics;
using FoodDiary.UseCases.Validation;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UnitTests.Validation;

public class UpdateBodyMetricsCommandValidatorTests
{
    private readonly UpdateBodyMetricsCommandValidator _validator;

    public UpdateBodyMetricsCommandValidatorTests()
    {
        _validator = new UpdateBodyMetricsCommandValidator();
    }

    [Fact]
    public void Should_Pass_When_Valid_Command()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Fail_When_UserId_Is_Empty()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.Empty,
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact]
    public void Should_Fail_When_Height_Is_Negative()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = -10,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.HeightCm);
    }

    [Fact]
    public void Should_Fail_When_Height_Exceeds_Maximum()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 350,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.HeightCm);
    }

    [Fact]
    public void Should_Fail_When_Weight_Is_Negative()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 175,
            WeightKg = -10,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.WeightKg);
    }

    [Fact]
    public void Should_Fail_When_Weight_Exceeds_Maximum()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 175,
            WeightKg = 600,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.WeightKg);
    }

    [Fact]
    public void Should_Fail_When_Age_Is_Negative()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 175,
            WeightKg = 70,
            Age = -5,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Age);
    }

    [Fact]
    public void Should_Fail_When_Age_Exceeds_Maximum()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 175,
            WeightKg = 70,
            Age = 200,
            Gender = Gender.Male,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Age);
    }

    [Fact]
    public void Should_Fail_When_Gender_Is_Invalid()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = (Gender)999,
            ActivityLevel = ActivityLevel.ModeratelyActive
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.Gender);
    }

    [Fact]
    public void Should_Fail_When_ActivityLevel_Is_Invalid()
    {
        var command = new UpdateBodyMetricsCommand
        {
            UserId = Guid.NewGuid(),
            HeightCm = 175,
            WeightKg = 70,
            Age = 30,
            Gender = Gender.Male,
            ActivityLevel = (ActivityLevel)999
        };

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ActivityLevel);
    }
} 