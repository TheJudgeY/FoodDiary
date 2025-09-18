import React, { useState, useEffect } from 'react';
import { format, subDays, eachDayOfInterval } from 'date-fns';
import { 
  TrendingUp, 
  BarChart3, 
  Target, 
  Calendar,
  Activity,
  Info,
  Lightbulb
} from 'lucide-react';
import { NUTRITION_ICONS } from '../../constants/icons';
import { 
  LineChart, 
  Line, 
  BarChart, 
  Bar, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer
} from 'recharts';
import { useAuthStore } from '../../stores/authStore';
import { useProductStore } from '../../stores/productStore';
import { calculateNutritionalTargets, needsProfileCompletion, getProfileCompletionMessage, getErrorMessage } from '../../utils';
import { analyticsService } from '../../services/analyticsService';
import type { DailyAnalysis, NutritionalTrendsDTO, ProcessedRecommendation } from '../../types';
import Card from '../../components/UI/Card';
import LoadingSpinner from '../../components/UI/LoadingSpinner';

interface NutritionData {
  date: string;
  calories: number;
  proteins: number;
  fats: number;
  carbohydrates: number;
}

const AnalyticsPage: React.FC = () => {
  const { user } = useAuthStore();
  const { fetchProducts } = useProductStore();
  
  const [selectedPeriod, setSelectedPeriod] = useState<'week' | 'month'>('week');
  const [isLoading, setIsLoading] = useState(true);
  const [nutritionData, setNutritionData] = useState<NutritionData[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [trends, setTrends] = useState<NutritionalTrendsDTO | null>(null);
  const [recommendations, setRecommendations] = useState<ProcessedRecommendation[]>([]);
  const [dailyAnalysis, setDailyAnalysis] = useState<DailyAnalysis | null>(null);
  
  useEffect(() => {
    const loadAnalyticsData = async () => {
      if (!user) return;

      setIsLoading(true);
      setError(null);
      
      try {
        await fetchProducts();
        
      const days = selectedPeriod === 'week' ? 7 : 30;
        const endDate = new Date();
        const startDate = subDays(endDate, days - 1);
        
        // Get real data from the analytics API
        const trendsResponse = await analyticsService.getTrends(days);
        
        // Get trends data
        
        setTrends(trendsResponse.trends);
        setRecommendations(trendsResponse.recommendations || []);
        
        // Get today's daily analysis
      const today = format(new Date(), 'yyyy-MM-dd');
        const todayAnalysis = await analyticsService.getDailyAnalysis(today);
        setDailyAnalysis(todayAnalysis);
        
        const dateRange = eachDayOfInterval({ start: startDate, end: endDate });
        
        // Fetch daily analysis for each day in the range
        const dailyDataPromises = dateRange.map(async (date) => {
          try {
            const dateString = format(date, 'yyyy-MM-dd');
            const dailyAnalysis = await analyticsService.getDailyAnalysis(dateString);
            return {
              date: format(date, 'MMM dd'),
              calories: dailyAnalysis.totalCalories || 0,
              proteins: dailyAnalysis.totalProtein || 0,
              fats: dailyAnalysis.totalFat || 0,
              carbohydrates: dailyAnalysis.totalCarbohydrates || 0,
            };
          } catch {
            // If no data for this date, return zeros
    return {
              date: format(date, 'MMM dd'),
              calories: 0,
              proteins: 0,
              fats: 0,
              carbohydrates: 0,
            };
          }
        });
        
        const realData = await Promise.all(dailyDataPromises);
        setNutritionData(realData);
      } catch (loadError) {
        console.error('Failed to load analytics data:', loadError);
        setError(getErrorMessage(loadError));
      } finally {
        setIsLoading(false);
      }
    };

    loadAnalyticsData();
  }, [user, selectedPeriod, fetchProducts]);

  const nutritionalTargets = user ? calculateNutritionalTargets(user) : null;

  if (!user) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Card className="p-8 text-center">
          <Info className="h-12 w-12 text-warning-500 mx-auto mb-4" />
          <h2 className="text-xl font-semibold mb-2">Please log in to view analytics</h2>
          <p className="text-surface-600">You need to be logged in to access your nutrition analytics.</p>
        </Card>
      </div>
    );
  }

  if (needsProfileCompletion(user)) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Card className="p-8 text-center">
          <Target className="h-12 w-12 text-primary-500 mx-auto mb-4" />
          <h2 className="text-xl font-semibold mb-2">Complete Your Profile</h2>
          <p className="text-surface-600 mb-4">{getProfileCompletionMessage()}</p>
          <Lightbulb className="h-6 w-6 text-warning-500 inline mr-2" />
          <span className="text-sm text-surface-600">
            Analytics will be available once you complete your profile setup.
          </span>
        </Card>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="flex justify-center items-center h-64">
          <LoadingSpinner />
        </div>
      </div>
    );
  }

  if (error) {
  return (
      <div className="container mx-auto px-4 py-8">
        <Card className="p-8 text-center">
          <div className="text-error-500 mb-4">
            <Activity className="h-12 w-12 mx-auto" />
        </div>
          <h2 className="text-xl font-semibold mb-2">Error Loading Analytics</h2>
          <p className="text-surface-600">{error}</p>
        </Card>
        </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-surface-900 mb-2">Nutrition Analytics</h1>
        <p className="text-surface-600">Track your nutrition progress and trends</p>
      </div>

      {/* Daily Analysis Section */}
      {dailyAnalysis && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
          {/* Overall Status */}
          <Card className="p-6">
            <div className="flex items-center mb-4">
              <div className={`p-2 rounded-lg mr-3 ${
                dailyAnalysis.overallStatus === 'Excellent' ? 'bg-green-100' :
                dailyAnalysis.overallStatus === 'Good' ? 'bg-blue-100' :
                dailyAnalysis.overallStatus === 'Fair' ? 'bg-yellow-100' :
                dailyAnalysis.overallStatus === 'Poor' ? 'bg-red-100' :
                'bg-gray-100'
              }`}>
                <TrendingUp className={`h-5 w-5 ${
                  dailyAnalysis.overallStatus === 'Excellent' ? 'text-green-600' :
                  dailyAnalysis.overallStatus === 'Good' ? 'text-blue-600' :
                  dailyAnalysis.overallStatus === 'Fair' ? 'text-yellow-600' :
                  dailyAnalysis.overallStatus === 'Poor' ? 'text-red-600' :
                  'text-gray-600'
                }`} />
            </div>
            <div>
                <h3 className="text-lg font-semibold text-surface-900">Today's Analysis</h3>
                <p className="text-sm text-surface-600">Current day nutritional status</p>
            </div>
          </div>
            
            <div className="space-y-4">
              <div className="flex justify-between items-center">
                <span className="text-surface-700">Status</span>
                <span className={`font-semibold px-3 py-1 rounded-full text-sm ${
                  dailyAnalysis.overallStatus === 'Excellent' ? 'bg-green-100 text-green-800' :
                  dailyAnalysis.overallStatus === 'Good' ? 'bg-blue-100 text-blue-800' :
                  dailyAnalysis.overallStatus === 'Fair' ? 'bg-yellow-100 text-yellow-800' :
                  dailyAnalysis.overallStatus === 'Poor' ? 'bg-red-100 text-red-800' :
                  'bg-gray-100 text-gray-800'
                }`}>
                  {dailyAnalysis.overallStatus}
                </span>
            </div>

          <div className="space-y-2">
                <h4 className="font-medium text-surface-900 flex items-center">
                  <Activity className="h-4 w-4 mr-2 text-surface-600" />
                  Goal Achievement
                </h4>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-surface-600">Calories</span>
                    <span className={dailyAnalysis.isCalorieGoalMet ? 'text-green-600' : 'text-red-600'}>
                      {dailyAnalysis.isCalorieGoalMet ? '✓' : '✗'}
                    </span>
            </div>
                  <div className="flex justify-between">
                    <span className="text-surface-600">Protein</span>
                    <span className={dailyAnalysis.isProteinGoalMet ? 'text-green-600' : 'text-red-600'}>
                      {dailyAnalysis.isProteinGoalMet ? '✓' : '✗'}
                    </span>
          </div>
                  <div className="flex justify-between">
                    <span className="text-surface-600">Fat</span>
                    <span className={dailyAnalysis.isFatGoalMet ? 'text-green-600' : 'text-red-600'}>
                      {dailyAnalysis.isFatGoalMet ? '✓' : '✗'}
                    </span>
            </div>
                  <div className="flex justify-between">
                    <span className="text-surface-600">Carbs</span>
                    <span className={dailyAnalysis.isCarbohydrateGoalMet ? 'text-green-600' : 'text-red-600'}>
                      {dailyAnalysis.isCarbohydrateGoalMet ? '✓' : '✗'}
                    </span>
          </div>
            </div>
            </div>
          </div>
        </Card>

          {/* Today's Recommendations */}
          {dailyAnalysis.recommendations && dailyAnalysis.recommendations.length > 0 && (
        <Card className="p-6">
              <div className="flex items-center mb-4">
                <div className="p-2 bg-blue-100 rounded-lg mr-3">
                  <Lightbulb className="h-5 w-5 text-blue-600" />
            </div>
                <div>
                  <h3 className="text-lg font-semibold text-surface-900">Today's Insights</h3>
                  <p className="text-sm text-surface-600">Daily nutrition recommendations</p>
          </div>
              </div>
              
              <div className="space-y-3">
                {dailyAnalysis.recommendations.slice(0, 4).map((recommendation: ProcessedRecommendation, index: number) => (
                  <div key={index} className="flex items-start p-3 bg-blue-50 rounded-lg">
                    <span className="text-blue-500 mr-2 mt-0.5 flex-shrink-0">💡</span>
                    <p className="text-sm text-blue-800 leading-relaxed">
                      {recommendation.text}
                    </p>
            </div>
                ))}
                {dailyAnalysis.recommendations.length > 4 && (
                  <p className="text-xs text-surface-500 text-center">
                    +{dailyAnalysis.recommendations.length - 4} more recommendations below
                  </p>
                )}
          </div>
        </Card>
          )}
        </div>
      )}

      {/* Period Selector */}
      <div className="mb-6">
        <div className="flex gap-2">
          {(['week', 'month'] as const).map((period) => (
            <button
              key={period}
              onClick={() => setSelectedPeriod(period)}
              className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${
                selectedPeriod === period
                  ? 'bg-primary-600 text-white'
                  : 'bg-surface-100 text-surface-700 hover:bg-surface-200'
              }`}
            >
              <Calendar className="h-4 w-4 inline mr-2" />
              {period === 'week' ? 'Last 7 Days' : 'Last 30 Days'}
            </button>
          ))}
        </div>
      </div>

      {/* Nutrition Overview */}
      {nutritionalTargets && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          {[
            { key: 'calories', label: 'Calories', iconConfig: NUTRITION_ICONS.calories, target: nutritionalTargets.calories },
            { key: 'proteins', label: 'Protein', iconConfig: NUTRITION_ICONS.protein, target: nutritionalTargets.proteins },
            { key: 'fats', label: 'Fat', iconConfig: NUTRITION_ICONS.fat, target: nutritionalTargets.fats },
            { key: 'carbohydrates', label: 'Carbs', iconConfig: NUTRITION_ICONS.carbs, target: nutritionalTargets.carbohydrates },
          ].map(({ key, label, iconConfig, target }) => {
            const average = nutritionData.length > 0 
              ? Math.round(nutritionData.reduce((sum, day) => sum + (day[key as keyof NutritionData] as number), 0) / nutritionData.length)
              : 0;
            
            return (
              <Card key={key} className="p-6">
          <div className="flex items-center justify-between mb-4">
                  <div className="flex items-center">
                    <div className={`p-2 ${iconConfig.bgColor} rounded-lg mr-3`}>
                      <iconConfig.icon className={`h-5 w-5 ${iconConfig.color}`} />
            </div>
                    <div>
                      <h3 className="font-medium text-surface-900">{label}</h3>
                      <p className="text-sm text-surface-600">Daily Average</p>
          </div>
            </div>
                  <TrendingUp className={`h-5 w-5 ${iconConfig.color}`} />
          </div>
          <div className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-2xl font-bold text-surface-900">{average}</span>
                    <span className="text-sm text-surface-600">/ {target}</span>
                  </div>
            <div className="w-full bg-surface-200 rounded-full h-2">
              <div 
                      className={`${iconConfig.progressColor} h-2 rounded-full transition-all duration-300`}
                      style={{ width: `${Math.min((average / target) * 100, 100)}%` }}
                    />
            </div>
          </div>
        </Card>
            );
          })}
      </div>
      )}

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Calories Chart */}
        <Card className="p-6">
          <div className="flex items-center mb-6">
            <BarChart3 className="h-5 w-5 text-primary-600 mr-2" />
            <h3 className="text-lg font-semibold">Calorie Intake</h3>
            </div>
          <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
              <LineChart data={nutritionData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="date" />
                  <YAxis />
                  <Tooltip />
                  <Line 
                    type="monotone" 
                    dataKey="calories" 
                  stroke="#3B82F6" 
                    strokeWidth={2}
                  dot={{ fill: '#3B82F6' }}
                  />
                </LineChart>
              </ResponsiveContainer>
          </div>
        </Card>

        {/* Macronutrients Chart */}
        <Card className="p-6">
          <div className="flex items-center mb-6">
            <Target className="h-5 w-5 text-primary-600 mr-2" />
            <h3 className="text-lg font-semibold">Macronutrients</h3>
            </div>
          <div className="h-64">
              <ResponsiveContainer width="100%" height="100%">
              <BarChart data={nutritionData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis dataKey="date" />
                  <YAxis />
                  <Tooltip />
                <Bar dataKey="proteins" fill="#10B981" />
                <Bar dataKey="fats" fill="#F59E0B" />
                <Bar dataKey="carbohydrates" fill="#8B5CF6" />
                </BarChart>
              </ResponsiveContainer>
          </div>
        </Card>
      </div>

      {/* Trends Section */}
      {trends && (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mt-8">
          {/* Trend Overview */}
        <Card className="p-6">
            <div className="flex items-center mb-6">
              <TrendingUp className="h-5 w-5 text-primary-600 mr-2" />
              <h3 className="text-lg font-semibold">Nutrition Trends</h3>
            </div>
              <div className="space-y-4">
              <div className="flex justify-between items-center">
                <div className="flex items-center">
                  <div className="p-1 bg-purple-200 rounded-full mr-2">
                    <Target className="h-3 w-3 text-purple-600" />
                      </div>
                  <span className="text-sm text-purple-700 font-medium">Goal Adherence Rate</span>
                    </div>
                <span className={`font-bold text-lg ${
                  trends.goalAdherenceRate >= 0.8 ? 'text-green-600' :
                  trends.goalAdherenceRate >= 0.6 ? 'text-yellow-600' :
                  'text-red-600'
                }`}>{Math.round(Math.min(trends.goalAdherenceRate * 100, 100))}%</span>
                  </div>
              <div className="w-full bg-gray-200 rounded-full h-3 shadow-inner">
                <div 
                  className={`h-3 rounded-full transition-all duration-500 ${
                    trends.goalAdherenceRate >= 0.8 ? 'bg-gradient-to-r from-green-400 to-green-600' :
                    trends.goalAdherenceRate >= 0.6 ? 'bg-gradient-to-r from-yellow-400 to-yellow-600' :
                    'bg-gradient-to-r from-red-400 to-red-600'
                  }`}
                  style={{ width: `${Math.min(trends.goalAdherenceRate * 100, 100)}%` }}
                />
                  </div>
                  
              <div className="grid grid-cols-2 gap-4 mt-6">
                <div className="p-4 bg-gradient-to-r from-red-50 to-red-100 rounded-lg border border-red-200">
                  <div className="flex items-center mb-2">
                    <div className="p-1 bg-red-200 rounded-full mr-2">
                      <NUTRITION_ICONS.calories.icon className="h-3 w-3 text-red-600" />
                    </div>
                    <p className="text-sm text-red-700 font-medium">Average Calories</p>
                    </div>
                  <p className="text-xl font-bold text-red-900">{Math.round(trends.averageCalories)}</p>
                  </div>
                <div className="p-4 bg-gradient-to-r from-green-50 to-green-100 rounded-lg border border-green-200">
                  <div className="flex items-center mb-2">
                    <div className="p-1 bg-green-200 rounded-full mr-2">
                      <Target className="h-3 w-3 text-green-600" />
                    </div>
                    <p className="text-sm text-green-700 font-medium">Days Goals Met</p>
                    </div>
                  <p className="text-xl font-bold text-green-900">{trends.daysGoalsMet}</p>
                  </div>
                <div className="p-4 bg-gradient-to-r from-indigo-50 to-indigo-100 rounded-lg border border-indigo-200">
                  <div className="flex items-center mb-2">
                    <div className="p-1 bg-indigo-200 rounded-full mr-2">
                      <Calendar className="h-3 w-3 text-indigo-600" />
                    </div>
                    <p className="text-sm text-indigo-700 font-medium">Avg Meals/Day</p>
                    </div>
                  <p className="text-xl font-bold text-indigo-900">{trends.averageMealsPerDay?.toFixed(1) || '0.0'}</p>
                  </div>
                <div className={`p-4 rounded-lg border ${
                  trends.isConsistent 
                    ? 'bg-gradient-to-r from-emerald-50 to-emerald-100 border-emerald-200' 
                    : 'bg-gradient-to-r from-red-50 to-red-100 border-red-200'
                }`}>
                  <div className="flex items-center mb-2">
                    <div className={`p-1 rounded-full mr-2 ${
                      trends.isConsistent ? 'bg-emerald-200' : 'bg-red-200'
                    }`}>
                      <Activity className={`h-3 w-3 ${
                        trends.isConsistent ? 'text-emerald-600' : 'text-red-600'
                      }`} />
                </div>
                    <p className={`text-sm font-medium ${
                      trends.isConsistent ? 'text-emerald-700' : 'text-red-700'
                    }`}>Consistency</p>
                      </div>
                  <div className="flex items-center">
                    <p className={`text-xl font-bold ${
                      trends.isConsistent ? 'text-emerald-900' : 'text-red-900'
                    }`}>
                      {trends.isConsistent ? 'Consistent' : 'Inconsistent'}
                    </p>
                    <span className="ml-2 text-lg">
                      {trends.isConsistent ? '✓' : '✗'}
                    </span>
                      </div>
                    </div>
                  </div>

              {trends.trendInsights && trends.trendInsights.length > 0 && (
                <div className="mt-6 p-4 bg-gradient-to-r from-indigo-50 to-indigo-100 rounded-lg border border-indigo-200">
                  <div className="flex items-center mb-3">
                    <div className="p-1 bg-indigo-200 rounded-full mr-2">
                      <Lightbulb className="h-3 w-3 text-indigo-600" />
                    </div>
                    <h4 className="font-semibold text-indigo-900">Key Insights</h4>
                  </div>
                  <ul className="space-y-2">
                    {trends.trendInsights.map((insight: string, index: number) => (
                      <li key={index} className="flex items-start text-sm text-indigo-800">
                        <span className="text-indigo-500 mr-2 mt-0.5">💡</span>
                        <span className="leading-relaxed">{insight}</span>
                      </li>
                    ))}
                  </ul>
                  </div>
                )}
                        </div>
          </Card>

          {/* Macronutrient Trends */}
          <Card className="p-6">
            <div className="flex items-center mb-6">
              <Target className="h-5 w-5 text-primary-600 mr-2" />
              <h3 className="text-lg font-semibold">Macronutrient Trends</h3>
                      </div>
            <div className="space-y-4">
              {[
                { key: 'calorie', label: 'Calories', trend: trends.calorieTrend, avg: trends.averageCalories, iconConfig: NUTRITION_ICONS.calories },
                { key: 'protein', label: 'Protein', trend: trends.proteinTrend, avg: trends.averageProtein, iconConfig: NUTRITION_ICONS.protein },
                { key: 'fat', label: 'Fat', trend: trends.fatTrend, avg: trends.averageFat, iconConfig: NUTRITION_ICONS.fat },
                { key: 'carbohydrate', label: 'Carbs', trend: trends.carbohydrateTrend, avg: trends.averageCarbohydrates, iconConfig: NUTRITION_ICONS.carbs },
              ].map(({ key, label, trend, avg, iconConfig }) => (
                <div key={key} className={`flex justify-between items-center p-4 rounded-lg border-l-4 shadow-sm bg-gradient-to-r ${iconConfig.bgProgressColor} ${iconConfig.borderColor}`}>
                  <div className="flex items-center">
                    <div className={`p-2 rounded-full mr-3 ${iconConfig.bgColor}`}>
                      <iconConfig.icon className={`h-4 w-4 ${iconConfig.color}`} />
                      </div>
                      <div>
                      <p className={`font-semibold ${iconConfig.color.replace('text-', 'text-').replace('-600', '-900')}`}>{label}</p>
                      <p className={`text-sm ${iconConfig.color.replace('-600', '-700')}`}>Avg: {Math.round(avg)}</p>
                      </div>
                    </div>
                  <div className="flex items-center">
                    <span className={`text-sm font-semibold flex items-center ${
                      trend === 'Increasing' || trend === 'Improving' ? 'text-green-600' :
                      trend === 'Decreasing' || trend === 'Declining' ? 'text-red-600' :
                      'text-gray-600'
                    }`}>
                      {(trend === 'Increasing' || trend === 'Improving') && <span className="mr-1 text-lg">↗</span>}
                      {(trend === 'Decreasing' || trend === 'Declining') && <span className="mr-1 text-lg">↘</span>}
                      {trend === 'Stable' && <span className="mr-1 text-lg">→</span>}
                      {trend}
                    </span>
                  </div>
                      </div>
                    ))}
                  </div>
          </Card>
              </div>
            )}

      {/* Recommendations Section */}
      {recommendations && recommendations.length > 0 && (
        <Card className="p-6 mt-6">
          <div className="flex items-center mb-6">
            <Lightbulb className="h-5 w-5 text-warning-600 mr-2" />
            <h3 className="text-lg font-semibold">Personalized Recommendations</h3>
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {recommendations.map((rec, index) => (
              <div key={index} className={`p-4 rounded-lg border-l-4 ${
                rec.type === 'goal' ? 'bg-blue-50 border-blue-400' :
                rec.type === 'consistency' ? 'bg-yellow-50 border-yellow-400' :
                rec.type === 'meal-pattern' ? 'bg-green-50 border-green-400' :
                rec.type === 'fitness' ? 'bg-purple-50 border-purple-400' :
                'bg-gray-50 border-gray-400'
              }`}>
                <div className="flex items-start">
                  <div className={`p-1 rounded-full mr-3 mt-1 ${
                          rec.type === 'goal' ? 'bg-blue-100' :
                    rec.type === 'consistency' ? 'bg-yellow-100' :
                    rec.type === 'meal-pattern' ? 'bg-green-100' :
                    rec.type === 'fitness' ? 'bg-purple-100' :
                          'bg-gray-100'
                        }`}>
                    {rec.type === 'goal' && <Target className="h-3 w-3 text-blue-600" />}
                    {rec.type === 'consistency' && <Activity className="h-3 w-3 text-yellow-600" />}
                    {rec.type === 'meal-pattern' && <Calendar className="h-3 w-3 text-green-600" />}
                    {rec.type === 'fitness' && <TrendingUp className="h-3 w-3 text-purple-600" />}
                    {!['goal', 'consistency', 'meal-pattern', 'fitness'].includes(rec.type) && <Info className="h-3 w-3 text-gray-600" />}
                        </div>
                  <p className="text-sm text-surface-700 leading-relaxed">{rec.text}</p>
                      </div>
                        </div>
            ))}
                      </div>
        </Card>
      )}

      {/* Note about analytics */}
      <Card className="p-6 mt-6 bg-green-50 border-green-200">
        <div className="flex items-start">
          <Info className="h-5 w-5 text-green-600 mr-3 mt-0.5" />
                          <div>
            <h4 className="font-medium text-green-900 mb-1">Real-Time Analytics</h4>
            <p className="text-sm text-green-700">
              This analytics page displays real data from your food diary entries. 
              Charts and statistics are calculated based on your actual nutrition tracking data.
              {nutritionData.length === 0 ? " Add some food entries to see your analytics here!" : ""}
            </p>
                          </div>
          </div>
        </Card>
    </div>
  );
};

export default AnalyticsPage;