import React, { useEffect, useState } from 'react';
import { 
  Calendar, 
  Target
} from 'lucide-react';
import { NUTRITION_ICONS } from '../../constants/icons';
import { useAuthStore } from '../../stores/authStore';
import { useFoodEntryStore } from '../../stores/foodEntryStore';
import { useProductStore } from '../../stores/productStore';
import { analyticsService } from '../../services/analyticsService';
import type { DailyAnalysis, FoodEntry } from '../../types';
import { formatDate, formatCalories, formatNumber, calculateNutritionalTargets, calculateProgressPercentage, needsProfileCompletion, getProfileCompletionMessage } from '../../utils';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import LoadingSpinner from '../../components/UI/LoadingSpinner';

const DashboardPage: React.FC = () => {
  const { user } = useAuthStore();
  const { selectedDate, fetchEntries } = useFoodEntryStore();
  const { products, fetchProducts } = useProductStore();
  const [dailyAnalysis, setDailyAnalysis] = useState<DailyAnalysis | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const loadDashboardData = async () => {
      setIsLoading(true);
      try {
        await fetchEntries();
        await fetchProducts(1, true); 
        const analysis = await analyticsService.getDailyAnalysis(selectedDate);

        setDailyAnalysis(analysis);
      } catch (error) {
        console.error('Failed to load dashboard data:', error);
        
        setDailyAnalysis({
          date: selectedDate,
          totalCalories: 0,
          totalProtein: 0,
          totalFat: 0,
          totalCarbohydrates: 0,
          dailyCalorieGoal: 0,
          dailyProteinGoal: 0,
          dailyFatGoal: 0,
          dailyCarbohydrateGoal: 0,
          isCalorieGoalMet: false,
          isProteinGoalMet: false,
          isFatGoalMet: false,
          isCarbohydrateGoalMet: false,
          isOverCalorieLimit: false,
          isOverProteinLimit: false,
          isOverFatLimit: false,
          isOverCarbohydrateLimit: false,
          overallStatus: 'No Data',
          recommendations: [],
        });
      } finally {
        setIsLoading(false);
      }
    };

    loadDashboardData();
  }, [selectedDate, fetchEntries, fetchProducts]);

  
  const getEntryNutrition = (entry: FoodEntry) => {
    // If nutrition is already calculated by the API, use those values
    if (entry.calories !== undefined && entry.protein !== undefined && 
        entry.fat !== undefined && entry.carbohydrates !== undefined) {
      return {
        calories: entry.calories || 0,
        proteins: entry.protein || 0, // API uses 'protein', frontend expects 'proteins'
        fats: entry.fat || 0, // API uses 'fat', frontend expects 'fats'
        carbohydrates: entry.carbohydrates || 0,
      };
    }
    
    // Fallback: calculate from product data if API doesn't provide calculated values
    const product = products.find(p => p.id === entry.productId);
    if (!product) { 
      return { calories: 0, proteins: 0, fats: 0, carbohydrates: 0 }; 
    }
    
    const weight = entry.quantity;
    if (!weight) { 
      return { calories: 0, proteins: 0, fats: 0, carbohydrates: 0 }; 
    }
    
    const ratio = weight / 100;
    return {
      calories: Math.round(product.caloriesPer100g * ratio),
      proteins: Math.round(product.proteinsPer100g * ratio * 10) / 10,
      fats: Math.round(product.fatsPer100g * ratio * 10) / 10,
      carbohydrates: Math.round(product.carbohydratesPer100g * ratio * 10) / 10,
    };
  };

  
  const getTotalNutrition = () => {
    const { entries } = useFoodEntryStore.getState();
    return entries.reduce(
      (total, entry) => {
        const nutrition = getEntryNutrition(entry);
        return {
          calories: total.calories + nutrition.calories,
          proteins: total.proteins + nutrition.proteins,
          fats: total.fats + nutrition.fats,
          carbohydrates: total.carbohydrates + nutrition.carbohydrates,
        };
      },
      { calories: 0, proteins: 0, fats: 0, carbohydrates: 0 }
    );
  };

  const totalNutrition = getTotalNutrition();

  const nutritionalTargets = calculateNutritionalTargets(user);

  const nutritionCards = [
    {
      title: 'Calories',
      value: formatCalories(totalNutrition.calories),
      target: formatCalories(nutritionalTargets.calories),
      progress: calculateProgressPercentage(totalNutrition.calories, nutritionalTargets.calories),
      iconConfig: NUTRITION_ICONS.calories,
      currentValue: totalNutrition.calories,
      targetValue: nutritionalTargets.calories,
      unit: 'kcal',
    },
    {
      title: 'Protein',
      value: `${formatNumber(totalNutrition.proteins, 1)}g`,
      target: `${nutritionalTargets.proteins}g`,
      progress: calculateProgressPercentage(totalNutrition.proteins, nutritionalTargets.proteins),
      iconConfig: NUTRITION_ICONS.protein,
      currentValue: totalNutrition.proteins,
      targetValue: nutritionalTargets.proteins,
      unit: 'g protein',
    },
    {
      title: 'Fat',
      value: `${formatNumber(totalNutrition.fats, 1)}g`,
      target: `${nutritionalTargets.fats}g`,
      progress: calculateProgressPercentage(totalNutrition.fats, nutritionalTargets.fats),
      iconConfig: NUTRITION_ICONS.fat,
      currentValue: totalNutrition.fats,
      targetValue: nutritionalTargets.fats,
      unit: 'g fat',
    },
    {
      title: 'Carbs',
      value: `${formatNumber(totalNutrition.carbohydrates, 1)}g`,
      target: `${nutritionalTargets.carbohydrates}g`,
      progress: calculateProgressPercentage(totalNutrition.carbohydrates, nutritionalTargets.carbohydrates),
      iconConfig: NUTRITION_ICONS.carbs,
      currentValue: totalNutrition.carbohydrates,
      targetValue: nutritionalTargets.carbohydrates,
      unit: 'g carbs',
    },
  ];

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  
  if (!dailyAnalysis) {
    return (
      <div className="space-y-6">
        <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
          <div>
            <h1 className="text-2xl font-bold text-surface-900">Dashboard</h1>
            <p className="text-surface-600">
              Welcome back, {user?.name}! Here's your nutrition overview for {formatDate(selectedDate)}.
            </p>
          </div>
        </div>
        
        <Card className="p-6">
          <div className="text-center py-12">
            <h3 className="text-lg font-semibold text-surface-900 mb-2">Loading Analytics Data</h3>
            <p className="text-surface-600">Please wait while we load your daily analysis...</p>
          </div>
        </Card>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">Dashboard</h1>
          <p className="text-surface-600">
            Welcome back, {user?.name}! Here's your nutrition overview for {formatDate(selectedDate)}.
          </p>
        </div>
        <div className="mt-4 sm:mt-0 flex space-x-3">
          <Button variant="outline" leftIcon={<Calendar className="h-4 w-4" />}>
            {formatDate(selectedDate, 'MMM dd, yyyy')}
          </Button>
        </div>
      </div>

      {/* Profile Completion Warning */}
      {needsProfileCompletion(user) && (
        <Card className="p-4 bg-amber-50 border-amber-200">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 bg-amber-100 rounded-lg flex items-center justify-center">
              <Target className="h-4 w-4 text-amber-600" />
            </div>
            <div>
              <p className="text-sm font-medium text-amber-800">Complete Your Profile</p>
              <p className="text-xs text-amber-700">{getProfileCompletionMessage()}</p>
            </div>
          </div>
        </Card>
      )}

      {/* Nutrition Overview */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
        {nutritionCards.map((card) => (
          <Card key={card.title} className="relative overflow-hidden p-8">
            <div className="flex items-center justify-between mb-6">
              <div>
                <p className="text-lg font-medium text-surface-600 mb-2">{card.title}</p>
                <p className="text-4xl font-bold text-surface-900 mb-2">{card.value}</p>
                <p className="text-base text-surface-500">Target: {card.target}</p>
              </div>
              <div className={`p-4 rounded-xl ${card.iconConfig.bgColor}`}>
                <card.iconConfig.icon className={`h-8 w-8 ${card.iconConfig.color}`} />
              </div>
            </div>
            
            {/* Progress bar */}
            <div className="mt-6">
              <div className="flex justify-between text-sm text-surface-500 mb-2">
                <span>Progress</span>
                <span>{Math.round(card.progress)}%</span>
              </div>
              <div className="w-full bg-surface-200 rounded-full h-3">
                <div
                  className={`h-3 rounded-full ${card.iconConfig.progressColor} transition-all duration-300`}
                  style={{ width: `${Math.min(card.progress, 100)}%` }}
                />
              </div>
              <p className="text-sm text-surface-500 text-right mt-2">
                {formatNumber(card.currentValue)}/{formatNumber(card.targetValue)} {card.unit}
              </p>
            </div>
          </Card>
        ))}
      </div>


    </div>
  );
};

export default DashboardPage;
