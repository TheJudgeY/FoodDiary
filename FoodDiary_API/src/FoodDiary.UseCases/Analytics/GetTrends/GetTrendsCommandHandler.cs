using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ardalis.Result;
using AutoMapper;
using FoodDiary.Core.Interfaces;
using FoodDiary.UseCases.Analytics.GetTrends;

namespace FoodDiary.UseCases.Analytics.GetTrends;

public class GetTrendsCommandHandler : IRequestHandler<GetTrendsCommand, Result<GetTrendsResponse>>
{
    private readonly INutritionalAnalysisService _analysisService;
    private readonly IMapper _mapper;
    private readonly IAnalyticsCalculationService _analyticsCalculationService;

    public GetTrendsCommandHandler(
        INutritionalAnalysisService analysisService, 
        IMapper mapper,
        IAnalyticsCalculationService analyticsCalculationService)
    {
        _analysisService = analysisService;
        _mapper = mapper;
        _analyticsCalculationService = analyticsCalculationService;
    }

    public async Task<Result<GetTrendsResponse>> Handle(GetTrendsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var trends = await _analysisService.GenerateTrendsAsync(request.UserId, request.Days);
            var trendsDto = _mapper.Map<NutritionalTrendsDTO>(trends);
            
            trendsDto.OverallTrend = _analyticsCalculationService.CalculateOverallTrend(trends);
            trendsDto.TrendInsights = _analyticsCalculationService.GenerateTrendInsights(trends);
            trendsDto.IsConsistent = _analyticsCalculationService.EvaluateConsistency(trends);
            trendsDto.IsImproving = _analyticsCalculationService.EvaluateImprovement(trends);

            var response = new GetTrendsResponse
            {
                Trends = trendsDto,
                Message = $"Nutritional trends generated successfully for {request.Days} days"
            };

            return Result.Success(response);
        }
        catch (ArgumentException ex)
        {
            return Result.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error generating trends: {ex.Message}");
        }
    }
} 