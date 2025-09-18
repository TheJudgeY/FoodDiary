using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public interface IProductDisplayService
{
    string GetCategoryDisplayName(ProductCategory category);
}

public class ProductDisplayService : IProductDisplayService
{
    public string GetCategoryDisplayName(ProductCategory category)
    {
        return category switch
        {
            ProductCategory.Fruits => "Fruits",
            ProductCategory.Vegetables => "Vegetables",
            ProductCategory.Grains => "Grains",
            ProductCategory.Proteins => "Proteins",
            ProductCategory.Dairy => "Dairy",
            ProductCategory.NutsAndSeeds => "Nuts & Seeds",
            ProductCategory.Beverages => "Beverages",
            ProductCategory.Snacks => "Snacks",
            ProductCategory.Condiments => "Condiments",
            ProductCategory.Supplements => "Supplements",
            ProductCategory.Other => "Other",
            _ => category.ToString()
        };
    }
}
