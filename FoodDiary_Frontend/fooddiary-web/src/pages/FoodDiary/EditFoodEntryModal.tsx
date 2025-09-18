import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { X, Trash2 } from 'lucide-react';
import { useFoodEntryStore } from '../../stores/foodEntryStore';
import { useProductStore } from '../../stores/productStore';
import { FoodEntry, MealType } from '../../types';
import { getMealTypeLabel } from '../../utils';
import Button from '../../components/UI/Button';
import Input from '../../components/UI/Input';
import Card from '../../components/UI/Card';

const editFoodEntrySchema = z.object({
  quantity: z.number().min(1, 'Quantity must be at least 1g'),
  mealType: z.number().min(0).max(3), 
});

type EditFoodEntryFormData = z.infer<typeof editFoodEntrySchema>;




const getMealTypeNumericValue = (mealType: MealType): number => {
  console.log('Converting meal type:', mealType);
  console.log('MealType enum values:', {
    Breakfast: MealType.Breakfast,
    Lunch: MealType.Lunch,
    Dinner: MealType.Dinner,
    Snack: MealType.Snack
  });
  
  let result: number;
  switch (mealType) {
    case MealType.Breakfast: 
      result = 0;
      break;
    case MealType.Lunch: 
      result = 1;
      break;
    case MealType.Dinner: 
      result = 2;
      break;
    case MealType.Snack: 
      result = 3;
      break;
    default: 
      result = 0;
      break;
  }
  
  console.log('Conversion result:', result);
  return result;
};



interface EditFoodEntryModalProps {
  isOpen: boolean;
  onClose: () => void;
  entry: FoodEntry | null;
}

