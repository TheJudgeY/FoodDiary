import { format, parseISO, isValid, differenceInDays } from 'date-fns';
import { clsx, type ClassValue } from 'clsx';
import { twMerge } from 'tailwind-merge';
import { API_BASE_URL } from '../constants';
import { MealType, Gender, ActivityLevel, FitnessGoal } from '../types';
import { PRODUCT_CATEGORIES } from '../constants/productCategories';
import { RECIPE_CATEGORIES } from '../constants/recipeCategories';


export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}


export function formatDate(date: string | Date, formatString: string = 'MMM dd, yyyy'): string {
  try {
    const dateObj = typeof date === 'string' ? parseISO(date) : date;
    if (!isValid(dateObj)) return 'Invalid date';
    return format(dateObj, formatString);
  } catch {
    return 'Invalid date';
  }
}

export function formatDateTime(date: string | Date): string {
  return formatDate(date, 'MMM dd, yyyy HH:mm');
}

export function formatTime(date: string | Date): string {
  return formatDate(date, 'HH:mm');
}

export function getTodayString(): string {
  return format(new Date(), 'yyyy-MM-dd');
}

export function getDateFromString(dateString: string): Date {
  return parseISO(dateString);
}

export function isToday(date: string | Date): boolean {
  const dateObj = typeof date === 'string' ? parseISO(date) : date;
  const today = new Date();
  return format(dateObj, 'yyyy-MM-dd') === format(today, 'yyyy-MM-dd');
}

export function isYesterday(date: string | Date): boolean {
  const dateObj = typeof date === 'string' ? parseISO(date) : date;
  const yesterday = new Date();
  yesterday.setDate(yesterday.getDate() - 1);
  return format(dateObj, 'yyyy-MM-dd') === format(yesterday, 'yyyy-MM-dd');
}

export function getDaysDifference(date1: string | Date, date2: string | Date): number {
  const date1Obj = typeof date1 === 'string' ? parseISO(date1) : date1;
  const date2Obj = typeof date2 === 'string' ? parseISO(date2) : date2;
  return differenceInDays(date1Obj, date2Obj);
}


export function formatNumber(value: number, decimals: number = 0): string {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: decimals,
    maximumFractionDigits: decimals,
  }).format(value);
}

export function formatCalories(calories: number): string {
  return formatNumber(calories, 0);
}

export function formatWeight(weight: number): string {
  return formatNumber(weight, 1);
}

export function formatPercentage(value: number, total: number): string {
  if (total === 0) return '0%';
  return formatNumber((value / total) * 100, 1) + '%';
}

export function formatProgress(value: number, total: number): number {
  if (total === 0) return 0;
  return Math.min((value / total) * 100, 100);
}


export function isValidEmail(email: string): boolean {
  const emailRegex = /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i;
  return emailRegex.test(email);
}

export function isValidPassword(password: string): boolean {
  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
  return passwordRegex.test(password);
}

export function validateRequired(value: unknown): boolean {
  return value !== null && value !== undefined && value !== '';
}

export function validateMinLength(value: string, minLength: number): boolean {
  return value.length >= minLength;
}

export function validateMaxLength(value: string, maxLength: number): boolean {
  return value.length <= maxLength;
}

export function validateRange(value: number, min: number, max: number): boolean {
  return value >= min && value <= max;
}


export function getStorageItem<T>(key: string): T | null {
  try {
    const item = localStorage.getItem(key);
    return item ? JSON.parse(item) : null;
  } catch {
    return null;
  }
}

export function setStorageItem<T>(key: string, value: T): void {
  try {
    localStorage.setItem(key, JSON.stringify(value));
  } catch (error) {
    console.error('Error saving to localStorage:', error);
  }
}

export function removeStorageItem(key: string): void {
  try {
    localStorage.removeItem(key);
  } catch (error) {
    console.error('Error removing from localStorage:', error);
  }
}

export function clearStorage(): void {
  try {
    localStorage.clear();
  } catch (error) {
    console.error('Error clearing localStorage:', error);
  }
}


export function getAuthToken(): string | null {
  return getStorageItem<string>('fooddiary_auth_token');
}

export function setAuthToken(token: string): void {
  setStorageItem('fooddiary_auth_token', token);
}

export function removeAuthToken(): void {
  removeStorageItem('fooddiary_auth_token');
}

export function clearAllAuthData(): void {
  removeStorageItem('fooddiary_auth_token');
  localStorage.removeItem('auth-storage');
  sessionStorage.removeItem('fooddiary_auth_token');
}

export function isAuthenticated(): boolean {
  return !!getAuthToken();
}


