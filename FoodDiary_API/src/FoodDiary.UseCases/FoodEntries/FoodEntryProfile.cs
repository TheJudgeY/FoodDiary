using AutoMapper;
using FoodDiary.Core.FoodEntryAggregate;
using FoodDiary.UseCases.FoodEntries;

namespace FoodDiary.UseCases.FoodEntries;

public class FoodEntryProfile : Profile
{
    public FoodEntryProfile()
    {
        CreateMap<FoodEntry, FoodEntryDTO>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.MealTypeDisplayName, opt => opt.Ignore())
            .ForMember(dest => dest.Calories, opt => opt.MapFrom(src => src.Product.CaloriesPer100g * src.WeightGrams / 100.0))
            .ForMember(dest => dest.Protein, opt => opt.MapFrom(src => src.Product.ProteinsPer100g * src.WeightGrams / 100.0))
            .ForMember(dest => dest.Fat, opt => opt.MapFrom(src => src.Product.FatsPer100g * src.WeightGrams / 100.0))
            .ForMember(dest => dest.Carbohydrates, opt => opt.MapFrom(src => src.Product.CarbohydratesPer100g * src.WeightGrams / 100.0));
    }
} 