const EditFoodEntryModal: React.FC<EditFoodEntryModalProps> = ({
  isOpen,
  onClose,
  entry,
}) => {
  const [nutritionPreview, setNutritionPreview] = useState({
    calories: 0,
    proteins: 0,
    fats: 0,
    carbohydrates: 0,
  });

  const { updateEntry, deleteEntry, isLoading } = useFoodEntryStore();
  const { products } = useProductStore();

  const defaultValues = {
    quantity: entry?.quantity || 100,
    mealType: entry?.mealType ? getMealTypeNumericValue(entry.mealType) : 0,
  };

  console.log('Entry for default values:', entry);
  console.log('Default values:', defaultValues);
  console.log('Meal type conversion for defaults:', {
    original: entry?.mealType,
    converted: entry?.mealType ? getMealTypeNumericValue(entry.mealType) : 0
  });

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
    reset,
  } = useForm<EditFoodEntryFormData>({
    resolver: zodResolver(editFoodEntrySchema),
    defaultValues,
  });

  const watchedQuantity = watch('quantity');
  const watchedMealType = watch('mealType');

  useEffect(() => {
    if (entry) {
      console.log('Entry for form reset:', entry);
      console.log('Entry meal type:', entry.mealType);
      console.log('Entry quantity:', entry.quantity);
      
      const resetData = {
        quantity: entry.quantity,
        mealType: entry.mealType,
      };
      
      console.log('Resetting form with data:', resetData);
      reset(resetData);
    }
  }, [entry, reset]);

  useEffect(() => {
    if (entry && watchedQuantity) {
      const product = products.find(p => p.id === entry.productId);
      if (product) {
        const ratio = watchedQuantity / 100;
        setNutritionPreview({
          calories: Math.round(product.caloriesPer100g * ratio),
          proteins: Math.round(product.proteinsPer100g * ratio * 10) / 10,
          fats: Math.round(product.fatsPer100g * ratio * 10) / 10,
          carbohydrates: Math.round(product.carbohydratesPer100g * ratio * 10) / 10,
        });
      }
    }
  }, [entry, watchedQuantity, products]);

  const onSubmit = async (data: EditFoodEntryFormData) => {
    if (!entry) return;
    
    
    console.log('Original entry:', entry);
    console.log('Form data:', data);
    console.log('Watched quantity:', watchedQuantity);
    console.log('Watched meal type:', watchedMealType);
    
    
    const formatDateForAPI = (dateString: string) => {
      if (!dateString) return new Date().toISOString().split('T')[0];
      
      return dateString.split('T')[0];
    };

    const updatePayload = {
      date: formatDateForAPI(entry.consumedAt || entry.date), 
      mealType: Number(data.mealType), 
      productId: entry.productId,
      quantityGrams: data.quantity, 
      notes: entry.notes || "", 
    };
    
    console.log('Update payload being sent:', updatePayload);
    console.log('Entry ID:', entry.id);
    console.log('Product ID:', entry.productId);
    console.log('Entry object keys:', Object.keys(entry));
    console.log('Entry date field:', entry.date);
    console.log('Entry consumedAt field:', entry.consumedAt);
    console.log('Entry quantity field:', entry.quantity);
    console.log('Meal type conversion:', {
      original: data.mealType,
      converted: Number(data.mealType),
      type: typeof Number(data.mealType)
    });
    
    try {
      console.log('Calling updateEntry with:', {
        id: entry.id,
        payload: updatePayload
      });
      
      const result = await updateEntry(entry.id, updatePayload);
      console.log('Update result:', result);
      onClose();
    } catch (error) {
      console.error('=== EditFoodEntryModal Error ===');
      console.error('Failed to update food entry:', error);
      console.error('Error details:', {
        message: error instanceof Error ? error.message : 'Unknown error',
        stack: error instanceof Error ? error.stack : undefined,
        error: error
      });
      
      
      if (error instanceof Error && 'response' in error) {
        const apiError = error as { response?: { status?: number; data?: unknown } };
        console.error('API Error Response:', apiError.response);
        console.error('API Error Status:', apiError.response?.status);
        console.error('API Error Data:', apiError.response?.data);
      }
    }
  };

  const handleDelete = async () => {
    if (!entry) return;
    
    console.log('Deleting entry:', entry);
    console.log('Entry ID:', entry.id);
    
    if (window.confirm('Are you sure you want to delete this food entry?')) {
      try {
        console.log('Calling deleteEntry with ID:', entry.id);
        const result = await deleteEntry(entry.id);
        console.log('Delete result:', result);
        onClose();
      } catch (error) {
        console.error('=== Delete Error ===');
        console.error('Failed to delete food entry:', error);
        console.error('Error details:', {
          message: error instanceof Error ? error.message : 'Unknown error',
          stack: error instanceof Error ? error.stack : undefined,
          error: error
        });
      }
    }
  };

  if (!isOpen || !entry) return null;

  const product = products.find(p => p.id === entry.productId);

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-6 border-b">
          <h2 className="text-xl font-semibold text-surface-900">Edit Food Entry</h2>
          <button
            onClick={onClose}
            className="text-surface-400 hover:text-surface-600"
          >
            <X className="h-6 w-6" />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-6">
          {/* Product Info */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-surface-700">
              Product
            </label>
            <div className="p-3 bg-surface-50 rounded-lg border">
              <h4 className="font-medium text-surface-900">{product?.name || 'Unknown Product'}</h4>
              <p className="text-sm text-surface-600">
                {product?.caloriesPer100g || 0} kcal per 100g
              </p>
            </div>
          </div>

          {/* Quantity */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-surface-700">
              Quantity (grams)
            </label>
            <Input
              type="number"
              {...register('quantity', { valueAsNumber: true })}
              min="1"
              step="1"
              placeholder="100"
            />
            {errors.quantity && (
              <p className="text-sm text-error-600">{errors.quantity.message}</p>
            )}
          </div>

          {/* Meal Type */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-surface-700">
              Meal Type
            </label>
            <select
              {...register('mealType', { valueAsNumber: true })}
              className="w-full px-3 py-2 border border-surface-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-primary-500 max-h-32 overflow-y-auto"
              size={4}
            >
              <option value={0}>{getMealTypeLabel(0)}</option>
              <option value={1}>{getMealTypeLabel(1)}</option>
              <option value={2}>{getMealTypeLabel(2)}</option>
              <option value={3}>{getMealTypeLabel(3)}</option>
            </select>
            {errors.mealType && (
              <p className="text-sm text-error-600">{errors.mealType.message}</p>
            )}
          </div>

          {/* Nutrition Preview */}
          {product && (
            <Card className="p-4">
              <h4 className="font-medium text-surface-900 mb-3">Nutrition Preview</h4>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div>
                  <span className="text-surface-600">Calories:</span>
                  <span className="ml-2 font-medium">{nutritionPreview.calories} kcal</span>
                </div>
                <div>
                  <span className="text-surface-600">Protein:</span>
                  <span className="ml-2 font-medium">{nutritionPreview.proteins}g</span>
                </div>
                <div>
                  <span className="text-surface-600">Fat:</span>
                  <span className="ml-2 font-medium">{nutritionPreview.fats}g</span>
                </div>
                <div>
                  <span className="text-surface-600">Carbs:</span>
                  <span className="ml-2 font-medium">{nutritionPreview.carbohydrates}g</span>
                </div>
              </div>
            </Card>
          )}

          {/* Actions */}
          <div className="flex gap-3 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={handleDelete}
              className="flex-1 text-error-600 border-error-300 hover:bg-error-50"
            >
              <Trash2 className="h-4 w-4 mr-2" />
              Delete
            </Button>
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              className="flex-1"
            >
              Cancel
            </Button>
            <Button
              type="submit"
              disabled={isLoading}
              className="flex-1"
            >
              {isLoading ? 'Updating...' : 'Update'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditFoodEntryModal;
