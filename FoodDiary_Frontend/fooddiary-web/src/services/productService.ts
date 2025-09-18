import { API_BASE_URL } from '../constants';
import { getAuthToken } from '../utils';
import type { 
  Product, 
  CreateProductRequest, 
  UpdateProductRequest,
  DeleteProductResponse
} from '../types';

export const productService = {
  
  async getProducts(params?: {
    search?: string;
    category?: number; 
  }): Promise<Product[]> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const queryParams = new URLSearchParams();
    if (params?.search) queryParams.append('search', params.search);
    if (params?.category !== undefined) queryParams.append('category', params.category.toString());

    const url = `${API_BASE_URL}/api/products${queryParams.toString() ? `?${queryParams.toString()}` : ''}`;

    const response = await fetch(url, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch products: ${response.statusText}`);
    }

    return response.json();
  },

  
  async createProduct(data: CreateProductRequest, imageFile?: File): Promise<Product> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    
    const formData = new FormData();
    
    
    Object.entries(data).forEach(([key, value]) => {
      formData.append(key, value.toString());
    });
    
    
    if (imageFile) {
      formData.append('image', imageFile);
    }

    const response = await fetch(`${API_BASE_URL}/api/products`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });

    if (!response.ok) {
      throw new Error(`Failed to create product: ${response.statusText}`);
    }

    return response.json();
  },

  
  async getProduct(id: string): Promise<Product> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to fetch product: ${response.statusText}`);
    }

    return response.json();
  },

  
  async updateProduct(id: string, data: UpdateProductRequest): Promise<Product> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });

    if (!response.ok) {
      throw new Error(`Failed to update product: ${response.statusText}`);
    }

    return response.json();
  },

  
  async updateProductWithImage(id: string, data: UpdateProductRequest & { image?: File }): Promise<Product> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const formData = new FormData();
    
    
    Object.entries(data).forEach(([key, value]) => {
      if (key !== 'image' && value !== undefined) {
        if (value !== null && value !== undefined) {
          formData.append(key, value.toString());
        }
      }
    });
    
    
    if (data.image) {
      formData.append('image', data.image);
    }

    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });

    if (!response.ok) {
      throw new Error(`Failed to update product: ${response.statusText}`);
    }

    return response.json();
  },

  
  async updateProductImage(id: string, imageFile: File): Promise<{
    productId: string;
    imageFileName: string;
    imageContentType: string;
    imageSizeInBytes: number;
  }> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const formData = new FormData();
    formData.append('image', imageFile);

    const response = await fetch(`${API_BASE_URL}/api/products/${id}/image`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${token}`
      },
      body: formData
    });

    if (!response.ok) {
      throw new Error(`Failed to update product image: ${response.statusText}`);
    }

    return response.json();
  },

  
  async deleteProduct(id: string): Promise<DeleteProductResponse> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      method: 'DELETE',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Accept': 'application/json'
      }
    });

    const data = await response.json();

    if (!response.ok) {
      
      if (data.message) {
        throw new Error(data.message);
      }
      throw new Error(`Failed to delete product: ${response.statusText}`);
    }

    return data;
  },

  
  async getProductImage(id: string): Promise<string> {
    const token = getAuthToken();
    if (!token) {
      throw new Error('Authentication token not found');
    }

    const response = await fetch(`${API_BASE_URL}/api/products/${id}/image`, {
      headers: {
        'Authorization': `Bearer ${token}`
      }
    });

    if (!response.ok) {
      throw new Error(`Failed to get image: ${response.statusText}`);
    }

    
    return response.text();
  }
};

export default productService;