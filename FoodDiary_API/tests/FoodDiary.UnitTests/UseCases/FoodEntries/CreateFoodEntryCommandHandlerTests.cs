using Xunit;
using MediatR;
using NSubstitute;
using Ardalis.Result;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.Core.ProductAggregate;
using FoodDiary.UseCases.FoodEntries.Create;
using FoodDiary.UseCases.FoodEntries;
using AutoMapper;

namespace FoodDiary.UnitTests.UseCases.FoodEntries;

public class CreateFoodEntryCommandHandlerTests
{
    private readonly FoodDiary.Core.Interfaces.IRepository<FoodEntry> _foodEntryRepository;
    private readonly FoodDiary.Core.Interfaces.IRepository<Product> _productRepository;
    private readonly IMapper _mapper;
    private readonly IFoodEntryService _foodEntryService;
    private readonly CreateFoodEntryCommandHandler _handler;

    public CreateFoodEntryCommandHandlerTests()
    {
        _foodEntryRepository = Substitute.For<FoodDiary.Core.Interfaces.IRepository<FoodEntry>>();
        _productRepository = Substitute.For<FoodDiary.Core.Interfaces.IRepository<Product>>();
        _mapper = Substitute.For<IMapper>();
        _foodEntryService = Substitute.For<IFoodEntryService>();
        _handler = new CreateFoodEntryCommandHandler(_foodEntryRepository, _productRepository, _mapper, _foodEntryService);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product("Chicken Breast", 165.0, 31.0, 3.6, 0.0);

        var command = new CreateFoodEntryCommand
        {
            UserId = userId,
            ProductId = productId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            ConsumedAt = DateTime.UtcNow,
            Notes = "Grilled chicken"
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        
        var createdFoodEntry = new FoodEntry(userId, productId, 200.0, MealType.Lunch, DateTime.UtcNow, "Grilled chicken");
        var productProperty = typeof(FoodEntry).GetProperty("Product");
        productProperty?.SetValue(createdFoodEntry, product);
        
        _foodEntryRepository.AddAsync(Arg.Any<FoodEntry>()).Returns(Result<FoodEntry>.Success(createdFoodEntry));

        _foodEntryService.GetMealTypeDisplayName(Arg.Any<FoodEntry>()).Returns("Lunch");
        _foodEntryService.CalculateCalories(Arg.Any<FoodEntry>()).Returns(330.0);
        _foodEntryService.CalculateProtein(Arg.Any<FoodEntry>()).Returns(62.0);
        _foodEntryService.CalculateFat(Arg.Any<FoodEntry>()).Returns(7.2);
        _foodEntryService.CalculateCarbohydrates(Arg.Any<FoodEntry>()).Returns(0.0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(productId, result.Value.ProductId);
        Assert.Equal("Chicken Breast", result.Value.ProductName);
        Assert.Equal(200.0, result.Value.WeightGrams);
        Assert.Equal(MealType.Lunch, result.Value.MealType);
        Assert.Equal("Lunch", result.Value.MealTypeDisplayName);
        Assert.Equal("Grilled chicken", result.Value.Notes);
        Assert.Equal(330.0, result.Value.Calories);
        Assert.Equal(62.0, result.Value.Protein);
        Assert.Equal(7.2, result.Value.Fat);
        Assert.Equal(0.0, result.Value.Carbohydrates);
        Assert.Equal("Food entry created successfully", result.Value.Message);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsNotFound()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new CreateFoodEntryCommand
        {
            UserId = userId,
            ProductId = productId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            ConsumedAt = DateTime.UtcNow
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.NotFound());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(Ardalis.Result.ResultStatus.NotFound, result.Status);
        Assert.Contains($"Product with ID {productId} not found", result.Errors.First());
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 15);
        var command = new CreateFoodEntryCommand
        {
            UserId = userId,
            ProductId = productId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            ConsumedAt = DateTime.UtcNow
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _foodEntryRepository.AddAsync(Arg.Any<FoodEntry>()).Returns(Result<FoodEntry>.Error("Database error"));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(Ardalis.Result.ResultStatus.Error, result.Status);
        Assert.Contains("Failed to create food entry", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ExceptionOccurs_ReturnsError()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = new CreateFoodEntryCommand
        {
            UserId = userId,
            ProductId = productId,
            WeightGrams = 200.0,
            MealType = MealType.Lunch,
            ConsumedAt = DateTime.UtcNow
        };

        _productRepository.GetByIdAsync(productId).Returns(Task.FromException<Result<Product>>(new Exception("Unexpected error")));

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(Ardalis.Result.ResultStatus.Error, result.Status);
        Assert.Contains("Error creating food entry", result.Errors.First());
    }

    [Fact]
    public async Task Handle_ZeroWeight_CalculatesZeroNutritionalValues()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100.0, 10.0, 5.0, 15.0);

        var command = new CreateFoodEntryCommand
        {
            UserId = userId,
            ProductId = productId,
            WeightGrams = 0.0,
            MealType = MealType.Snack,
            ConsumedAt = DateTime.UtcNow
        };

        _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
        _foodEntryRepository.AddAsync(Arg.Any<FoodEntry>()).Returns(Result<FoodEntry>.Success(new FoodEntry(userId, productId, 0.0, MealType.Snack, DateTime.UtcNow)));

        _foodEntryService.CalculateCalories(Arg.Any<FoodEntry>()).Returns(0.0);
        _foodEntryService.CalculateProtein(Arg.Any<FoodEntry>()).Returns(0.0);
        _foodEntryService.CalculateFat(Arg.Any<FoodEntry>()).Returns(0.0);
        _foodEntryService.CalculateCarbohydrates(Arg.Any<FoodEntry>()).Returns(0.0);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0.0, result.Value.Calories);
        Assert.Equal(0.0, result.Value.Protein);
        Assert.Equal(0.0, result.Value.Fat);
        Assert.Equal(0.0, result.Value.Carbohydrates);
    }

    [Fact]
    public async Task Handle_DifferentMealTypes_ReturnsCorrectDisplayNames()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product("Test Product", 100, 10, 5, 15);

        var mealTypes = new[] { MealType.Breakfast, MealType.Lunch, MealType.Dinner, MealType.Snack };
        var expectedDisplayNames = new[] { "Breakfast", "Lunch", "Dinner", "Snack" };

        for (int i = 0; i < mealTypes.Length; i++)
        {
            var command = new CreateFoodEntryCommand
            {
                UserId = userId,
                ProductId = productId,
                WeightGrams = 100.0,
                MealType = mealTypes[i],
                ConsumedAt = DateTime.UtcNow
            };

            _productRepository.GetByIdAsync(productId).Returns(Result<Product>.Success(product));
            _foodEntryRepository.AddAsync(Arg.Any<FoodEntry>()).Returns(Result<FoodEntry>.Success(new FoodEntry(userId, productId, 100.0, mealTypes[i], DateTime.UtcNow)));

            _foodEntryService.GetMealTypeDisplayName(Arg.Any<FoodEntry>()).Returns(expectedDisplayNames[i]);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedDisplayNames[i], result.Value.MealTypeDisplayName);
        }
    }
} 