import React, { useState, useEffect, useCallback } from 'react';
import { useDebounce } from '../../hooks/useDebounce';
import { 
  Plus, 
  Search, 
  Filter, 
  Grid3X3, 
  List,
  Heart,
  HeartOff,
  Globe,
  ChefHat
} from 'lucide-react';
import { useAuthStore } from '../../stores/authStore';
import { recipeService } from '../../services/recipeService';
import { Recipe } from '../../types';
import { RECIPE_CATEGORIES_ARRAY } from '../../constants/recipeCategories';
import { toastService } from '../../services/toastService';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import LoadingSpinner from '../../components/UI/LoadingSpinner';
import CreateRecipeModal from './CreateRecipeModal';
import EditRecipeModal from './EditRecipeModal';
import RecipeDetailsModal from './RecipeDetailsModal';
import RecipeCard from './RecipeCard';
import RecipeList from './RecipeList';

const RecipesPage: React.FC = () => {
  useAuthStore();
  const [recipes, setRecipes] = useState<Recipe[]>([]);
  const [filteredRecipes, setFilteredRecipes] = useState<Recipe[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<string>('');
  const [showFavorites, setShowFavorites] = useState(false);
  const [showPublic, setShowPublic] = useState(false); 
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [selectedRecipe, setSelectedRecipe] = useState<Recipe | null>(null);

  
  const debouncedSearchTerm = useDebounce(searchTerm, 300); 

  const loadRecipes = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const response = await recipeService.getRecipes({
        searchTerm: debouncedSearchTerm || undefined,
        category: selectedCategory !== '' ? selectedCategory : undefined,
        includeFavorites: showFavorites,
        includePublic: showPublic ? true : false, 
        page: 1,
        pageSize: 50
      });
      setRecipes(response.recipes || []);
      setFilteredRecipes(response.recipes || []);
    } catch (err) {
      console.error('Failed to load recipes:', err);
      setError('Failed to load recipes. Please try again.');
    } finally {
      setIsLoading(false);
    }
  }, [debouncedSearchTerm, selectedCategory, showFavorites, showPublic]);

  useEffect(() => {
    loadRecipes();
  }, [loadRecipes]);

  useEffect(() => {
    loadRecipes();
  }, [debouncedSearchTerm, selectedCategory, showFavorites, showPublic, loadRecipes]);

  const handleCreateRecipe = async (recipeData: {
    name: string;
    description: string;
    category: string; 
    servings: number;
    preparationTimeMinutes: number;
    cookingTimeMinutes: number;
    instructions: string;
    isPublic: boolean;
    ingredients: Array<{
      productId: string;
      quantityGrams: number;
      notes?: string;
    }>;
  }, imageFile: File | undefined): Promise<{ recipeId: string; message?: string }> => {
    try {
      const result = await recipeService.createRecipe(recipeData, imageFile);
      setIsCreateModalOpen(false);
      await loadRecipes();
      return { recipeId: result.id, message: 'Recipe created successfully' };
    } catch (err) {
      console.error('Failed to create recipe:', err);
      throw err;
    }
  };

  const handleEditRecipe = async (recipeId: string, recipeData: {
    name: string;
    description: string;
    category: string; 
    servings: number;
    preparationTimeMinutes: number;
    cookingTimeMinutes: number;
    instructions: string;
    isPublic: boolean;
  }) => {
    try {
      await recipeService.updateRecipe(recipeId, {
        ...recipeData,
        category: recipeData.category.toString()
      });
      setIsEditModalOpen(false);
      setSelectedRecipe(null);
      await loadRecipes(); 
    } catch (err) {
      console.error('Failed to update recipe:', err);
      throw err;
    }
  };

  const handleViewDetails = async (recipe: Recipe) => {
    try {
      
      const fullRecipe = await recipeService.getRecipe(recipe.id);
      setSelectedRecipe(fullRecipe);
      setIsDetailsModalOpen(true);
    } catch (err) {
      console.error('Failed to load recipe details:', err);
      alert('Failed to load recipe details. Please try again.');
    }
  };

  const handleEditClick = async (recipe: Recipe) => {
    try {
      
      const fullRecipe = await recipeService.getRecipe(recipe.id);
      setSelectedRecipe(fullRecipe);
      setIsEditModalOpen(true);
    } catch (err) {
      console.error('Failed to load recipe details:', err);
      alert('Failed to load recipe details. Please try again.');
    }
  };

  const handleToggleFavorite = async (recipeId: string) => {
    try {
      const result = await recipeService.toggleFavorite(recipeId);
      
      
      const updateRecipe = (recipe: Recipe) => 
        recipe.id === recipeId 
          ? { 
              ...recipe, 
              isFavorite: result.isFavorite,
              isCreator: result.isCreator,
              isContributor: result.isContributor,
              isPublic: result.isPublic
            }
          : recipe;
      
      
      setRecipes(prevRecipes => prevRecipes.map(updateRecipe));
      setFilteredRecipes(prevRecipes => prevRecipes.map(updateRecipe));
      
      
      toastService.success('Success', result.message);
    } catch (err: unknown) {
      console.error('Failed to toggle favorite:', err);
      
      const apiError = err as { status?: number; data?: { message?: string; isFavorite?: boolean } };
      if (apiError.status === 403 && apiError.data) {
        const { message, isFavorite } = apiError.data;
        
        
        toastService.warning('Cannot Unfavorite', message || 'Cannot unfavorite');
        
        
        const updateRecipe = (recipe: Recipe) => 
          recipe.id === recipeId 
            ? { ...recipe, isFavorite: isFavorite ?? recipe.isFavorite }
            : recipe;
        
        setRecipes(prevRecipes => prevRecipes.map(updateRecipe));
        setFilteredRecipes(prevRecipes => prevRecipes.map(updateRecipe));
      } else {
        
        const errorMessage = err instanceof Error ? err.message : 'Failed to toggle favorite';
        toastService.error('Error', errorMessage);
      }
    }
  };

  const handleDeleteRecipe = async (recipeId: string) => {
    if (window.confirm('Are you sure you want to delete this recipe?')) {
      try {
        await recipeService.deleteRecipe(recipeId);
        await loadRecipes(); 
      } catch (err) {
        console.error('Failed to delete recipe:', err);
      }
    }
  };


  const categories = RECIPE_CATEGORIES_ARRAY;

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <LoadingSpinner size="lg" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">Recipes</h1>
          <p className="text-surface-600">Create, manage, and discover delicious recipes.</p>
        </div>
        <Button
          onClick={() => setIsCreateModalOpen(true)}
          className="mt-4 sm:mt-0 flex items-center gap-2"
        >
          <Plus className="h-4 w-4" />
          Create Recipe
        </Button>
      </div>

      {/* Error Display */}
      {error && (
        <Card className="border-error-200 bg-error-50">
          <div className="p-4 text-error-700">
            <p className="font-medium">Error loading recipes</p>
            <p className="text-sm">{error}</p>
          </div>
        </Card>
      )}

      {/* Filters and Search */}
      <Card className="p-6">
        <div className="space-y-4">
          {/* Search Bar */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-surface-400" />
            <input
              type="text"
              placeholder="Search recipes..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full pl-10 pr-4 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
            />
          </div>

          {/* Filters Row */}
          <div className="flex flex-wrap gap-4">
            {/* Category Filter */}
            <div className="flex items-center gap-2">
              <Filter className="h-4 w-4 text-surface-500" />
              <select
                value={selectedCategory}
                onChange={(e) => setSelectedCategory(e.target.value === '' ? '' : e.target.value)}
                className="px-3 py-1 border border-surface-300 rounded-md text-sm focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              >
                {categories.map((category, index) => (
                  <option key={index} value={category.value}>
                    {category.label}
                  </option>
                ))}
              </select>
            </div>

            {/* Favorites Filter */}
            <Button
              variant={showFavorites ? 'primary' : 'outline'}
              size="sm"
              onClick={() => setShowFavorites(!showFavorites)}
              className="flex items-center gap-2"
            >
              {showFavorites ? <Heart className="h-4 w-4" /> : <HeartOff className="h-4 w-4" />}
              Favorites
            </Button>

            {/* Public Filter */}
            <Button
              variant={showPublic ? 'primary' : 'outline'}
              size="sm"
              onClick={() => setShowPublic(!showPublic)}
              className="flex items-center gap-2"
            >
              <Globe className="h-4 w-4" />
              Public
            </Button>

            {/* View Mode Toggle */}
            <div className="flex items-center gap-1 ml-auto">
              <Button
                variant={viewMode === 'grid' ? 'primary' : 'outline'}
                size="sm"
                onClick={() => setViewMode('grid')}
              >
                <Grid3X3 className="h-4 w-4" />
              </Button>
              <Button
                variant={viewMode === 'list' ? 'primary' : 'outline'}
                size="sm"
                onClick={() => setViewMode('list')}
              >
                <List className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </div>
      </Card>

      {/* Recipes Display */}
      {filteredRecipes.length === 0 ? (
        <Card className="p-12">
          <div className="text-center">
            <ChefHat className="h-12 w-12 text-surface-400 mx-auto mb-4" />
            <h3 className="text-lg font-semibold text-surface-900 mb-2">
              {searchTerm || selectedCategory !== '' || showFavorites || showPublic
                ? 'No recipes found'
                : 'No recipes yet'
              }
            </h3>
            <p className="text-surface-600 mb-4">
              {searchTerm || selectedCategory !== '' || showFavorites || showPublic
                ? 'Try adjusting your search or filters.'
                : 'Create your first recipe to get started!'
              }
            </p>
            {!searchTerm && selectedCategory === '' && !showFavorites && !showPublic && (
              <Button onClick={() => setIsCreateModalOpen(true)}>
                <Plus className="h-4 w-4 mr-2" />
                Create Your First Recipe
              </Button>
            )}
          </div>
        </Card>
      ) : (
        <div>
          {/* Results Count */}
          <div className="flex items-center justify-between mb-4">
            <p className="text-sm text-surface-600">
              Showing {filteredRecipes.length} of {recipes.length} recipes
            </p>
          </div>

          {/* Recipes Grid/List */}
          {viewMode === 'grid' ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {filteredRecipes.map((recipe) => (
                <RecipeCard
                  key={recipe.id}
                  recipe={recipe}
                  onToggleFavorite={handleToggleFavorite}
                  onDelete={handleDeleteRecipe}
                  onEdit={handleEditClick}
                  onViewDetails={handleViewDetails}
                />
              ))}
            </div>
          ) : (
            <div className="space-y-4">
              {filteredRecipes.map((recipe) => (
                <RecipeList
                  key={recipe.id}
                  recipe={recipe}
                  onToggleFavorite={handleToggleFavorite}
                  onDelete={handleDeleteRecipe}
                  onEdit={handleEditClick}
                  onViewDetails={handleViewDetails}
                />
              ))}
            </div>
          )}
        </div>
      )}

      {/* Modals */}
      <CreateRecipeModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSubmit={handleCreateRecipe}
      />
      <EditRecipeModal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedRecipe(null);
        }}
        onSubmit={handleEditRecipe}
        recipe={selectedRecipe}
      />
      <RecipeDetailsModal
        isOpen={isDetailsModalOpen}
        onClose={() => {
          setIsDetailsModalOpen(false);
          setSelectedRecipe(null);
        }}
        recipe={selectedRecipe}
        onToggleFavorite={async () => {
          if (selectedRecipe) {
            await handleToggleFavorite(selectedRecipe.id);
          }
        }}
        onAddToDiary={async () => {
          if (selectedRecipe) {
            // TODO: Implement add to diary functionality
          }
        }}
      />
    </div>
  );
};

export default RecipesPage;
