using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Ardalis.Result;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UseCases.Analytics.GetRecommendations;

public class GetRecommendationsCommandHandler : IRequestHandler<GetRecommendationsCommand, Result<GetRecommendationsResponse>>
{
    private readonly IRecommendationsService _recommendationsService;

    public GetRecommendationsCommandHandler(IRecommendationsService recommendationsService)
    {
        _recommendationsService = recommendationsService;
    }

    public async Task<Result<GetRecommendationsResponse>> Handle(GetRecommendationsCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var recommendations = await _recommendationsService.GetPersonalizedRecommendationsAsync(request.UserId, 7);

            var response = new GetRecommendationsResponse
            {
                Recommendations = recommendations,
                Message = "Personalized recommendations generated successfully"
            };

            return Result.Success(response);
        }
        catch (ArgumentException ex)
        {
            return Result.NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Error($"Error generating recommendations: {ex.Message}");
        }
    }
} 