import { Recipe } from '../types';

/**
 * Determines if a user can edit a recipe based on their relationship
 */
export const canEditRecipe = (recipe: Recipe): boolean => {
  return recipe.isCreator || recipe.isContributor || false;
};

/**
 * Determines if a user can delete a recipe based on their relationship
 */
export const canDeleteRecipe = (recipe: Recipe): boolean => {
  return recipe.isCreator || false;
};

/**
 * Determines if a user can favorite a recipe based on their relationship and recipe visibility
 */
export const canFavoriteRecipe = (recipe: Recipe): boolean => {
  
  return recipe.isCreator || recipe.isPublic || false;
};

/**
 * Determines if a user can unfavorite a recipe based on their relationship
 */
export const canUnfavoriteRecipe = (recipe: Recipe): boolean => {
  
  if (recipe.isCreator && recipe.isFavorite) {
    return false;
  }
  
  return recipe.isFavorite && !recipe.isCreator;
};

/**
 * Determines if a user can toggle the favorite status of a recipe
 * This combines both favoriting and unfavoriting logic
 */
export const canToggleFavorite = (recipe: Recipe): boolean => {
  
  if (recipe.isCreator && recipe.isFavorite) {
    return false;
  }
  
  if (!recipe.isCreator) {
    return recipe.isPublic || recipe.isFavorite;
  }
  
  return !recipe.isFavorite;
};

/**
 * Gets a user-friendly message explaining why a recipe can't be favorited/unfavorited
 */
export const getFavoriteToggleMessage = (recipe: Recipe): string => {
  if (recipe.isCreator && recipe.isFavorite) {
    return "Recipe creators cannot unfavorite their own recipes";
  }
  if (!recipe.isCreator && !recipe.isPublic && !recipe.isFavorite) {
    return "You can only favorite public recipes";
  }
  return "";
};

/**
 * Gets the user's relationship type as a string for display
 */
export const getUserRelationshipLabel = (recipe: Recipe): string | null => {
  if (recipe.isCreator) return 'Creator';
  if (recipe.isContributor) return 'Contributor';
  if (recipe.isFavorite) return 'Favorited';
  return null;
};

/**
 * Gets the appropriate icon for the user's relationship
 */
export const getUserRelationshipIcon = (recipe: Recipe): string => {
  if (recipe.isCreator) return '👨‍🍳'; 
  if (recipe.isContributor) return '👥'; 
  if (recipe.isFavorite) return '❤️'; 
  return '👁️'; 
};

/**
 * Gets the color class for the relationship indicator
 */
export const getRelationshipColorClass = (recipe: Recipe): string => {
  if (recipe.isCreator) return 'text-blue-600';
  if (recipe.isContributor) return 'text-purple-600';
  if (recipe.isFavorite) return 'text-red-600';
  return 'text-gray-600';
};
