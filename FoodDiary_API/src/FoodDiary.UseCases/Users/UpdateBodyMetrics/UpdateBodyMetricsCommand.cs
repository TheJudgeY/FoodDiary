using MediatR;
using Ardalis.Result;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Users.UpdateBodyMetrics;

public record UpdateBodyMetricsCommand : IRequest<Result<UpdateBodyMetricsResponse>>
{
    public Guid UserId { get; init; }
    public double HeightCm { get; init; }
    public double WeightKg { get; init; }
    public int Age { get; init; }
    public Gender Gender { get; init; }
    public ActivityLevel ActivityLevel { get; init; }
}

public record UpdateBodyMetricsResponse
{
    public Guid UserId { get; init; }
    public double HeightCm { get; init; }
    public double WeightKg { get; init; }
    public int Age { get; init; }
    public Gender Gender { get; init; }
    public ActivityLevel ActivityLevel { get; init; }
    public double? BMI { get; init; }
    public string? BMICategory { get; init; }
    public double? BMR { get; init; }
    public double? TDEE { get; init; }
    public string Message { get; init; } = string.Empty;
} 