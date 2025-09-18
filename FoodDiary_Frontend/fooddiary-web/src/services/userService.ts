import { apiService } from './api';
import { API_ENDPOINTS } from '../constants';
import type { 
  User, 
  BodyMetricsRequest, 
  FitnessGoalRequest, 
  MacronutrientGoalsRequest 
} from '../types';

export const userService = {
  
  getCurrentUser: async (): Promise<{ user: User }> => {
    const response = await apiService.get<{ user: User }>(API_ENDPOINTS.USER.ME);
    return response;
  },

  
  updateBodyMetrics: async (data: BodyMetricsRequest): Promise<{
    userId: string;
    heightCm: number;
    weightKg: number;
    age: number;
    gender: string;
    activityLevel: string;
    bmi: number;
    bmiCategory: string;
    bmr: number;
    tdee: number;
    recommendedCalories: number;
  }> => {
    return await apiService.put<{
      userId: string;
      heightCm: number;
      weightKg: number;
      age: number;
      gender: string;
      activityLevel: string;
      bmi: number;
      bmiCategory: string;
      bmr: number;
      tdee: number;
      recommendedCalories: number;
    }>(API_ENDPOINTS.USER.BODY_METRICS, data);
  },

  
  updateFitnessGoal: async (data: FitnessGoalRequest): Promise<{
    userId: string;
    fitnessGoal: string;
    targetWeightKg: number;
  }> => {
    return await apiService.put<{
      userId: string;
      fitnessGoal: string;
      targetWeightKg: number;
    }>(API_ENDPOINTS.USER.FITNESS_GOAL, data);
  },

  
  updateMacronutrientGoals: async (data: MacronutrientGoalsRequest): Promise<{
    userId: string;
    dailyCalorieGoal: number;
    dailyProteinGoal: number;
    dailyFatGoal: number;
    dailyCarbohydrateGoal: number;
  }> => {
    return await apiService.put<{
      userId: string;
      dailyCalorieGoal: number;
      dailyProteinGoal: number;
      dailyFatGoal: number;
      dailyCarbohydrateGoal: number;
    }>(API_ENDPOINTS.USER.MACRONUTRIENT_GOALS, data);
  },
};

export default userService;
