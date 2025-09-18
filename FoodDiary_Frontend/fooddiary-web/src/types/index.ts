export interface ApiResponse<T> {
  data?: T;
  message?: string;
  errors?: Record<string, string[]>;
  statusCode?: number;
}
export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
}
export interface LoginRequest {
  email: string;
  password: string;
}
export interface AuthResponse {
  userId: string;
  email: string;
  name: string;
  token?: string;
  message?: string;
}
export interface User {
  id: string;
  email: string;
  name: string;
  heightCm?: number;
  weightKg?: number;
  age?: number;
  gender?: Gender;
  activityLevel?: ActivityLevel;
  fitnessGoal?: FitnessGoal;
  targetWeightKg?: number;
  dailyCalorieGoal?: number;
  dailyProteinGoal?: number;
  dailyFatGoal?: number;
  dailyCarbohydrateGoal?: number;
  bmi?: number;
  bmiCategory?: string;
  bmr?: number;
  tdee?: number;
  recommendedCalories?: number;
  hasCompleteProfile: boolean;
  createdAt: string;
  updatedAt: string;
  timeZoneId?: string;
}
export interface BodyMetricsRequest {
  heightCm: number;
  weightKg: number;
  age: number;
  gender: Gender;
  activityLevel: ActivityLevel;
}
export interface FitnessGoalRequest {
  fitnessGoal: FitnessGoal;
  targetWeightKg: number;
}
export interface MacronutrientGoalsRequest {
  dailyCalorieGoal: number;
  dailyProteinGoal: number;
  dailyFatGoal: number;
  dailyCarbohydrateGoal: number;
}
export interface FoodEntry {
  id: string;
  date: string;
  consumedAt?: string;
  mealType: number | MealType; 
  mealTypeDisplayName?: string; 
  productId: string;
  productName?: string; 
  quantity: number;
  calories: number;
  protein: number; 
  fat: number; 
  carbohydrates: number;
  notes?: string; 
  createdAt?: string;
  updatedAt?: string;
}
export interface CreateFoodEntryRequest {
  date: string;
  consumedAt: string;
  mealType: number; 
  productId: string;
  quantity: number;
  notes?: string;
}
export interface UpdateFoodEntryRequest {
  date: string;
  consumedAt?: string;
  mealType: number; 
  productId: string;
  quantity: number;
  notes?: string;
}
export interface AddRecipeToDiaryRequest {
  recipeId: string;                    
  mealType: MealType;                  
  consumedAt: string;                  
  servingsConsumed: number;            
  notes?: string;                      
}
export interface FoodEntrySummary {
  id: string;
  productId: string;
  productName: string;
  weightGrams: number;
  calories: number;
  protein: number;
  fat: number;
  carbohydrates: number;
}
export interface AddRecipeToDiaryResponse {
  recipeId: string;
  recipeName: string;
  foodEntriesCreated: number;
  totalCalories: number;
  totalProtein: number;
  totalFat: number;
  totalCarbohydrates: number;
  foodEntries: FoodEntrySummary[];
  message: string;
}
export interface Product {
  id: string;
  name: string;
  caloriesPer100g: number;
  proteinsPer100g: number;
  fatsPer100g: number;
  carbohydratesPer100g: number;
  description?: string;
  category: number; 
  categoryDisplayName?: string; 
  imageFileName?: string;
  imageContentType?: string;
  imageUrl?: string;
  hasImage: boolean;
  createdAt: string;
  updatedAt: string;
}
export interface DeleteProductResponse {
  canDelete: boolean;
  message: string;
  foodEntryCount: number;
  recipeIngredientCount: number;
  references: string[];
}
export interface CreateProductRequest {
  name: string;
  caloriesPer100g: number;
  proteinsPer100g: number;
  fatsPer100g: number;
  carbohydratesPer100g: number;
  description?: string;
  category: number; 
}
export interface UpdateProductRequest {
  name: string;
  caloriesPer100g: number;
  proteinsPer100g: number;
  fatsPer100g: number;
  carbohydratesPer100g: number;
  description?: string;
  category: number; 
}
export interface Recipe {
  id: string;
  name: string;
  description?: string;
  instructions?: string;
  servings: number;
  preparationTimeMinutes: number;
  cookingTimeMinutes: number;
  category: number; 
  categoryDisplayName?: string; 
  isPublic: boolean;
  isFavorite: boolean;
  hasImage: boolean;
  imageFileName?: string;
  imageContentType?: string;
  ingredients?: RecipeIngredient[];
  totalCalories: number;
  totalProtein: number;
  totalFat: number;
  totalCarbohydrates: number;
  caloriesPerServing: number;
  proteinPerServing: number;
  fatPerServing: number;
  carbohydratesPerServing: number;
  ingredientCount: number;
  createdAt: string;
  updatedAt: string;
  isCreator?: boolean;
  isContributor?: boolean;
  userRelationship?: RelationshipType;
  canEdit?: boolean;
  canDelete?: boolean;
}
export interface RecipeIngredient {
  id: string;
  productId: string;
  productName?: string;
  quantityGrams: number;
  notes?: string;
  calories?: number;
  protein?: number;
  fat?: number;
  carbohydrates?: number;
  product?: Product;
  customIngredientName?: string;
  customCaloriesPer100g?: number;
  customProteinPer100g?: number;
  customFatPer100g?: number;
  customCarbohydratesPer100g?: number;
}
export interface RecipeRelationship {
  id: string;
  userId: string;
  recipeId: string;
  relationshipType: RelationshipType;
  createdAt: string;
}
export interface CreateRecipeRequest {
  name: string;
  description?: string;
  instructions?: string;
  servings: number;
  preparationTimeMinutes?: number;
  cookingTimeMinutes?: number;
  category: number; 
  isPublic: boolean;
  ingredients: {
    productId: string;
    quantityGrams: number;
    notes?: string;
  }[];
}
export interface UpdateRecipeRequest {
  name: string;
  description?: string;
  instructions?: string;
  servings: number;
  preparationTimeMinutes?: number;
  cookingTimeMinutes?: number;
  category: number; 
  isPublic: boolean;
  ingredients?: {
    ingredientId?: string;
    productId: string;
    quantityGrams: number;
    notes?: string;
  }[];
}
export interface DailyAnalysis {
  date: string;
  totalCalories: number;
  totalProtein: number;
  totalFat: number;
  totalCarbohydrates: number;
  dailyCalorieGoal: number;
  dailyProteinGoal: number;
  dailyFatGoal: number;
  dailyCarbohydrateGoal: number;
  isCalorieGoalMet: boolean;
  isProteinGoalMet: boolean;
  isFatGoalMet: boolean;
  isCarbohydrateGoalMet: boolean;
  overallStatus: string;
  recommendations: ProcessedRecommendation[];
  isOverCalorieLimit: boolean;
  isOverProteinLimit: boolean;
  isOverFatLimit: boolean;
  isOverCarbohydrateLimit: boolean;
  mealBreakdown?: {
    breakfast?: {
      calories: number;
      protein: number;
      fat: number;
      carbohydrates: number;
    };
    lunch?: {
      calories: number;
      protein: number;
      fat: number;
      carbohydrates: number;
    };
    dinner?: {
      calories: number;
      protein: number;
      fat: number;
      carbohydrates: number;
    };
  };
  totalProteins?: number;
  totalFats?: number;
  goalCalories?: number;
  goalProteins?: number;
  goalFats?: number;
  goalCarbohydrates?: number;
  calorieProgress?: number;
  proteinProgress?: number;
  fatProgress?: number;
  carbohydrateProgress?: number;
  bmi?: number;
  bmiCategory?: string;
  weightChange?: number;
  calorieGoalMet?: boolean;
  proteinGoalMet?: boolean;
  fatGoalMet?: boolean;
  carbohydrateGoalMet?: boolean;
}
export interface TrendData {
  date: string;
  calories: number;
  proteins: number;
  fats: number;
  carbohydrates: number;
  weight?: number;
}
export interface NutritionalTrendsDTO {
  id: string;
  userId: string;
  analysisDate: string;
  daysAnalyzed: number;
  averageCalories: number;
  averageProtein: number;
  averageFat: number;
  averageCarbohydrates: number;
  calorieTrend: string;        
  proteinTrend: string;
  fatTrend: string;
  carbohydrateTrend: string;
  calorieConsistency: number;
  proteinConsistency: number;
  fatConsistency: number;
  carbohydrateConsistency: number;
  goalAdherenceRate: number;   
  daysGoalsMet: number;
  totalDaysAnalyzed: number;
  averageMealsPerDay: number;
  mostCommonMealTime: string;
  leastCommonMealTime: string;
  overallTrend: string;        
  trendInsights: string[];     
  isConsistent: boolean;
  isImproving: boolean;
}
export interface GetTrendsRequest {
  days?: number; 
}
export interface ProcessedRecommendation {
  index: number;
  text: string;
  type: string;
}

