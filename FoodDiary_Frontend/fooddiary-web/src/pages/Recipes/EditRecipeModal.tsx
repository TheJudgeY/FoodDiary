import React, { useState, useEffect } from 'react';
import { X, Upload, ChefHat } from 'lucide-react';
import { useProductStore } from '../../stores/productStore';
import { recipeService } from '../../services/recipeService';
import { Recipe } from '../../types';
import { RECIPE_CATEGORIES_ARRAY } from '../../constants/recipeCategories';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import Input from '../../components/UI/Input';

interface EditRecipeModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (recipeId: string, recipeData: {
    name: string;
    description: string;
    category: string; 
    servings: number;
    preparationTimeMinutes: number;
    cookingTimeMinutes: number;
    instructions: string;
    isPublic: boolean;
  }) => Promise<void>;
  recipe: Recipe | null;
}


const EditRecipeModal: React.FC<EditRecipeModalProps> = ({ isOpen, onClose, onSubmit, recipe }) => {
  const { fetchProducts } = useProductStore();
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    category: "8", 
    servings: 4,
    preparationTimeMinutes: 15,
    cookingTimeMinutes: 30,
    instructions: '',
    isPublic: true,
  });
  const [selectedImage, setSelectedImage] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  useEffect(() => {
    if (isOpen) {
      fetchProducts(1, true); 
    }
  }, [isOpen, fetchProducts]);

  
  useEffect(() => {
    if (recipe && isOpen) {
      console.log('Loading recipe data:', {
        name: recipe.name,
        instructions: recipe.instructions,
        instructionsLength: recipe.instructions?.length,
        hasInstructions: !!recipe.instructions
      });
      
      setFormData({
        name: recipe.name,
        description: recipe.description || '',
        category: recipe.category.toString(),
        servings: recipe.servings,
        preparationTimeMinutes: recipe.preparationTimeMinutes,
        cookingTimeMinutes: recipe.cookingTimeMinutes,
        instructions: recipe.instructions || '',
        isPublic: recipe.isPublic,
      });
    }
  }, [recipe, isOpen]);

  const handleInputChange = (field: string, value: unknown) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      setSelectedImage(file);
      const reader = new FileReader();
      reader.onload = (e) => {
        setImagePreview(e.target?.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!recipe) return;

    setIsLoading(true);

    try {
      
      if (!formData.name.trim()) {
        throw new Error('Recipe name is required');
      }
      if (!formData.instructions.trim()) {
        throw new Error('Instructions are required');
      }

      await onSubmit(recipe.id, formData);
      
      
      if (selectedImage) {
        try {
          console.log('Starting image upload for recipe:', recipe.id);
          await recipeService.uploadRecipeImage(recipe.id, selectedImage);
          console.log('Image upload completed successfully');
        } catch (imageError) {
          console.error('Failed to upload image:', imageError);
          
          const errorMessage = imageError instanceof Error ? imageError.message : 'Unknown error';
          alert(`Recipe updated successfully, but image upload failed: ${errorMessage}\nYou can try uploading the image again later.`);
        }
      }
      
      
      setFormData({
        name: '',
        description: '',
        category: "8", 
        servings: 4,
        preparationTimeMinutes: 15,
        cookingTimeMinutes: 30,
        instructions: '',
        isPublic: true,
      });
      setSelectedImage(null);
      setImagePreview(null);
    } catch (error) {
      console.error('Failed to update recipe:', error);
      alert(error instanceof Error ? error.message : 'Failed to update recipe');
    } finally {
      setIsLoading(false);
    }
  };

  if (!isOpen || !recipe) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <Card className="w-full max-w-4xl max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          {/* Header */}
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <ChefHat className="h-6 w-6 text-primary-600" />
              <h2 className="text-xl font-semibold text-surface-900">Edit Recipe</h2>
            </div>
            <button
              onClick={onClose}
              className="p-2 hover:bg-surface-100 rounded-full transition-colors"
            >
              <X className="h-5 w-5" />
            </button>
          </div>

          <form onSubmit={handleSubmit} className="space-y-6">
            {/* Basic Information */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Recipe Name *
                </label>
                <Input
                  type="text"
                  value={formData.name}
                  onChange={(e) => handleInputChange('name', e.target.value)}
                  placeholder="Enter recipe name"
                  required
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Category
                </label>
                <select
                  value={formData.category}
                  onChange={(e) => handleInputChange('category', e.target.value)}
                  className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                >
                  {RECIPE_CATEGORIES_ARRAY.slice(1).map(category => (
                    <option key={category.value} value={category.value}>
                      {category.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Servings
                </label>
                <Input
                  type="number"
                  value={formData.servings}
                  onChange={(e) => handleInputChange('servings', parseInt(e.target.value))}
                  min="1"
                  max="20"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Preparation Time (minutes)
                </label>
                <Input
                  type="number"
                  value={formData.preparationTimeMinutes}
                  onChange={(e) => handleInputChange('preparationTimeMinutes', parseInt(e.target.value))}
                  min="0"
                  max="300"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-surface-700 mb-2">
                  Cooking Time (minutes)
                </label>
                <Input
                  type="number"
                  value={formData.cookingTimeMinutes}
                  onChange={(e) => handleInputChange('cookingTimeMinutes', parseInt(e.target.value))}
                  min="0"
                  max="300"
                />
              </div>

              <div className="flex items-center gap-4">
                <label className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    checked={formData.isPublic}
                    onChange={(e) => handleInputChange('isPublic', e.target.checked)}
                    className="rounded border-surface-300 text-primary-600 focus:ring-primary-500"
                  />
                  <span className="text-sm text-surface-700">Make recipe public</span>
                </label>
              </div>
            </div>

            {/* Description */}
            <div>
              <label className="block text-sm font-medium text-surface-700 mb-2">
                Description
              </label>
              <textarea
                value={formData.description}
                onChange={(e) => handleInputChange('description', e.target.value)}
                placeholder="Describe your recipe..."
                rows={3}
                className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              />
            </div>

            {/* Image Upload */}
            <div>
              <label className="block text-sm font-medium text-surface-700 mb-2">
                Recipe Image (Optional)
              </label>
              <div className="flex items-center gap-4">
                <div className="relative">
                  <input
                    type="file"
                    accept="image/*"
                    onChange={handleImageChange}
                    className="hidden"
                    id="recipe-image-edit"
                  />
                  <label
                    htmlFor="recipe-image-edit"
                    className="flex items-center gap-2 px-4 py-2 border border-surface-300 rounded-lg cursor-pointer hover:bg-surface-50 transition-colors"
                  >
                    <Upload className="h-4 w-4" />
                    Choose Image
                  </label>
                </div>
                {imagePreview && (
                  <div className="relative">
                    <img
                      src={imagePreview}
                      alt="Preview"
                      className="w-20 h-20 object-cover rounded-lg"
                    />
                    <button
                      type="button"
                      onClick={() => {
                        setSelectedImage(null);
                        setImagePreview(null);
                      }}
                      className="absolute -top-2 -right-2 p-1 bg-red-500 text-white rounded-full hover:bg-red-600"
                    >
                      <X className="h-3 w-3" />
                    </button>
                  </div>
                )}
                {recipe.hasImage && !imagePreview && (
                  <div className="text-sm text-surface-600">
                    Current image will be replaced
                  </div>
                )}
              </div>
            </div>

            {/* Instructions */}
            <div>
              <label className="block text-sm font-medium text-surface-700 mb-2">
                Instructions *
              </label>
              <textarea
                value={formData.instructions}
                onChange={(e) => handleInputChange('instructions', e.target.value)}
                placeholder="Enter step-by-step instructions..."
                rows={6}
                className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                required
              />
            </div>

            {/* Actions */}
            <div className="flex items-center justify-end gap-3 pt-4 border-t border-surface-200">
              <Button
                type="button"
                variant="outline"
                onClick={onClose}
                disabled={isLoading}
              >
                Cancel
              </Button>
              <Button
                type="submit"
                disabled={isLoading}
                className="flex items-center gap-2"
              >
                {isLoading ? 'Updating...' : 'Update Recipe'}
              </Button>
            </div>
          </form>
        </div>
      </Card>
    </div>
  );
};

export default EditRecipeModal;
