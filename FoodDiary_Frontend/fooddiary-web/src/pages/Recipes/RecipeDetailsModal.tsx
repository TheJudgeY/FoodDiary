import React, { useState } from 'react';
import { X, Clock, Users, ChefHat, Heart, Plus, Flame } from 'lucide-react';
import { Recipe } from '../../types';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import { canToggleFavorite, getFavoriteToggleMessage } from '../../utils/recipePermissions';
import AddRecipeToDiaryModal from './AddRecipeToDiaryModal';

interface RecipeDetailsModalProps {
  isOpen: boolean;
  onClose: () => void;
  recipe: Recipe | null;
  onToggleFavorite?: () => Promise<void>;
  onAddToDiary?: () => void;
}

const RecipeDetailsModal: React.FC<RecipeDetailsModalProps> = ({
  isOpen,
  onClose,
  recipe,
  onToggleFavorite,
  onAddToDiary
}) => {
  const [showAddToDiaryModal, setShowAddToDiaryModal] = useState(false);

  if (!isOpen || !recipe) return null;

  const canToggle = canToggleFavorite(recipe);

  const handleAddToDiary = () => {
    setShowAddToDiaryModal(true);
  };

  const handleAddToDiarySuccess = () => {
    onAddToDiary?.();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <Card className="max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          {/* Header */}
          <div className="flex items-start justify-between mb-6">
            <div className="flex-1">
              <h2 className="text-2xl font-bold text-surface-900 mb-2">{recipe.name}</h2>
              {recipe.description && (
                <p className="text-surface-600">{recipe.description}</p>
              )}
            </div>
            <button
              onClick={onClose}
              className="ml-4 p-2 hover:bg-surface-100 rounded-full transition-colors"
            >
              <X className="h-5 w-5 text-surface-500" />
            </button>
          </div>

          {/* Recipe Stats */}
          <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-6">
            <div className="text-center p-3 bg-surface-50 rounded-lg">
              <Clock className="h-6 w-6 text-primary-600 mx-auto mb-2" />
              <p className="text-sm text-surface-600">Prep Time</p>
              <p className="font-semibold text-surface-900">{recipe.preparationTimeMinutes} min</p>
            </div>
            <div className="text-center p-3 bg-surface-50 rounded-lg">
              <ChefHat className="h-6 w-6 text-primary-600 mx-auto mb-2" />
              <p className="text-sm text-surface-600">Cook Time</p>
              <p className="font-semibold text-surface-900">{recipe.cookingTimeMinutes} min</p>
            </div>
            <div className="text-center p-3 bg-surface-50 rounded-lg">
              <Users className="h-6 w-6 text-primary-600 mx-auto mb-2" />
              <p className="text-sm text-surface-600">Servings</p>
              <p className="font-semibold text-surface-900">{recipe.servings}</p>
            </div>
            <div className="text-center p-3 bg-surface-50 rounded-lg">
              <Flame className="h-6 w-6 text-primary-600 mx-auto mb-2" />
              <p className="text-sm text-surface-600">Calories</p>
              <p className="font-semibold text-surface-900">{recipe.totalCalories}</p>
            </div>
          </div>

          {/* Nutrition Info */}
          <div className="mb-6">
            <h3 className="font-medium text-surface-900 mb-3">Nutrition per Serving</h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div className="text-center p-3 bg-surface-50 rounded-lg">
                <p className="text-sm text-surface-600">Protein</p>
                <p className="font-semibold text-surface-900">{recipe.totalProtein}g</p>
              </div>
              <div className="text-center p-3 bg-surface-50 rounded-lg">
                <p className="text-sm text-surface-600">Fat</p>
                <p className="font-semibold text-surface-900">{recipe.totalFat}g</p>
              </div>
              <div className="text-center p-3 bg-surface-50 rounded-lg">
                <p className="text-sm text-surface-600">Carbs</p>
                <p className="font-semibold text-surface-900">{recipe.totalCarbohydrates}g</p>
              </div>
              
            </div>
          </div>

          {/* Ingredients */}
          <div className="mb-6">
            <h3 className="font-medium text-surface-900 mb-3">Ingredients</h3>
            <div className="space-y-2">
              {recipe.ingredients && recipe.ingredients.length > 0 ? (
                recipe.ingredients.map((ingredient, index) => {
                  
                  const ingredientName = ingredient.product?.name || 
                                       ingredient.customIngredientName ||
                                       ingredient.notes ||
                                       ingredient.productName ||
                                       ingredient.id || 
                                       `Ingredient ${index + 1}`;
                  const quantity = ingredient.quantityGrams || 0;
                  
                  return (
                    <div key={ingredient.id || index} className="flex items-center justify-between p-3 bg-surface-50 rounded-lg">
                      <span className="text-surface-900">{ingredientName}</span>
                      <span className="text-surface-600 text-sm">{quantity}g</span>
                    </div>
                  );
                })
              ) : (
                <p className="text-surface-500 italic">No ingredients available</p>
              )}
            </div>
          </div>

          {/* Instructions */}
          <div className="mb-6">
            <h3 className="font-medium text-surface-900 mb-3">Instructions</h3>
            <div className="prose prose-surface max-w-none">
              {recipe.instructions ? (
                recipe.instructions.split('\n').map((step, index) => (
                  <p key={index} className="mb-2">{step}</p>
                ))
              ) : (
                <p className="text-surface-500 italic">No instructions available</p>
              )}
            </div>
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-4 border-t border-surface-200">
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              className="flex-1"
            >
              Close
            </Button>
            {onToggleFavorite && (
              <Button
                type="button"
                variant="outline"
                onClick={onToggleFavorite}
                disabled={!canToggle}
                title={getFavoriteToggleMessage(recipe) || (recipe.isFavorite ? "Remove from favorites" : "Add to favorites")}
                className={`flex items-center gap-2 ${!canToggle ? 'opacity-75 cursor-not-allowed' : ''}`}
              >
                <Heart className={`h-4 w-4 ${recipe.isFavorite ? 'fill-current' : ''} ${!canToggle ? 'text-orange-500' : 'text-red-500'}`} />
                {recipe.isFavorite ? 'Remove from Favorites' : 'Add to Favorites'}
              </Button>
            )}
            <Button
              type="button"
              onClick={handleAddToDiary}
              className="flex items-center gap-2"
            >
              <Plus className="h-4 w-4" />
              Add to Diary
            </Button>
          </div>
        </div>
      </Card>

      {/* Add Recipe to Diary Modal */}
      <AddRecipeToDiaryModal
        isOpen={showAddToDiaryModal}
        onClose={() => setShowAddToDiaryModal(false)}
        recipe={recipe}
        onSuccess={handleAddToDiarySuccess}
      />
    </div>
  );
};

export default RecipeDetailsModal;
