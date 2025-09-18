using System;
using System.Collections.Generic;

namespace FoodDiary.Core.ProductAggregate;

public enum ProductCategory
{
    Fruits,
    Vegetables,
    Grains,
    Proteins,
    Dairy,
    NutsAndSeeds,
    Beverages,
    Snacks,
    Condiments,
    Supplements,
    Other
}

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public double CaloriesPer100g { get; private set; }
    public double ProteinsPer100g { get; private set; }
    public double FatsPer100g { get; private set; }
    public double CarbohydratesPer100g { get; private set; }
    public string? Description { get; private set; }
    public ProductCategory Category { get; private set; } = ProductCategory.Other;
    
    public byte[]? ImageData { get; private set; }
    public string? ImageContentType { get; private set; }
    public string? ImageFileName { get; private set; }

    public Product(string name, double caloriesPer100g, double proteinsPer100g, double fatsPer100g, double carbohydratesPer100g, string? description = null, ProductCategory category = ProductCategory.Other)
    {
        Id = Guid.NewGuid();
        Name = name;
        CaloriesPer100g = caloriesPer100g;
        ProteinsPer100g = proteinsPer100g;
        FatsPer100g = fatsPer100g;
        CarbohydratesPer100g = carbohydratesPer100g;
        Description = description;
        Category = category;
    }

    private Product() { }

    public static Product CreateForSeeding(Guid id, string name, double caloriesPer100g, double proteinsPer100g, double fatsPer100g, double carbohydratesPer100g, string? description = null, ProductCategory category = ProductCategory.Other)
    {
        var product = new Product(name, caloriesPer100g, proteinsPer100g, fatsPer100g, carbohydratesPer100g, description, category);
        product.SetIdForSeeding(id);
        return product;
    }
    
    internal void SetIdForSeeding(Guid id)
    {
        Id = id;
    }

    public void UpdateDetails(string name, double caloriesPer100g, double proteinsPer100g, double fatsPer100g, double carbohydratesPer100g, string? description = null, ProductCategory category = ProductCategory.Other)
    {
        Name = name;
        CaloriesPer100g = caloriesPer100g;
        ProteinsPer100g = proteinsPer100g;
        FatsPer100g = fatsPer100g;
        CarbohydratesPer100g = carbohydratesPer100g;
        Description = description;
        Category = category;
    }

    public void UpdateImage(byte[]? imageData, string? imageContentType, string? imageFileName)
    {
        ImageData = imageData;
        ImageContentType = imageContentType;
        ImageFileName = imageFileName;
    }
    
    public void RemoveImage()
    {
        ImageData = null;
        ImageContentType = null;
        ImageFileName = null;
    }
}
