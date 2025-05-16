using cloudinary_service.Models;
using cloudinary_service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cloudinary_service.Controllers
{
    [Route("api/v1/cloudinary")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinary;

        public UploadController(ICloudinaryService cloudinary)
        {
            _cloudinary = cloudinary;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest file)
        {
            if (file.Image == null || file.Image.Length == 0)
                return BadRequest("Không có dữ liệu !");
            var url = await _cloudinary.UploadImageAsync(file.Image);
            return Ok(new { url });
        }
    }
}