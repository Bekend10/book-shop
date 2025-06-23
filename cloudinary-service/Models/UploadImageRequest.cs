using Microsoft.AspNetCore.Mvc;

namespace cloudinary_service.Models
{
    public class UploadImageRequest
    {
        [FromForm(Name = "image")]
        public IFormFile Image { get; set; }
    }
}
