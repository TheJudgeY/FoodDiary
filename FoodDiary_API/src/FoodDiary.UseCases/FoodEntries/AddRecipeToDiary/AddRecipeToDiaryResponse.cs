namespace FoodDiary.UseCases.FoodEntries.AddRecipeToDiary;

public record AddRecipeToDiaryResponse
{
    public Guid RecipeId { get; init; }
    public string RecipeName { get; init; } = string.Empty;
    public int FoodEntriesCreated { get; init; }
    public double TotalCalories { get; init; }
    public double TotalProtein { get; init; }
    public double TotalFat { get; init; }
    public double TotalCarbohydrates { get; init; }
    public List<FoodEntrySummary> FoodEntries { get; init; } = new();
    public string Message { get; init; } = string.Empty;
}

public record FoodEntrySummary
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public double WeightGrams { get; init; }
    public double Calories { get; init; }
    public double Protein { get; init; }
    public double Fat { get; init; }
    public double Carbohydrates { get; init; }
} 