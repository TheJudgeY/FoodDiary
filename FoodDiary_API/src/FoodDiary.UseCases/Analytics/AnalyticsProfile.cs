using AutoMapper;
using FoodDiary.Core.AnalyticsAggregate;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UseCases.Analytics;

public class AnalyticsProfile : Profile
{
    public AnalyticsProfile()
    {
        CreateMap<DailyNutritionalAnalysis, DailyNutritionalAnalysisDTO>();
        CreateMap<NutritionalTrends, NutritionalTrendsDTO>();
    }
} 