export function getErrorMessage(error: unknown): string {
  if (typeof error === 'string') return error;
  
  const apiError = error as { response?: { data?: { message?: string; errors?: Record<string, string[]> } }; message?: string };
  
  if (apiError?.response?.data?.message) {
    return apiError.response.data.message;
  }
  
  if (apiError?.response?.data?.errors) {
    const errors = apiError.response.data.errors;
    const firstError = Object.values(errors)[0];
    if (Array.isArray(firstError) && firstError.length > 0) {
      return firstError[0];
    }
  }
  
  if (apiError?.message) return apiError.message;
  
  return 'An unexpected error occurred';
}

export function handleApiError(error: unknown): string {
  const message = getErrorMessage(error);
  
  const apiError = error as { response?: { status?: number } };
  if (apiError?.response?.status === 401) {
    removeAuthToken();
    window.location.href = '/login';
  }
  
  return message;
}


export function groupBy<T>(array: T[], key: keyof T): Record<string, T[]> {
  return array.reduce((groups, item) => {
    const group = String(item[key]);
    groups[group] = groups[group] || [];
    groups[group].push(item);
    return groups;
  }, {} as Record<string, T[]>);
}

export function sortBy<T>(array: T[], key: keyof T, direction: 'asc' | 'desc' = 'asc'): T[] {
  return [...array].sort((a, b) => {
    const aValue = a[key];
    const bValue = b[key];
    
    if (aValue < bValue) return direction === 'asc' ? -1 : 1;
    if (aValue > bValue) return direction === 'asc' ? 1 : -1;
    return 0;
  });
}

export function uniqueBy<T>(array: T[], key: keyof T): T[] {
  const seen = new Set();
  return array.filter(item => {
    const value = item[key];
    if (seen.has(value)) return false;
    seen.add(value);
    return true;
  });
}


export function capitalize(str: string): string {
  return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
}

export function truncate(str: string, length: number): string {
  if (str.length <= length) return str;
  return str.slice(0, length) + '...';
}

export function slugify(str: string): string {
  return str
    .toLowerCase()
    .replace(/[^a-z0-9 -]/g, '')
    .replace(/\s+/g, '-')
    .replace(/-+/g, '-')
    .trim();
}


export function getContrastColor(hexColor: string): string {
  
  const hex = hexColor.replace('#', '');
  
  
  const r = parseInt(hex.substr(0, 2), 16);
  const g = parseInt(hex.substr(2, 2), 16);
  const b = parseInt(hex.substr(4, 2), 16);
  
  
  const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;
  
  
  return luminance > 0.5 ? '#000000' : '#ffffff';
}


export function debounce<T extends (...args: unknown[]) => unknown>(
  func: T,
  wait: number
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout;
  return (...args: Parameters<T>) => {
    clearTimeout(timeout);
    timeout = setTimeout(() => func(...args), wait);
  };
}


export function throttle<T extends (...args: unknown[]) => unknown>(
  func: T,
  limit: number
): (...args: Parameters<T>) => void {
  let inThrottle: boolean;
  return (...args: Parameters<T>) => {
    if (!inThrottle) {
      func(...args);
      inThrottle = true;
      setTimeout(() => (inThrottle = false), limit);
    }
  };
}


export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 Bytes';
  
  const k = 1024;
  const sizes = ['Bytes', 'KB', 'MB', 'GB'];
  const i = Math.floor(Math.log(bytes) / Math.log(k));
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

export function isValidImageFile(file: File): boolean {
  const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/webp'];
  const maxSize = 5 * 1024 * 1024; 
  
  return validTypes.includes(file.type) && file.size <= maxSize;
}


export function getQueryParam(name: string): string | null {
  const urlParams = new URLSearchParams(window.location.search);
  return urlParams.get(name);
}

export function setQueryParam(name: string, value: string): void {
  const url = new URL(window.location.href);
  url.searchParams.set(name, value);
  window.history.replaceState({}, '', url.toString());
}

export function removeQueryParam(name: string): void {
  const url = new URL(window.location.href);
  url.searchParams.delete(name);
  window.history.replaceState({}, '', url.toString());
}


export function isMobile(): boolean {
  return window.innerWidth < 768;
}

export function isTablet(): boolean {
  return window.innerWidth >= 768 && window.innerWidth < 1024;
}

export function isDesktop(): boolean {
  return window.innerWidth >= 1024;
}

export function isTouchDevice(): boolean {
  return 'ontouchstart' in window || navigator.maxTouchPoints > 0;
}


