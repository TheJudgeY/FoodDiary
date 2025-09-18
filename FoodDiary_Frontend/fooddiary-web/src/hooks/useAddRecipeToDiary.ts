import { useState } from 'react';
import { foodEntryService } from '../services/foodEntryService';
import { AddRecipeToDiaryRequest, AddRecipeToDiaryResponse } from '../types';

interface UseAddRecipeToDiary {
  addRecipeToDiary: (request: AddRecipeToDiaryRequest) => Promise<AddRecipeToDiaryResponse>;
  isLoading: boolean;
  error: string | null;
}

export const useAddRecipeToDiary = (): UseAddRecipeToDiary => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const addRecipeToDiary = async (request: AddRecipeToDiaryRequest) => {
    setIsLoading(true);
    setError(null);
    
    try {
      const result = await foodEntryService.addRecipeToDiary(request);
      return result;
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Unknown error';
      setError(errorMessage);
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  return { addRecipeToDiary, isLoading, error };
};
