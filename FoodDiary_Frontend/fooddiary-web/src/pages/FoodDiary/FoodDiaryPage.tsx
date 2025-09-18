import React, { useState, useEffect } from 'react';
import { format, addDays, subDays, isToday } from 'date-fns';
import { 
  ChevronLeft, 
  ChevronRight, 
  Plus, 
  Edit, 
  Trash2,
  Calendar,
  Target
} from 'lucide-react';
import { NUTRITION_ICONS } from '../../constants/icons';
import { useFoodEntryStore } from '../../stores/foodEntryStore';
import { useProductStore } from '../../stores/productStore';
import { useAuthStore } from '../../stores/authStore';
import { FoodEntry } from '../../types';
import { getMealTypeLabel, getMealTypeIcon, formatNumber, calculateNutritionalTargets, calculateProgressPercentage, needsProfileCompletion, getProfileCompletionMessage } from '../../utils';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import LoadingSpinner from '../../components/UI/LoadingSpinner';
import AddFoodEntryModal from './AddFoodEntryModal';
import EditFoodEntryModal from './EditFoodEntryModal';

const FoodDiaryPage: React.FC = () => {
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [selectedEntry, setSelectedEntry] = useState<FoodEntry | null>(null);

  const { 
    entries, 
    isLoading, 
    error, 
    fetchEntries, 
    deleteEntry, 
    clearError 
  } = useFoodEntryStore();
  
  const { products, fetchProducts } = useProductStore();
  const { user } = useAuthStore();

  useEffect(() => {
    fetchEntries(format(selectedDate, 'yyyy-MM-dd'));
  }, [selectedDate, fetchEntries]);

  useEffect(() => {
    fetchProducts(1, true); 
  }, [fetchProducts]);

  useEffect(() => {
    if (isLoading) {
      clearError();
    }
  }, [isLoading, clearError]);

  const handleDateChange = (direction: 'prev' | 'next') => {
    setSelectedDate(prev => 
      direction === 'prev' ? subDays(prev, 1) : addDays(prev, 1)
    );
  };

  const handleAddEntry = () => {
    setShowAddModal(true);
  };

  const handleEditEntry = (entry: FoodEntry) => {
    setSelectedEntry(entry);
    setShowEditModal(true);
  };

  const handleDeleteEntry = async (entry: FoodEntry) => {
    if (window.confirm('Are you sure you want to delete this food entry?')) {
      try {
        await deleteEntry(entry.id);
      } catch (error) {
        console.error('Failed to delete food entry:', error);
      }
    }
  };

  const closeModals = () => {
    setShowAddModal(false);
    setShowEditModal(false);
    setSelectedEntry(null);
  };

  
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

  
  const mealTypes = [
    { type: 0, label: getMealTypeLabel(0), icon: getMealTypeIcon(0) },
    { type: 1, label: getMealTypeLabel(1), icon: getMealTypeIcon(1) },
    { type: 2, label: getMealTypeLabel(2), icon: getMealTypeIcon(2) },
    { type: 3, label: getMealTypeLabel(3), icon: getMealTypeIcon(3) },
  ];

  const getEntriesByMeal = (mealType: number) => {
    return entries.filter(entry => {
      
      const entryMealType = typeof entry.mealType === 'number' 
        ? entry.mealType 
        : entry.mealType;
      return entryMealType === mealType;
    });
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center min-h-64">
        <LoadingSpinner />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">Food Diary</h1>
          <p className="text-surface-600">Track your daily food consumption and nutritional intake.</p>
        </div>
        <Button onClick={() => handleAddEntry()} className="flex items-center gap-2 w-full sm:w-auto justify-center">
          <Plus className="h-4 w-4" />
          Add Entry
        </Button>
      </div>

      {/* Date Navigation */}
      <Card>
        <div className="flex flex-col sm:flex-row items-center justify-between p-4 gap-4">
          <Button
            variant="outline"
            onClick={() => handleDateChange('prev')}
            className="flex items-center gap-2 w-full sm:w-auto justify-center"
          >
            <ChevronLeft className="h-4 w-4" />
            Previous
          </Button>
          
          <div className="flex items-center gap-3 order-first sm:order-none">
            <Calendar className="h-5 w-5 text-surface-500" />
            <span className="text-lg font-semibold text-surface-900 text-center">
              {format(selectedDate, 'EEEE, MMMM d, yyyy')}
            </span>
            {isToday(selectedDate) && (
              <span className="px-2 py-1 text-xs font-medium bg-primary-100 text-primary-700 rounded-full">
                Today
              </span>
            )}
          </div>
          
          <Button
            variant="outline"
            onClick={() => handleDateChange('next')}
            className="flex items-center gap-2 w-full sm:w-auto justify-center"
          >
            Next
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </Card>

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

      {/* Nutrition Summary - Mobile Responsive */}
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-6">
        {/* Calories Card */}
        <Card className="p-3 sm:p-6">
          <div className="flex items-center justify-between mb-2 sm:mb-4">
            <h3 className="text-sm sm:text-lg font-semibold text-surface-900">Calories</h3>
            <div className={`w-8 h-8 sm:w-10 sm:h-10 ${NUTRITION_ICONS.calories.bgColor} rounded-lg flex items-center justify-center`}>
              <NUTRITION_ICONS.calories.icon className={`h-4 w-4 sm:h-5 sm:w-5 ${NUTRITION_ICONS.calories.color}`} />
            </div>
          </div>
          <div className="space-y-1 sm:space-y-2">
            <p className="text-xl sm:text-3xl font-bold text-surface-900">{formatNumber(totalNutrition.calories)}</p>
            <p className="text-xs sm:text-sm text-surface-600 hidden sm:block">Target: {formatNumber(nutritionalTargets.calories)}</p>
            <div className="w-full bg-surface-200 rounded-full h-2">
              <div 
                className={`${NUTRITION_ICONS.calories.progressColor} h-2 rounded-full transition-all duration-300`}
                style={{ width: `${calculateProgressPercentage(totalNutrition.calories, nutritionalTargets.calories)}%` }}
              ></div>
            </div>
            <p className="text-xs text-surface-500 text-right">
              {formatNumber(totalNutrition.calories)}/{formatNumber(nutritionalTargets.calories)} kcal
            </p>
          </div>
        </Card>

        {/* Protein Card */}
        <Card className="p-3 sm:p-6">
          <div className="flex items-center justify-between mb-2 sm:mb-4">
            <h3 className="text-sm sm:text-lg font-semibold text-surface-900">Protein</h3>
            <div className={`w-8 h-8 sm:w-10 sm:h-10 ${NUTRITION_ICONS.protein.bgColor} rounded-lg flex items-center justify-center`}>
              <NUTRITION_ICONS.protein.icon className={`h-4 w-4 sm:h-5 sm:w-5 ${NUTRITION_ICONS.protein.color}`} />
            </div>
          </div>
          <div className="space-y-1 sm:space-y-2">
            <p className="text-xl sm:text-3xl font-bold text-surface-900">{formatNumber(totalNutrition.proteins, 1)}g</p>
            <p className="text-xs sm:text-sm text-surface-600 hidden sm:block">Target: {formatNumber(nutritionalTargets.proteins, 1)}g</p>
            <div className="w-full bg-surface-200 rounded-full h-2">
              <div 
                className={`${NUTRITION_ICONS.protein.progressColor} h-2 rounded-full transition-all duration-300`}
                style={{ width: `${calculateProgressPercentage(totalNutrition.proteins, nutritionalTargets.proteins)}%` }}
              ></div>
            </div>
            <p className="text-xs text-surface-500 text-right">
              {formatNumber(totalNutrition.proteins, 1)}/{formatNumber(nutritionalTargets.proteins, 1)}g protein
            </p>
          </div>
        </Card>

        {/* Fat Card */}
        <Card className="p-3 sm:p-6">
          <div className="flex items-center justify-between mb-2 sm:mb-4">
            <h3 className="text-sm sm:text-lg font-semibold text-surface-900">Fat</h3>
            <div className={`w-8 h-8 sm:w-10 sm:h-10 ${NUTRITION_ICONS.fat.bgColor} rounded-lg flex items-center justify-center`}>
              <NUTRITION_ICONS.fat.icon className={`h-4 w-4 sm:h-5 sm:w-5 ${NUTRITION_ICONS.fat.color}`} />
            </div>
          </div>
          <div className="space-y-1 sm:space-y-2">
            <p className="text-xl sm:text-3xl font-bold text-surface-900">{formatNumber(totalNutrition.fats, 1)}g</p>
            <p className="text-xs sm:text-sm text-surface-600 hidden sm:block">Target: {formatNumber(nutritionalTargets.fats, 1)}g</p>
            <div className="w-full bg-surface-200 rounded-full h-2">
              <div 
                className={`${NUTRITION_ICONS.fat.progressColor} h-2 rounded-full transition-all duration-300`}
                style={{ width: `${calculateProgressPercentage(totalNutrition.fats, nutritionalTargets.fats)}%` }}
              ></div>
            </div>
            <p className="text-xs text-surface-500 text-right">
              {formatNumber(totalNutrition.fats, 1)}/{formatNumber(nutritionalTargets.fats, 1)}g fat
            </p>
          </div>
        </Card>

        {/* Carbs Card */}
        <Card className="p-3 sm:p-6">
          <div className="flex items-center justify-between mb-2 sm:mb-4">
            <h3 className="text-sm sm:text-lg font-semibold text-surface-900">Carbs</h3>
            <div className={`w-8 h-8 sm:w-10 sm:h-10 ${NUTRITION_ICONS.carbs.bgColor} rounded-lg flex items-center justify-center`}>
              <NUTRITION_ICONS.carbs.icon className={`h-4 w-4 sm:h-5 sm:w-5 ${NUTRITION_ICONS.carbs.color}`} />
            </div>
          </div>
          <div className="space-y-1 sm:space-y-2">
            <p className="text-xl sm:text-3xl font-bold text-surface-900">{formatNumber(totalNutrition.carbohydrates, 1)}g</p>
            <p className="text-xs sm:text-sm text-surface-600 hidden sm:block">Target: {formatNumber(nutritionalTargets.carbohydrates, 1)}g</p>
            <div className="w-full bg-surface-200 rounded-full h-2">
              <div 
                className={`${NUTRITION_ICONS.carbs.progressColor} h-2 rounded-full transition-all duration-300`}
                style={{ width: `${calculateProgressPercentage(totalNutrition.carbohydrates, nutritionalTargets.carbohydrates)}%` }}
              ></div>
            </div>
            <p className="text-xs text-surface-500 text-right">
              {formatNumber(totalNutrition.carbohydrates, 1)}/{formatNumber(nutritionalTargets.carbohydrates, 1)}g carbs
            </p>
          </div>
        </Card>
      </div>

      {/* Error Display */}
      {error && (
        <Card className="border-error-200 bg-error-50">
          <div className="p-4 text-error-700">
            <p className="font-medium">Error loading food entries</p>
            <p className="text-sm">{error}</p>
          </div>
        </Card>
      )}

      {/* Meal Sections */}
      <div className="space-y-6">
        {mealTypes.map(({ type, label, icon }) => {
          const mealEntries = getEntriesByMeal(type);
          const mealNutrition = mealEntries.reduce(
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

          return (
            <Card key={type}>
              <div className="p-4 sm:p-6">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between mb-4 gap-3">
                  <div className="flex items-center gap-3">
                    <span className="text-2xl">{icon}</span>
                    <div>
                      <h3 className="text-lg font-semibold text-surface-900">{label}</h3>
                      <p className="text-sm text-surface-600">
                        {mealEntries.length} {mealEntries.length === 1 ? 'entry' : 'entries'} • {formatNumber(mealNutrition.calories)} kcal
                      </p>
                    </div>
                  </div>
                  
                  <Button
                    variant="outline"
                    size="sm"
                    onClick={() => handleAddEntry()}
                    className="flex items-center gap-2 w-full sm:w-auto justify-center"
                  >
                    <Plus className="h-4 w-4" />
                    Add to {label}
                  </Button>
                </div>

                {mealEntries.length === 0 ? (
                  <div className="text-center py-8 text-surface-500">
                    <p>No entries for {label.toLowerCase()}</p>
                    <p className="text-sm">Click "Add to {label}" to get started</p>
                  </div>
                ) : (
                  <div className="space-y-3">
                    {mealEntries.map((entry) => {
                      const product = products.find(p => p.id === entry.productId);
                      const nutrition = getEntryNutrition(entry);
                      
                      return (
                        <div
                          key={entry.id}
                          className="flex flex-col sm:flex-row sm:items-center sm:justify-between p-3 bg-surface-50 rounded-lg border gap-3"
                        >
                          <div className="flex-1">
                            <div className="flex items-center justify-between mb-1">
                              <h4 className="font-medium text-surface-900">
                                {product?.name || 'Unknown Product'}
                              </h4>
                              <p className="text-sm text-surface-600">
                                {formatNumber(entry.quantity)}g • {formatNumber(nutrition.calories)} kcal
                              </p>
                            </div>
                            <div className="text-sm text-surface-600">
                              {formatNumber(nutrition.proteins)}g protein • {formatNumber(nutrition.fats)}g fat • {formatNumber(nutrition.carbohydrates)}g carbs
                            </div>
                          </div>
                          
                          <div className="flex items-center gap-2 sm:ml-4 justify-end">
                            <button
                              onClick={() => handleEditEntry(entry)}
                              className="p-1 text-surface-400 hover:text-primary-600 rounded"
                            >
                              <Edit className="h-4 w-4" />
                            </button>
                            <button
                              onClick={() => handleDeleteEntry(entry)}
                              className="p-1 text-surface-400 hover:text-error-600 rounded"
                            >
                              <Trash2 className="h-4 w-4" />
                            </button>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                )}
              </div>
            </Card>
          );
        })}
      </div>

      {/* Modals */}
      <AddFoodEntryModal
        isOpen={showAddModal}
        onClose={closeModals}
        selectedDate={format(selectedDate, 'yyyy-MM-dd')}
        defaultMealType={0} 
      />
      
      <EditFoodEntryModal
        isOpen={showEditModal}
        onClose={closeModals}
        entry={selectedEntry}
      />
    </div>
  );
};

export default FoodDiaryPage;
