import React from 'react';
import { Plus, Clock } from 'lucide-react';
import { Recipe, MealType, AddRecipeToDiaryRequest } from '../../types';
import { useAddRecipeToDiary } from '../../hooks/useAddRecipeToDiary';
import { toastService } from '../../services/toastService';
import Button from '../UI/Button';

interface QuickAddRecipeProps {
  recipe: Recipe;
  mealType?: MealType;
  servings?: number;
  variant?: 'button' | 'icon';
  className?: string;
  onSuccess?: () => void;
}

const QuickAddRecipe: React.FC<QuickAddRecipeProps> = ({
  recipe,
  mealType = MealType.Lunch,
  servings = 1,
  variant = 'button',
  className = '',
  onSuccess
}) => {
  const { addRecipeToDiary, isLoading } = useAddRecipeToDiary();

  const handleQuickAdd = async () => {
    try {
      const request: AddRecipeToDiaryRequest = {
        recipeId: recipe.id,
        mealType,
        servingsConsumed: servings,
        consumedAt: new Date().toISOString(),
        notes: ''
      };
      
      const result = await addRecipeToDiary(request);
      
      
      const recipeName = result?.recipeName || recipe.name || 'Recipe';
      toastService.success(`Added ${recipeName} to diary!`);
      onSuccess?.();
    } catch (err) {
      console.error('Failed to add recipe to diary:', err);
      
    }
  };

  const getMealTypeName = (mealType: MealType): string => {
    switch (mealType) {
      case MealType.Breakfast: return 'Breakfast';
      case MealType.Lunch: return 'Lunch';
      case MealType.Dinner: return 'Dinner';
      case MealType.Snack: return 'Snack';
      default: return 'Diary';
    }
  };

  if (variant === 'icon') {
    return (
      <button
        onClick={handleQuickAdd}
        disabled={isLoading}
        className={`p-2 hover:bg-primary-50 rounded-full transition-colors ${className}`}
        title={`Quick add to ${getMealTypeName(mealType)}`}
      >
        {isLoading ? (
          <Clock className="h-4 w-4 text-primary-600 animate-spin" />
        ) : (
          <Plus className="h-4 w-4 text-primary-600" />
        )}
      </button>
    );
  }

  return (
    <Button
      onClick={handleQuickAdd}
      disabled={isLoading}
      size="sm"
      className={className}
    >
      {isLoading ? (
        <>
          <Clock className="h-4 w-4 animate-spin" />
          Adding...
        </>
      ) : (
        <>
          <Plus className="h-4 w-4" />
          Add to {getMealTypeName(mealType)}
        </>
      )}
    </Button>
  );
};

export default QuickAddRecipe;
