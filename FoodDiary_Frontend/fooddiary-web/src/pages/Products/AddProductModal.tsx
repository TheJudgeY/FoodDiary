import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { X } from 'lucide-react';
import ImageUpload from '../../components/UI/ImageUpload';
import { useProductStore } from '../../stores/productStore';
import { PRODUCT_CATEGORIES_ARRAY } from '../../constants/productCategories';
import Button from '../../components/UI/Button';
import Input from '../../components/UI/Input';
import LoadingSpinner from '../../components/UI/LoadingSpinner';

const productSchema = z.object({
  name: z.string().min(1, 'Product name is required'),
  description: z.string().optional(),
  caloriesPer100g: z.number().min(0, 'Calories must be 0 or greater'),
  proteinsPer100g: z.number().min(0, 'Protein must be 0 or greater'),
  fatsPer100g: z.number().min(0, 'Fat must be 0 or greater'),
  carbohydratesPer100g: z.number().min(0, 'Carbohydrates must be 0 or greater'),
  category: z.number().int().min(0).max(10),
});

type ProductFormData = z.infer<typeof productSchema>;

interface AddProductModalProps {
  product?: {
    id: string;
    name: string;
    description?: string;
    caloriesPer100g: number;
    proteinsPer100g: number;
    fatsPer100g: number;
    carbohydratesPer100g: number;
    category: number;
    imageUrl?: string;
  }; 
  onClose: () => void;
  onSuccess: () => void;
}

const AddProductModal: React.FC<AddProductModalProps> = ({ product, onClose, onSuccess }) => {
  const { createProduct, updateProduct, isLoading, error } = useProductStore();
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string | null>(product?.imageUrl || null);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
    watch,
  } = useForm<ProductFormData>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      name: product?.name || '',
      description: product?.description || '',
      caloriesPer100g: product?.caloriesPer100g || 0,
      proteinsPer100g: product?.proteinsPer100g || 0,
      fatsPer100g: product?.fatsPer100g || 0,
      carbohydratesPer100g: product?.carbohydratesPer100g || 0,
      category: product?.category || 10, 
    },
  });

  
  const watchedCategory = watch('category');
  console.log('Watched category:', watchedCategory, 'Type:', typeof watchedCategory);

  useEffect(() => {
    if (product) {
      setValue('name', product.name);
      setValue('description', product.description || '');
      setValue('caloriesPer100g', product.caloriesPer100g);
      setValue('proteinsPer100g', product.proteinsPer100g);
      setValue('fatsPer100g', product.fatsPer100g);
      setValue('carbohydratesPer100g', product.carbohydratesPer100g);
      setValue('category', product.category);
      setImagePreview(product.imageUrl || null);
    }
  }, [product, setValue]);

  

  const onSubmit = async (data: ProductFormData) => {
    try {
      
      console.log('Form submission data:', data);
      console.log('Category type:', typeof data.category, 'Value:', data.category);
      console.log('Image file:', imageFile);
      
      if (product) {
        
        if (imageFile) {
          
          await updateProduct(product.id, data, imageFile);
        } else {
          
          await updateProduct(product.id, data);
        }
      } else {
        
        if (imageFile) {
          
          await createProduct(data, imageFile);
        } else {
          
          await createProduct(data);
        }
      }
      onSuccess();
    } catch (productError) {
      // Error is handled in the product store
      console.error('Failed to save product:', productError);
    }
  };

  const categories = PRODUCT_CATEGORIES_ARRAY.slice(1).map((category) => ({
    value: category.value,
    label: category.label,
  }));

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-surface-200">
          <h2 className="text-xl font-semibold text-surface-900">
            {product ? 'Edit Product' : 'Add New Product'}
          </h2>
          <button
            onClick={onClose}
            className="p-2 text-surface-400 hover:text-surface-600 rounded-lg hover:bg-surface-100"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-6">
          {error && (
            <div className="bg-error-50 border border-error-200 rounded-lg p-4">
              <p className="text-sm text-error-600">{error}</p>
            </div>
          )}

          {/* Image Upload */}
          <ImageUpload
            onImageSelect={(file) => setImageFile(file)}
            onImageRemove={() => setImageFile(null)}
            currentImage={imagePreview}
            label="Product Image"
          />

          {/* Basic Information */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <Input
              label="Product Name"
              error={errors.name?.message}
              {...register('name')}
            />

            <div>
              <label className="block text-sm font-medium text-surface-700 mb-2">
                Category
              </label>
              <select
                {...register('category', { 
                  setValueAs: (value) => {
                    
                    const numValue = parseInt(value, 10);
                    console.log('Select value change:', value, 'Converted to:', numValue);
                    return numValue;
                  }
                })}
                className="w-full px-3 py-2 border border-surface-300 rounded-lg bg-white text-sm focus:border-primary-500 focus:ring-primary-500"
              >
                {categories.map((category) => (
                  <option key={category.value} value={category.value}>
                    {category.label}
                  </option>
                ))}
              </select>
              {errors.category && (
                <p className="mt-1 text-sm text-error-600">{errors.category.message}</p>
              )}
            </div>
          </div>

          <Input
            label="Description (Optional)"
            error={errors.description?.message}
            {...register('description')}
          />

          {/* Nutritional Information */}
          <div className="space-y-4">
            <h3 className="text-lg font-medium text-surface-900">Nutritional Information (per 100g)</h3>
            
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Input
                label="Calories"
                type="number"
                step="0.1"
                error={errors.caloriesPer100g?.message}
                {...register('caloriesPer100g', { valueAsNumber: true })}
              />

              <Input
                label="Protein (g)"
                type="number"
                step="0.1"
                error={errors.proteinsPer100g?.message}
                {...register('proteinsPer100g', { valueAsNumber: true })}
              />

              <Input
                label="Fat (g)"
                type="number"
                step="0.1"
                error={errors.fatsPer100g?.message}
                {...register('fatsPer100g', { valueAsNumber: true })}
              />

              <Input
                label="Carbohydrates (g)"
                type="number"
                step="0.1"
                error={errors.carbohydratesPer100g?.message}
                {...register('carbohydratesPer100g', { valueAsNumber: true })}
              />
            </div>
          </div>

          {/* Actions */}
          <div className="flex justify-end space-x-3 pt-6 border-t border-surface-200">
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
              loading={isLoading}
              leftIcon={isLoading ? <LoadingSpinner size="sm" /> : undefined}
            >
              {product ? 'Update Product' : 'Add Product'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddProductModal;
