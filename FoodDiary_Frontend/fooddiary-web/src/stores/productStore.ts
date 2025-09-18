import { create } from 'zustand';
import type { Product, ProductCategory } from '../types';
import { getAuthToken, getErrorMessage } from '../utils';
import { API_BASE_URL } from '../constants';

interface ProductState {
  products: Product[];
  filteredProducts: Product[];
  selectedProduct: Product | null;
  isLoading: boolean;
  error: string | null;
  searchTerm: string;
  selectedCategory: ProductCategory | 'All';
  currentPage: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
  
  
  fetchProducts: (page?: number, fetchAll?: boolean) => Promise<void>;
  searchProducts: (search: string, page?: number) => Promise<void>;
  filterByCategory: (category: ProductCategory | 'All', page?: number) => void;
  getProduct: (id: string) => Promise<void>;
  createProduct: (product: { name: string; caloriesPer100g: number; proteinsPer100g: number; fatsPer100g: number; carbohydratesPer100g: number; description?: string; category: number; }, imageFile?: File | null) => Promise<Product>;
  updateProduct: (id: string, product: Partial<Product>, imageFile?: File | null) => Promise<void>;
  deleteProduct: (id: string) => Promise<void>;
  uploadProductImage: (id: string, file: File) => Promise<void>;
  clearSelectedProduct: () => void;
  clearError: () => void;
  resetFilteredProducts: () => void;
  setPage: (page: number) => void;
}

const handleApiError = (response: Response) => {
  if (response.status === 401) {
    
    window.location.href = '/login';
  } else if (response.status === 403) {
    throw new Error('Access denied');
  } else if (response.status === 404) {
    throw new Error('Product not found');
  } else {
    throw new Error('An error occurred');
  }
};

