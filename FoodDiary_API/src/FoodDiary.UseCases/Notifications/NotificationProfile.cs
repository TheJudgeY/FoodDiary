using AutoMapper;
using FoodDiary.Core.NotificationAggregate;
using FoodDiary.UseCases.Notifications;

namespace FoodDiary.UseCases.Notifications;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            
        CreateMap<NotificationPreferences, NotificationPreferencesDTO>();
    }
} 