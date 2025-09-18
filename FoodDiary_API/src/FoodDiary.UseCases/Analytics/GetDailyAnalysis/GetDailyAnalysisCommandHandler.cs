using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.Analytics.GetDailyAnalysis;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UseCases.Analytics.GetDailyAnalysis;

public class GetDailyAnalysisCommandHandler : IRequestHandler<GetDailyAnalysisCommand, Result<GetDailyAnalysisResponse>>
{
    private readonly INutritionalAnalysisService _analysisService;
    private readonly IMapper _mapper;
    private readonly IAnalyticsService _analyticsService;
    private readonly ITrendsService _trendsService;
    private readonly IRecommendationsService _recommendationsService;

    public GetDailyAnalysisCommandHandler(
        INutritionalAnalysisService analysisService, 
        IMapper mapper, 
        IAnalyticsService analyticsService,
        ITrendsService trendsService,
        IRecommendationsService recommendationsService)
    {
        _analysisService = analysisService;
        _mapper = mapper;
        _analyticsService = analyticsService;
        _trendsService = trendsService;
        _recommendationsService = recommendationsService;
    }

    public async Task<Result<GetDailyAnalysisResponse>> Handle(GetDailyAnalysisCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var analysis = await _analysisService.GenerateDailyAnalysisAsync(request.UserId, request.Date);
            var analysisDto = _mapper.Map<DailyNutritionalAnalysisDTO>(analysis);
            
            analysisDto.IsCalorieGoalMet = _analyticsService.IsCalorieGoalMet(analysis);
            analysisDto.IsProteinGoalMet = _analyticsService.IsProteinGoalMet(analysis);
            analysisDto.IsFatGoalMet = _analyticsService.IsFatGoalMet(analysis);
            analysisDto.IsCarbohydrateGoalMet = _analyticsService.IsCarbohydrateGoalMet(analysis);
            analysisDto.IsOverCalorieLimit = _analyticsService.IsOverCalorieLimit(analysis);
            analysisDto.IsOverProteinLimit = _analyticsService.IsOverProteinLimit(analysis);
            analysisDto.IsOverFatLimit = _analyticsService.IsOverFatLimit(analysis);
            analysisDto.IsOverCarbohydrateLimit = _analyticsService.IsOverCarbohydrateLimit(analysis);
            analysisDto.OverallStatus = _analyticsService.GetOverallStatus(analysis);
            
            var trendsInsights = await _trendsService.GetTrendInsightsAsync(request.UserId, 7);
            var trendsRecommendations = await _recommendationsService.GetPersonalizedRecommendationsAsync(request.UserId, 7);
            
            analysisDto.KeyInsights = trendsInsights;
            analysisDto.Recommendations = trendsRecommendations;

            var response = new GetDailyAnalysisResponse
            {
                Analysis = analysisDto,
                Message = "Daily nutritional analysis generated successfully"
            };

            return Result.Success(response);
        }
        catch (ArgumentException ex)
        {
            return Result.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error generating daily analysis: {ex.Message}");
        }
    }
} 