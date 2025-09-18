using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public interface IProductService
{
    bool IsValidNutritionalData(Product product);
    double GetTotalMacronutrientsPer100g(Product product);
    bool IsNutritionalDataConsistent(Product product);
    bool HasImage(Product product);
    long GetImageSizeInBytes(Product product);
}

public class ProductService : IProductService
{
    private const double MinimumNutritionalValue = 0.0;
    private const double CalorieTolerance = 50.0;
    
    private const double CaloriesPerGramProtein = 4.0;
    private const double CaloriesPerGramFat = 9.0;
    private const double CaloriesPerGramCarbohydrate = 4.0;

    public bool IsValidNutritionalData(Product product) =>
        product.CaloriesPer100g >= MinimumNutritionalValue && 
        product.ProteinsPer100g >= MinimumNutritionalValue && 
        product.FatsPer100g >= MinimumNutritionalValue && 
        product.CarbohydratesPer100g >= MinimumNutritionalValue;

    public double GetTotalMacronutrientsPer100g(Product product) =>
        product.ProteinsPer100g + product.FatsPer100g + product.CarbohydratesPer100g;

    public bool IsNutritionalDataConsistent(Product product)
    {
        var calculatedCalories = CalculateExpectedCalories(product);
        var difference = Math.Abs(calculatedCalories - product.CaloriesPer100g);
        
        return difference <= CalorieTolerance;
    }

    public bool HasImage(Product product) =>
        product.ImageData != null && product.ImageData.Length > 0;

    public long GetImageSizeInBytes(Product product) =>
        product.ImageData?.Length ?? 0;

    private static double CalculateExpectedCalories(Product product) =>
        (product.ProteinsPer100g * CaloriesPerGramProtein) + 
        (product.FatsPer100g * CaloriesPerGramFat) + 
        (product.CarbohydratesPer100g * CaloriesPerGramCarbohydrate);
} 