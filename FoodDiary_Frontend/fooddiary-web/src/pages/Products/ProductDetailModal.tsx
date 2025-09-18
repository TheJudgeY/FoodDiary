import React from 'react';
import { X, Calendar, Tag } from 'lucide-react';
import { formatNumber, formatDate, getProductCategoryLabel } from '../../utils';
import { Product } from '../../types';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';

interface ProductDetailModalProps {
  product: Product;
  onClose: () => void;
}

const ProductDetailModal: React.FC<ProductDetailModalProps> = ({ product, onClose }) => {
  const nutritionFacts = [
    { label: 'Calories', value: `${formatNumber(product.caloriesPer100g)} cal`, color: 'text-error-600' },
    { label: 'Protein', value: `${formatNumber(product.proteinsPer100g)}g`, color: 'text-success-600' },
    { label: 'Fat', value: `${formatNumber(product.fatsPer100g)}g`, color: 'text-warning-600' },
    { label: 'Carbohydrates', value: `${formatNumber(product.carbohydratesPer100g)}g`, color: 'text-primary-600' },
  ];

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
      <div className="bg-white rounded-xl max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex items-center justify-between p-6 border-b border-surface-200">
          <h2 className="text-xl font-semibold text-surface-900">Product Details</h2>
          <button
            onClick={onClose}
            className="p-2 text-surface-400 hover:text-surface-600 rounded-lg hover:bg-surface-100"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="p-6 space-y-6">
          {/* Product Image and Basic Info */}
          <div className="flex flex-col md:flex-row gap-6">
            {/* Image */}
            <div className="w-full md:w-48 h-48 bg-surface-100 rounded-lg flex items-center justify-center overflow-hidden flex-shrink-0">
              {product.imageUrl ? (
                <img
                  src={product.imageUrl}
                  alt={product.name}
                  className="w-full h-full object-cover"
                />
              ) : (
                <div className="text-center text-surface-400">
                  <Tag className="h-12 w-12 mx-auto mb-2" />
                  <p className="text-sm">No image</p>
                </div>
              )}
            </div>

            {/* Basic Info */}
            <div className="flex-1 space-y-4">
              <div>
                <h3 className="text-2xl font-bold text-surface-900 mb-2">{product.name}</h3>
                <div className="inline-block bg-primary-100 text-primary-800 text-sm font-medium px-3 py-1 rounded-full">
                  {getProductCategoryLabel(product.category)}
                </div>
              </div>

              {product.description && (
                <div>
                  <h4 className="text-sm font-medium text-surface-700 mb-2">Description</h4>
                  <p className="text-surface-600">{product.description}</p>
                </div>
              )}

              <div className="flex items-center text-sm text-surface-500">
                <Calendar className="h-4 w-4 mr-2" />
                <span>Added on {formatDate(product.createdAt)}</span>
              </div>
            </div>
          </div>

          {/* Nutrition Facts */}
          <Card>
            <h4 className="text-lg font-semibold text-surface-900 mb-4">Nutrition Facts (per 100g)</h4>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              {nutritionFacts.map((fact) => (
                <div key={fact.label} className="text-center">
                  <div className={`text-2xl font-bold ${fact.color}`}>{fact.value}</div>
                  <div className="text-sm text-surface-600">{fact.label}</div>
                </div>
              ))}
            </div>
          </Card>

          {/* Nutritional Breakdown */}
          <Card>
            <h4 className="text-lg font-semibold text-surface-900 mb-4">Nutritional Breakdown</h4>
            <div className="space-y-3">
              {/* Calories */}
              <div>
                <div className="flex justify-between text-sm mb-1">
                  <span>Calories</span>
                  <span>{formatNumber(product.caloriesPer100g)} cal</span>
                </div>
                <div className="w-full bg-surface-200 rounded-full h-2">
                  <div 
                    className="bg-error-500 h-2 rounded-full" 
                    style={{ width: `${Math.min((product.caloriesPer100g / 900) * 100, 100)}%` }}
                  />
                </div>
              </div>

              {/* Protein */}
              <div>
                <div className="flex justify-between text-sm mb-1">
                  <span>Protein</span>
                  <span>{formatNumber(product.proteinsPer100g)}g</span>
                </div>
                <div className="w-full bg-surface-200 rounded-full h-2">
                  <div 
                    className="bg-success-500 h-2 rounded-full" 
                    style={{ width: `${Math.min((product.proteinsPer100g / 50) * 100, 100)}%` }}
                  />
                </div>
              </div>

              {/* Fat */}
              <div>
                <div className="flex justify-between text-sm mb-1">
                  <span>Fat</span>
                  <span>{formatNumber(product.fatsPer100g)}g</span>
                </div>
                <div className="w-full bg-surface-200 rounded-full h-2">
                  <div 
                    className="bg-warning-500 h-2 rounded-full" 
                    style={{ width: `${Math.min((product.fatsPer100g / 65) * 100, 100)}%` }}
                  />
                </div>
              </div>

              {/* Carbohydrates */}
              <div>
                <div className="flex justify-between text-sm mb-1">
                  <span>Carbohydrates</span>
                  <span>{formatNumber(product.carbohydratesPer100g)}g</span>
                </div>
                <div className="w-full bg-surface-200 rounded-full h-2">
                  <div 
                    className="bg-primary-500 h-2 rounded-full" 
                    style={{ width: `${Math.min((product.carbohydratesPer100g / 300) * 100, 100)}%` }}
                  />
                </div>
              </div>
            </div>
          </Card>

          {/* Quick Actions */}
          <div className="flex justify-end space-x-3 pt-6 border-t border-surface-200">
            <Button variant="outline" onClick={onClose}>
              Close
            </Button>
            <Button>
              Add to Diary
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductDetailModal;
