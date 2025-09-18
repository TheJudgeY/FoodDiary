import React, { useState } from 'react';
import { X, Clock, Users, ChefHat, Calendar } from 'lucide-react';
import { Recipe, MealType, AddRecipeToDiaryRequest } from '../../types';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import { useAddRecipeToDiary } from '../../hooks/useAddRecipeToDiary';
import { toastService } from '../../services/toastService';

interface AddRecipeToDiaryModalProps {
  isOpen: boolean;
  onClose: () => void;
  recipe: Recipe;
  onSuccess?: () => void;
}

const AddRecipeToDiaryModal: React.FC<AddRecipeToDiaryModalProps> = ({
  isOpen,
  onClose,
  recipe,
  onSuccess
}) => {
  const { addRecipeToDiary, isLoading, error } = useAddRecipeToDiary();
  const [formData, setFormData] = useState({
    mealType: MealType.Breakfast,
    servingsConsumed: 1,
    notes: '',
    consumedAt: new Date().toISOString()
  });

  if (!isOpen || !recipe) return null;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    try {
      const request: AddRecipeToDiaryRequest = {
        recipeId: recipe.id,
        ...formData
      };
      
      const result = await addRecipeToDiary(request);
      
      
      const recipeName = result?.recipeName || recipe.name || 'Recipe';
      
      toastService.success(`Successfully added ${recipeName} to diary!`);
      onSuccess?.();
      onClose();
    } catch (err) {
      console.error('Failed to add recipe to diary:', err);
      
    }
  };

  const handleInputChange = (field: string, value: unknown) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  const getMealTypeName = (mealType: MealType): string => {
    switch (mealType) {
      case MealType.Breakfast: return 'Breakfast';
      case MealType.Lunch: return 'Lunch';
      case MealType.Dinner: return 'Dinner';
      case MealType.Snack: return 'Snack';
      default: return 'Unknown';
    }
  };

  const validateForm = (): string[] => {
    const errors: string[] = [];
    
    if (formData.servingsConsumed < 1 || formData.servingsConsumed > 20) {
      errors.push('Servings must be between 1 and 20');
    }
    
    const consumedDate = new Date(formData.consumedAt);
    if (consumedDate > new Date()) {
      errors.push('Cannot add food entries for future dates');
    }
    
    if (formData.notes && formData.notes.length > 500) {
      errors.push('Notes must be 500 characters or less');
    }
    
    return errors;
  };

  const formErrors = validateForm();
  const isFormValid = formErrors.length === 0;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <Card className="max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          {/* Header */}
          <div className="flex items-start justify-between mb-6">
            <div className="flex-1">
              <h2 className="text-2xl font-bold text-surface-900 mb-2">
                Add "{recipe.name}" to Diary
              </h2>
              <p className="text-surface-600">
                This will create food entries for all recipe ingredients
              </p>
            </div>
            <button
              onClick={onClose}
              className="ml-4 p-2 hover:bg-surface-100 rounded-full transition-colors"
              disabled={isLoading}
            >
              <X className="h-5 w-5 text-surface-500" />
            </button>
          </div>

          {/* Recipe Summary */}
          <div className="mb-6 p-4 bg-surface-50 rounded-lg">
            <h3 className="font-medium text-surface-900 mb-3">Recipe Summary</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-4">
              <div className="text-center">
                <Users className="h-5 w-5 text-primary-600 mx-auto mb-1" />
                <p className="text-sm text-surface-600">Recipe Serves</p>
                <p className="font-semibold text-surface-900">{recipe.servings}</p>
              </div>
              <div className="text-center">
                <Clock className="h-5 w-5 text-primary-600 mx-auto mb-1" />
                <p className="text-sm text-surface-600">Prep Time</p>
                <p className="font-semibold text-surface-900">{recipe.preparationTimeMinutes} min</p>
              </div>
              <div className="text-center">
                <ChefHat className="h-5 w-5 text-primary-600 mx-auto mb-1" />
                <p className="text-sm text-surface-600">Cook Time</p>
                <p className="font-semibold text-surface-900">{recipe.cookingTimeMinutes} min</p>
              </div>
              <div className="text-center">
                <Calendar className="h-5 w-5 text-primary-600 mx-auto mb-1" />
                <p className="text-sm text-surface-600">Ingredients</p>
                <p className="font-semibold text-surface-900">{recipe.ingredientCount}</p>
              </div>
            </div>
            
            {/* Ingredients Preview */}
            <div>
              <p className="text-sm text-surface-600 mb-2">Ingredients that will be added:</p>
              <div className="space-y-1 max-h-32 overflow-y-auto">
                {recipe.ingredients?.map((ingredient, index) => {
                  const ingredientName = ingredient.product?.name || 
                                       ingredient.customIngredientName ||
                                       ingredient.productName ||
                                       ingredient.notes ||
                                       `Ingredient ${index + 1}`;
                  const quantity = ingredient.quantityGrams || 0;
                  const scaledQuantity = quantity * (formData.servingsConsumed / recipe.servings);
                  
                  return (
                    <div key={ingredient.id || index} className="flex justify-between text-sm">
                      <span className="text-surface-700">{ingredientName}</span>
                      <span className="text-surface-600">
                        {scaledQuantity.toFixed(1)}g
                      </span>
                    </div>
                  );
                })}
              </div>
            </div>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit} className="space-y-4">
            {/* Meal Type */}
            <div>
              <label htmlFor="mealType" className="block text-sm font-medium text-surface-700 mb-2">
                Meal Type
              </label>
              <select
                id="mealType"
                value={formData.mealType}
                onChange={(e) => handleInputChange('mealType', Number(e.target.value) as MealType)}
                className="w-full border border-surface-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                required
              >
                <option value={MealType.Breakfast}>Breakfast</option>
                <option value={MealType.Lunch}>Lunch</option>
                <option value={MealType.Dinner}>Dinner</option>
                <option value={MealType.Snack}>Snack</option>
              </select>
            </div>

            {/* Servings Consumed */}
            <div>
              <label htmlFor="servingsConsumed" className="block text-sm font-medium text-surface-700 mb-2">
                Servings Consumed
              </label>
              <div className="flex items-center gap-2">
                <input
                  id="servingsConsumed"
                  type="number"
                  min="1"
                  max="20"
                  value={formData.servingsConsumed}
                  onChange={(e) => handleInputChange('servingsConsumed', Number(e.target.value))}
                  className="flex-1 border border-surface-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                  required
                />
                <span className="text-sm text-surface-600">
                  of {recipe.servings} recipe servings
                </span>
              </div>
              <p className="text-xs text-surface-500 mt-1">
                Quantities will be automatically scaled based on servings consumed
              </p>
            </div>

            {/* Consumed At */}
            <div>
              <label htmlFor="consumedAt" className="block text-sm font-medium text-surface-700 mb-2">
                When did you eat this?
              </label>
              <input
                id="consumedAt"
                type="datetime-local"
                value={formData.consumedAt.slice(0, 16)}
                onChange={(e) => handleInputChange('consumedAt', new Date(e.target.value).toISOString())}
                className="w-full border border-surface-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
                required
              />
            </div>

            {/* Notes */}
            <div>
              <label htmlFor="notes" className="block text-sm font-medium text-surface-700 mb-2">
                Notes (optional)
              </label>
              <textarea
                id="notes"
                value={formData.notes}
                onChange={(e) => handleInputChange('notes', e.target.value)}
                maxLength={500}
                rows={3}
                placeholder="Any additional notes about this meal..."
                className="w-full border border-surface-300 rounded-lg px-3 py-2 focus:ring-2 focus:ring-primary-500 focus:border-primary-500"
              />
              <p className="text-xs text-surface-500 mt-1">
                {formData.notes.length}/500 characters
              </p>
            </div>

            {/* Error Display */}
            {error && (
              <div className="p-3 bg-red-50 border border-red-200 rounded-lg">
                <p className="text-red-700 text-sm">{error}</p>
              </div>
            )}

            {/* Form Validation Errors */}
            {formErrors.length > 0 && (
              <div className="p-3 bg-yellow-50 border border-yellow-200 rounded-lg">
                <ul className="text-yellow-700 text-sm space-y-1">
                  {formErrors.map((error, index) => (
                    <li key={index}>• {error}</li>
                  ))}
                </ul>
              </div>
            )}

            {/* Actions */}
            <div className="flex gap-3 pt-4 border-t border-surface-200">
              <Button
                type="button"
                variant="outline"
                onClick={onClose}
                disabled={isLoading}
                className="flex-1"
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isLoading || !isFormValid}
                className="flex-1"
              >
                {isLoading ? 'Adding...' : `Add to ${getMealTypeName(formData.mealType)}`}
              </Button>
            </div>
          </form>
        </div>
      </Card>
    </div>
  );
};

export default AddRecipeToDiaryModal;
