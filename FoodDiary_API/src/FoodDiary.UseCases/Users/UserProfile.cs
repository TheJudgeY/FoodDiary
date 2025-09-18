using AutoMapper;
using FoodDiary.Core.UserAggregate;
using FoodDiary.UseCases.Users;

namespace FoodDiary.UseCases.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.BMI, opt => opt.Ignore())
            .ForMember(dest => dest.BMICategory, opt => opt.Ignore())
            .ForMember(dest => dest.BMR, opt => opt.Ignore())
            .ForMember(dest => dest.TDEE, opt => opt.Ignore())
            .ForMember(dest => dest.RecommendedCalories, opt => opt.Ignore())
            .ForMember(dest => dest.HasCompleteProfile, opt => opt.Ignore())
            .ForMember(dest => dest.TimeZoneId, opt => opt.MapFrom(src => src.TimeZoneId));
        
    }
} 