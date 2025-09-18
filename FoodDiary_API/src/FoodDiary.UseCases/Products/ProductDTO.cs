using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double CaloriesPer100g { get; set; }
    public double ProteinsPer100g { get; set; }
    public double FatsPer100g { get; set; }
    public double CarbohydratesPer100g { get; set; }
    public string? Description { get; set; }
    public ProductCategory Category { get; set; }
    public string CategoryDisplayName { get; set; } = string.Empty;
    public string? ImageFileName { get; set; }
    public string? ImageContentType { get; set; }
    public bool HasImage { get; set; }
} 