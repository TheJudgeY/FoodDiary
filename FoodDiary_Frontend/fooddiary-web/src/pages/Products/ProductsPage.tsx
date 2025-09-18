import React, { useState, useEffect } from 'react';
import { useDebounce } from '../../hooks/useDebounce';
import { 
  Search, 
  Filter, 
  Plus, 
  Grid, 
  List, 
  MoreVertical,
  Edit,
  Trash2,
  Eye,
  Upload
} from 'lucide-react';
import { useProductStore } from '../../stores/productStore';
import { formatNumber, getProductImageWithAuth, getProductCategoryLabel } from '../../utils';
import { Product } from '../../types';
import Card from '../../components/UI/Card';
import Button from '../../components/UI/Button';
import LoadingSpinner from '../../components/UI/LoadingSpinner';
import Pagination from '../../components/UI/Pagination';
import AddProductModal from './AddProductModal';
import ProductDetailModal from './ProductDetailModal';

const ProductsPage: React.FC = () => {
  const { 
    filteredProducts, 
    isLoading, 
    error, 
    fetchProducts, 
    searchProducts, 
    filterByCategory,
    deleteProduct,
    resetFilteredProducts,
    currentPage,
    totalPages,
    totalCount,
    setPage
  } = useProductStore();

  
  const products = Array.isArray(filteredProducts) ? filteredProducts : [];

  const [searchTerm, setSearchTerm] = useState('');
  const [selectedCategory, setSelectedCategory] = useState<number | 'All'>('All');
  const [viewMode, setViewMode] = useState<'grid' | 'list'>('grid');
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState<Product | null>(null);
  const [showDetailModal, setShowDetailModal] = useState(false);

  useEffect(() => {
    fetchProducts();
  }, [fetchProducts]); 

  
  const debouncedSearchTerm = useDebounce(searchTerm, 300); 

  
  useEffect(() => {        
    if (currentPage !== 1) {
      setPage(1);
      return;
    }

    
    if (debouncedSearchTerm.length >= 2 || debouncedSearchTerm.length === 0) {
      if (selectedCategory !== 'All') {
        filterByCategory(selectedCategory);
      } else if (debouncedSearchTerm) {
        searchProducts(debouncedSearchTerm);
      } else {
        resetFilteredProducts();
      }
    }
  }, [debouncedSearchTerm, selectedCategory, searchProducts, filterByCategory, resetFilteredProducts, currentPage, setPage]); 

  const handleDeleteProduct = async (productId: string) => {
    if (window.confirm('Are you sure you want to delete this product?')) {
      try {
        await deleteProduct(productId);
       } catch (error: unknown) {
        alert(`Failed to delete product: ${error instanceof Error ? error.message : 'Unknown error'}`);
      }
    }
  };

  const categories = [
    { value: 'All', label: 'All Categories' },
    { value: 0, label: 'Fruits' },
    { value: 1, label: 'Vegetables' },
    { value: 2, label: 'Grains' },
    { value: 3, label: 'Proteins' },
    { value: 4, label: 'Dairy' },
    { value: 5, label: 'Nuts & Seeds' },
    { value: 6, label: 'Beverages' },
    { value: 7, label: 'Snacks' },
    { value: 8, label: 'Condiments' },
    { value: 9, label: 'Supplements' },
    { value: 10, label: 'Other' },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-surface-900">Products</h1>
          <p className="text-surface-600">Manage your food products and nutritional database.</p>
        </div>
        <Button
          onClick={() => setShowAddModal(true)}
          className="mt-4 sm:mt-0"
          leftIcon={<Plus className="h-4 w-4" />}
        >
          Add Product
        </Button>
      </div>

      {/* Filters and Search */}
      <Card className="p-6">
        <div className="space-y-4">
          {/* Search Bar */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-surface-400" />
            <input
              type="text"
              placeholder="Search products..."
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
                onChange={(e) => {
                  const value = e.target.value;
                  if (value === 'All') {
                    setSelectedCategory('All');
                  } else {
                    const parsedValue = parseInt(value);
                    setSelectedCategory(parsedValue);
                  }
                }}
                className="px-3 py-1 border border-surface-300 rounded-md text-sm focus:ring-2 focus:ring-primary-500 focus:border-transparent"
              >
                {categories.map(category => (
                  <option key={category.value} value={category.value}>
                    {category.label}
                  </option>
                ))}
              </select>
            </div>

            {/* View Mode Toggle */}
            <div className="flex items-center gap-1 ml-auto">
              <Button
                variant={viewMode === 'grid' ? 'primary' : 'outline'}
                size="sm"
                onClick={() => setViewMode('grid')}
              >
                <Grid className="h-4 w-4" />
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

      {/* Error Display */}
      {error && (
        <Card>
          <div className="bg-error-50 border border-error-200 rounded-lg p-4">
            <p className="text-sm text-error-600">{error}</p>
          </div>
        </Card>
      )}

      {/* Products Display */}
      <div className="relative min-h-[400px]">
        {/* Loading Overlay */}
        {isLoading && (
          <div className="absolute inset-0 bg-white/80 backdrop-blur-sm flex items-center justify-center z-10">
            <LoadingSpinner size="lg" />
          </div>
        )}

        {/* Content */}
        {products.length === 0 ? (
          <Card className="p-12">
            <div className="text-center">
              <div className="w-16 h-16 bg-surface-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <Search className="h-8 w-8 text-surface-400" />
              </div>
              <h3 className="text-lg font-semibold text-surface-900 mb-2">
                {searchTerm || selectedCategory !== 'All'
                  ? 'No products found'
                  : 'No products yet'
                }
              </h3>
              <p className="text-surface-600 mb-4">
                {searchTerm || selectedCategory !== 'All'
                  ? 'Try adjusting your search or filters.'
                  : 'Create your first product to get started!'
                }
              </p>
              {!searchTerm && selectedCategory === 'All' && (
                <Button onClick={() => setShowAddModal(true)}>
                  <Plus className="h-4 w-4 mr-2" />
                  Add Your First Product
                </Button>
              )}
            </div>
          </Card>
        ) : (
          <div>
            {/* Results Count */}
            <div className="flex items-center justify-between mb-4">
              <p className="text-sm text-surface-600">
                Showing {products.length} of {totalCount} products
              </p>
            </div>

            {/* Products Grid/List */}
            <div className="space-y-6">
              <div className={viewMode === 'grid' ? 'grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6' : 'space-y-4'}>
                {products.map((product) => (
                  <ProductCard
                    key={product.id}
                    product={product}
                    viewMode={viewMode}
                    onView={() => {
                      setSelectedProduct(product);
                      setShowDetailModal(true);
                    }}
                    onEdit={() => {
                      setSelectedProduct(product);
                      setShowAddModal(true);
                    }}
                    onDelete={() => handleDeleteProduct(product.id)}
                  />
                ))}
              </div>
              
              {/* Pagination */}
              <Pagination
                currentPage={currentPage}
                totalPages={totalPages}
                onPageChange={setPage}
              />
            </div>
          </div>
        )}
      </div>

      {/* Add/Edit Product Modal */}
      {showAddModal && (
        <AddProductModal
          product={selectedProduct || undefined}
          onClose={() => {
            setShowAddModal(false);
            setSelectedProduct(null);
          }}
          onSuccess={() => {
            setShowAddModal(false);
            setSelectedProduct(null);
            fetchProducts();
          }}
        />
      )}

      {/* Product Detail Modal */}
      {showDetailModal && selectedProduct && (
        <ProductDetailModal
          product={selectedProduct}
          onClose={() => {
            setShowDetailModal(false);
            setSelectedProduct(null);
          }}
        />
      )}
    </div>
  );
};


interface ProductCardProps {
   product: Product;
  viewMode: 'grid' | 'list';
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
}


interface ProductMenuProps {
  onView: () => void;
  onEdit: () => void;
  onDelete: () => void;
  showMenu: boolean;
  setShowMenu: (show: boolean) => void;
  className?: string;
}

const ProductMenu: React.FC<ProductMenuProps> = ({ onView, onEdit, onDelete, showMenu, setShowMenu, className = '' }) => {
  const menuItems = [
    { key: 'view', label: 'View Details', icon: Eye, onClick: onView, className: 'text-surface-700 hover:bg-surface-50' },
    { key: 'edit', label: 'Edit', icon: Edit, onClick: onEdit, className: 'text-surface-700 hover:bg-surface-50' },
    { key: 'delete', label: 'Delete', icon: Trash2, onClick: onDelete, className: 'text-error-600 hover:bg-error-50' }
  ];

  return (
    <div className={`relative ${className}`}>
      <button
        onClick={() => setShowMenu(!showMenu)}
        className="p-2 text-surface-400 hover:text-surface-600 rounded-lg hover:bg-surface-100"
      >
        <MoreVertical className="h-4 w-4" />
      </button>
      
      {showMenu && (
        <div className="absolute right-0 top-full mt-2 w-48 bg-white rounded-lg shadow-lg border border-surface-200 z-10">
          {menuItems.map(({ key, label, icon: Icon, onClick, className }) => (
            <button
              key={key}
              onClick={() => { onClick(); setShowMenu(false); }}
              className={`w-full px-4 py-2 text-left text-sm flex items-center ${className}`}
            >
              <Icon className="h-4 w-4 mr-2" />
              {label}
            </button>
          ))}
        </div>
      )}
    </div>
  );
};

const ProductCard: React.FC<ProductCardProps> = ({ product, viewMode, onView, onEdit, onDelete }) => {
  const [showMenu, setShowMenu] = useState(false);
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [imageError, setImageError] = useState(false);

  useEffect(() => {
    if (product.hasImage) {
      
      getProductImageWithAuth(product.id)
        .then(url => {
          setImageUrl(url);
          setImageError(false);
        })
        .catch(error => {
          console.error('Failed to load image:', error);
          setImageError(true);
        });
    }
  }, [product.id, product.hasImage]);

  const handleImageError = () => {
    setImageError(true);
  };

  if (viewMode === 'list') {
    return (
      <Card>
        <div className="flex items-center space-x-4">
          {/* Product Image */}
          <div className="w-16 h-16 bg-surface-100 rounded-lg flex items-center justify-center flex-shrink-0">
            {imageUrl && !imageError ? (
              <img
                src={imageUrl}
                alt={product.name}
                className="w-full h-full object-cover rounded-lg"
                onError={handleImageError}
              />
            ) : (
              <Upload className="h-6 w-6 text-surface-400" />
            )}
          </div>

          {/* Product Info */}
          <div className="flex-1 min-w-0">
            <h3 className="text-lg font-semibold text-surface-900 truncate">{product.name}</h3>
            <p className="text-sm text-surface-600 mb-2">{getProductCategoryLabel(product.category)}</p>
            <div className="flex space-x-4 text-sm text-surface-500">
              <span>{formatNumber(product.caloriesPer100g)} cal/100g</span>
              <span>{formatNumber(product.proteinsPer100g)}g protein</span>
              <span>{formatNumber(product.fatsPer100g)}g fat</span>
              <span>{formatNumber(product.carbohydratesPer100g)}g carbs</span>
            </div>
          </div>

          {/* Actions */}
          <ProductMenu
            onView={onView}
            onEdit={onEdit}
            onDelete={onDelete}
            showMenu={showMenu}
            setShowMenu={setShowMenu}
          />
        </div>
      </Card>
    );
  }

  
  return (
    <Card className="group hover:shadow-lg transition-shadow">
      <div className="relative">
        {/* Product Image */}
        <div className="w-full h-48 bg-surface-100 rounded-lg flex items-center justify-center mb-4">
          {imageUrl && !imageError ? (
            <img
              src={imageUrl}
              alt={product.name}
              className="w-full h-full object-cover rounded-lg"
              onError={handleImageError}
            />
          ) : (
            <Upload className="h-12 w-12 text-surface-400" />
          )}
        </div>

        {/* Actions Menu */}
        <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition-opacity">
          <ProductMenu
            onView={onView}
            onEdit={onEdit}
            onDelete={onDelete}
            showMenu={showMenu}
            setShowMenu={setShowMenu}
            className="opacity-0 group-hover:opacity-100 transition-opacity"
          />
        </div>

        {/* Product Info */}
        <div>
          <h3 className="text-lg font-semibold text-surface-900 mb-2">{product.name}</h3>
          <div className="inline-block bg-primary-100 text-primary-800 text-xs font-medium px-2 py-1 rounded-full mb-3">
            {getProductCategoryLabel(product.category)}
          </div>
          
          <div className="grid grid-cols-2 gap-2 text-sm text-surface-600">
            <div>
              <span className="font-medium">Calories:</span> {formatNumber(product.caloriesPer100g)} cal/100g
            </div>
            <div>
              <span className="font-medium">Protein:</span> {formatNumber(product.proteinsPer100g)}g
            </div>
            <div>
              <span className="font-medium">Fat:</span> {formatNumber(product.fatsPer100g)}g
            </div>
            <div>
              <span className="font-medium">Carbs:</span> {formatNumber(product.carbohydratesPer100g)}g
            </div>
          </div>
        </div>
      </div>
    </Card>
  );
};

export default ProductsPage;