export function getSystemTheme(): 'light' | 'dark' {
  return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

export function applyTheme(theme: 'light' | 'dark' | 'system'): void {
  const actualTheme = theme === 'system' ? getSystemTheme() : theme;
  document.documentElement.classList.toggle('dark', actualTheme === 'dark');
}


export const getProductImageUrl = (productId: string) => {
  return `${API_BASE_URL}/api/products/${productId}?includeImage=true`;
};


export const getProductImageWithAuth = async (productId: string): Promise<string> => {
  const token = getAuthToken();
  if (!token) {
    throw new Error('No authentication token');
  }

  const response = await fetch(`${API_BASE_URL}/api/products/${productId}?includeImage=true`, {
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });

  if (!response.ok) {
    throw new Error('Failed to fetch image');
  }

  const blob = await response.blob();
  return URL.createObjectURL(blob);
};


export const getRecipeImageWithAuth = async (recipeId: string): Promise<string> => {
  const token = getAuthToken();
  if (!token) {
    throw new Error('No authentication token');
  }

  try {
    

    const response = await fetch(`${API_BASE_URL}/api/recipes/${recipeId}/image`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });



    if (!response.ok) {
      const errorText = await response.text();
      console.error(`Failed to fetch recipe image: ${response.status} ${response.statusText}`);
      console.error(`Error response: ${errorText}`);
      throw new Error(`Failed to fetch recipe image: ${response.status} ${response.statusText}`);
    }

    
    const contentType = response.headers.get('content-type');
    if (contentType && contentType.includes('application/json')) {
      
      
      const jsonResponse = await response.json();
      
      
      if (jsonResponse.imageDataUrl || jsonResponse.imageData || jsonResponse.imageUrl || jsonResponse.image) {
        if (jsonResponse.imageDataUrl) {
          return jsonResponse.imageDataUrl;
        }
        
        if (jsonResponse.imageData) {
          const base64Data = jsonResponse.imageData;
          const mimeType = jsonResponse.imageContentType || jsonResponse.contentType || 'image/png';
          return `data:${mimeType};base64,${base64Data}`;
        }
        
        if (jsonResponse.imageUrl) {
          return jsonResponse.imageUrl;
        }
        
        if (jsonResponse.image) {
          return jsonResponse.image;
        }
      }
      
      
      throw new Error('Recipe image data not found in response');
    }

    const blob = await response.blob();
    
    if (blob.size === 0) {
      throw new Error('Recipe image blob is empty');
    }

    const url = URL.createObjectURL(blob);
    return url;
  } catch (error) {
    console.error(`Error in getRecipeImageWithAuth for recipe ${recipeId}:`, error);
    throw error;
  }
};


export const getRecipeImageUrl = (recipeId: string, imageFileName?: string): string => {
  if (!imageFileName) {
    throw new Error('No image filename provided');
  }
  
  
  
  
  return `${API_BASE_URL}/api/recipes/${recipeId}/image`;
};


export const getMealTypeLabel = (mealType: MealType | number): string => {
  
  if (typeof mealType === 'number') {
    const MEAL_TYPES = {
      0: "Breakfast",
      1: "Lunch", 
      2: "Dinner",
      3: "Snack"
    } as const;
    return MEAL_TYPES[mealType as keyof typeof MEAL_TYPES] || 'Unknown';
  }
  
  
  switch (mealType) {
    case MealType.Breakfast: return 'Breakfast';
    case MealType.Lunch: return 'Lunch';
    case MealType.Dinner: return 'Dinner';
    case MealType.Snack: return 'Snack';
    default: return 'Unknown';
  }
};

export const getMealTypeIcon = (mealType: MealType | number): string => {
  
  if (typeof mealType === 'number') {
    const MEAL_ICONS = {
      0: '🌅', 
      1: '🌞', 
      2: '🌙', 
      3: '🍎'  
    } as const;
    return MEAL_ICONS[mealType as keyof typeof MEAL_ICONS] || '🍽️';
  }
  
  
  switch (mealType) {
    case MealType.Breakfast: return '🌅';
    case MealType.Lunch: return '🌞';
    case MealType.Dinner: return '🌙';
    case MealType.Snack: return '🍎';
    default: return '🍽️';
  }
};

export const getMealTypeNumericValue = (mealType: MealType | string | number): number => {
  
  if (typeof mealType === 'number') {
    return mealType;
  }
  
  
  if (typeof mealType === 'string') {
    const mealTypeMap = {
      'Breakfast': 0,
      'Lunch': 1,
      'Dinner': 2,
      'Snack': 3
    } as const;
    return mealTypeMap[mealType as keyof typeof mealTypeMap] ?? 0;
  }
  
  
  return mealType;
};


export const getMealTypeFromNumber = (mealTypeNumber: number): MealType => {
  switch (mealTypeNumber) {
    case 0: return MealType.Breakfast;
    case 1: return MealType.Lunch;
    case 2: return MealType.Dinner;
    case 3: return MealType.Snack;
    default: return MealType.Breakfast; 
  }
};


