import React, { useState, useEffect } from 'react';
import { 
  Heart, 
  HeartOff, 
  Clock, 
  Users, 
  Edit, 
  Trash2, 
  Globe, 
  Lock,
  ChefHat
} from 'lucide-react';
import { Recipe } from '../../types';
import { getRecipeCategoryLabel, getRecipeImageWithAuth } from '../../utils';
import { canEditRecipe, canDeleteRecipe, canToggleFavorite, getFavoriteToggleMessage } from '../../utils/recipePermissions';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';

interface RecipeListProps {
  recipe: Recipe;
  onToggleFavorite: (recipeId: string) => void;
  onDelete: (recipeId: string) => void;
  onEdit: (recipe: Recipe) => void;
  onViewDetails: (recipe: Recipe) => void;
}

const RecipeList: React.FC<RecipeListProps> = ({ recipe, onToggleFavorite, onDelete, onEdit, onViewDetails }) => {
  const [isImageLoading, setIsImageLoading] = useState(true);
  const [imageError, setImageError] = useState(false);
  const [isFavoriteLoading, setIsFavoriteLoading] = useState(false);
  const [imageUrl, setImageUrl] = useState<string | null>(null);

  useEffect(() => {
    console.log(`RecipeList useEffect - recipe ${recipe.id}:`, {
      hasImage: recipe.hasImage,
      imageFileName: recipe.imageFileName,
      imageContentType: recipe.imageContentType,
      recipe: recipe
    });
    
    if (recipe.hasImage) {
      console.log(`Loading image for recipe ${recipe.id}`);
      setIsImageLoading(true);
      
      getRecipeImageWithAuth(recipe.id)
        .then(url => {
          console.log(`Successfully loaded image for recipe ${recipe.id}:`, url);
          setImageUrl(url);
          setImageError(false);
          setIsImageLoading(false);
        })
        .catch(error => {
          console.error(`Failed to load recipe image for recipe ${recipe.id}:`, error);
          setImageError(true);
          setIsImageLoading(false);
        });
    } else {
      console.log(`Recipe ${recipe.id} has no image`);
      setIsImageLoading(false);
    }
  }, [recipe]);

  const handleImageLoad = () => {
    setIsImageLoading(false);
  };

  const handleImageError = () => {
    setIsImageLoading(false);
    setImageError(true);
  };

  const getCategoryColor = (category: string | number) => {
    const categoryStr = typeof category === 'number' ? getRecipeCategoryLabel(category) : category;
    switch (categoryStr) {
      case 'Breakfast': return 'bg-orange-100 text-orange-800';
      case 'Lunch': return 'bg-green-100 text-green-800';
      case 'Dinner': return 'bg-blue-100 text-blue-800';
      case 'Snack': return 'bg-yellow-100 text-yellow-800';
      case 'Dessert': return 'bg-pink-100 text-pink-800';
      case 'Appetizer': return 'bg-purple-100 text-purple-800';
      case 'Soup': return 'bg-red-100 text-red-800';
      case 'Salad': return 'bg-emerald-100 text-emerald-800';
      case 'Main Course': return 'bg-indigo-100 text-indigo-800';
      case 'Side Dish': return 'bg-amber-100 text-amber-800';
      case 'Beverage': return 'bg-cyan-100 text-cyan-800';
      case 'Smoothie': return 'bg-fuchsia-100 text-fuchsia-800';
      case 'Bread': return 'bg-amber-100 text-amber-800';
      case 'Pasta': return 'bg-orange-100 text-orange-800';
      case 'Rice': return 'bg-yellow-100 text-yellow-800';
      case 'Meat': return 'bg-red-100 text-red-800';
      case 'Fish': return 'bg-blue-100 text-blue-800';
      case 'Vegetarian': return 'bg-green-100 text-green-800';
      case 'Vegan': return 'bg-emerald-100 text-emerald-800';
      case 'Gluten Free': return 'bg-lime-100 text-lime-800';
      case 'Low Carb': return 'bg-blue-100 text-blue-800';
      case 'High Protein': return 'bg-purple-100 text-purple-800';
      case 'Quick Meal': return 'bg-green-100 text-green-800';
      case 'Slow Cooker': return 'bg-orange-100 text-orange-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  const getCategoryLabel = (category: string | number) => {
    const categoryStr = typeof category === 'number' ? getRecipeCategoryLabel(category) : category;
    switch (categoryStr) {
      case 'Breakfast': return 'Breakfast';
      case 'Lunch': return 'Lunch';
      case 'Dinner': return 'Dinner';
      case 'Snack': return 'Snack';
      case 'Dessert': return 'Dessert';
      case 'Other': return 'Other';
      default: return categoryStr;
    }
  };

  return (
    <Card className="p-3 sm:p-4">
      <div className="flex flex-col sm:flex-row items-start gap-3 sm:gap-4">
        {/* Recipe Image */}
        <div className="relative w-16 h-16 sm:w-20 sm:h-20 bg-surface-100 rounded-lg overflow-hidden flex-shrink-0">
          {imageUrl && !imageError ? (
            <img
              src={imageUrl}
              alt={recipe.name}
              className="w-full h-full object-cover"
              onLoad={handleImageLoad}
              onError={handleImageError}
            />
          ) : imageError ? (
            <div className="w-full h-full flex items-center justify-center bg-surface-100">
              <div className="text-center">
                <ChefHat className="h-4 w-4 sm:h-6 sm:w-6 text-surface-400 mx-auto mb-1" />
                <p className="text-xs text-surface-500">Image unavailable</p>
              </div>
            </div>
          ) : (
            <div className="w-full h-full flex items-center justify-center bg-surface-100">
              <ChefHat className="h-4 w-4 sm:h-6 sm:w-6 text-surface-400" />
            </div>
          )}
          
          {/* Loading overlay */}
          {isImageLoading && recipe.hasImage && (
            <div className="absolute inset-0 bg-surface-200 flex items-center justify-center">
              <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary-600"></div>
            </div>
          )}
        </div>

        {/* Recipe Info */}
        <div className="flex-1 min-w-0">
          <div className="flex items-start justify-between mb-2">
            <div className="flex-1 min-w-0">
              <h3 className="text-base sm:text-lg font-semibold text-surface-900 truncate">
                {recipe.name}
              </h3>
              {recipe.description && (
                <p className="text-xs sm:text-sm text-surface-600 mt-1 line-clamp-2 hidden sm:block">
                  {recipe.description}
                </p>
              )}
            </div>
            
            {/* Favorite button */}
            <button
              onClick={async () => {
                setIsFavoriteLoading(true);
                try {
                  await onToggleFavorite(recipe.id);
                } finally {
                  setIsFavoriteLoading(false);
                }
              }}
              disabled={isFavoriteLoading || !canToggleFavorite(recipe)}
              title={getFavoriteToggleMessage(recipe) || (recipe.isFavorite ? "Remove from favorites" : "Add to favorites")}
              className={`p-1 sm:p-2 rounded-full transition-colors ml-2 disabled:opacity-50 disabled:cursor-not-allowed ${
                !canToggleFavorite(recipe)
                  ? 'opacity-75 cursor-not-allowed' 
                  : 'hover:bg-surface-100'
              }`}
            >
              {isFavoriteLoading ? (
                <div className="animate-spin rounded-full h-4 w-4 sm:h-5 sm:w-5 border-b-2 border-primary-600"></div>
              ) : recipe.isFavorite ? (
                <Heart className={`h-4 w-4 sm:h-5 sm:w-5 ${recipe.isCreator ? 'text-orange-500' : 'text-red-500'} fill-current`} />
              ) : (
                <HeartOff className="h-4 w-4 sm:h-5 sm:w-5 text-surface-400" />
              )}
            </button>
          </div>

          {/* Recipe stats and badges - Mobile Compact */}
          <div className="flex flex-wrap items-center gap-2 sm:gap-4 mb-2 sm:mb-3">
            <div className="flex items-center gap-1 text-xs sm:text-sm text-surface-500">
              <Clock className="h-3 w-3 sm:h-4 sm:w-4" />
              <span>{recipe.preparationTimeMinutes + recipe.cookingTimeMinutes}m</span>
            </div>
            <div className="flex items-center gap-1 text-xs sm:text-sm text-surface-500">
              <Users className="h-3 w-3 sm:h-4 sm:w-4" />
              <span>{recipe.servings}s</span>
            </div>
            <span className={`px-2 py-1 rounded-full text-xs font-medium ${getCategoryColor(recipe.category)}`}>
              {getCategoryLabel(recipe.category)}
            </span>
            {recipe.isPublic ? (
              <div className="flex items-center gap-1 text-xs text-green-600">
                <Globe className="h-3 w-3" />
                <span className="hidden sm:inline">Public</span>
              </div>
            ) : (
              <div className="flex items-center gap-1 text-xs text-surface-500">
                <Lock className="h-3 w-3" />
                <span className="hidden sm:inline">Private</span>
              </div>
            )}
            {/* Relationship indicator */}
            {recipe.isCreator && (
              <div className="flex items-center gap-1 text-xs text-blue-600">
                <ChefHat className="h-3 w-3" />
                <span className="hidden sm:inline">Creator</span>
              </div>
            )}
            {recipe.isContributor && !recipe.isCreator && (
              <div className="flex items-center gap-1 text-xs text-purple-600">
                <Users className="h-3 w-3" />
                <span className="hidden sm:inline">Contributor</span>
              </div>
            )}
          </div>

          {/* Nutritional info - Mobile Compact */}
          <div className="flex items-center gap-2 sm:gap-4 text-xs sm:text-sm mb-3">
            <div className="flex items-center gap-1">
              <span className="font-medium text-surface-900">{Math.round(recipe.caloriesPerServing)}</span>
              <span className="text-surface-600 hidden sm:inline">cal/serving</span>
              <span className="text-surface-600 sm:hidden">cal</span>
            </div>
            <div className="flex items-center gap-1">
              <span className="font-medium text-surface-900">{Math.round(recipe.proteinPerServing)}g</span>
              <span className="text-surface-600 hidden sm:inline">protein</span>
              <span className="text-surface-600 sm:hidden">p</span>
            </div>
            <div className="flex items-center gap-1">
              <span className="font-medium text-surface-900">{Math.round(recipe.fatPerServing)}g</span>
              <span className="text-surface-600 hidden sm:inline">fat</span>
              <span className="text-surface-600 sm:hidden">f</span>
            </div>
            <div className="flex items-center gap-1">
              <span className="font-medium text-surface-900">{Math.round(recipe.carbohydratesPerServing)}g</span>
              <span className="text-surface-600 hidden sm:inline">carbs</span>
              <span className="text-surface-600 sm:hidden">c</span>
            </div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center gap-2 flex-shrink-0 w-full sm:w-auto justify-end sm:justify-start">
          <Button
            variant="outline"
            size="sm"
            onClick={() => onViewDetails(recipe)}
            className="text-xs sm:text-sm px-2 sm:px-3 py-1 sm:py-2"
          >
            View
          </Button>
          {/* Show edit button for creators and contributors */}
          {canEditRecipe(recipe) && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => onEdit(recipe)}
              className="text-xs sm:text-sm px-2 sm:px-3 py-1 sm:py-2"
            >
              <Edit className="h-3 w-3 sm:h-4 sm:w-4" />
            </Button>
          )}
          {/* Show delete button only for creators */}
          {canDeleteRecipe(recipe) && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => onDelete(recipe.id)}
              className="text-error-600 hover:text-error-700 text-xs sm:text-sm px-2 sm:px-3 py-1 sm:py-2"
            >
              <Trash2 className="h-3 w-3 sm:h-4 sm:w-4" />
            </Button>
          )}
        </div>
      </div>
    </Card>
  );
};

export default RecipeList;