export const useProductStore = create<ProductState>((set, get) => ({
  products: [],
  filteredProducts: [],
  selectedProduct: null,
  isLoading: false,
  error: null,
  searchTerm: '',
  selectedCategory: 'All',
  currentPage: 1,
  totalPages: 1,
  pageSize: 12,
  totalCount: 0,

  fetchProducts: async (page?: number, fetchAll: boolean = false) => {
    const { pageSize } = get();
    const currentPage = page || get().currentPage;
    set({ isLoading: true, error: null });
    try {
      const token = getAuthToken();
      
      
      const effectivePageSize = fetchAll ? 1000 : pageSize;
      
      const response = await fetch(`${API_BASE_URL}/api/products?page=${currentPage}&pageSize=${effectivePageSize}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      });
      

      
      if (!response.ok) {
        await response.text();

        handleApiError(response);
      }
      
      const responseText = await response.text();

      
      if (!responseText) {
        throw new Error('Empty response from server');
      }
      
      let data;
      try {
        data = JSON.parse(responseText);
      } catch (error) {
        console.error('JSON parse error:', error);
        console.error('Response text that failed to parse:', responseText);
        throw new Error(`Invalid JSON response: ${responseText}`);
      }
      

      
      
      let productsArray: Product[] = [];
      
      if (Array.isArray(data)) {
        
        productsArray = data;
      } else if (data && typeof data === 'object') {
        
        if (data.products && Array.isArray(data.products)) {
          productsArray = data.products;
        } else if (data.data && Array.isArray(data.data)) {
          productsArray = data.data;
        } else if (data.items && Array.isArray(data.items)) {
          productsArray = data.items;
        } else if (data.value && Array.isArray(data.value)) {
          productsArray = data.value;
        } else {
          
          
          console.warn('Unknown response format:', data);

          productsArray = [];
        }
      } else {
        console.warn('Invalid response format:', data);
        productsArray = [];
      }
      
      set({ 
        products: productsArray, 
        filteredProducts: productsArray, 
        isLoading: false,
        currentPage: data.page || currentPage,
        totalPages: data.totalPages || 1,
        totalCount: data.totalCount || productsArray.length
      });
     } catch (error: unknown) {
        console.error('fetchProducts error:', error);
      set({ 
        error: getErrorMessage(error), 
        isLoading: false,
        products: [],
        filteredProducts: [],
      });
    }
  },

  searchProducts: async (search: string, page?: number) => {
    const { pageSize, selectedCategory } = get();
    const currentPage = page || get().currentPage;
    
    
    set({ searchTerm: search, isLoading: true, error: null });
    
    try {
      
      if (search.length === 1) {
        set({ isLoading: false });
        return;
      }

      
      const params = new URLSearchParams({
        page: currentPage.toString(),
        pageSize: pageSize.toString()
      });

      
      if (search.length >= 2) {
        params.append('search', search.trim());
      }

      
      if (selectedCategory !== 'All') {
        params.append('category', selectedCategory.toString());
      }



      const response = await fetch(`${API_BASE_URL}/api/products?${params}`, {
        headers: {
          'Authorization': `Bearer ${getAuthToken()}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const data = await response.json();
      
      
      const productsArray = data.products || [];
      
      
      set({ 
        products: productsArray, 
        filteredProducts: productsArray,
        isLoading: false,
        currentPage: data.page || currentPage,
        totalPages: data.totalPages || 1,
        totalCount: data.totalCount || productsArray.length
      });
    } catch (error: unknown) {
      console.error('Search error:', error);
      set({
        isLoading: false,
        error: getErrorMessage(error),
        filteredProducts: [],
      });
    }
  },

  filterByCategory: async (category: ProductCategory | 'All', page?: number) => {
    const { pageSize, searchTerm } = get();
    const currentPage = page || get().currentPage;
    set({ selectedCategory: category, isLoading: true, error: null });

    try {
      
      const params = new URLSearchParams({
        page: currentPage.toString(),
        pageSize: pageSize.toString()
      });

      
      if (searchTerm && searchTerm.length >= 2) {
        params.append('search', searchTerm.trim());
      }

      
      if (category !== 'All') {
        params.append('category', category.toString());
      }


      const response = await fetch(`${API_BASE_URL}/api/products?${params}`, {
        headers: {
          'Authorization': `Bearer ${getAuthToken()}`,
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        handleApiError(response);
      }

      const data = await response.json();
      
      const productsArray = data.products || [];

      set({
        products: productsArray, 
        filteredProducts: productsArray,
        isLoading: false,
        currentPage: data.page || currentPage,
        totalPages: data.totalPages || 1,
        totalCount: data.totalCount || productsArray.length
      });
    } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error),
        filteredProducts: []
      });
    }
  },

  getProduct: async (id: string) => {
    set({ isLoading: true, error: null });
    try {
      const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
        headers: {
          'Authorization': `Bearer ${getAuthToken()}`,
          'Content-Type': 'application/json'
        }
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const product = await response.json();
      set({ selectedProduct: product, isLoading: false });
    } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error),
      });
    }
  },

    createProduct: async (productData: {
      name: string;
      caloriesPer100g: number;
      proteinsPer100g: number;
      fatsPer100g: number;
      carbohydratesPer100g: number;
      description?: string;
      category: number;
    }, imageFile?: File | null) => {
    set({ isLoading: true, error: null });
    try {
      
      const formData = new FormData();
      formData.append('name', productData.name);
      formData.append('caloriesPer100g', productData.caloriesPer100g.toString());
      formData.append('proteinsPer100g', productData.proteinsPer100g.toString());
      formData.append('fatsPer100g', productData.fatsPer100g.toString());
      formData.append('carbohydratesPer100g', productData.carbohydratesPer100g.toString());
      formData.append('description', productData.description || '');
      formData.append('category', productData.category.toString());
      
      
      if (imageFile) {
        formData.append('image', imageFile);
      }

      const response = await fetch(`${API_BASE_URL}/api/products`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${getAuthToken()}`
        },
        body: formData
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const newProduct = await response.json();
      set(state => ({
        products: [...state.products, newProduct],
        filteredProducts: [...state.filteredProducts, newProduct],
        isLoading: false
      }));
      
      return newProduct;
    } catch (error: unknown) {
      set({ error: getErrorMessage(error), isLoading: false });
      throw error;
    }
  },

  updateProduct: async (id: string, product: Partial<Product>, imageFile?: File | null) => {
    set({ isLoading: true, error: null });
    try {
      
      const formData = new FormData();
      if (product.name) formData.append('name', product.name);
      if (product.caloriesPer100g) formData.append('caloriesPer100g', product.caloriesPer100g.toString());
      if (product.proteinsPer100g) formData.append('proteinsPer100g', product.proteinsPer100g.toString());
      if (product.fatsPer100g) formData.append('fatsPer100g', product.fatsPer100g.toString());
      if (product.carbohydratesPer100g) formData.append('carbohydratesPer100g', product.carbohydratesPer100g.toString());
      formData.append('description', product.description || '');
      if (product.category) formData.append('category', product.category.toString());
      
      
      if (imageFile) {
        formData.append('image', imageFile);
      }

      const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${getAuthToken()}`
        },
        body: formData
        });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const updatedProduct = await response.json();
      
      
      if (imageFile) {
        await get().uploadProductImage(id, imageFile);
      }
      
      set(state => ({
        products: state.products.map(product => 
          product.id === id ? updatedProduct : product
        ),
        filteredProducts: state.filteredProducts.map(product => 
          product.id === id ? updatedProduct : product
        ),
        selectedProduct: state.selectedProduct?.id === id ? updatedProduct : state.selectedProduct,
        isLoading: false,
      }));
    } catch (error: unknown) {
      set({
        isLoading: false,
        error: getErrorMessage(error),
      });
    }
  },

  deleteProduct: async (id: string) => {
    set({ isLoading: true, error: null });
    try {
      const token = getAuthToken();

      
      
      if (!id || id.trim() === '') {
        throw new Error('Invalid product ID');
      }
      
      const url = `${API_BASE_URL}/api/products/${id}`;

      
      const response = await fetch(url, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Accept': 'application/json'
        }
      });
      

      
      const data = await response.json();
      
      if (!response.ok) {
        
        if (response.status === 401) {
          throw new Error('Unauthorized - Please log in again');
        } else if (response.status === 403) {
          throw new Error('Access denied - You do not have permission to delete this product');
        } else if (response.status === 404) {
          throw new Error('Product not found');
        } else if (response.status === 400) {
          
          if (data.message) {
            throw new Error(data.message);
          }
          throw new Error(`Delete failed: ${response.status} ${response.statusText}`);
        } else if (response.status === 500) {
          throw new Error(`Server error: ${data.message || response.statusText}`);
        } else {
          throw new Error(`Delete failed: ${response.status} ${response.statusText} - ${data.message || 'Unknown error'}`);
        }
      }
      

      
      
      if (data.canDelete) {
        set(state => ({
          products: state.products.filter(p => p.id !== id),
          filteredProducts: state.filteredProducts.filter(p => p.id !== id),
          selectedProduct: state.selectedProduct?.id === id ? null : state.selectedProduct,
          isLoading: false,
        }));
      } else {
        
        set({ isLoading: false });
        throw new Error(data.message);
      }
    } catch (error: unknown) {
      console.error('deleteProduct error:', error);
      set({ error: getErrorMessage(error), isLoading: false });
      throw error;
    }
  },

  uploadProductImage: async (productId: string, imageFile: File) => {
    set({ isLoading: true, error: null });
    try {
      const formData = new FormData();
      formData.append('image', imageFile);

              const response = await fetch(`${API_BASE_URL}/api/products/${productId}/image`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${getAuthToken()}`
        },
        body: formData
      });
      
      if (!response.ok) {
        handleApiError(response);
      }
      
      const result = await response.json();
      
      
      set(state => ({
        products: state.products.map(p => 
          p.id === productId 
            ? { ...p, imageFileName: result.imageFileName, imageContentType: result.imageContentType }
            : p
        ),
        filteredProducts: state.filteredProducts.map(p => 
          p.id === productId 
            ? { ...p, imageFileName: result.imageFileName, imageContentType: result.imageContentType }
            : p
        ),
        selectedProduct: state.selectedProduct?.id === productId 
          ? { ...state.selectedProduct, imageFileName: result.imageFileName, imageContentType: result.imageContentType }
          : state.selectedProduct,
        isLoading: false
      }));
      
      return result;
    } catch (error: unknown) {
      set({ error: getErrorMessage(error), isLoading: false });
      throw error;
    }
  },

  clearSelectedProduct: () => {
    set({ selectedProduct: null });
  },

  clearError: () => {
    set({ error: null });
  },

  resetFilteredProducts: async () => {
    const { currentPage, pageSize } = get();
    set({ searchTerm: '', selectedCategory: 'All', isLoading: true });
    try {
      const response = await fetch(`${API_BASE_URL}/api/products?page=${currentPage}&pageSize=${pageSize}`, {
        headers: {
          'Authorization': `Bearer ${getAuthToken()}`,
          'Content-Type': 'application/json'
        }
      });

      if (!response.ok) {
        handleApiError(response);
      }

      const data = await response.json();
      
      const productsArray = data.products || [];

      set({
        products: productsArray,
        filteredProducts: productsArray,
        isLoading: false,
        currentPage: data.page || currentPage,
        totalPages: data.totalPages || 1,
        totalCount: data.totalCount || productsArray.length
      });
    } catch (error: unknown) {
      console.error('Reset error:', error);
      set({
        isLoading: false,
        error: getErrorMessage(error),
        products: [],
        filteredProducts: []
      });
    }
  },

  setPage: (page: number) => {
    const { searchTerm, selectedCategory } = get();
    set({ currentPage: page });
    
    if (searchTerm) {
      get().searchProducts(searchTerm, page);
    } else if (selectedCategory !== 'All') {
      get().filterByCategory(selectedCategory, page);
    } else {
      get().fetchProducts(page);
    }
  },
}));
