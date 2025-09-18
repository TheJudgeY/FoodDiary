using AutoMapper;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Products;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDTO>()
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom(src => GetCategoryDisplayName(src.Category)))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.ImageData != null && src.ImageData.Length > 0));
        
        CreateMap<Product, GetProductResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id));
        
        CreateMap<Product, ProductSummaryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom(src => GetCategoryDisplayName(src.Category)))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.ImageData != null && src.ImageData.Length > 0));
        
        CreateMap<Product, CreateProductResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom(src => GetCategoryDisplayName(src.Category)))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.ImageData != null && src.ImageData.Length > 0));
        
        CreateMap<Product, UpdateProductResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom(src => GetCategoryDisplayName(src.Category)))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.ImageData != null && src.ImageData.Length > 0));
    }

    private static string GetCategoryDisplayName(ProductCategory category)
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