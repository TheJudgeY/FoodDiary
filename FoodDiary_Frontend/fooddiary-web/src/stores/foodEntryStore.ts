import { create } from 'zustand';
import type { FoodEntry, MealType, CreateFoodEntryRequest } from '../types';
import { getTodayString, getAuthToken, getMealTypeNumericValue, getErrorMessage } from '../utils';
import { API_BASE_URL } from '../constants';

interface FoodEntryState {
  entries: FoodEntry[];
  selectedDate: string;
  isLoading: boolean;
  error: string | null;
  
  
  fetchEntries: (date?: string) => Promise<void>;
  addEntry: (entry: CreateFoodEntryRequest) => Promise<void>;
  updateEntry: (id: string, entry: Partial<FoodEntry>) => Promise<void>;
  deleteEntry: (id: string) => Promise<void>;
  addRecipeToDiary: (recipeId: string, servings: number, mealType: MealType, date?: string) => Promise<void>;
  setSelectedDate: (date: string) => void;
  clearError: () => void;
  getEntriesByMeal: (mealType: MealType) => FoodEntry[];
  getTotalNutrition: () => { calories: number; proteins: number; fats: number; carbohydrates: number };
  setEntriesForAnalytics: (entries: FoodEntry[]) => void;
}

const handleApiError = (response: Response) => {
  if (response.status === 401) {
    
    window.location.href = '/login';
  } else if (response.status === 403) {
    throw new Error('Access denied');
  } else if (response.status === 404) {
    throw new Error('Food entry not found');
  } else {
    throw new Error('An error occurred');
  }
};

export const useFoodEntryStore = create<FoodEntryState>((set, get) => ({
  entries: [],
  selectedDate: getTodayString(),
  isLoading: false,
  error: null,

  fetchEntries: async (date?: string) => {
    const targetDate = date || get().selectedDate;
    set({ isLoading: true, error: null });
    
    try {
      const token = getAuthToken();
      if (!token) {
        throw new Error('No authentication token');
      }

      const response = await fetch(`${API_BASE_URL}/api/food-entries?date=${encodeURIComponent(targetDate)}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const responseData = await response.json();
      
      
      let entriesArray = [];
      if (Array.isArray(responseData)) {
        entriesArray = responseData;
      } else if (responseData && Array.isArray(responseData.foodEntries)) {
        entriesArray = responseData.foodEntries;
      } else if (responseData && Array.isArray(responseData.entries)) {
        entriesArray = responseData.entries;
      } else if (responseData && Array.isArray(responseData.data)) {
        entriesArray = responseData.data;
      } else {
        entriesArray = [];
      }
      
      set({ entries: entriesArray, isLoading: false });
     } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error) || 'Failed to fetch food entries',
        entries: [],
      });
    }
  },

  addEntry: async (entryData) => {
    set({ isLoading: true, error: null });
    try {
      const token = getAuthToken();
      if (!token) {
        throw new Error('No authentication token');
      }

      const response = await fetch(`${API_BASE_URL}/api/food-entries`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          consumedAt: entryData.date,
          mealType: getMealTypeNumericValue(entryData.mealType),
          productId: entryData.productId,
          weightGrams: Number(entryData.quantity),
          notes: '',
        })
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const newEntry = await response.json();
      set(state => ({
        entries: [...state.entries, newEntry],
        isLoading: false,
      }));
     } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error) || 'Failed to add food entry',
      });
    }
  },

  updateEntry: async (id, entryData) => {
    set({ isLoading: true, error: null });
    try {
      const token = getAuthToken();
      if (!token) {
        throw new Error('No authentication token');
      }

      const body: Record<string, unknown> = {
        mealType: entryData.mealType,
        weightGrams: Number((entryData as { quantityGrams?: number; quantity: number }).quantityGrams || entryData.quantity),
        notes: entryData.notes || '',
        consumedAt: entryData.date,
      };

      const response = await fetch(`${API_BASE_URL}/api/food-entries/${id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
      });
      
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        console.error('API Error Response:', {
          status: response.status,
          statusText: response.statusText,
          data: errorData
        });
        throw new Error(errorData.message || `HTTP ${response.status}: ${response.statusText}`);
      }
      
      const updatedEntry = await response.json();
      set(state => ({
        entries: state.entries.map(entry => 
          entry.id === id ? updatedEntry : entry
        ),
        isLoading: false,
      }));
      
      
      await get().fetchEntries();
     } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error) || 'Failed to update food entry',
      });
    }
  },

  deleteEntry: async (id) => {
    set({ isLoading: true, error: null });
    try {
      const token = getAuthToken();
      if (!token) {
        throw new Error('No authentication token');
      }

      const response = await fetch(`${API_BASE_URL}/api/food-entries/${id}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`
        }
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      set(state => ({
        entries: state.entries.filter(entry => entry.id !== id),
        isLoading: false,
      }));
     } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error) || 'Failed to delete food entry',
      });
    }
  },

  addRecipeToDiary: async (recipeId, servings, mealType, date) => {
    const targetDate = date || get().selectedDate;
    set({ isLoading: true, error: null });
    
    try {
      const token = getAuthToken();
      if (!token) {
        throw new Error('No authentication token');
      }

      const response = await fetch(`${API_BASE_URL}/api/food-entries/add-recipe`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          consumedAt: targetDate,
          mealType: getMealTypeNumericValue(mealType),
          recipeId,
          servings,
          notes: '',
        })
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const newEntries = await response.json();
      set(state => ({
        entries: [...state.entries, ...(newEntries || [])],
        isLoading: false,
      }));
     } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error) || 'Failed to add recipe to diary',
      });
    }
  },

  setSelectedDate: (date) => {
    set({ selectedDate: date });
    get().fetchEntries(date);
  },

  clearError: () => {
    set({ error: null });
  },

  getEntriesByMeal: (mealType) => {
    return get().entries.filter(entry => entry.mealType === mealType);
  },

  getTotalNutrition: () => {
    const entries = get().entries;
    
    if (!Array.isArray(entries)) {
      console.warn('entries is not an array:', entries);
      return { calories: 0, proteins: 0, fats: 0, carbohydrates: 0 };
    }
    
    return entries.reduce(
      (total, entry) => {
        // Use calculated values from API if available, otherwise fallback to 0
        const calories = entry.calories || 0;
        const proteins = entry.protein || 0; // Note: API uses 'protein', not 'proteins'
        const fats = entry.fat || 0; // Note: API uses 'fat', not 'fats'
        const carbohydrates = entry.carbohydrates || 0;
        
        return {
          calories: total.calories + calories,
          proteins: total.proteins + proteins,
          fats: total.fats + fats,
          carbohydrates: total.carbohydrates + carbohydrates,
        };
      },
      { calories: 0, proteins: 0, fats: 0, carbohydrates: 0 }
    );
  },

  setEntriesForAnalytics: (entries) => {
    set({ entries });
  },
}));
