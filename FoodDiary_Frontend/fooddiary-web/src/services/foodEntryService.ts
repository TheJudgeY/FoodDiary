import { apiService } from './api';
import { API_ENDPOINTS } from '../constants';
import type { 
  FoodEntry, 
  CreateFoodEntryRequest, 
  UpdateFoodEntryRequest, 
  AddRecipeToDiaryRequest,
  AddRecipeToDiaryResponse
} from '../types';

export const foodEntryService = {
  
  getFoodEntries: async (params?: {
    date?: string;
    mealType?: number; 
  }): Promise<FoodEntry[]> => {
    const response = await apiService.get<FoodEntry[]>(API_ENDPOINTS.FOOD_ENTRIES.LIST, {
      params
    });
    return response;
  },

  
  createFoodEntry: async (data: CreateFoodEntryRequest): Promise<FoodEntry> => {
    const response = await apiService.post<FoodEntry>(API_ENDPOINTS.FOOD_ENTRIES.CREATE, data);
    return response;
  },

  
  getFoodEntry: async (id: string): Promise<FoodEntry> => {
    const response = await apiService.get<FoodEntry>(API_ENDPOINTS.FOOD_ENTRIES.GET(id));
    return response;
  },

  
  updateFoodEntry: async (id: string, data: UpdateFoodEntryRequest): Promise<FoodEntry> => {
    const response = await apiService.put<FoodEntry>(API_ENDPOINTS.FOOD_ENTRIES.UPDATE(id), data);
    return response;
  },

  
  deleteFoodEntry: async (id: string): Promise<void> => {
    await apiService.delete(API_ENDPOINTS.FOOD_ENTRIES.DELETE(id));
  },

  
  addRecipeToDiary: async (data: AddRecipeToDiaryRequest): Promise<AddRecipeToDiaryResponse> => {
    const response = await apiService.post<AddRecipeToDiaryResponse>(API_ENDPOINTS.FOOD_ENTRIES.ADD_RECIPE, data);
    return response;
  },
};

export default foodEntryService;
