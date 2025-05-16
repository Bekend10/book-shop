using book_shop.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text.Json;

namespace book_shop.Services.Implementations
{
    public class CloudService : ICloudService
    {
        private readonly HttpClient _httpClient;

        public CloudService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("CloudinaryService");
        }

        public async Task<string> UploadImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("Ảnh không hợp lệ");

            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(image.OpenReadStream());
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);

            content.Add(streamContent, "image", image.FileName);

            var response = await _httpClient.PostAsync("/api/v1/cloudinary/upload", content);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var url = doc.RootElement.GetProperty("url").GetString();

            return url;
        }

        public async Task<string> PingCloudinary()
        {
            var response = await _httpClient.GetAsync("/api/v1/cloudinary/ping");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
