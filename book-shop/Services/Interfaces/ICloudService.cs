namespace book_shop.Services.Interfaces
{
    public interface ICloudService
    {
        Task<string> UploadImageAsync(IFormFile image);
        Task<string> PingCloudinary();
    }
}
