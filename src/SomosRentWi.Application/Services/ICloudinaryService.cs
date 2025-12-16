using Microsoft.AspNetCore.Http;

namespace SomosRentWi.Application.Services;

public interface ICloudinaryService
{
    /// <summary>
    /// Uploads a photo to Cloudinary
    /// </summary>
    /// <param name="file">The photo file to upload</param>
    /// <param name="folder">The Cloudinary folder to upload to (e.g., "client-documents", "car-photos")</param>
    /// <returns>The secure URL of the uploaded photo</returns>
    Task<string> UploadPhotoAsync(IFormFile file, string folder);
    
    /// <summary>
    /// Deletes a photo from Cloudinary
    /// </summary>
    /// <param name="publicId">The public ID of the photo to delete</param>
    /// <returns>True if deletion was successful</returns>
    Task<bool> DeletePhotoAsync(string publicId);
}
