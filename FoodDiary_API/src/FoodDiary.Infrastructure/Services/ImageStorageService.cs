using FoodDiary.Core.Interfaces;

namespace FoodDiary.Infrastructure.Services;

public class ImageStorageService : IImageStorageService
{
    private const long MaxImageSizeInBytes = 10 * 1024 * 1024;
    private readonly string[] ValidImageTypes = { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };

    public async Task<ImageValidationResult> ValidateImageAsync(byte[] imageData, string contentType, string fileName)
    {
        return await Task.Run(() =>
        {
            var originalSize = imageData.Length;

            if (originalSize > MaxImageSizeInBytes)
            {
                return new ImageValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Image size ({originalSize / 1024 / 1024}MB) exceeds maximum allowed size (10MB)",
                    OriginalSizeInBytes = originalSize
                };
            }

            if (!IsValidImageType(contentType))
            {
                return new ImageValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Invalid image type: {contentType}. Supported types: {string.Join(", ", ValidImageTypes)}",
                    OriginalSizeInBytes = originalSize
                };
            }

            return new ImageValidationResult
            {
                IsValid = true,
                ProcessedImageData = imageData,
                ProcessedContentType = contentType,
                OriginalSizeInBytes = originalSize,
                ProcessedSizeInBytes = originalSize
            };
        });
    }

    public async Task<byte[]> ProcessImageAsync(byte[] imageData, string contentType, int maxWidth = 800, int maxHeight = 600, int quality = 80)
    {
        return await Task.Run(() =>
        {
            Thread.Sleep(10);
            return imageData;
        });
    }

    public async Task<string> GetImageDataUrlAsync(byte[] imageData, string contentType)
    {
        return await Task.Run(() =>
        {
            var base64 = Convert.ToBase64String(imageData);
            return $"data:{contentType};base64,{base64}";
        });
    }

    public bool IsValidImageType(string contentType)
    {
        return ValidImageTypes.Contains(contentType.ToLower());
    }

    public long GetMaxImageSizeInBytes() => MaxImageSizeInBytes;
} 