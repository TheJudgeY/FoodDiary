/**
 * Type-safe error handling utilities for store error management
 */

export interface ApiError {
  response?: {
    status?: number;
    data?: {
      message?: string;
      errors?: Record<string, string[]>;
    };
  };
  message?: string;
}

/**
 * Safely extracts error message from unknown error
 */
export const getErrorMessage = (error: unknown): string => {
  if (error instanceof Error) {
    return error.message;
  }
  
  if (typeof error === 'string') {
    return error;
  }
  
  const apiError = error as ApiError;
  if (apiError?.response?.data?.message) {
    return apiError.response.data.message;
  }
  
  return 'An unexpected error occurred';
};

/**
 * Type-safe error logging
 */
export const logError = (context: string, error: unknown): void => {
  console.error(`${context}:`, error);
  
  const apiError = error as ApiError;
  if (apiError?.response) {
    console.error('API Error Response:', apiError.response);
  }
};