export const calculateNutritionalTargets = (user: unknown) => {
  const typedUser = user as { 
    hasCompleteProfile?: boolean; 
    dailyCalorieGoal?: number; 
    recommendedCalories?: number;
    dailyProteinGoal?: number;
    dailyFatGoal?: number;
    dailyCarbohydrateGoal?: number;
  } | null;
  
  if (!typedUser) {
    return {
      calories: 2000,
      proteins: 150,
      fats: 65,
      carbohydrates: 250,
    };
  }

  
  if (!typedUser.hasCompleteProfile) {
    
    return {
      calories: 2000,
      proteins: 150,
      fats: 65,
      carbohydrates: 250,
    };
  }

  
  const dailyCalories = typedUser.dailyCalorieGoal || typedUser.recommendedCalories || 2000;
  
  return {
    calories: dailyCalories,
    proteins: typedUser.dailyProteinGoal || (dailyCalories * 0.25) / 4, 
    fats: typedUser.dailyFatGoal || (dailyCalories * 0.30) / 9, 
    carbohydrates: typedUser.dailyCarbohydrateGoal || (dailyCalories * 0.45) / 4, 
  };
};


export const calculateProgressPercentage = (current: number, target: number) => {
  if (target === 0) return 0;
  return Math.min((current / target) * 100, 100);
};


export const needsProfileCompletion = (user: unknown) => {
  const typedUser = user as { hasCompleteProfile?: boolean } | null;
  return typedUser && !typedUser.hasCompleteProfile;
};


export const getProfileCompletionMessage = () => {
  return "Complete your profile to get personalized nutritional targets based on your body metrics and fitness goals.";
};


export const getGenderLabel = (gender: Gender): string => {
  switch (gender) {
    case Gender.Male: return 'Male';
    case Gender.Female: return 'Female';
    case Gender.Other: return 'Other';
    default: return 'Unknown';
  }
};

export const getGenderFromNumber = (genderNumber: number): Gender => {
  switch (genderNumber) {
    case 0: return Gender.Male;
    case 1: return Gender.Female;
    case 2: return Gender.Other;
    default: return Gender.Male; 
  }
};


export const getActivityLevelLabel = (activityLevel: ActivityLevel): string => {
  switch (activityLevel) {
    case ActivityLevel.Sedentary: return 'Sedentary';
    case ActivityLevel.LightlyActive: return 'Lightly Active';
    case ActivityLevel.ModeratelyActive: return 'Moderately Active';
    case ActivityLevel.VeryActive: return 'Very Active';
    case ActivityLevel.ExtremelyActive: return 'Extremely Active';
    default: return 'Unknown';
  }
};

export const getActivityLevelFromNumber = (activityLevelNumber: number): ActivityLevel => {
  switch (activityLevelNumber) {
    case 0: return ActivityLevel.Sedentary;
    case 1: return ActivityLevel.LightlyActive;
    case 2: return ActivityLevel.ModeratelyActive;
    case 3: return ActivityLevel.VeryActive;
    case 4: return ActivityLevel.ExtremelyActive;
    default: return ActivityLevel.ModeratelyActive; 
  }
};


export const getFitnessGoalLabel = (fitnessGoal: FitnessGoal): string => {
  switch (fitnessGoal) {
    case FitnessGoal.LoseWeight: return 'Lose Weight';
    case FitnessGoal.MaintainWeight: return 'Maintain Weight';
    case FitnessGoal.GainWeight: return 'Gain Weight';
    default: return 'Maintain Weight';
  }
};

export const getFitnessGoalFromNumber = (fitnessGoalNumber: number): FitnessGoal => {
  switch (fitnessGoalNumber) {
    case 0: return FitnessGoal.LoseWeight;
    case 1: return FitnessGoal.MaintainWeight;
    case 2: return FitnessGoal.GainWeight;
    default: return FitnessGoal.MaintainWeight; 
  }
};


export const getProductCategoryLabel = (category: string | number): string => {
  
  if (typeof category === 'string') {
    return category;
  }
  
  
  if (PRODUCT_CATEGORIES[category as keyof typeof PRODUCT_CATEGORIES] !== undefined) {
    return PRODUCT_CATEGORIES[category as keyof typeof PRODUCT_CATEGORIES];
  }
  
  
  console.warn(`Unknown product category number: ${category}`);
  return 'Other';
};


export const getRecipeCategoryLabel = (category: string | number): string => {
  
  if (typeof category === 'string') {
    return category;
  }
  
  
  if (RECIPE_CATEGORIES[category as keyof typeof RECIPE_CATEGORIES] !== undefined) {
    return RECIPE_CATEGORIES[category as keyof typeof RECIPE_CATEGORIES];
  }
  
  
  console.warn(`Unknown recipe category number: ${category}`);
  return 'Other';
};
