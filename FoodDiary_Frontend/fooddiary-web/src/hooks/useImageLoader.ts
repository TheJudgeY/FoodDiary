import { useState, useEffect } from 'react';
import { productService } from '../services/productService';
import { recipeService } from '../services/recipeService';

interface UseImageLoaderOptions {
  type: 'product' | 'recipe';
  id: string;
  hasImage: boolean;
}

interface UseImageLoaderResult {
  imageUrl: string | null;
  isLoading: boolean;
  error: string | null;
  reload: () => void;
}

export const useImageLoader = ({ type, id, hasImage }: UseImageLoaderOptions): UseImageLoaderResult => {
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [reloadTrigger, setReloadTrigger] = useState(0);

  useEffect(() => {
    const loadImage = async () => {
      if (!hasImage) {
        setImageUrl(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        let imageData: string;
        
        if (type === 'product') {
          imageData = await productService.getProductImage(id);
        } else {
          imageData = await recipeService.getRecipeImage(id);
        }
        
        setImageUrl(imageData);
      } catch (err) {
        console.error(`Failed to load ${type} image:`, err);
        setError(err instanceof Error ? err.message : 'Failed to load image');
        setImageUrl(null);
      } finally {
        setIsLoading(false);
      }
    };

    loadImage();
  }, [type, id, hasImage, reloadTrigger]);

  const reload = () => {
    setReloadTrigger(prev => prev + 1);
  };

  return {
    imageUrl,
    isLoading,
    error,
    reload
  };
};

export default useImageLoader;