using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using SomosRentWi.Application.Services;

namespace SomosRentWi.Infrastructure.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary? _cloudinary;

    public CloudinaryService(Cloudinary? cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string> UploadPhotoAsync(IFormFile file, string folder)
    {
        if (_cloudinary == null)
            throw new InvalidOperationException("Cloudinary is not configured. Please set CLOUDINARY_* environment variables.");

        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        // Validate file is an image
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            throw new ArgumentException("Only image files (JPEG, PNG, WEBP) are allowed");

        // Max file size: 10MB
        if (file.Length > 10 * 1024 * 1024)
            throw new ArgumentException("File size cannot exceed 10MB");

        using var stream = file.OpenReadStream();
        
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
        };

        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error != null)
            throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");

        return uploadResult.SecureUrl.ToString();
    }

    public async Task<bool> DeletePhotoAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok";
    }
}
