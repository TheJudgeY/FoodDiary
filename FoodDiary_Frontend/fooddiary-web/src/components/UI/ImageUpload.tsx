import React, { useState } from 'react';
import { Upload, X } from 'lucide-react';

interface ImageUploadProps {
  onImageSelect: (file: File) => void;
  onImageRemove: () => void;
  currentImage?: string | null;
  label?: string;
  className?: string;
}

const ImageUpload: React.FC<ImageUploadProps> = ({
  onImageSelect,
  onImageRemove,
  currentImage,
  label = 'Upload Image',
  className = '',
}) => {
  const [imagePreview, setImagePreview] = useState<string | null>(currentImage || null);

  
  const validateImage = (file: File): { isValid: boolean; error?: string } => {
    
    if (file.size > 10 * 1024 * 1024) {
      return { isValid: false, error: 'Image size must be less than 10MB' };
    }

    
    const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!validTypes.includes(file.type)) {
      return { isValid: false, error: 'Image must be JPEG, PNG, GIF, or WebP format' };
    }

    return { isValid: true };
  };

  const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const validation = validateImage(file);
      if (!validation.isValid) {
        alert(validation.error); 
        return;
      }

      
      const reader = new FileReader();
      reader.onload = (e) => {
        setImagePreview(e.target?.result as string);
      };
      reader.readAsDataURL(file);

      
      onImageSelect(file);
    }
  };

  const handleRemove = () => {
    setImagePreview(null);
    onImageRemove();
  };

  return (
    <div className={`space-y-2 ${className}`}>
      <label className="block text-sm font-medium text-surface-700">
        {label}
      </label>
      <div className="flex items-center gap-4">
        <div className="relative">
          <input
            type="file"
            accept="image/*"
            onChange={handleImageChange}
            className="hidden"
            id="image-upload"
          />
          <label
            htmlFor="image-upload"
            className="flex items-center gap-2 px-4 py-2 border border-surface-300 rounded-lg cursor-pointer hover:bg-surface-50 transition-colors"
          >
            <Upload className="h-4 w-4" />
            Choose Image
          </label>
        </div>
        {imagePreview && (
          <div className="relative">
            <img
              src={imagePreview}
              alt="Preview"
              className="w-20 h-20 object-cover rounded-lg"
            />
            <button
              type="button"
              onClick={handleRemove}
              className="absolute -top-2 -right-2 p-1 bg-red-500 text-white rounded-full hover:bg-red-600"
            >
              <X className="h-3 w-3" />
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

export default ImageUpload;
