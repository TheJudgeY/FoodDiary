import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { X, Search, Info } from 'lucide-react';
import { useFoodEntryStore } from '../../stores/foodEntryStore';
import { useProductStore } from '../../stores/productStore';
import { MealType, Product } from '../../types';
import Button from '../../components/UI/Button';
import Input from '../../components/UI/Input';
import Card from '../../components/UI/Card';

const addFoodEntrySchema = z.object({
  productId: z.string().min(1, 'Please select a product'),
  quantity: z.number().min(1, 'Quantity must be at least 1g').max(10000, 'Quantity cannot exceed 10kg'),
  mealType: z.number().min(0).max(3), 
});

type AddFoodEntryFormData = z.infer<typeof addFoodEntrySchema>;


const MEAL_TYPE_MAPPING = [
  { value: 0, displayName: 'Breakfast' },
  { value: 1, displayName: 'Lunch' },
  { value: 2, displayName: 'Dinner' },
  { value: 3, displayName: 'Snack' }
];


const getMealTypeNumericValue = (mealType: MealType): number => {
  switch (mealType) {
    case MealType.Breakfast: return 0;
    case MealType.Lunch: return 1;
    case MealType.Dinner: return 2;
    case MealType.Snack: return 3;
    default: return 0;
  }
};


const getMealTypeFromNumber = (mealTypeNumber: number): MealType => {
  switch (mealTypeNumber) {
    case 0: return MealType.Breakfast;
    case 1: return MealType.Lunch;
    case 2: return MealType.Dinner;
    case 3: return MealType.Snack;
    default: return MealType.Breakfast;
  }
};

interface AddFoodEntryModalProps {
  isOpen: boolean;
  onClose: () => void;
  selectedDate: string;
  defaultMealType?: MealType;
}

