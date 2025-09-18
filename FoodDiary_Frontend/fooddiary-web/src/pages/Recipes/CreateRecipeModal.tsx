import React, { useState, useEffect } from 'react';
import { X, Plus, Trash2, ChefHat } from 'lucide-react';
import ImageUpload from '../../components/UI/ImageUpload';
import { useProductStore } from '../../stores/productStore';
import { RECIPE_CATEGORIES_ARRAY } from '../../constants/recipeCategories';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import Input from '../../components/UI/Input';

interface CreateRecipeModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSubmit: (recipeData: {
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
  }, imageFile: File | undefined) => Promise<{ recipeId: string; message?: string }>;
}

interface Ingredient {
  productId: string;
  quantityGrams: number;
  notes?: string;
}

const CreateRecipeModal: React.FC<CreateRecipeModalProps> = ({ isOpen, onClose, onSubmit }) => {
  const { products, fetchProducts } = useProductStore();
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
    ingredients: [] as Ingredient[],
  });
  const [selectedImage, setSelectedImage] = useState<File | null>(null);
  
  useEffect(() => {
    if (isOpen) {
      fetchProducts(1, true); 
    }
  }, [isOpen, fetchProducts]);

  const handleInputChange = (field: string, value: unknown) => {
    setFormData(prev => ({
      ...prev,
      [field]: value
    }));
  };

  



  const addIngredient = () => {
    setFormData(prev => ({
      ...prev,
      ingredients: [...prev.ingredients, { productId: '', quantityGrams: 0, notes: '' }]
    }));
  };

  const removeIngredient = (index: number) => {
    setFormData(prev => ({
      ...prev,
      ingredients: prev.ingredients.filter((_, i) => i !== index)
    }));
  };

  const updateIngredient = (index: number, field: string, value: unknown) => {
    setFormData(prev => ({
      ...prev,
      ingredients: prev.ingredients.map((ingredient, i) =>
        i === index ? { ...ingredient, [field]: value } : ingredient
      )
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      
      if (!formData.name.trim()) {
        throw new Error('Recipe name is required');
      }
      if (formData.ingredients.length === 0) {
        throw new Error('At least one ingredient is required');
      }
      
      const invalidIngredient = formData.ingredients.find(
        ing => !ing.productId || ing.quantityGrams <= 0
      );
      if (invalidIngredient) {
        throw new Error('All ingredients must have a product selected and a valid quantity');
      }
      if (!formData.instructions.trim()) {
        throw new Error('Instructions are required');
      }
      
      if (formData.servings < 1) {
        throw new Error('Number of servings must be at least 1');
      }
      if (formData.preparationTimeMinutes < 0) {
        throw new Error('Preparation time cannot be negative');
      }
      if (formData.cookingTimeMinutes < 0) {
        throw new Error('Cooking time cannot be negative');
      }

      
      const submitData = {
        name: formData.name.trim(),
        description: formData.description.trim(),
        category: formData.category.toString(),
        servings: Number(formData.servings),
        preparationTimeMinutes: Number(formData.preparationTimeMinutes),
        cookingTimeMinutes: Number(formData.cookingTimeMinutes),
        instructions: formData.instructions.trim(),
        isPublic: Boolean(formData.isPublic),
        ingredients: formData.ingredients.map(ingredient => ({
          productId: ingredient.productId,
          quantityGrams: Number(ingredient.quantityGrams),
          notes: ingredient.notes?.trim()
        }))
      };

      
      
      await onSubmit(submitData, selectedImage || undefined);
      
      
      setFormData({
        name: '',
        description: '',
        category: "8", 
        servings: 4,
        preparationTimeMinutes: 15,
        cookingTimeMinutes: 30,
        instructions: '',
        isPublic: true,
        ingredients: []
      });
      setSelectedImage(null);

    } catch (error) {
      console.error('Failed to create recipe:', error);
      alert(error instanceof Error ? error.message : 'Failed to create recipe');
    } finally {
      setIsLoading(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
      <Card className="w-full max-w-4xl max-h-[90vh] overflow-y-auto">
        <div className="p-6">
          {/* Header */}
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <ChefHat className="h-6 w-6 text-primary-600" />
              <h2 className="text-xl font-semibold text-surface-900">Create New Recipe</h2>
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
                  onChange={(e) => {
                    const value = parseInt(e.target.value, 10);
                    handleInputChange('servings', isNaN(value) ? 1 : value);
                  }}
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
                  onChange={(e) => {
                    const value = parseInt(e.target.value, 10);
                    handleInputChange('preparationTimeMinutes', isNaN(value) ? 0 : value);
                  }}
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
                  onChange={(e) => {
                    const value = parseInt(e.target.value, 10);
                    handleInputChange('cookingTimeMinutes', isNaN(value) ? 0 : value);
                  }}
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
            <ImageUpload
              onImageSelect={(file) => setSelectedImage(file)}
              onImageRemove={() => setSelectedImage(null)}
              currentImage={null} 
              label="Recipe Image (Optional)"
            />



            {/* Ingredients */}
            <div>
              <div className="flex items-center justify-between mb-4">
                <label className="block text-sm font-medium text-surface-700">
                  Ingredients *
                </label>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={addIngredient}
                  className="flex items-center gap-2"
                >
                  <Plus className="h-4 w-4" />
                  Add Ingredient
                </Button>
              </div>

              <div className="space-y-3">
                {formData.ingredients.map((ingredient, index) => (
                  <div key={index} className="flex items-center gap-3 p-3 border border-surface-200 rounded-lg">
                    <div className="flex-1">
                      <select
                        value={ingredient.productId}
                        onChange={(e) => updateIngredient(index, 'productId', e.target.value)}
                        className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                        required
                      >
                        <option value="">Select a product</option>
                        {products.map(product => (
                          <option key={product.id} value={product.id}>
                            {product.name}
                          </option>
                        ))}
                      </select>
                    </div>
                    <div className="w-24">
                      <Input
                        type="number"
                        value={ingredient.quantityGrams}
                        onChange={(e) => {
                          const value = e.target.value === '' ? 0 : parseFloat(e.target.value);
                          updateIngredient(index, 'quantityGrams', isNaN(value) ? 0 : value);
                        }}
                        placeholder="g"
                        min="0"
                        step="0.1"
                        required
                      />
                    </div>
                    <div className="flex-1">
                      <Input
                        type="text"
                        value={ingredient.notes || ''}
                        onChange={(e) => updateIngredient(index, 'notes', e.target.value)}
                        placeholder="Notes (optional)"
                      />
                    </div>
                    <button
                      type="button"
                      onClick={() => removeIngredient(index)}
                      className="p-2 text-error-600 hover:bg-error-50 rounded-lg transition-colors"
                    >
                      <Trash2 className="h-4 w-4" />
                    </button>
                  </div>
                ))}
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
                {isLoading ? 'Creating...' : 'Create Recipe'}
              </Button>
            </div>
          </form>
        </div>
      </Card>
    </div>
  );
};

export default CreateRecipeModal;
