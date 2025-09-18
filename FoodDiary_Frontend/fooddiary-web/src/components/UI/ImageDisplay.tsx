import React from 'react';
import { RefreshCw } from 'lucide-react';
import useImageLoader from '../../hooks/useImageLoader';

interface ImageDisplayProps {
  type: 'product' | 'recipe';
  id: string;
  hasImage: boolean;
  alt: string;
  className?: string;
  fallbackClassName?: string;
}

const ImageDisplay: React.FC<ImageDisplayProps> = ({
  type,
  id,
  hasImage,
  alt,
  className = 'w-full h-full object-cover',
  fallbackClassName = 'w-full h-full flex items-center justify-center bg-surface-100 text-surface-400'
}) => {
  const { imageUrl, isLoading, error, reload } = useImageLoader({ type, id, hasImage });

  if (isLoading) {
    return (
      <div className={fallbackClassName}>
        <div className="animate-spin">
          <RefreshCw className="h-6 w-6" />
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className={`${fallbackClassName} cursor-pointer`} onClick={reload} title="Click to retry">
        <div className="text-center">
          <RefreshCw className="h-6 w-6 mx-auto mb-2" />
          <span className="text-xs">Failed to load image</span>
        </div>
      </div>
    );
  }

  if (!imageUrl) {
    return (
      <div className={fallbackClassName}>
        <span className="text-sm">No image</span>
      </div>
    );
  }

  return (
    <img
      src={imageUrl}
      alt={alt}
      className={className}
    />
  );
};

export default ImageDisplay;