export interface GetTrendsResponse {
  trends: NutritionalTrendsDTO;
  recommendations: ProcessedRecommendation[];
  message: string;
}
export interface RecommendationsResponse {
  date?: string;
  recommendations: string[];
  priority?: string;
  nextMealSuggestions?: Array<{
    mealType: string;
    suggestedCalories: number;
    suggestedProtein: number;
    suggestedFat: number;
    suggestedCarbohydrates: number;
  }>;
  nextActions?: string[];
}
export interface Notification {
  id: string;
  userId: string;
  title: string;
  message: string;
  type: NotificationType;
  priority: NotificationPriority;
  status: NotificationStatus;
  contextData: string;
  imageUrl?: string;
  createdAt: string;
  readAt?: string;
  isUnread: boolean;
  isRead: boolean;
  age: string;
  isRecent: boolean;
  priorityColor: string;
  typeIcon: string;
}
export interface NotificationPreferences {
  waterRemindersEnabled: boolean;
  mealRemindersEnabled: boolean;
  calorieLimitWarningsEnabled: boolean;
  goalAchievementsEnabled: boolean;
  weeklyProgressEnabled: boolean;
  dailySummaryEnabled: boolean;
  waterReminderTime: string;
  breakfastReminderTime: string;
  lunchReminderTime: string;
  dinnerReminderTime: string;
  waterReminderFrequencyHours: number;
  sendNotificationsOnWeekends: boolean;
  createdAt: string;
  updatedAt: string;
}
export enum MealType {
  Breakfast = 0,
  Lunch = 1,
  Dinner = 2,
  Snack = 3,
}
export enum ProductCategory {
  Fruits = 0,
  Vegetables = 1,
  Grains = 2,
  Proteins = 3,
  Dairy = 4,
  NutsAndSeeds = 5,
  Beverages = 6,
  Snacks = 7,
  Condiments = 8,
  Supplements = 9,
  Other = 10
}
export enum RecipeCategory {
  Breakfast = 0,
  Lunch = 1,
  Dinner = 2,
  Snack = 3,
  Dessert = 4,
  Appetizer = 5,
  Soup = 6,
  Salad = 7,
  MainCourse = 8,
  SideDish = 9,
  Beverage = 10,
  Smoothie = 11,
  Bread = 12,
  Pasta = 13,
  Rice = 14,
  Meat = 15,
  Fish = 16,
  Vegetarian = 17,
  Vegan = 18,
  GlutenFree = 19,
  LowCarb = 20,
  HighProtein = 21,
  QuickMeal = 22,
  SlowCooker = 23,
  Other = 24
}
export enum Gender {
  Male = 0,
  Female = 1,
  Other = 2,
}
export enum ActivityLevel {
  Sedentary = 0,
  LightlyActive = 1,
  ModeratelyActive = 2,
  VeryActive = 3,
  ExtremelyActive = 4,
}
export enum FitnessGoal {
  LoseWeight = 0,
  MaintainWeight = 1,
  GainWeight = 2,
}
export enum NotificationType {
  WaterReminder = 'WaterReminder',
  MealReminder = 'MealReminder',
  CalorieLimitWarning = 'CalorieLimitWarning',
  GoalAchievement = 'GoalAchievement',
  WeeklyProgress = 'WeeklyProgress',
  DailySummary = 'DailySummary'
}
export enum NotificationPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High',
  Urgent = 'Urgent'
}
export enum NotificationStatus {
  Unread = 'Unread',
  Read = 'Read'
}
export enum RelationshipType {
  Creator = 'Creator',
  Contributor = 'Contributor',
  Favorite = 'Favorite',
  Viewer = 'Viewer'
}
export interface Theme {
  mode: 'light' | 'dark' | 'system';
}
export interface AppState {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  theme: Theme;
  notifications: Notification[];
}
export interface LoginFormData {
  email: string;
  password: string;
}
export interface RegisterFormData {
  email: string;
  password: string;
  confirmPassword: string;
  name: string;
}
export interface ProfileFormData {
  name: string;
  heightCm: number;
  weightKg: number;
  age: number;
  gender: Gender;
  activityLevel: ActivityLevel;
  fitnessGoal: FitnessGoal;
  targetWeightKg: number;
  dailyCalorieGoal: number;
  dailyProteinGoal: number;
  dailyFatGoal: number;
  dailyCarbohydrateGoal: number;
}
export interface ServiceType {
  [key: string]: unknown;
  getImage: (id: string) => Promise<string>;
}