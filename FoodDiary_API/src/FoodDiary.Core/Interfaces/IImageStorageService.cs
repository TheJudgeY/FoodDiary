namespace FoodDiary.Core.Interfaces;

public interface IImageStorageService
{
    Task<ImageValidationResult> ValidateImageAsync(byte[] imageData, string contentType, string fileName);
    Task<byte[]> ProcessImageAsync(byte[] imageData, string contentType, int maxWidth = 800, int maxHeight = 600, int quality = 80);
    Task<string> GetImageDataUrlAsync(byte[] imageData, string contentType);
    bool IsValidImageType(string contentType);
    long GetMaxImageSizeInBytes();
}

public record ImageValidationResult
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public byte[]? ProcessedImageData { get; init; }
    public string? ProcessedContentType { get; init; }
    public long OriginalSizeInBytes { get; init; }
    public long ProcessedSizeInBytes { get; init; }
} 