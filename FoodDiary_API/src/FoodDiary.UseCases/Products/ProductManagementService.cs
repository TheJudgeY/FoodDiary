using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public interface IProductManagementService
{
    void UpdateDetails(Product product, string name, double caloriesPer100g, double proteinsPer100g, double fatsPer100g, double carbohydratesPer100g, string? description = null, ProductCategory category = ProductCategory.Other);
    void UpdateImage(Product product, byte[]? imageData, string? imageContentType, string? imageFileName);
    void RemoveImage(Product product);
}

public class ProductManagementService : IProductManagementService
{
    public void UpdateDetails(Product product, string name, double caloriesPer100g, double proteinsPer100g, double fatsPer100g, double carbohydratesPer100g, string? description = null, ProductCategory category = ProductCategory.Other)
    {
        product.UpdateDetails(name, caloriesPer100g, proteinsPer100g, fatsPer100g, carbohydratesPer100g, description, category);
    }

    public void UpdateImage(Product product, byte[]? imageData, string? imageContentType, string? imageFileName)
    {
        product.UpdateImage(imageData, imageContentType, imageFileName);
    }

    public void RemoveImage(Product product)
    {
        product.RemoveImage();
    }
}
