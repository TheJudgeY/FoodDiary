import { API_ENDPOINTS, API_BASE_URL } from '../constants';
import type { 
  RegisterRequest, 
  LoginRequest, 
  AuthResponse, 
  User,
  ProfileFormData
} from '../types';
import { setAuthToken, removeAuthToken, getAuthToken } from '../utils';
import timezoneService from './timezoneService';

const handleApiError = (response: Response) => {
  if (response.status === 401) {
    
    window.location.href = '/login';
  } else if (response.status === 403) {
    throw new Error('Access denied');
  } else if (response.status === 404) {
    throw new Error('User not found');
  } else {
    throw new Error('An error occurred');
  }
};

export const authService = {
  
  register: async (data: RegisterRequest): Promise<AuthResponse> => {
    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.AUTH.REGISTER}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });
    
    if (!response.ok) {
      handleApiError(response);
    }
    
    const result = await response.json();
    
    
    if (result.token) {
      setAuthToken(result.token);
      try {
        const userTimezone = timezoneService.getCurrentTimezone();
        await timezoneService.updateTimezone(userTimezone);
      } catch (error) {
        console.error('Failed to set user timezone:', error);
      }
    }
    
    return result;
  },

  
  login: async (data: LoginRequest): Promise<AuthResponse> => {
    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.AUTH.LOGIN}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });
    
    if (!response.ok) {
      handleApiError(response);
    }
    
    const result = await response.json();
    
    
    if (result.token) {
      setAuthToken(result.token);
      
      
      try {
        const userTimezone = timezoneService.getCurrentTimezone();
        await timezoneService.updateTimezone(userTimezone);
      } catch (error) {
        console.error('Failed to set user timezone:', error);
      }
    }
    
    return result;
  },

  
  getCurrentUser: async (): Promise<User> => {
    const token = getAuthToken();
    if (!token) {
      throw new Error('No authentication token');
    }

    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.USER.ME}`, {
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
    
    try {
      const result = JSON.parse(responseText);
      
      return result.user || result;
    } catch (error) {
      console.error('JSON parse error:', error);
      console.error('Response text that failed to parse:', responseText);
      throw new Error(`Invalid JSON response: ${responseText}`);
    }
  },

  
  confirmEmail: async (email: string, token: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.AUTH.CONFIRM_EMAIL}?email=${encodeURIComponent(email)}&token=${encodeURIComponent(token)}`);
    
    if (!response.ok) {
      handleApiError(response);
    }
  },

  
  resendEmailConfirmation: async (email: string): Promise<void> => {
    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.AUTH.RESEND_EMAIL_CONFIRMATION}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ email })
    });
    
    if (!response.ok) {
      handleApiError(response);
    }
  },

  
  getEmailConfirmationToken: async (email: string): Promise<{ token: string }> => {
    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.AUTH.EMAIL_CONFIRMATION_TOKEN}?email=${encodeURIComponent(email)}`);
    
    if (!response.ok) {
      handleApiError(response);
    }
    
    return await response.json();
  },

  
  logout: (): void => {
    removeAuthToken();
    
  },

  
  isAuthenticated: (): boolean => {
    return !!getAuthToken();
  },

  
  updateUser: async (data: ProfileFormData): Promise<User> => {
    const token = getAuthToken();
    if (!token) {
      throw new Error('No authentication token');
    }

    const response = await fetch(`${API_BASE_URL}${API_ENDPOINTS.USER.UPDATE_PROFILE}`, {
      method: 'PUT',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
    });
    
    if (!response.ok) {
      await response.text();
      
      handleApiError(response);
    }
    
    const result = await response.json();
    return result.user || result;
  },
};

export default authService;