const AddFoodEntryModal: React.FC<AddFoodEntryModalProps> = ({
  isOpen,
  onClose,
  selectedDate,
  defaultMealType = MealType.Breakfast,
}) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [showProductList, setShowProductList] = useState(false);
  const [nutritionPreview, setNutritionPreview] = useState({
    calories: 0,
    proteins: 0,
    fats: 0,
    carbohydrates: 0,
  });

  const { addEntry, isLoading } = useFoodEntryStore();
  const { products, fetchProducts, isLoading: productsLoading } = useProductStore();

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
    reset,
  } = useForm<AddFoodEntryFormData>({
    resolver: zodResolver(addFoodEntrySchema),
    defaultValues: {
      mealType: getMealTypeNumericValue(defaultMealType),
      quantity: 100,
    },
  });

  const watchedQuantity = watch('quantity');
  const watchedProductId = watch('productId');

  useEffect(() => {
    if (isOpen) {
      fetchProducts(1, true); 
      reset({
        mealType: getMealTypeNumericValue(defaultMealType),
        quantity: 100,
      });
      setSelectedProduct(null);
      setSearchTerm('');
      setShowProductList(false);
    }
  }, [isOpen, fetchProducts, reset, defaultMealType]);

  useEffect(() => {
    if (watchedProductId && watchedQuantity) {
      const product = products.find(p => p.id === watchedProductId);
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
  }, [watchedProductId, watchedQuantity, products]);

  const handleSearch = (term: string) => {
    setSearchTerm(term);
    setShowProductList(term.length > 0);
  };

  const handleProductSelect = (product: Product) => {
    setSelectedProduct(product);
    setValue('productId', product.id);
    setSearchTerm(product.name);
    setShowProductList(false);
  };

  const handleClearProduct = () => {
    setSelectedProduct(null);
    setValue('productId', '');
    setSearchTerm('');
    setShowProductList(false);
  };

  const onSubmit = async (data: AddFoodEntryFormData) => {
    try {
      await addEntry({
        date: selectedDate,
        consumedAt: selectedDate,
        mealType: getMealTypeFromNumber(data.mealType), 
        productId: data.productId,
        quantity: data.quantity
      });
      reset();
      setSelectedProduct(null);
      setSearchTerm('');
      setShowProductList(false);
      onClose();
    } catch (error) {
      console.error('Failed to add food entry:', error);
    }
  };

  const filteredProducts = products.filter(product =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase())
  ).slice(0, 10); 

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-lg shadow-xl max-w-md w-full max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-6 border-b">
          <h2 className="text-xl font-semibold text-surface-900">Add Food Entry</h2>
          <button
            onClick={onClose}
            className="text-surface-400 hover:text-surface-600"
          >
            <X className="h-6 w-6" />
          </button>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-6">
          {/* Product Search */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-surface-700">
              Product *
            </label>
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-surface-400" />
              <Input
                type="text"
                placeholder="Search for a product..."
                value={searchTerm}
                onChange={(e) => handleSearch(e.target.value)}
                onFocus={() => setShowProductList(searchTerm.length > 0)}
                className="pl-10"
                disabled={productsLoading}
              />
              {productsLoading && (
                <div className="absolute right-3 top-1/2 transform -translate-y-1/2">
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary-600"></div>
                </div>
              )}
            </div>
            
            {/* Product Selection */}
            {selectedProduct ? (
              <div className="mt-2 p-3 bg-surface-50 rounded-lg border">
                <div className="flex items-center justify-between">
                  <div className="flex-1">
                    <h4 className="font-medium text-surface-900">{selectedProduct.name}</h4>
                    <p className="text-sm text-surface-600">
                      {selectedProduct.caloriesPer100g} kcal per 100g
                    </p>
                    <p className="text-xs text-surface-500">
                      {selectedProduct.proteinsPer100g}g protein • {selectedProduct.fatsPer100g}g fat • {selectedProduct.carbohydratesPer100g}g carbs
                    </p>
                  </div>
                  <button
                    type="button"
                    onClick={handleClearProduct}
                    className="text-surface-400 hover:text-surface-600 ml-2"
                  >
                    <X className="h-4 w-4" />
                  </button>
                </div>
              </div>
            ) : (
              showProductList && (
                <div className="mt-2 max-h-48 overflow-y-auto border rounded-lg bg-white shadow-lg">
                  {filteredProducts.length > 0 ? (
                    filteredProducts.map((product) => (
                      <button
                        key={product.id}
                        type="button"
                        onClick={() => handleProductSelect(product)}
                        className="w-full p-3 text-left hover:bg-surface-50 border-b last:border-b-0 focus:bg-surface-50 focus:outline-none"
                      >
                        <div className="font-medium text-surface-900">{product.name}</div>
                        <div className="text-sm text-surface-600">
                          {product.caloriesPer100g} kcal per 100g
                        </div>
                      </button>
                    ))
                  ) : (
                    <div className="p-3 text-surface-500 text-sm">
                      {searchTerm ? 'No products found' : 'Start typing to search products'}
                    </div>
                  )}
                </div>
              )
            )}
            
            <input
              {...register('productId')}
              type="hidden"
            />
            {errors.productId && (
              <p className="text-sm text-error-600">{errors.productId.message}</p>
            )}
          </div>

          {/* Quantity */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-surface-700">
              Quantity (grams) *
            </label>
            <Input
              type="number"
              {...register('quantity', { valueAsNumber: true })}
              min="1"
              max="10000"
              step="1"
              placeholder="100"
            />
            {errors.quantity && (
              <p className="text-sm text-error-600">{errors.quantity.message}</p>
            )}
            <p className="text-xs text-surface-500">
              Enter the amount in grams (1-10,000g)
            </p>
          </div>

          {/* Meal Type */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-surface-700">
              Meal Type *
            </label>
            <select
              {...register('mealType', { valueAsNumber: true })}
              className="w-full p-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
            >
              {MEAL_TYPE_MAPPING.map(({ value, displayName }) => (
                <option key={value} value={value}>
                  {displayName}
                </option>
              ))}
            </select>
            {errors.mealType && (
              <p className="text-sm text-error-600">{errors.mealType.message}</p>
            )}
          </div>

          {/* Nutrition Preview */}
          {selectedProduct && (
            <Card className="p-4">
              <div className="flex items-center gap-2 mb-3">
                <Info className="h-4 w-4 text-primary-600" />
                <h4 className="font-medium text-surface-900">Nutrition Preview</h4>
              </div>
              <div className="grid grid-cols-2 gap-4 text-sm">
                <div className="flex justify-between">
                  <span className="text-surface-600">Calories:</span>
                  <span className="font-medium">{nutritionPreview.calories} kcal</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-surface-600">Protein:</span>
                  <span className="font-medium">{nutritionPreview.proteins}g</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-surface-600">Fat:</span>
                  <span className="font-medium">{nutritionPreview.fats}g</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-surface-600">Carbs:</span>
                  <span className="font-medium">{nutritionPreview.carbohydrates}g</span>
                </div>
              </div>
            </Card>
          )}

          {/* Actions */}
          <div className="flex gap-3 pt-4">
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
              disabled={isLoading || !selectedProduct}
              className="flex-1"
            >
              {isLoading ? 'Adding...' : 'Add Entry'}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddFoodEntryModal;
