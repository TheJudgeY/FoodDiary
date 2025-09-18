using System;
using System.Collections.Generic;
using FoodDiary.Core.UserAggregate;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.Core.FoodEntryAggregate;

public enum MealType
{
    Breakfast,
    Lunch,
    Dinner,
    Snack
}

public class FoodEntry
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid ProductId { get; private set; }
    public double WeightGrams { get; private set; }
    public MealType MealType { get; private set; }
    public DateTime ConsumedAt { get; private set; }
    public string? Notes { get; private set; }
    
    public User User { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    
    public FoodEntry(
        Guid userId,
        Guid productId,
        double weightGrams,
        MealType mealType,
        DateTime consumedAt,
        string? notes = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ProductId = productId;
        WeightGrams = weightGrams;
        MealType = mealType;
        ConsumedAt = DateTime.SpecifyKind(consumedAt, DateTimeKind.Utc);
        Notes = notes;
    }
    
    private FoodEntry() { }
    
    public void UpdateDetails(double weightGrams, MealType mealType, string? notes = null)
    {
        WeightGrams = weightGrams;
        MealType = mealType;
        Notes = notes;
    }
    
    public void UpdateConsumptionTime(DateTime consumedAt)
    {
        ConsumedAt = DateTime.SpecifyKind(consumedAt, DateTimeKind.Utc);
    }
}
