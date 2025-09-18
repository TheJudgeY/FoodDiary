import { getAuthToken } from '../utils';
import { Recipe, FoodEntry } from '../types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

interface RecipeService {
  getRecipes(params?: {
    searchTerm?: string;
    category?: string;
    includeFavorites?: boolean;
    includePublic?: boolean;
    page?: number;
    pageSize?: number;
  }): Promise<{ recipes: Recipe[]; totalCount: number; page: number; pageSize: number; totalPages: number }>;
  
  getRecipe(id: string): Promise<Recipe>;
  
  createRecipe(recipeData: {
    name: string;
    description?: string;
    category: string;
    servings: number;
    preparationTimeMinutes: number;
    cookingTimeMinutes: number;
    instructions: string;
    isPublic?: boolean;
    ingredients: Array<{
      productId: string;
      quantityGrams: number;
      notes?: string;
    }>;
  }, imageFile?: File): Promise<Recipe>;
  
  updateRecipe(recipeId: string, recipeData: {
    name?: string;
    description?: string;
    category?: string;
    servings?: number;
    preparationTimeMinutes?: number;
    cookingTimeMinutes?: number;
    instructions?: string;
    isPublic?: boolean;
    ingredients?: Array<{
      ingredientId?: string;
      productId: string;
      quantityGrams: number;
      notes?: string;
    }>;
  }): Promise<Recipe>;

  updateRecipeWithImage(recipeId: string, recipeData: {
    name?: string;
    description?: string;
    category?: string;
    servings?: number;
    preparationTimeMinutes?: number;
    cookingTimeMinutes?: number;
    instructions?: string;
    isPublic?: boolean;
    ingredients?: Array<{
      ingredientId?: string;
      productId: string;
      quantityGrams: number;
      notes?: string;
    }>;
    image?: File;
  }): Promise<Recipe>;
  
  uploadRecipeImage(recipeId: string, imageFile: File): Promise<{
    recipeId: string;
    imageFileName: string;
    imageContentType: string;
    imageSizeInBytes: number;
  }>;
  
  toggleFavorite(recipeId: string): Promise<{ 
    recipeId: string; 
    isFavorite: boolean; 
    isCreator: boolean;
    isContributor: boolean;
    isPublic: boolean;
    message: string; 
  }>;
  
  deleteRecipe(id: string): Promise<void>;
  
  getRecipeImage(recipeId: string): Promise<string>;
  
  addRecipeToDiary(data: {
    date: string;
    mealType: number;
    recipeId: string;
    servings: number;
  }): Promise<{ message: string; foodEntries: FoodEntry[] }>;
}

export const recipeService: RecipeService = {
  
  async getRecipes(params?) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const queryParams = new URLSearchParams();
    if (params?.searchTerm) queryParams.append('searchTerm', params.searchTerm);
    if (params?.category !== undefined) queryParams.append('category', params.category.toString());
    if (params?.includeFavorites) queryParams.append('includeFavorites', 'true');
    
    queryParams.append('includePublic', params?.includePublic ? 'true' : 'false');
    if (params?.page) queryParams.append('page', params.page.toString());
    if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());

    const url = `${API_BASE_URL}/api/recipes${queryParams.toString() ? `?${queryParams.toString()}` : ''}`;

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch recipes: ${response.statusText}`);
    }

    return response.json();
  },

  
  async getRecipe(id) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/recipes/${id}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch recipe: ${response.statusText}`);
    }

    return response.json();
  },

  
  async createRecipe(recipeData, imageFile?) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    
    const formData = new FormData();
    
    
    formData.append('name', recipeData.name);
    formData.append('category', recipeData.category);
    formData.append('servings', recipeData.servings.toString());
    formData.append('preparationTimeMinutes', recipeData.preparationTimeMinutes.toString());
    formData.append('cookingTimeMinutes', recipeData.cookingTimeMinutes.toString());
    formData.append('instructions', recipeData.instructions);
    formData.append('isPublic', recipeData.isPublic?.toString() || 'false');
    if (recipeData.description) {
      formData.append('description', recipeData.description);
    }

    
    recipeData.ingredients.forEach((ingredient, index) => {
      formData.append(`ingredients[${index}][productId]`, ingredient.productId);
      formData.append(`ingredients[${index}][quantityGrams]`, ingredient.quantityGrams.toString());
      if (ingredient.notes) {
        formData.append(`ingredients[${index}][notes]`, ingredient.notes);
      }
    });

    
    if (imageFile) {
      formData.append('image', imageFile);
    }

    const response = await fetch(`${API_BASE_URL}/api/recipes`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData,
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `Failed to create recipe: ${response.statusText}`);
    }

    return response.json();
  },

  
  async updateRecipe(recipeId, recipeData) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/recipes/${recipeId}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(recipeData)
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `Failed to update recipe: ${response.statusText}`);
    }

    return response.json();
  },

  
  async updateRecipeWithImage(recipeId, recipeData) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const formData = new FormData();
    
    
    Object.entries(recipeData).forEach(([key, value]) => {
      if (key === 'ingredients' && Array.isArray(value)) {
        
        value.forEach((ingredient, index) => {
          Object.entries(ingredient).forEach(([ingKey, ingValue]) => {
            if (ingValue !== undefined) {
              formData.append(`ingredients[${index}][${ingKey}]`, ingValue.toString());
            }
          });
        });
      } else if (key !== 'image' && value !== undefined) {
        formData.append(key, value.toString());
      }
    });
    
    
    if (recipeData.image) {
      formData.append('image', recipeData.image);
    }

    const response = await fetch(`${API_BASE_URL}/api/recipes/${recipeId}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `Failed to update recipe: ${response.statusText}`);
    }

    return response.json();
  },

  
  async uploadRecipeImage(recipeId, imageFile) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const formData = new FormData();
    formData.append('image', imageFile);

    const response = await fetch(`${API_BASE_URL}/api/recipes/${recipeId}/image`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `Failed to update recipe image: ${response.statusText}`);
    }

    return response.json();
  },

  
  async toggleFavorite(recipeId) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/recipes/${recipeId}/toggle-favorite`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      
      if (response.status === 403) {
        
        if (errorData.message && errorData.message.includes("Recipe creators cannot unfavorite")) {
          
          throw {
            status: 403,
            data: errorData,
            message: errorData.message
          };
        } else {
          throw new Error('You can only favorite your own recipes or public recipes');
        }
      } else if (response.status === 404) {
        throw new Error('Recipe not found');
      } else {
        throw new Error(errorData.message || `Failed to toggle favorite: ${response.statusText}`);
      }
    }

    return response.json();
  },

  
  async deleteRecipe(id) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/recipes/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Failed to delete recipe: ${response.statusText}`);
    }
  },

  
  async getRecipeImage(recipeId) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/recipes/${recipeId}/image`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get image: ${response.statusText}`);
    }

    
    return response.text();
  },

  
  async addRecipeToDiary(data) {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/food-entries/add-recipe`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData.message || `Failed to add recipe to diary: ${response.statusText}`);
    }

    return response.json();
  },
};

export default recipeService;