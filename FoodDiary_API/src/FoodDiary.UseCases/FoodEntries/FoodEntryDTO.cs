using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries;

public class FoodEntryDTO
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public double WeightGrams { get; set; }
    public MealType MealType { get; set; }
    public string MealTypeDisplayName { get; set; } = string.Empty;
    public DateTime ConsumedAt { get; set; }
    public string? Notes { get; set; }
    
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbohydrates { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 