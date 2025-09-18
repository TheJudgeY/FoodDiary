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
import QuickAddRecipe from '../../components/Recipes/QuickAddRecipe';

interface RecipeCardProps {
  recipe: Recipe;
  onToggleFavorite: (recipeId: string) => void;
  onDelete: (recipeId: string) => void;
  onEdit: (recipe: Recipe) => void;
  onViewDetails: (recipe: Recipe) => void;
}

const RecipeCard: React.FC<RecipeCardProps> = ({ recipe, onToggleFavorite, onDelete, onEdit, onViewDetails }) => {
  const [isImageLoading, setIsImageLoading] = useState(true);
  const [imageError, setImageError] = useState(false);
  const [isFavoriteLoading, setIsFavoriteLoading] = useState(false);
  const [imageUrl, setImageUrl] = useState<string | null>(null);

  useEffect(() => {
    if (recipe.hasImage) {
      setIsImageLoading(true);
      
      
      getRecipeImageWithAuth(recipe.id)
        .then(url => {
          setImageUrl(url);
          setImageError(false);
          setIsImageLoading(false);
        })
                .catch(() => {
          setImageError(true);
          setIsImageLoading(false);
        });
    } else {
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
    <Card className="overflow-hidden hover:shadow-lg transition-shadow duration-200">
      {/* Recipe Image */}
      <div className="relative h-48 bg-surface-100">
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
              <ChefHat className="h-12 w-12 text-surface-400 mx-auto mb-2" />
              <p className="text-xs text-surface-500">Image unavailable</p>
            </div>
          </div>
        ) : (
          <div className="w-full h-full flex items-center justify-center bg-surface-100">
            <ChefHat className="h-12 w-12 text-surface-400" />
          </div>
        )}
        
        {/* Loading overlay */}
        {isImageLoading && recipe.hasImage && (
          <div className="absolute inset-0 bg-surface-200 flex items-center justify-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
          </div>
        )}

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
          className={`absolute top-2 right-2 p-2 bg-white rounded-full shadow-md transition-all ${
            !canToggleFavorite(recipe)
              ? 'opacity-75 cursor-not-allowed' 
              : 'hover:shadow-lg hover:scale-105'
          } disabled:opacity-50 disabled:cursor-not-allowed`}
        >
          {isFavoriteLoading ? (
            <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary-600"></div>
          ) : recipe.isFavorite ? (
            <Heart className={`h-4 w-4 ${recipe.isCreator ? 'text-orange-500' : 'text-red-500'} fill-current`} />
          ) : (
            <HeartOff className="h-4 w-4 text-surface-400" />
          )}
        </button>

        {/* Public/Private indicator */}
        <div className="absolute top-2 left-2">
          {recipe.isPublic ? (
            <div className="flex items-center gap-1 px-2 py-1 bg-white rounded-full shadow-md">
              <Globe className="h-3 w-3 text-green-600" />
              <span className="text-xs text-green-600">Public</span>
            </div>
          ) : (
            <div className="flex items-center gap-1 px-2 py-1 bg-white rounded-full shadow-md">
              <Lock className="h-3 w-3 text-surface-500" />
              <span className="text-xs text-surface-500">Private</span>
            </div>
          )}
        </div>

        {/* Relationship indicator */}
        {(recipe.isCreator || recipe.isContributor) && (
          <div className="absolute top-2 left-16">
            {recipe.isCreator ? (
              <div className="flex items-center gap-1 px-2 py-1 bg-white rounded-full shadow-md">
                <ChefHat className="h-3 w-3 text-blue-600" />
                <span className="text-xs text-blue-600">Creator</span>
              </div>
            ) : recipe.isContributor ? (
              <div className="flex items-center gap-1 px-2 py-1 bg-white rounded-full shadow-md">
                <Users className="h-3 w-3 text-purple-600" />
                <span className="text-xs text-purple-600">Contributor</span>
              </div>
            ) : null}
          </div>
        )}

        {/* Category badge */}
        <div className="absolute bottom-2 left-2">
          <span className={`px-2 py-1 rounded-full text-xs font-medium ${getCategoryColor(recipe.category)}`}>
            {getCategoryLabel(recipe.category)}
          </span>
        </div>
      </div>

      {/* Recipe Content */}
      <div className="p-4">
        <h3 className="text-lg font-semibold text-surface-900 mb-2 line-clamp-2">
          {recipe.name}
        </h3>
        
        {recipe.description && (
          <p className="text-sm text-surface-600 mb-3 line-clamp-2">
            {recipe.description}
          </p>
        )}

        {/* Recipe stats */}
        <div className="flex items-center gap-4 mb-3 text-sm text-surface-500">
          <div className="flex items-center gap-1">
            <Clock className="h-4 w-4" />
            <span>{recipe.preparationTimeMinutes + recipe.cookingTimeMinutes} min</span>
          </div>
          <div className="flex items-center gap-1">
            <Users className="h-4 w-4" />
            <span>{recipe.servings} servings</span>
          </div>
        </div>

        {/* Nutritional info */}
        <div className="grid grid-cols-2 gap-2 mb-4 text-xs">
          <div className="text-center p-2 bg-surface-50 rounded">
            <div className="font-semibold text-surface-900">{Math.round(recipe.caloriesPerServing)}</div>
            <div className="text-surface-600">cal/serving</div>
          </div>
          <div className="text-center p-2 bg-surface-50 rounded">
            <div className="font-semibold text-surface-900">{Math.round(recipe.proteinPerServing)}g</div>
            <div className="text-surface-600">protein</div>
          </div>
          <div className="text-center p-2 bg-surface-50 rounded">
            <div className="font-semibold text-surface-900">{Math.round(recipe.fatPerServing)}g</div>
            <div className="text-surface-600">fat</div>
          </div>
          <div className="text-center p-2 bg-surface-50 rounded">
            <div className="font-semibold text-surface-900">{Math.round(recipe.carbohydratesPerServing)}g</div>
            <div className="text-surface-600">carbs</div>
          </div>
        </div>

        {/* Actions */}
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            className="flex-1"
            onClick={() => onViewDetails(recipe)}
          >
            View Details
          </Button>
          <QuickAddRecipe
            recipe={recipe}
            variant="icon"
            className="text-primary-600 hover:text-primary-700"
          />
          {/* Show edit button for creators and contributors */}
          {canEditRecipe(recipe) && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => onEdit(recipe)}
            >
              <Edit className="h-4 w-4" />
            </Button>
          )}
          {/* Show delete button only for creators */}
          {canDeleteRecipe(recipe) && (
            <Button
              variant="outline"
              size="sm"
              onClick={() => onDelete(recipe.id)}
              className="text-error-600 hover:text-error-700"
            >
              <Trash2 className="h-4 w-4" />
            </Button>
          )}
        </div>
      </div>
    </Card>
  );
};

export default RecipeCard;
