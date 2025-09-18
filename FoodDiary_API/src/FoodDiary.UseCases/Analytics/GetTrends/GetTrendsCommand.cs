using System;
using MediatR;
using Ardalis.Result;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UseCases.Analytics.GetTrends;

public record GetTrendsCommand : IRequest<Result<GetTrendsResponse>>
{
    public Guid UserId { get; init; }
    public int Days { get; init; } = 7;
}

public record GetTrendsResponse
{
    public NutritionalTrendsDTO Trends { get; init; } = null!;
    public List<string> Recommendations { get; init; } = new List<string>();
    public string Message { get; init; } = string.Empty;
} 