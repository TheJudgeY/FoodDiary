using Xunit;
using FoodDiary.Core.AnalyticsAggregate;
using FoodDiary.UseCases.Analytics;

namespace FoodDiary.UnitTests.Core.AnalyticsAggregate;

public class NutritionalTrendsTests
{
    private readonly AnalyticsCalculationService _calculationService;

    public NutritionalTrendsTests()
    {
        _calculationService = new AnalyticsCalculationService();
    }

    [Fact]
    public void GetOverallTrend_WithImprovingTrends_ReturnsStronglyImproving()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetTrends("Improving", "Improving", "Improving", "Improving");

        var result = _calculationService.CalculateOverallTrend(trends);

        Assert.Equal("Strongly Improving", result);
    }

    [Fact]
    public void GetOverallTrend_WithMixedTrends_ReturnsImproving()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetTrends("Improving", "Stable", "Improving", "Declining");

        var result = _calculationService.CalculateOverallTrend(trends);

        Assert.Equal("Improving", result);
    }

    [Fact]
    public void GetOverallTrend_WithNoData_ReturnsNoData()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 0);

        var result = _calculationService.CalculateOverallTrend(trends);

        Assert.Equal("No data", result);
    }

    [Fact]
    public void IsConsistent_WithHighConsistency_ReturnsTrue()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetConsistencyMetrics(85, 80, 75, 70);

        var result = _calculationService.EvaluateConsistency(trends);

        Assert.True(result);
    }

    [Fact]
    public void IsConsistent_WithLowConsistency_ReturnsFalse()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetConsistencyMetrics(60, 65, 70, 55);

        var result = _calculationService.EvaluateConsistency(trends);

        Assert.False(result);
    }

    [Fact]
    public void IsImproving_WithImprovingTrend_ReturnsTrue()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetTrends("Improving", "Improving", "Stable", "Stable");

        var result = _calculationService.EvaluateImprovement(trends);

        Assert.True(result);
    }

    [Fact]
    public void IsImproving_WithDecliningTrend_ReturnsFalse()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetTrends("Declining", "Declining", "Stable", "Stable");

        var result = _calculationService.EvaluateImprovement(trends);

        Assert.False(result);
    }

    [Fact]
    public void GetTrendInsights_WithImprovingTrends_ReturnsPositiveInsights()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetTrends("Improving", "Improving", "Stable", "Stable");

        var result = _calculationService.GenerateTrendInsights(trends);

        var resultText = string.Join(" ", result);
        Assert.Contains("individual goals met", resultText.ToLowerInvariant());
        Assert.Contains("goal adherence", resultText.ToLowerInvariant());
    }

    [Fact]
    public void GetTrendInsights_WithDecliningTrends_ReturnsNegativeInsights()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetTrends("Declining", "Declining", "Stable", "Stable");

        var result = _calculationService.GenerateTrendInsights(trends);

        var resultText = string.Join(" ", result);
        Assert.Contains("individual goals met", resultText.ToLowerInvariant());
        Assert.Contains("goal adherence", resultText.ToLowerInvariant());
    }

    [Fact]
    public void GetTrendInsights_WithMixedTrends_ReturnsBalancedInsights()
    {
        var trends = new NutritionalTrends(Guid.NewGuid(), DateTime.Today, 7);
        trends.SetTrends("Improving", "Stable", "Declining", "Stable");

        var result = _calculationService.GenerateTrendInsights(trends);

        var resultText = string.Join(" ", result);
        Assert.Contains("individual goals met", resultText.ToLowerInvariant());
        Assert.Contains("goal adherence", resultText.ToLowerInvariant());
    }
} 