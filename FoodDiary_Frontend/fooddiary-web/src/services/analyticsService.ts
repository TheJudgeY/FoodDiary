import { API_BASE_URL } from '../constants';
import { getAuthToken } from '../utils';
import type { 
  DailyAnalysis, 
  GetTrendsResponse
} from '../types';

export const analyticsService = {
  
  getDailyAnalysis: async (date: string): Promise<DailyAnalysis> => {
    const token = getAuthToken();
    if (!token) {
      throw new Error('No authentication token available');
    }
    
    const requestBody = { date };
    const response = await fetch(`${API_BASE_URL}/api/analytics/daily`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestBody),
    });
    
    if (!response.ok) {
      throw new Error(`Failed to fetch daily analysis: ${response.statusText}`);
    }
    
    const data = await response.json();
    
    
    const analysisData = data.analysis || data;
    
    
    const processedData = {
      ...analysisData,
      
      overallStatus: analysisData.overallStatus || 'Poor',
      
      
      isCalorieGoalMet: analysisData.isCalorieGoalMet || false,
      isProteinGoalMet: analysisData.isProteinGoalMet || false,
      isFatGoalMet: analysisData.isFatGoalMet || false,
      isCarbohydrateGoalMet: analysisData.isCarbohydrateGoalMet || false,
      
      
      calorieProgressPercentage: analysisData.calorieProgressPercentage || 0,
      proteinProgressPercentage: analysisData.proteinProgressPercentage || 0,
      fatProgressPercentage: analysisData.fatProgressPercentage || 0,
      carbohydrateProgressPercentage: analysisData.carbohydrateProgressPercentage || 0,
      
      
      totalCalories: analysisData.totalCalories || 0,
      totalProtein: analysisData.totalProtein || 0,
      totalFat: analysisData.totalFat || 0,
      totalCarbohydrates: analysisData.totalCarbohydrates || 0,
      totalFoodEntries: analysisData.totalFoodEntries || 0,
      
      
      dailyCalorieGoal: analysisData.dailyCalorieGoal || 0,
      dailyProteinGoal: analysisData.dailyProteinGoal || 0,
      dailyFatGoal: analysisData.dailyFatGoal || 0,
      dailyCarbohydrateGoal: analysisData.dailyCarbohydrateGoal || 0,
      
      
      isOverCalorieLimit: analysisData.isOverCalorieLimit || false,
      isOverProteinLimit: analysisData.isOverProteinLimit || false,
      isOverFatLimit: analysisData.isOverFatLimit || false,
      isOverCarbohydrateLimit: analysisData.isOverCarbohydrateLimit || false,
      
      
      recommendations: analysisData.recommendations?.map((rec: string, index: number) => {
        const text = rec.toLowerCase();
        let type = 'info';

        
        if (text.includes('varies significantly') || text.includes('varies day to day')) {
          type = 'consistency';
        }
        
        else if (text.includes('meals per day') || text.includes('meal timing') || text.includes('meal prep')) {
          type = 'meal-pattern';
        }
        
        else if (text.includes('rarely meet') || text.includes('sometimes miss')) {
          type = 'goal';
        }
        
        else if (text.includes('highly inconsistent') || text.includes('moderate inconsistency')) {
          type = 'consistency';
        }
        
        else if (text.includes('weight loss') || text.includes('weight gain') || text.includes('maintenance')) {
          type = 'fitness';
        }

        return {
          index,
          text: rec,
          type
        };
      }) || []
    };
    

    
    return processedData;
  },

  
  getTrends: async (days: number = 7): Promise<GetTrendsResponse> => {
    const token = getAuthToken();
    if (!token) {
      throw new Error('No authentication token available');
    }
    
    const requestBody = { days };
    
    const response = await fetch(`${API_BASE_URL}/api/analytics/trends`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(requestBody),
    });
    
    if (!response.ok) {
      throw new Error(`Failed to fetch trends: ${response.statusText}`);
    }
    
    const data = await response.json();
    
    
    const { trends, recommendations, message } = data;
    
    
    const processedRecommendations = recommendations?.map((rec: string, index: number) => {
      const text = rec.toLowerCase();
      let type = 'info';

      
      if (text.includes('varies significantly') || text.includes('varies day to day') || 
          text.includes('highly inconsistent') || text.includes('inconsistent')) {
        type = 'consistency';
      }
      
      else if (text.includes('meals per day') || text.includes('meal timing') || text.includes('meal prep')) {
        type = 'meal-pattern';
      }
      
      else if (text.includes('rarely meet') || text.includes('sometimes miss') || text.includes('meet your')) {
        type = 'goal';
      }
      
      else if (text.includes('weight loss') || text.includes('weight gain') || text.includes('maintenance')) {
        type = 'fitness';
      }

      return {
        index,
        text: rec,
        type
      };
    });
    
    
    const processedTrends = {
      ...trends,
      
      averageCalories: trends.averageCalories || 0,
      averageProtein: trends.averageProtein || 0,
      averageFat: trends.averageFat || 0,
      averageCarbohydrates: trends.averageCarbohydrates || 0,
      
      calorieConsistency: trends.calorieConsistency || 0,
      proteinConsistency: trends.proteinConsistency || 0,
      fatConsistency: trends.fatConsistency || 0,
      carbohydrateConsistency: trends.carbohydrateConsistency || 0,
      
      goalAdherenceRate: (trends.goalAdherenceRate || 0) / 100, // Convert percentage to decimal (0-1)
      daysGoalsMet: trends.daysGoalsMet || 0,
      
      averageMealsPerDay: trends.averageMealsPerDay || 0,
      mostCommonMealTime: trends.mostCommonMealTime || 'Not enough data',
      leastCommonMealTime: trends.leastCommonMealTime || 'Not enough data',
      
      calorieTrend: trends.calorieTrend || 'Stable',
      proteinTrend: trends.proteinTrend || 'Stable',
      fatTrend: trends.fatTrend || 'Stable',
      carbohydrateTrend: trends.carbohydrateTrend || 'Stable',
      overallTrend: trends.overallTrend || 'Stable',
      
      isConsistent: trends.isConsistent || false,
      isImproving: trends.isImproving || false,
      trendInsights: trends.trendInsights || []
    };
    

    
    return { 
      trends: processedTrends, 
      recommendations: processedRecommendations, 
      message 
    };
  },
};

export default analyticsService;
