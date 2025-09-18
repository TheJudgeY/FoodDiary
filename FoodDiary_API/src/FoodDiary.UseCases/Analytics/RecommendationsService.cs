using FoodDiary.Core.AnalyticsAggregate;
using FoodDiary.Core.Interfaces;
using FoodDiary.Core.UserAggregate;

namespace FoodDiary.UseCases.Analytics;

public interface IRecommendationsService
{
    Task<List<string>> GetPersonalizedRecommendationsAsync(Guid userId, int days = 7);
    Task<List<string>> GetRecommendationsForTrendsAsync(Guid userId, int days = 7);
    Task<List<string>> GetRecommendationsForGoalsAsync(Guid userId, int days = 7);
    Task<List<string>> GetRecommendationsForConsistencyAsync(Guid userId, int days = 7);
}

public class RecommendationsService : IRecommendationsService
{
    private readonly INutritionalAnalysisService _nutritionalAnalysisService;
    private readonly IAnalyticsService _analyticsService;
    private readonly ITrendsService _trendsService;
    private readonly FoodDiary.Core.Interfaces.IRepository<User> _userRepository;

    public RecommendationsService(
        INutritionalAnalysisService nutritionalAnalysisService, 
        IAnalyticsService analyticsService,
        ITrendsService trendsService,
        FoodDiary.Core.Interfaces.IRepository<User> userRepository)
    {
        _nutritionalAnalysisService = nutritionalAnalysisService;
        _analyticsService = analyticsService;
        _trendsService = trendsService;
        _userRepository = userRepository;
    }

    public async Task<List<string>> GetPersonalizedRecommendationsAsync(Guid userId, int days = 7)
    {
        try
        {
            var recommendations = new List<string>();
            
            recommendations.AddRange(await GetRecommendationsForTrendsAsync(userId, days));
            recommendations.AddRange(await GetRecommendationsForGoalsAsync(userId, days));
            recommendations.AddRange(await GetRecommendationsForConsistencyAsync(userId, days));
            recommendations.AddRange(await GetUserSpecificRecommendationsAsync(userId, days));

            return recommendations.Distinct().ToList();
        }
        catch (Exception ex)
        {
            return new List<string> { $"Unable to generate recommendations: {ex.Message}" };
        }
    }

