using AutoMapper;
using FoodDiary.UseCases.Products;
using FoodDiary.UseCases.FoodEntries;

namespace FoodDiary.Web.Mapping;

public class EndpointMappingProfile : Profile
{
    public EndpointMappingProfile()
    {
        CreateMap<UseCases.Products.CreateProductResponse, Products.CreateProductResponse>();
        CreateMap<UseCases.Products.GetProductResponse, Products.GetProductResponse>();
    }
} 