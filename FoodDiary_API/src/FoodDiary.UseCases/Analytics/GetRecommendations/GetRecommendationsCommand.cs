using System;
using System.Collections.Generic;
using MediatR;
using Ardalis.Result;

namespace FoodDiary.UseCases.Analytics.GetRecommendations;

public record GetRecommendationsCommand : IRequest<Result<GetRecommendationsResponse>>
{
    public Guid UserId { get; init; }
    public DateTime Date { get; init; } = DateTime.UtcNow.Date;
}

public record GetRecommendationsResponse
{
    public List<string> Recommendations { get; init; } = new List<string>();
    public string Message { get; init; } = string.Empty;
} 