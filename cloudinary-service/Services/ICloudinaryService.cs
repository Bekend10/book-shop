using cloudinary_service.Models;
using Microsoft.Extensions.Options;

namespace cloudinary_service.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile image);
        Task<bool> DeleteImageAsync(string publicId);
    }
}
