using FoodDiary.Core.FoodEntryAggregate;

namespace FoodDiary.UseCases.FoodEntries;

public interface IFoodEntryService
{
    double CalculateCalories(FoodEntry foodEntry);
    double CalculateProtein(FoodEntry foodEntry);
    double CalculateFat(FoodEntry foodEntry);
    double CalculateCarbohydrates(FoodEntry foodEntry);
    
    bool IsConsumedToday(FoodEntry foodEntry);
    bool IsConsumedOnDate(FoodEntry foodEntry, DateTime date);
    
    string GetMealTypeDisplayName(FoodEntry foodEntry);
}

public class FoodEntryService : IFoodEntryService
{
    private const double GramsToPercentageRatio = 100.0;

    public double CalculateCalories(FoodEntry foodEntry) =>
        CalculateNutritionalValue(foodEntry.Product?.CaloriesPer100g, foodEntry.WeightGrams);

    public double CalculateProtein(FoodEntry foodEntry) =>
        CalculateNutritionalValue(foodEntry.Product?.ProteinsPer100g, foodEntry.WeightGrams);

    public double CalculateFat(FoodEntry foodEntry) =>
        CalculateNutritionalValue(foodEntry.Product?.FatsPer100g, foodEntry.WeightGrams);

    public double CalculateCarbohydrates(FoodEntry foodEntry) =>
        CalculateNutritionalValue(foodEntry.Product?.CarbohydratesPer100g, foodEntry.WeightGrams);

    public bool IsConsumedToday(FoodEntry foodEntry) =>
        foodEntry.ConsumedAt.Date == DateTime.Today;

    public bool IsConsumedOnDate(FoodEntry foodEntry, DateTime date) =>
        foodEntry.ConsumedAt.Date == date.Date;

    public string GetMealTypeDisplayName(FoodEntry foodEntry) =>
        foodEntry.MealType switch
        {
            MealType.Breakfast => "Breakfast",
            MealType.Lunch => "Lunch",
            MealType.Dinner => "Dinner",
            MealType.Snack => "Snack",
            _ => "Unknown"
        };

    private static double CalculateNutritionalValue(double? nutritionalValuePer100g, double weightGrams) =>
        (nutritionalValuePer100g * weightGrams / GramsToPercentageRatio) ?? 0;
} 