    public async Task<List<string>> GetRecommendationsForTrendsAsync(Guid userId, int days = 7)
    {
        try
        {
            var trends = await _nutritionalAnalysisService.GenerateTrendsAsync(userId, days);
            var analyses = await _nutritionalAnalysisService.GenerateWeeklyAnalysisAsync(userId, DateTime.UtcNow.AddDays(-days));
            
            if (!analyses.Any()) return new List<string>();

            var recommendations = new List<string>();

            AddTrendRecommendations(recommendations, trends);
            
            AddConsistencyRecommendations(recommendations, trends);
            
            AddMealPatternRecommendations(recommendations, trends);

            return recommendations;
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }

    public async Task<List<string>> GetRecommendationsForGoalsAsync(Guid userId, int days = 7)
    {
        try
        {
            var analyses = await _nutritionalAnalysisService.GenerateWeeklyAnalysisAsync(userId, DateTime.UtcNow.AddDays(-days));
            
            if (!analyses.Any()) return new List<string>();

            var recommendations = new List<string>();
            var totalDays = analyses.Count;
            
            var adherenceMetrics = CalculateAdherenceMetrics(analyses, totalDays);
            
            AddGoalRecommendations(recommendations, "calorie", adherenceMetrics.calorieAdherence);
            AddGoalRecommendations(recommendations, "protein", adherenceMetrics.proteinAdherence);
            AddGoalRecommendations(recommendations, "fat", adherenceMetrics.fatAdherence);
            AddGoalRecommendations(recommendations, "carbohydrate", adherenceMetrics.carbAdherence);

            return recommendations;
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }

    public async Task<List<string>> GetRecommendationsForConsistencyAsync(Guid userId, int days = 7)
    {
        try
        {
            var trends = await _nutritionalAnalysisService.GenerateTrendsAsync(userId, days);
            var recommendations = new List<string>();
            
            AddMacronutrientConsistencyRecommendations(recommendations, "calorie", trends.CalorieConsistency);
            AddMacronutrientConsistencyRecommendations(recommendations, "protein", trends.ProteinConsistency);
            AddMacronutrientConsistencyRecommendations(recommendations, "fat", trends.FatConsistency);
            AddMacronutrientConsistencyRecommendations(recommendations, "carbohydrate", trends.CarbohydrateConsistency);

            return recommendations;
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }

    private async Task<List<string>> GetUserSpecificRecommendationsAsync(Guid userId, int days = 7)
    {
        try
        {
            var userResult = await _userRepository.GetByIdAsync(userId);
            if (!userResult.IsSuccess || userResult.Value?.FitnessGoal == null)
                return new List<string>();

            var user = userResult.Value;
            var analyses = await _nutritionalAnalysisService.GenerateWeeklyAnalysisAsync(userId, DateTime.UtcNow.AddDays(-days));
            
            if (!analyses.Any()) return new List<string>();

            var recommendations = new List<string>();
            var averageCalories = analyses.Average(a => a.TotalCalories);
            var averageProtein = analyses.Average(a => a.TotalProtein);

            if (user.FitnessGoal.HasValue)
            {
                AddFitnessGoalRecommendations(recommendations, user.FitnessGoal.Value, averageCalories, averageProtein, user.DailyCalorieGoal, user.DailyProteinGoal);
            }

            return recommendations;
        }
        catch (Exception)
        {
            return new List<string>();
        }
    }

    private static void AddTrendRecommendations(List<string> recommendations, NutritionalTrends trends)
    {
        var trendRecommendations = new Dictionary<string, string>
        {
            ["Calorie"] = "Your calorie intake is declining. Consider adding healthy calorie-dense foods like nuts, avocados, or whole grains to maintain energy levels.",
            ["Protein"] = "Your protein intake is declining. Try incorporating more lean meats, fish, eggs, or plant-based proteins like legumes and quinoa.",
            ["Fat"] = "Your fat intake is declining. Include healthy fats from sources like olive oil, nuts, seeds, and fatty fish.",
            ["Carbohydrate"] = "Your carbohydrate intake is declining. Add complex carbohydrates from whole grains, fruits, and vegetables for sustained energy."
        };

        foreach (var (nutrient, recommendation) in trendRecommendations)
        {
            var trendProperty = typeof(NutritionalTrends).GetProperty($"{nutrient}Trend");
            var trend = trendProperty?.GetValue(trends) as string;
            
            if (trend == "Declining")
                recommendations.Add(recommendation);
        }
    }

    private static void AddConsistencyRecommendations(List<string> recommendations, NutritionalTrends trends)
    {
        var consistencyRecommendations = new Dictionary<string, (double threshold, string message)>
        {
            ["Calorie"] = (60, "Your daily calorie intake varies significantly. Try meal planning to achieve more consistent daily nutrition."),
            ["Protein"] = (60, "Your protein intake varies day to day. Consider distributing protein more evenly across all meals.")
        };

        foreach (var (nutrient, (threshold, message)) in consistencyRecommendations)
        {
            var consistencyProperty = typeof(NutritionalTrends).GetProperty($"{nutrient}Consistency");
            var consistency = consistencyProperty?.GetValue(trends) as double? ?? 0;
            
            if (consistency < threshold)
                recommendations.Add(message);
        }
    }

    private static void AddMealPatternRecommendations(List<string> recommendations, NutritionalTrends trends)
    {
        if (trends.AverageMealsPerDay < 3)
            recommendations.Add("You're eating fewer than 3 meals per day. Try adding a healthy snack or splitting larger meals to improve nutrient distribution.");
    }

    private static (double calorieAdherence, double proteinAdherence, double fatAdherence, double carbAdherence) CalculateAdherenceMetrics(List<DailyNutritionalAnalysis> analyses, int totalDays)
    {
        var calorieGoalDays = analyses.Count(a => IsCalorieGoalMet(a));
        var proteinGoalDays = analyses.Count(a => IsProteinGoalMet(a));
        var fatGoalDays = analyses.Count(a => IsFatGoalMet(a));
        var carbGoalDays = analyses.Count(a => IsCarbohydrateGoalMet(a));

        return (
            (double)calorieGoalDays / totalDays * 100,
            (double)proteinGoalDays / totalDays * 100,
            (double)fatGoalDays / totalDays * 100,
            (double)carbGoalDays / totalDays * 100
        );
    }

    private static void AddGoalRecommendations(List<string> recommendations, string nutrient, double adherence)
    {
        if (adherence < 50)
        {
            var message = nutrient switch
            {
                "calorie" => "You rarely meet your calorie goals. Consider using a food scale to measure portions and track your intake more accurately.",
                "protein" => "You rarely meet your protein goals. Include a protein source with every meal - eggs for breakfast, chicken for lunch, fish for dinner.",
                "fat" => "You rarely meet your fat goals. Add healthy fats like olive oil to salads, nuts as snacks, or avocado to meals.",
                "carbohydrate" => "You rarely meet your carbohydrate goals. Include whole grains, fruits, and vegetables with every meal for sustained energy.",
                _ => string.Empty
            };
            
            if (!string.IsNullOrEmpty(message))
                recommendations.Add(message);
        }
        else if (adherence < 70)
        {
            var message = nutrient switch
            {
                "calorie" => "You sometimes miss your calorie goals. Try meal prepping to ensure you have healthy options available throughout the day.",
                "protein" => "You sometimes miss your protein goals. Consider adding protein-rich snacks like Greek yogurt or protein shakes between meals.",
                "fat" => "You sometimes miss your fat goals. Try cooking with healthy oils and including fatty fish in your diet.",
                "carbohydrate" => "You sometimes miss your carbohydrate goals. Try adding complex carbs like oatmeal, brown rice, or sweet potatoes to your meals.",
                _ => string.Empty
            };
            
            if (!string.IsNullOrEmpty(message))
                recommendations.Add(message);
        }
    }

    private static void AddMacronutrientConsistencyRecommendations(List<string> recommendations, string nutrient, double consistency)
    {
        if (consistency < 50)
        {
            var message = nutrient switch
            {
                "calorie" => "Your calorie intake is highly inconsistent. Try eating at regular times and planning meals in advance to improve consistency.",
                "protein" => "Your protein intake is highly inconsistent. Distribute protein evenly across all meals to maintain muscle mass and satiety.",
                "fat" => "Your fat intake is highly inconsistent. Include healthy fats consistently throughout the day for better hormone regulation.",
                "carbohydrate" => "Your carbohydrate intake is highly inconsistent. This can cause energy fluctuations. Try to eat carbs consistently with each meal.",
                _ => string.Empty
            };
            
            if (!string.IsNullOrEmpty(message))
                recommendations.Add(message);
        }
        else if (consistency < 70)
        {
            var message = nutrient switch
            {
                "calorie" => "Your calorie intake shows moderate inconsistency. Consider using meal timing strategies to eat more regularly.",
                "protein" => "Your protein intake shows moderate inconsistency. Try to include similar protein portions at each meal.",
                "fat" => "Your fat intake shows moderate inconsistency. Try to maintain consistent fat portions across meals.",
                "carbohydrate" => "Your carbohydrate intake shows moderate inconsistency. Consider timing carbs around your most active periods.",
                _ => string.Empty
            };
            
            if (!string.IsNullOrEmpty(message))
                recommendations.Add(message);
        }
    }

    private static void AddFitnessGoalRecommendations(List<string> recommendations, FitnessGoal fitnessGoal, double averageCalories, double averageProtein, double? dailyCalorieGoal, double? dailyProteinGoal)
    {
        switch (fitnessGoal)
        {
            case FitnessGoal.LoseWeight:
                if (dailyCalorieGoal.HasValue && averageCalories > dailyCalorieGoal.Value)
                    recommendations.Add("For weight loss, focus on creating a moderate calorie deficit. Consider reducing portion sizes and choosing lower-calorie alternatives.");
                if (dailyProteinGoal.HasValue && averageProtein < dailyProteinGoal.Value * 0.8)
                    recommendations.Add("For weight loss, adequate protein is crucial to preserve muscle mass. Increase your protein intake through lean sources.");
                recommendations.Add("For weight loss, focus on whole foods and avoid processed snacks. Consider increasing your daily activity level.");
                break;

            case FitnessGoal.GainWeight:
                if (dailyCalorieGoal.HasValue && averageCalories < dailyCalorieGoal.Value)
                    recommendations.Add("For weight gain, you need to eat above your maintenance calories. Consider adding healthy calorie-dense foods like nuts and avocados.");
                if (dailyProteinGoal.HasValue && averageProtein < dailyProteinGoal.Value * 0.8)
                    recommendations.Add("For muscle gain, increase your protein intake to support muscle growth. Consider protein shakes between meals.");
                recommendations.Add("For weight gain, combine increased caloric intake with strength training to build muscle rather than just fat.");
                break;

            case FitnessGoal.MaintainWeight:
                if (dailyCalorieGoal.HasValue && Math.Abs(averageCalories - dailyCalorieGoal.Value) > 100)
                    recommendations.Add("For weight maintenance, try to stay within 100 calories of your daily goal. Monitor your weight weekly and adjust as needed.");
                recommendations.Add("For weight maintenance, focus on balanced nutrition and regular physical activity. Consider tracking your intake periodically to stay on track.");
                break;
        }
    }

    private static bool IsCalorieGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyCalorieGoal.HasValue && analysis.TotalCalories >= analysis.DailyCalorieGoal.Value * 0.8;

    private static bool IsProteinGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyProteinGoal.HasValue && analysis.TotalProtein >= analysis.DailyProteinGoal.Value * 0.8;

    private static bool IsFatGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyFatGoal.HasValue && analysis.TotalFat >= analysis.DailyFatGoal.Value * 0.8;

    private static bool IsCarbohydrateGoalMet(DailyNutritionalAnalysis analysis) =>
        analysis.DailyCarbohydrateGoal.HasValue && analysis.TotalCarbohydrates >= analysis.DailyCarbohydrateGoal.Value * 0.8;
}
