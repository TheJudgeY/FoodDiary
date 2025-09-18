using AutoMapper;
using FoodDiary.Core.RecipeAggregate;
using FoodDiary.Core.ProductAggregate;

namespace FoodDiary.UseCases.Recipes;

public class RecipeProfile : Profile
{
    public RecipeProfile()
    {
        CreateMap<Recipe, RecipeDTO>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients))
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom(src => src.Category.GetDisplayName()))
            .ForMember(dest => dest.CaloriesPerServing, opt => opt.Ignore())
            .ForMember(dest => dest.ProteinPerServing, opt => opt.Ignore())
            .ForMember(dest => dest.FatPerServing, opt => opt.Ignore())
            .ForMember(dest => dest.CarbohydratesPerServing, opt => opt.Ignore())
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.ImageData != null && src.ImageData.Length > 0));

        CreateMap<RecipeIngredient, RecipeIngredientDTO>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));

        CreateMap<Product, ProductDTO>();

        CreateMap<Recipe, CreateRecipeResponse>()
            .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<RecipeIngredient, CreateRecipeIngredientResponse>()
            .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => 
                src.ProductId != Guid.Empty ? src.ProductId : (Guid?)null));

        CreateMap<Recipe, GetRecipeResponse>()
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<RecipeIngredient, GetRecipeIngredientResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => 
                src.ProductId != Guid.Empty ? src.ProductId : (Guid?)null));

        CreateMap<Product, GetRecipeProductResponse>();

        CreateMap<Recipe, ListRecipeItemResponse>()
            .ForMember(dest => dest.IngredientCount, opt => opt.MapFrom(src => 
                src.Ingredients != null ? src.Ingredients.Count : 0))
            .ForMember(dest => dest.CategoryDisplayName, opt => opt.MapFrom(src => src.Category.GetDisplayName()))
            .ForMember(dest => dest.HasImage, opt => opt.MapFrom(src => src.ImageData != null && src.ImageData.Length > 0));

        CreateMap<Recipe, UpdateRecipeResponse>()
            .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Ingredients, opt => opt.MapFrom(src => src.Ingredients));

        CreateMap<RecipeIngredient, UpdateRecipeIngredientResponse>()
            .ForMember(dest => dest.IngredientId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => 
                src.ProductId != Guid.Empty ? src.ProductId : (Guid?)null));
    }
}
