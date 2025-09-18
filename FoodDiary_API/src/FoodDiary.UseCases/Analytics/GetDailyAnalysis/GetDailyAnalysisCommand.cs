using System;
using MediatR;
using Ardalis.Result;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UseCases.Analytics.GetDailyAnalysis;

public record GetDailyAnalysisCommand : IRequest<Result<GetDailyAnalysisResponse>>
{
    public Guid UserId { get; init; }
    public DateTime Date { get; init; } = DateTime.UtcNow.Date;
}

public record GetDailyAnalysisResponse
{
    public DailyNutritionalAnalysisDTO Analysis { get; init; } = null!;
    public string Message { get; init; } = string.Empty;
} 